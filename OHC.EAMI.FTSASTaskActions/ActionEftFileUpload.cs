using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.IO;

using FTSAS.Integration;

using OHC.EAMI.Common;
using OHC.EAMI.DataAccess;
using OHC.EAMI.CommonEntity;
using OHC.EAMI.Util.FileTransfer;

namespace OHC.EAMI.FTSASTaskActions
{
    
    [TaskAction(ActionName= "ActionEftFileUpload", IsPhantom = false)] 
    public class ActionEftFileUpload : TaskActionBase
    {
        public override TaskResult Execute()
        {
            string dataSourceKey = Context.GetExecutionArgTextValueByKey("DATA_SOURCE_KEY");
            string sourceFolderPath = Context.GetExecutionArgTextValueByKey("FTP_FOLDER_SOURCE");
            string destinationFolderPath = Context.GetExecutionArgTextValueByKey("FTP_FOLDER_DESTINATION");

            EAMIDBConnection.EAMIDBContext = dataSourceKey;
            TaskResult result = new TaskResult(true,string.Empty,enProductiveOutcome.NONE);
            
            try
            {
                string scheduledTaskNumber = Context == null ? string.Empty : Context.TaskNumber;

                //GET SCO FILES
                List<string> scoFileList = GetSCOFileList(sourceFolderPath);

                //PROCEED IF ECS EXIST
                if (scoFileList.Count() > 0)
                {

                    // UPLOAD SCO FILES TO SFTP
                    List<Tuple<string, CommonStatus>> sentToSCOfileList = SendFilesToSCO(scoFileList, sourceFolderPath, destinationFolderPath);

                    //Clear upload Folder - delete ecs Files
                    ClearUploadFolder(sourceFolderPath, sentToSCOfileList);

                    //SET TASK result
                    result.Status = sentToSCOfileList.Where(_ => _.Item2.Status == true).Count() > 0;
                   
                    //by default set outcome to FULL
                    result.Outcome = enProductiveOutcome.FULL;
                    
                    if (result.Status && sentToSCOfileList.Where(_ => _.Item2.Status == false).Count() > 0)
                    {
                        //If Status is true - Capture note outcome and set otcome as PARTIAL
                        result.Outcome = enProductiveOutcome.PARTIAL;
                        result.Note = sentToSCOfileList.Where(_ => _.Item2.Status == false).First().Item2.GetFirstDetailMessage();
                    }
                    else if (!result.Status)
                    {
                        //If Status is false - Capture note outcome and set otcome as NONE
                        result.Outcome = enProductiveOutcome.NONE;
                        result.Note = sentToSCOfileList.Where(_ => _.Item2.Status == false).First().Item2.GetFirstDetailMessage();
                    }
                }
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
        
        private List<string> GetSCOFileList(string localFolderPath)
        {
            List<string> ecsFileList = new List<string>();
            ecsFileList.AddRange(Directory.GetFiles(localFolderPath).Select(Path.GetFileName).ToList());
            return ecsFileList;
        }
        
        private List<Tuple<string, CommonStatus>> SendFilesToSCO(List<string> ecsFileList, string localFolderPath, string scoUploadFolderName)
        {            
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
                    File.Delete(string.Format(@"{0}\{1}", localFolderPath, ecsFile.Item1));
                }
            }
            catch
            {
                throw (new Exception(errorMessage));
            }
        }
    }
}

