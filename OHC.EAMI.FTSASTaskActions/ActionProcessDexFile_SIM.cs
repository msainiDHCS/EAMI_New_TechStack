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

    [TaskAction(ActionName="ActionProcessDexFile_SIM", IsPhantom = false)]
    public class ActionProcessDexFile_SIM : TaskActionBase
    {
        public override TaskResult Execute()
        {
            TaskResult result = new TaskResult(true, string.Empty, enProductiveOutcome.NONE);

            try
            {
                EAMIDBConnection.EAMIDBContext = Context.GetExecutionArgTextValueByKey("DATA_SOURCE_KEY");
                string localSCODownloadFolder = string.Format(@"{0}\{1}\", ConfigurationManager.AppSettings["RootPath"].ToString(), ConfigurationManager.AppSettings["folder_sco_receive"].ToString());
                int currentStatusTypeId = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_ECS_STATUS_TYPE).Where(_ => _.Code == "SENT_TO_SCO").First().ID;

                //SIM GET A LIST OF SENT_TO_SCO ECS's
                List<ElectronicClaimSchedule> sentToSco_ecsList = new List<ElectronicClaimSchedule>();
                sentToSco_ecsList = DataAccess.ClaimScheduleDataDbMgr.GetElectronicClaimSchedulesByDateRangeStatusType(currentStatusTypeId, null, null);

                DateTime dexFileReceiveDate = DateTime.Now;
                bool hasErrors = false;

                if (sentToSco_ecsList.Count > 0)
                {
                    ElectronicClaimSchedule ecs = null;
                    CommonStatusPayload<ElectronicClaimSchedule> reconcileStatus = new CommonStatusPayload<ElectronicClaimSchedule>(ecs, false);

                    foreach (ElectronicClaimSchedule sentToSco_ecs in sentToSco_ecsList)
                    {
                        //PARSE FILE
                        List<WarrantRec> EamiDexRecordList = ParseDexFile(sentToSco_ecs);
                        string dexFileName = ComposeDexFileName(sentToSco_ecs.EcsFileName);

                        ////VALIDATE 
                        string taskNumber = Context == null ? "99999" : Context.TaskNumber;
                        reconcileStatus = ReconcileECSAndDexFile(dexFileName, dexFileReceiveDate, taskNumber, EamiDexRecordList);

                        if (reconcileStatus.Status && reconcileStatus.Payload != null)
                        {
                            UpdateECStatus(reconcileStatus.Payload);
                        }
                        else
                        {
                            hasErrors = true;
                        }

                        //ARCHIVE FILE
                        //ArchiveDexFile(localSCODownloadFolder, dexFileName);
                    }

                    result.Status = true;
                    result.Note = hasErrors ? reconcileStatus.GetCombinedMessage() : string.Empty;
                    result.Outcome = hasErrors ? enProductiveOutcome.PARTIAL : enProductiveOutcome.FULL;
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

        private CommonStatusPayload<List<string>> DownloadDexFiles(string localSCODownloadFolder)
        {
            List<string> dexFileList = new List<string>();
            CommonStatusPayload<List<string>> cs = new CommonStatusPayload<List<string>>(dexFileList, true);

            try
            {
                //DOWNLOAD FILES HERE
                bool fileDownloadIsComplete = true;

                //GET DOWNLOADED FILE NAMES
                if (fileDownloadIsComplete)
                {
                    dexFileList.AddRange(Directory.GetFiles(localSCODownloadFolder).Select(Path.GetFileName).ToList());
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

        private void ArchiveDexFile(string localSCODownloadFolder, string dexFileName)
        {
            string archiveFolderPath = string.Format(@"{0}\{1}\", ConfigurationManager.AppSettings["RootPath"].ToString(), ConfigurationManager.AppSettings["folder_archive_sco_dex"].ToString());
            string sourceFilePath = string.Format(@"{0}\{1}", localSCODownloadFolder, dexFileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFilePath);
            string fileExtenssion = Path.GetExtension(sourceFilePath).Replace(".", string.Empty);

            if (File.Exists(sourceFilePath))
            {
                File.Move(sourceFilePath, string.Format(@"{0}\{1}.{2}.{3}", archiveFolderPath, fileNameWithoutExtension, Guid.NewGuid(), fileExtenssion));
            }
        }

        private List<WarrantRec> ParseDexFile(ElectronicClaimSchedule ecs)
        {
            List<string> dexLines = new List<string>();
            List<WarrantRec> eamiDexRecordList = new List<WarrantRec>();
            CommonStatusPayload<List<WarrantRec>> cs = new CommonStatusPayload<List<WarrantRec>>(eamiDexRecordList, true);

            try
            {
                eamiDexRecordList.AddRange(PopulateWarrantRecord(ecs));
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

        private List<WarrantRec> PopulateWarrantRecord(ElectronicClaimSchedule sentToSco_ecs)
        {            
            DateTime issueDate = DateTime.Now;
            List<WarrantRec> eamiDexRecordList = new List<WarrantRec>();

            foreach (ClaimSchedule cs in sentToSco_ecs.ClaimScheduleList)
            {
                eamiDexRecordList.Add(new WarrantRec()
                {
                    ECS_NUMBER = sentToSco_ecs.EcsNumber,
                    ISSUE_DATE = issueDate,
                    SEQ_NUMBER = cs.SeqNumber,
                    WARRANT_AMOUNT = cs.Amount,
                    WARRANT_NUMBER = ComposeWarrantNumber(sentToSco_ecs.EcsNumber, cs.SeqNumber)
                });
            }
            return eamiDexRecordList;
        }

        private string ComposeWarrantNumber(string EcsNumber, int SeqNumber)
        {
            int startFrom = 40000;
            string result = string.Empty;
            result = (startFrom + int.Parse(EcsNumber.Substring(2))).ToString();
            result = string.Format("{0}{1:000}", result, SeqNumber);
            return result;
        }

        private string ComposeDexFileName(string scoFileName)
        {
            string result = scoFileName.Replace("PS.DEVL.DISB.FTP.", string.Empty).Replace(".MCMW.WARRANT", string.Empty);

            result = string.Format("{0}.MCMW.DEX", result);

            return result;
        }

    }
}
