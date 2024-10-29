using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

//using TSAE.Entity;
using FTSAS.Integration;

using OHC.EAMI.Common;
using OHC.EAMI.DataAccess;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.SCO;
using OHC.EAMI.Util.FileTransfer;

namespace OHC.EAMI.FTSASTaskActions
{    

    [TaskAction(ActionName= "ActionDexFileDownload", IsPhantom = false)]
    public class ActionDexFileDownload : TaskActionBase
    {
        public override TaskResult Execute()
        {
            TaskResult result = new TaskResult(true, string.Empty, enProductiveOutcome.NONE);

            try
            {
                string dataSourceKey = Context.GetExecutionArgTextValueByKey("DATA_SOURCE_KEY");
                string sourceFolderPath = Context.GetExecutionArgTextValueByKey("FTP_FOLDER_SOURCE");
                string destinationFolderPath = Context.GetExecutionArgTextValueByKey("FTP_FOLDER_DESTINATION");
                EAMIDBConnection.EAMIDBContext = dataSourceKey;
                
                //DOWNLOAD DEX FILE
                CommonStatusPayload<List<string>> cs = DownloadDexFiles(destinationFolderPath, sourceFolderPath);

                if (cs.Status || (!cs.Status && cs.Payload.Count > 0))
                {
                    if (cs.Status && cs.Payload.Count == 0)
                    {
                        //OUTCOME - NONE
                        result.Status = true;
                        result.Outcome = enProductiveOutcome.NONE;
                    }
                    else
                    {
                        //OUTCOME - PARTIAL or FULL
                        DateTime dexFileReceiveDate = DateTime.Now;
                        List<string> dexFileList = cs.Payload;

                        //CAPTURE DOWNLOAD STATUS
                        bool hasErrors = !cs.Status;
                        string errorMessage = cs.GetCombinedMessage();
                        result.Status = true;
                        result.Note = hasErrors ? errorMessage : string.Empty;
                        result.Outcome = hasErrors ? enProductiveOutcome.PARTIAL : enProductiveOutcome.FULL;
                    }
                }
                else
                {
                    result.Status = false;
                    result.Note = cs.GetCombinedMessage();
                    result.Outcome = enProductiveOutcome.NONE;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Outcome = enProductiveOutcome.NONE;
                result.Note = ex.Message + "; " + ex.StackTrace;
                EAMILogger.Instance.Error(ex);
            }

            //RETURN Status
            return result;
        }
 
        private CommonStatusPayload<List<string>> DownloadDexFiles(string localSCODownloadFolder, string sourceFolderPath)
        {
            List<string> dexFileList = new List<string>();
            CommonStatusPayload<List<string>> cs = new CommonStatusPayload<List<string>>(dexFileList, true);
            bool fileDownloadIsComplete = false;

            try
            {
                //DOWNLOAD FILES HERE          
                string dexFileExtenssion = ".DEX";
                FileTransferManager fileTransferManager = new FileTransferManager();
                fileTransferManager.SFTP_SetClient();
                List<string> ftpFileList = new List<string>();
                ftpFileList.AddRange(fileTransferManager.SFTP_ListFiles(sourceFolderPath, dexFileExtenssion).Select(_ => _.FileName));

                if (ftpFileList.Count > 0)
                {
                    //DETERMINE IF DEX FILE WAS PREVIOUSLY DOWNLOADED
                    List<string> dexFileListToDownload = new List<string>();

                    //GET FILE LIST THAT WAS NOT PREVIOUSLY DOWNLOADED
                    dexFileListToDownload.AddRange(DataAccess.ClaimScheduleDataDbMgr.GetDexFileNameList(ftpFileList).Payload);

                    //DOWNLOAD EVERY FILE THAT WAS NOT PREVIOUSLY DOWNLOADED
                    foreach (string dexFileName in dexFileListToDownload)
                    {
                        FileDownloadRequest fileDownloadRequest = new FileDownloadRequest();
                        fileDownloadRequest.DownloadFromFolder = sourceFolderPath;
                        fileDownloadRequest.DownloadToFolder = localSCODownloadFolder;
                        fileDownloadRequest.DownloadFileName = dexFileName;

                        //DOWNLOAD HERE - A SINGLE FILE
                        List<FileTransferInformation> fileTransferInformationList = fileTransferManager.SFTP_Download_SingleFile(fileDownloadRequest);

                        //CAPTURE DOWNLOADED FILE NAME
                        dexFileList.Add(dexFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                cs.Status = false;
                cs.AddMessageDetail(ex.Message);
                EAMILogger.Instance.Error(ex);
            }

            return cs;
        }
        
        private List<WarrantRec> ParseDexFile(string dexFilePath, string dexFile)
        {
            List<string> dexLines = new List<string>();
            List<WarrantRec> eamiDexRecordList = new List<WarrantRec>();
            CommonStatusPayload<List<WarrantRec>> cs = new CommonStatusPayload<List<WarrantRec>>(eamiDexRecordList, true);
            RefCodeTableList rcTableList = RefCodeDBMgr.GetRefCodeTableList();
            EAMIMasterData eamiMasterData = new EAMIMasterData();
            eamiMasterData.SystemProperty = rcTableList.GetRefCodeListByTableName(enRefTables.TB_System).GetRefCodeByCode<SystemProperty>(rcTableList.GetRefCodeListByTableName(enRefTables.TB_SYSTEM_OF_RECORD)[0].Code);
            int orgID = int.Parse(eamiMasterData.SystemProperty.OrganizationCode);

            try
            {
                //READ THE FILE
                using (StreamReader streamReader = new StreamReader(string.Format(@"{0}\{1}", dexFilePath, dexFile), Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        dexLines.Add(line);
                    }
                };

                //PARSE REORDS
                ScoServiceManager scoServiceManager = new ScoServiceManager(orgID);
                eamiDexRecordList.AddRange(PopulateWarrantRecord(scoServiceManager.ReadDEXFile(dexLines).Payload));
            }
            catch (Exception ex)
            {
                cs.Status = false;
                cs.AddMessageDetail(ex.Message);
                EAMILogger.Instance.Error(ex);
            }

            return eamiDexRecordList;
        }

        private CommonStatusPayload<ElectronicClaimSchedule> ReconcileECSAndDexFile(string dexFileName, DateTime dexFileReceiveDate, string warrantReceivedTaskNumber, List<WarrantRec> warrantRecList)
        {
            ElectronicClaimSchedule ecs = null;
            CommonStatusPayload<ElectronicClaimSchedule> cs = new CommonStatusPayload<ElectronicClaimSchedule>(ecs, false);

            if (warrantRecList.GroupBy(_ => _.ECS_NUMBER).Count() > 1)
            {
                cs.AddMessageDetail("Failed SCO DEX File Validation. Multiple ECS_NUMBER values in a single DEX file.");
            }
            else
            {
                var ecsNumber = warrantRecList.GroupBy(_ => _.ECS_NUMBER).Select(g => new { g.Key, Count = g.Count() }).First().Key;
                cs = DataAccess.ClaimScheduleDataDbMgr.ReconcileECS(ecsNumber.ToString(), dexFileName, dexFileReceiveDate, warrantReceivedTaskNumber, warrantRecList);
            }

            return cs;
        }

        private CommonStatus UpdateECStatus(ElectronicClaimSchedule ecs)
        {
            ecs.CurrentStatusType = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_ECS_STATUS_TYPE).Where(_ => _.Code == "WARRANT_RECEIVED").FirstOrDefault(); 
            ecs.CurrentStatusNote = string.Empty;
            return DataAccess.ClaimScheduleDataDbMgr.UpdateElectronicClaimScheduleStatus(ecs, "system");
        }

        private List<WarrantRec> PopulateWarrantRecord(List<EamiDexRecord> dexRecordList)
        {
            List<WarrantRec> eamiDexRecordList = new List<WarrantRec>();

            foreach (EamiDexRecord dexRec in dexRecordList)
            {
                eamiDexRecordList.Add(new WarrantRec(){
                    ECS_NUMBER = dexRec.CLAIM_SCHEDULE_NUMBER.Trim(),
                    ISSUE_DATE = DateTime.Parse(string.Format(@"{0}/{1}/{2}", 
                        dexRec.ISSUE_DATE.Trim().Substring(4, 2),
                        dexRec.ISSUE_DATE.Trim().Substring(6, 2),
                        dexRec.ISSUE_DATE.Trim().Substring(0, 4))),
                    SEQ_NUMBER = int.Parse(dexRec.SEQ_NUMBER.Trim()),
                    WARRANT_AMOUNT = decimal.Parse(dexRec.WARRANT_AMOUNT.Trim()),
                    WARRANT_NUMBER = dexRec.WARRANT_NUMBER     
                });
            }


            //20181129
            return eamiDexRecordList;
        }
    }
}
