using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.IO;

using FTSAS.Integration;

using OHC.EAMI.Common;
using OHC.EAMI.DataAccess;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.Util;
using OHC.EAMI.Util.FileTransfer;
using System.Text;

namespace OHC.EAMI.FTSASTaskActions
{
    
    [TaskAction(ActionName= "ActionAuditFileMove", IsPhantom = false)] 
    public class ActionAuditFileMove : TaskActionBase
    {
        public override TaskResult Execute()
        {
            TaskResult result = new TaskResult(true,string.Empty,enProductiveOutcome.NONE);

            try
            {
                string dataSourceKey = Context.GetExecutionArgTextValueByKey("DATA_SOURCE_KEY");
                string sourceFolderPath = Context.GetExecutionArgTextValueByKey("FTP_FOLDER_SOURCE");
                string destinationFolderPath = Context.GetExecutionArgTextValueByKey("FTP_FOLDER_DESTINATION");
                string fileMaskValue = Context.GetExecutionArgTextValueByKey("FILE_MASK_VALUE");
                string fileMaskExtValue = Context.GetExecutionArgTextValueByKey("FILE_MASK_EXT_VALUE");
                string emailValue = Context.GetExecutionArgTextValueByKey("SEND_TO_EMAIL");               
                string ftpHost = Context.GetExecutionArgTextValueByKey("FTP_HOST");
                string ftpPort = Context.GetExecutionArgTextValueByKey("FTP_PORT");

               


                EAMIDBConnection.EAMIDBContext = dataSourceKey;
                string scheduledTaskNumber = Context == null ? string.Empty : Context.TaskNumber;
                fileMaskValue = fileMaskValue.Trim().Length == 0 ? string.Empty : fileMaskValue.Trim();
                fileMaskExtValue = fileMaskExtValue.Trim().Length == 0 ? string.Empty : fileMaskExtValue.Trim();
                emailValue = emailValue.Trim().Length == 0 ? string.Empty : emailValue.Trim();
                ftpPort = ftpPort.Trim().Length == 0 ? "0" : ftpPort.Trim();

                


                EAMILogger.Instance.Trace(string.Format("PROCESS_START"));

                EAMILogger.Instance.Trace(string.Format("DATA_SOURCE_KEY={0}", dataSourceKey));

                //GET AVAILABLE LIST OF PAYDATES
                List<DateTime> paydateList = new List<DateTime>();
                paydateList = DataAccess.ClaimScheduleDataDbMgr.GetAvailableAuditFilePayDates();
                
                EAMILogger.Instance.Trace(string.Format("GetAvailableAuditFilePayDates={0}", paydateList.Count().ToString()));

                //PROCEED IF AVAILABLE PAYDATES EXIST
                if (paydateList.Count() > 0)
                {
                    CommonStatus actionStatus = new CommonStatus(true);
                    List<Tuple<string, DateTime, DateTime>> auditFileList_Uploaded = new List<Tuple<string, DateTime, DateTime>>();

                    //GET AVAILABLE AUDIT FILES
                    List<FTPFile> auditFileList = GetAuditFileList(sourceFolderPath,ftpHost,int.Parse(ftpPort));


                    EAMILogger.Instance.Trace(string.Format("GetAuditFileList={0}", auditFileList.Count().ToString()));

                    if (auditFileList.Count > 0)
                    {
                        //Filter Audit files by mask
                        List<FTPFile> auditFileList_Filtered = FilterAuditFileListByMask(fileMaskValue, fileMaskExtValue, auditFileList);

                        //MATCH AUDIT FILES TO PAYDATES
                        foreach (DateTime paydate in paydateList)
                        {
                            List<FTPFile> fileList_toUpload = MatchPaydateToAuditFiles(paydate, auditFileList_Filtered);

                            if (fileList_toUpload.Count > 0)
                            {
                                //Move Files
                                foreach (FTPFile file in fileList_toUpload)
                                {
                                    DateTime createDate = DateTime.Now;

                                    //Move audit file to OUT folder
                                    CommonStatus cs = MoveAuditFile(file, sourceFolderPath, destinationFolderPath, ftpHost, int.Parse(ftpPort));
                                    DateTime? uploadDate = null;
                                    if (cs.Status)
                                    {
                                        DateTime uploadDateNow = DateTime.Now;
                                        uploadDate = uploadDateNow;
                                        auditFileList_Uploaded.Add(new Tuple<string, DateTime, DateTime>(file.FileName,paydate, uploadDateNow));
                                    }
                                    else
                                    {
                                        actionStatus.Status = false;
                                        actionStatus.AddMessageDetail(cs.GetCombinedMessage());
                                    }

                                    //SAVE Audit File data to DB
                                    DataAccess.ClaimScheduleDataDbMgr.InsertAuditFileData(file.FileName, file.FileSize, paydate, scheduledTaskNumber, !cs.Status, createDate, uploadDate);
                                }
                            }
                            else
                            {
                                //Available Paydate exist, but no Audit files exist
                                actionStatus.Status = false;
                                actionStatus.AddMessageDetail(string.Format("Audit file does not exist for paydate: {0}", paydate.ToString("MM/dd/yyyy")));
                            }
                        }
                    }
                    else
                    {
                        //Available Paydate exist, but no Audit files exist
                        actionStatus.Status = false;
                        actionStatus.AddMessageDetail(string.Format("Audit file does not exist for paydate: {0}", String.Join("; ", paydateList.Select(_ => _.ToString("MM/dd/yyyy")))));
                    }
                    
                    //SET APPROPRIATE OUTCOME STATUS
                    if (actionStatus.Status && auditFileList_Uploaded.Count() > 0)
                    {
                        result.Outcome = enProductiveOutcome.FULL;
                    }
                    else if (!actionStatus.Status && auditFileList_Uploaded.Count() > 0)
                    {
                        //If Status is false - Capture note outcome and set otcome as PARTIAL
                        result.Outcome = enProductiveOutcome.PARTIAL;
                        result.Note = actionStatus.GetCombinedMessage();
                    }
                    else if (!actionStatus.Status && auditFileList_Uploaded.Count() == 0)
                    {
                        //If Status is false - Capture note outcome and set otcome as NONE
                        result.Status = false;
                        result.Outcome = enProductiveOutcome.NONE;
                        result.Note = actionStatus.GetCombinedMessage();
                    }
                }

                //NOTIFICATION
                //Get Audit File list to Notify
                List<Tuple<int, string, DateTime, DateTime>> auditFiles_ForNotification = DataAccess.ClaimScheduleDataDbMgr.GetAuditFilesForNotification();
                if (auditFiles_ForNotification.Count > 0)
                {
                    CommonStatus notification_status = SendAuditFileUploadNotification(auditFiles_ForNotification, emailValue);

                    if (notification_status.Status)
                    {
                        //UPDATE notification flag
                        DateTime notifiedDate = DateTime.Now;
                        foreach (Tuple<int, string, DateTime, DateTime> file in auditFiles_ForNotification)
                        {
                            DataAccess.ClaimScheduleDataDbMgr.UpdateAuditFileNotifiedDate(file.Item1, notifiedDate);
                        }
                    }
                    else
                    {
                        //If Status is false - Capture note outcome and set otcome as NONE
                        result.Status = false;
                        result.Outcome = enProductiveOutcome.NONE;
                        result.Note = notification_status.GetCombinedMessage();
                    }
                }

                EAMILogger.Instance.Trace(string.Format("PROCESS_START"));
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Outcome = enProductiveOutcome.NONE;
                result.Note = ex.Message + "; " + ex.StackTrace ;
                EAMILogger.Instance.Error(ex);
            }

            //RETURN Status
            return result;
        }
        
        private List<FTPFile> GetAuditFileList(string sourcerFolder, string ftpHost, int ftpPort)
        {
            FileTransferManager fileTransferManager = new FileTransferManager();
            fileTransferManager.SFTP_SetClient(ftpHost, ftpPort);
            List<FTPFile> ftpFileList = new List<FTPFile>();
            ftpFileList.AddRange(fileTransferManager.SFTP_ListFiles(sourcerFolder,string.Empty));
            
            return ftpFileList;
        }
        private List<FTPFile> FilterAuditFileListByMask(string maskValue, string maskExtValue, List<FTPFile> auditFileList)
        {
            List<string> fileMaskList = new List<string>();
            List<string> fileMaskExtList = new List<string>();

            //Determine mask list (separated by coma)
            if (!string.IsNullOrEmpty(maskValue))
            {
                fileMaskList = maskValue.Trim().ToLower().Split(',').ToList();
            }

            //Determine File Extenssion List (separated by coma)
            if (!string.IsNullOrEmpty(maskExtValue))
            {
                fileMaskExtList = maskExtValue.Trim().ToLower().Split(',').ToList();
            }

            List<FTPFile> filteredFileList = new List<FTPFile>();

            //Match against file name mast and file extenssion mask
            if (fileMaskList.Count > 0 || fileMaskExtList.Count > 0)
            {
                //Go through each available file and try to match it to mask
                foreach (FTPFile auditFile in auditFileList)
                {
                    bool mask_match = false;
                    bool mask_ext_match = false;

                    //File Name: Try to match File Name Mask
                    foreach (string mask in fileMaskList)
                    {
                        //find a mask match
                        if (auditFile.FileName.ToLower().IndexOf(mask.Trim()) >= 0)
                        {
                            //add matched oudit file to list
                            mask_match = true;
                            continue;
                        }
                    }

                    //File Extenssion Try to match File Extenssion Mask
                    foreach (string maskExt in fileMaskExtList)
                    {
                        //find a mask match
                        if (auditFile.FileName.ToLower().EndsWith(maskExt.Trim()))
                        {
                            //add matched oudit file to list
                            mask_ext_match = true;
                            continue;
                        }
                    }

                    //Check if both - file mane and file extenssion mask matched
                    if (mask_match && mask_ext_match)
                    {
                        //Audit File matched Name and Extenssion mask
                        filteredFileList.Add(auditFile);
                    }
                }
            }
            else
            {
                //if no mask exist, returned unfiltered list
                filteredFileList = auditFileList;
            }
            
            return filteredFileList;
        }
        
        private List<FTPFile> MatchPaydateToAuditFiles(DateTime paydate, List<FTPFile> auditFileList)
        {
            List<FTPFile> matchedAuditFileList = new List<FTPFile>();
            List<DateTime> paydateList = new List<DateTime>();
            string payDateValue = string.Format(".D{0}", paydate.ToString("MMddyy"));

            foreach (FTPFile auditFile in auditFileList)
            {
                //Match Audit file using the composed Paydate Value
                //Example of the Audit file name: MCDE0S.D121021A.MCDE.INPUT
                if (auditFile.FileName.IndexOf(payDateValue) >= 0)
                {
                    matchedAuditFileList.Add(auditFile);
                }
            }

            return matchedAuditFileList;
        }

        private CommonStatus MoveAuditFile(FTPFile auditFile, string sourceFolder, string targetFolder, string ftpHost, int ftpPort)
        {
            CommonStatus status = new CommonStatus(true, string.Empty);

            try
            {
                //MOVE Audit File           
                FileTransferManager fileTransferManager = new FileTransferManager();
                fileTransferManager.SFTP_SetClient(ftpHost, ftpPort);
                fileTransferManager.SFTP_MoveFile(sourceFolder, targetFolder, auditFile.FileName);
            }
            catch (Exception ex)
            {
                EAMILogger.Instance.Error(ex);
                status.Status = false;
                status.AddMessageDetail(ex.Message);
            }

            return status;
        }

        private List<Tuple<string, CommonStatus>> SendFilesToSCO(List<string> ecsFileList, string localFolderPath)
        {
            RefCodeTableList rcTableList = RefCodeDBMgr.GetRefCodeTableList();
            string scoUploadFolderName = string.Format(@"{0}", ConfigurationManager.AppSettings["ftp_folder_to_sco"].ToString());
            
            List<Tuple<string, CommonStatus>> ecsFileStatusList = new List<Tuple<string, CommonStatus>>(new List<Tuple<string, CommonStatus>>());

            //USE ECS with SUCCESS status
            foreach (string ecsFile in ecsFileList)
            {
                try
                {
                    //UPLOAD CSO FILE TO SFTP           
                    FileTransferManager fileTransferManager = new FileTransferManager();
                    fileTransferManager.SFTP_SetClient();
                    fileTransferManager.SFTP_Upload(new List<FileTransferInformation>() { new FileTransferInformation(localFolderPath, ecsFile, scoUploadFolderName) });
                    ecsFileStatusList.Add(new Tuple<string, CommonStatus>(ecsFile, new CommonStatus(true)));
                }
                catch (Exception ex)
                {
                    EAMILogger.Instance.Error(ex);
                    ecsFileStatusList.Add(new Tuple<string, CommonStatus>(ecsFile, new CommonStatus(false, ex.Message)));
                }                
            }
            return ecsFileStatusList;
        }

        private void ClearUploadFolder(string localFolderPath, List<Tuple<string, CommonStatus>> ecsFileList)
        {
            System.Threading.Thread.Sleep(1000);

            string errorMessage = "TransportECS Error. Failed to clear local folder. FileName={0}{1}";
            try
            { 
                foreach (Tuple<string, CommonStatus> ecsFile in ecsFileList)
                {
                    errorMessage = string.Format(errorMessage, localFolderPath, ecsFile.Item1);
                    File.Delete(string.Format("{0}{1}", localFolderPath, ecsFile.Item1));
                }
            }
            catch
            {
                throw (new Exception(errorMessage));
            }
        }

        private CommonStatus SendAuditFileUploadNotification(List<Tuple<int, string, DateTime, DateTime>> auditFileList, string emailValue)
        {
            CommonStatus cs = new CommonStatus(true);

            try
            {
                string emailValueaudit = Context.GetExecutionArgTextValueByKey("SEND_TO_EMAIL");
                List<string> emailList = new List<string>();
                emailList = emailValue.Trim().ToLower().Split(';').ToList();


                if (emailList.Count > 0)
                {
                    bool isProdEnv = bool.Parse(ConfigurationManager.AppSettings["IsProductionEnvironment"].ToString());
                    string serverName = ConfigurationManager.AppSettings["SMTPServer"].ToString();
                    string email_from = ConfigurationManager.AppSettings["NoReplyEmailAddr"].ToString();
                    string sysName = RefCodeDBMgr.GetRefCodeTableList().GetRefCodeListByTableName(enRefTables.TB_SYSTEM_OF_RECORD).First().Code;
                    //Modified subject as part of Trello Card EAMI Notification Code Changes
                    string subject = "EAMI(MDSDFFS) NOTIFICATION -["+ sysName+"] " + (isProdEnv ? string.Empty : " (" + Environment.MachineName + ")");

                    int portID = 0;
                    bool useDefaultCredentials = true;

                    string[] email_to = emailValue.Replace(" ", string.Empty).TrimEnd(';').Split(';'); // new string[] { recipientAddr };
                   // string[] email_to = emailList.ToArray();
                    string[] email_to_cc = new string[] { "" };
                    string[] email_to_bcc = new string[] { "" };

                    StringBuilder sb = new StringBuilder();
                    //Modified email body as part of Trello Card EAMI Notification Code Changes
                    sb.AppendLine(string.Format("Following MDSDFFS audit files have been successfully uploaded:", sysName));
                    sb.AppendLine();
                    
                    //item1=AuditFileID, item2=fileName, item4=PayDate
                    //ORDER BY item3 (upload date)
                    foreach (Tuple<int, string, DateTime, DateTime> file in auditFileList.OrderBy(_ => _.Item3))
                    {
                        sb.AppendLine(string.Format("Pay date: {0:MM/dd/yyyy};  Upload date: {1:MM/dd/yyyy};  Audit File Name: {2}", file.Item3, file.Item4, file.Item2)+ Environment.NewLine);
                       
                    }
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine("Please do not reply to this email.");

                    EmailAccess ea = EmailAccess.GetInstance(
                        serverName,
                        portID,
                        useDefaultCredentials
                    );

                    bool success = ea.SendMessage(email_to, email_to_cc, email_to_bcc, email_from, subject, sb.ToString(), false);
                    if (!success)
                    {
                        cs.Status = false;
                        StringBuilder sbError = new StringBuilder();
                        sbError.AppendLine("Error sending notification email of denied CS submission by processor");
                        sbError.AppendLine("  EmailTo: " + string.Join(",", email_to));
                        sbError.AppendLine("  Subject: " + subject);
                        sbError.AppendLine("  Message: " + sb.ToString());
                        throw new Exception(sbError.ToString());
                    }                
                }
            }
            catch (Exception ex)
            {
                cs.Status = false;
                cs.AddMessageDetail(ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace);
                EAMILogger.Instance.Error(ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace);
            }

            return cs;
        }
    }
}

