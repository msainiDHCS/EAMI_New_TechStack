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

    [TaskAction(ActionName= "ActionDexFileReconcile", IsPhantom = false)]
    public class ActionDexFileReconcile : TaskActionBase
    {
        public override TaskResult Execute()
        {
            TaskResult result = new TaskResult(true, string.Empty, enProductiveOutcome.NONE);

            try
            {
                string dataSourceKey = Context.GetExecutionArgTextValueByKey("DATA_SOURCE_KEY");
                string archiveFolder = Context.GetExecutionArgTextValueByKey("DEX_ARCHIVE_FOLDER");
                string receiveFolder = Context.GetExecutionArgTextValueByKey("DEX_RECEIVE_FOLDER");
                EAMIDBConnection.EAMIDBContext = dataSourceKey;

                //DOWNLOAD DEX FILE
                List<string> dexFileList = GetDEXFileList(receiveFolder);

                if (dexFileList.Count > 0)
                {
                    DateTime dexFileReceiveDate = DateTime.Now;
                    ElectronicClaimSchedule ecs = null;
                    CommonStatusPayload<ElectronicClaimSchedule> reconcileStatus = new CommonStatusPayload<ElectronicClaimSchedule>(ecs, false);
                    bool hasErrors = false;
                    bool hasSuccess = false;
                    string errorMessage = string.Empty;

                    foreach (string dexFile in dexFileList)
                    {
                        //PARSE FILE
                        List<WarrantRec> EamiDexRecordList = ParseDexFile(receiveFolder, dexFile);

                        ////VALIDATE 
                        string taskNumber = Context == null ? "99999" : Context.TaskNumber;
                        reconcileStatus = ReconcileECSAndDexFile(dexFile, dexFileReceiveDate, taskNumber, EamiDexRecordList);

                        if (reconcileStatus.Status && reconcileStatus.Payload != null)
                        {
                            UpdateECStatus(reconcileStatus.Payload);
                            hasSuccess = true;
                        }
                        else
                        {
                            hasErrors = true;
                            errorMessage = string.Format("{0}{1}{2}", errorMessage, string.IsNullOrEmpty(errorMessage) ? string.Empty : ";", reconcileStatus.GetCombinedMessage());

                            //IF ECS match is found but the failed validation, then Update The DexFileName of the ECS
                            if (reconcileStatus.Payload.EcsId > 0)
                            {
                                DataAccess.ClaimScheduleDataDbMgr.UpdateElectronicClaimScheduleDexFileName(reconcileStatus.Payload.EcsId, dexFile);
                            }
                        }

                        //ARCHIVE FILE
                        ArchiveDexFile(receiveFolder, archiveFolder, dexFile);
                    }

                    if (hasSuccess)
                    {
                        //One or all DEX files reconciled
                        result.Status = true;
                        result.Note = hasErrors ? errorMessage : string.Empty;
                        result.Outcome = hasErrors ? enProductiveOutcome.PARTIAL : enProductiveOutcome.FULL;
                    }
                    else
                    {
                        //None of the DEX files reconciled
                        result.Status = false;
                        result.Note = errorMessage ;
                        result.Outcome = enProductiveOutcome.NONE;
                    }
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

        private List<string> GetDEXFileList(string localFolderPath)
        {
            List<string> ecsFileList = new List<string>();
            ecsFileList.AddRange(Directory.GetFiles(localFolderPath).Select(Path.GetFileName).ToList());
            return ecsFileList;
        }
        
        private void ArchiveDexFile(string localSCODownloadFolder, string archiveFolder, string dexFileName)
        {            
            string sourceFilePath = string.Format(@"{0}\{1}", localSCODownloadFolder, dexFileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFilePath);
            string fileExtenssion = Path.GetExtension(sourceFilePath).Replace(".", string.Empty);

            if (File.Exists(sourceFilePath))
            {
                File.Move(sourceFilePath, string.Format(@"{0}\{1}.{2}.{3}", archiveFolder, fileNameWithoutExtension, Guid.NewGuid(), fileExtenssion));
            }
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
