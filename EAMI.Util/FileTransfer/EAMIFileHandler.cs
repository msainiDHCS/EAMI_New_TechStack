using EAMI.Common;
using System.Configuration;

namespace EAMI.Util
{
    public class EAMIFileHandler
    {
        #region Local Variables

        private string _host;
        private int _port;
        private string _username;
        private string _password;
        private bool _holdFiles;
        private List<string> _fileNameList;

        #endregion

        #region Constructors

        public EAMIFileHandler(string host, int port, string username, string password, bool holdFiles)
        {
            _host = host;
            _port = port;
            _username = username;
            _password = password;
            _holdFiles = holdFiles;

            //initialize list
            FileStringList = new List<string>();
        }

        #endregion

        #region Properties

        public List<string> FileStringList { get; set; }

        #endregion

        #region Public Methods

        public CommonStatusPayload<List<string>> UploadToSFTP(string remoteUploadFolder, string fileNameFormat)
        {
            CommonStatus cs = new CommonStatus(true);

            //compose local upload folder path
            string localUploadFolder = string.Format(@"{0}\{1}\", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ftp_localUploadFolderName"].ToString());
            
            //determine if copy only
            bool copyOnly = false;
            bool.TryParse(ConfigurationManager.AppSettings["ftp_copy_only"], out copyOnly);

            CreateLocalFiles(localUploadFolder, fileNameFormat, cs);

            if (cs.Status)
            {
                if (!_holdFiles)
                {
                    UploadFiles(localUploadFolder, remoteUploadFolder, cs);

                    //Determine if to hold files
                    if (!copyOnly)
                    {
                        foreach (string fileName in _fileNameList)
                        {
                            //delete files that were transfered
                            File.Delete(string.Format("{0}{1}", localUploadFolder, fileName));
                        }
                    }
                }
            }

            //return payload
            return new CommonStatusPayload<List<string>>(_fileNameList, cs.Status, cs.MessageDetailList);
        }

        #endregion

        #region Private Methods

        private void UploadFiles(string localUploadFolder, string remoteUploadFolder, CommonStatus cs)
        {
            try
            {
                //Establish connection to ftp host
                FileTransferManager fileTransferManager = new FileTransferManager();
                fileTransferManager.SFTP_SetClient();

                List<FileTransferInformation> fileTransferInformationList = new List<FileTransferInformation>();

                //Specify file(s) to upload
                foreach (string fileName in _fileNameList)
                {
                    fileTransferInformationList.Add(new FileTransferInformation(localUploadFolder, fileName, remoteUploadFolder));
                }

                //Upload
                fileTransferManager.SFTP_Upload(fileTransferInformationList);
            }
            catch (Exception ex)
            {
                cs.Status = false;
                cs.MessageDetailList.Add(ex.Message);
            }
        }

        private void CreateLocalFiles(string localUploadFolder, string fileNameFormat, CommonStatus cs)
        {
            //handle upload folder
            HandleLocalUploadFolder(localUploadFolder, cs);

            if (cs.Status) 
            {
                _fileNameList = new List<string>();

                //create file for each file string
                foreach (string fileString in FileStringList)
                {
                    //compose file name
                    string fileName = string.Format(fileNameFormat, DateTime.Now, Guid.NewGuid().ToString());

                    try
                    {
                        //Create test file for upload   
                        using (StreamWriter sw = new StreamWriter(string.Format("{0}{1}", localUploadFolder, fileName)))
                        {
                            //write string
                            sw.Write(fileString);
                            sw.Flush();
                        }

                        //capture the name of succesfully created file
                        _fileNameList.Add(fileName);
                    }
                    catch (Exception ex)
                    {
                        cs.Status = false;
                        cs.MessageDetailList.Add(string.Format("Error with file name: {0}; {1}", fileName, ex.Message));
                    }
                }
            }
        }

        private void HandleLocalUploadFolder(string localUploadFolder, CommonStatus cs)
        {
            try
            {
                //check if directory exist
                if (!Directory.Exists(localUploadFolder))
                {
                    //create directory if one doesn't exist
                    CreateUploadFolder(localUploadFolder, cs);
                }
            }
            catch (Exception ex)
            {
                cs.Status = false;
                cs.MessageDetailList.Add(string.Format("Error accessing folder: {0}; {1}", localUploadFolder, ex.Message));
            }
        }

        private void CreateUploadFolder(string localUploadFolder, CommonStatus cs)
        {
            try
            {
                //create directory 
                Directory.CreateDirectory(localUploadFolder);
            }
            catch (Exception ex)
            {
                cs.Status = false;
                cs.MessageDetailList.Add(string.Format("Error creating folder: {0}; {1}", localUploadFolder, ex.Message));
            }
        }


        #endregion
    }
}
