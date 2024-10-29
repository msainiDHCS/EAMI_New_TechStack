using OHC.EAMI.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.EAMI.Util.FileTransfer
{
    public class FileTransferManager : IFileTransferManager
    {
        private SFTPDriver SftpClient;

        public FileTransferManager()
        {
        }

        public void SFTP_SetClient()
        {
            string host = ConfigurationManager.AppSettings["ftp_host"].ToString();
            int port = int.Parse(ConfigurationManager.AppSettings["ftp_port"]);
            string username = ConfigurationManager.AppSettings["ftp_username"].ToString();
            string password = ConfigurationManager.AppSettings["ftp_password"].ToString();

            SftpClient = new SFTPDriver(host, port, username, Crypto.ToInsecureString(Crypto.DecryptString(password)));
        }

        public void SFTP_SetClient(string host, int port)
        {
            string username = ConfigurationManager.AppSettings["ftp_username"].ToString();
            string password = ConfigurationManager.AppSettings["ftp_password"].ToString();

            SftpClient = new SFTPDriver(host, port, username, Crypto.ToInsecureString(Crypto.DecryptString(password)));
        }

        public void SFTP_SetClient(string host, int port, string username, string password)
        {
            SftpClient = new SFTPDriver(host, port, username, password);
        }

        public List<FileTransferInformation> SFTP_Download(FileDownloadRequest request)
        {
            try
            {
                return SftpClient.DownloadFiles(request);
            }
            catch
            {
                throw;
            }
        }

        public List<FileTransferInformation> SFTP_Download_SingleFile(FileDownloadRequest request)
        {
            try
            {
                return SftpClient.DownloadSingleFile(request);
            }
            catch
            {
                throw;
            }
        }

        public void SFTP_Upload(List<FileTransferInformation> filesToUpload)
        {
            try
            {
                SftpClient.UploadFile(filesToUpload);
            }
            catch
            {
                throw;
            }
        }

        public List<FTPFile> SFTP_ListFiles(string remoteFolderPath, string allowedFileExtension)
        {
            try
            {
                return SftpClient.ListFiles(remoteFolderPath, allowedFileExtension);
            }
            catch
            {
                throw;
            }
        }
        public void SFTP_MoveFile(string source, string destination, string fileName)
        {
            SftpClient.MoveFile(source, destination, fileName);
        }
        public CommonStatus FTPS_Upload(string localfilePath, string remotefilePath)
        {
            string host = ConfigurationManager.AppSettings["ftp_host"].ToString();
            int port = int.Parse(ConfigurationManager.AppSettings["ftp_port"]);
            string username = ConfigurationManager.AppSettings["ftp_username"].ToString();
            string password = ConfigurationManager.AppSettings["ftp_password"].ToString();
            string command = ConfigurationManager.AppSettings["ftp_command"].ToString();
            string hostCertificateFingerprint = ConfigurationManager.AppSettings["ftp_host_fingerprint"].ToString();

            return new FTPSDriver().UploadFile(localfilePath, remotefilePath, host, port, username, password, command, hostCertificateFingerprint, true);
        }

        public CommonStatus FTPS_Download(string localfilePath, string remotefilePath)
        {
            string host = ConfigurationManager.AppSettings["ftp_host"].ToString();
            int port = int.Parse(ConfigurationManager.AppSettings["ftp_port"]);
            string username = ConfigurationManager.AppSettings["ftp_username"].ToString();
            string password = ConfigurationManager.AppSettings["ftp_password"].ToString();
            string command = ConfigurationManager.AppSettings["ftp_command"].ToString();
            string hostCertificateFingerprint = ConfigurationManager.AppSettings["ftp_host_fingerprint"].ToString();

            return new FTPSDriver().DownloadFile(localfilePath, remotefilePath, host, port, username, password, command, hostCertificateFingerprint);
        }
    }
}
