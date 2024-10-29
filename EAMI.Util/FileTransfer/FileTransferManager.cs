using EAMI.Common;
using Microsoft.Extensions.Configuration;

namespace EAMI.Util
{
    public class FileTransferManager : IFileTransferManager
    {
        private SFTPDriver _sftpClient;
        private IConfiguration _configuration;

        public FileTransferManager(IConfiguration configuration, SFTPDriver sftpClient)
        {
            _configuration = configuration;
            _sftpClient = sftpClient;
        }
        public FileTransferManager()
        {

        }

        public void SFTP_SetClient()
        {
            string host = _configuration.GetSection("AppSettings:ftp_host")?.ToString() ?? throw new ArgumentNullException("ftp_host");
            int port = Convert.ToInt32(_configuration.GetSection("AppSettings:ftp_port") ?? throw new ArgumentNullException("ftp_port"));
            string username = _configuration.GetSection("AppSettings:ftp_username")?.ToString() ?? throw new ArgumentNullException("ftp_username");
            string password = _configuration.GetSection("AppSettings:ftp_password")?.ToString() ?? throw new ArgumentNullException("ftp_password");

            _sftpClient = new SFTPDriver(host, port, username, Crypto.ToInsecureString(Crypto.DecryptString(password)));
        }

        public void SFTP_SetClient(string host, int port)
        {
            string username = _configuration.GetSection("AppSettings:ftp_username")?.ToString() ?? throw new ArgumentNullException("ftp_username");
            string password = _configuration.GetSection("AppSettings:ftp_password")?.ToString() ?? throw new ArgumentNullException("ftp_password");

            _sftpClient = new SFTPDriver(host, port, username, Crypto.ToInsecureString(Crypto.DecryptString(password)));
        }

        public void SFTP_SetClient(string host, int port, string username, string password)
        {
            _sftpClient = new SFTPDriver(host, port, username, password);
        }

        public List<FileTransferInformation> SFTP_Download(FileDownloadRequest request)
        {
            try
            {
                return _sftpClient.DownloadFiles(request);
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
                return _sftpClient.DownloadSingleFile(request);
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
                _sftpClient.UploadFile(filesToUpload);
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
                return _sftpClient.ListFiles(remoteFolderPath, allowedFileExtension);
            }
            catch
            {
                throw;
            }
        }
        public void SFTP_MoveFile(string source, string destination, string fileName)
        {
            _sftpClient.MoveFile(source, destination, fileName);
        }
        public CommonStatus FTPS_Upload(string localfilePath, string remotefilePath)
        {
            string host = _configuration.GetSection("AppSettings:ftp_host")?.ToString() ?? throw new ArgumentNullException("ftp_host");
            int port = Convert.ToInt32(_configuration.GetSection("AppSettings:ftp_port") ?? throw new ArgumentNullException("ftp_port"));
            string username = _configuration.GetSection("AppSettings:ftp_username")?.ToString() ?? throw new ArgumentNullException("ftp_username");
            string password = _configuration.GetSection("AppSettings:ftp_password")?.ToString() ?? throw new ArgumentNullException("ftp_password");
            string command = _configuration.GetSection("AppSettings:ftp_command")?.ToString() ?? throw new ArgumentNullException("ftp_command");
            string hostCertificateFingerprint = _configuration.GetSection("AppSettings:ftp_host_fingerprint").ToString() ?? throw new ArgumentNullException("ftp_command");

            return new FTPSDriver().UploadFile(localfilePath, remotefilePath, host, port, username, password, command, hostCertificateFingerprint, true)!;
        }

        public CommonStatus FTPS_Download(string localfilePath, string remotefilePath)
        {
            string host = _configuration.GetSection("AppSettings:ftp_host")?.ToString() ?? throw new ArgumentNullException("ftp_host");
            int port = Convert.ToInt32(_configuration.GetSection("AppSettings:ftp_port") ?? throw new ArgumentNullException("ftp_port"));
            string username = _configuration.GetSection("AppSettings:ftp_username")?.ToString() ?? throw new ArgumentNullException("ftp_username");
            string password = _configuration.GetSection("AppSettings:ftp_password")?.ToString() ?? throw new ArgumentNullException("ftp_password");
            string command = _configuration.GetSection("AppSettings:ftp_command")?.ToString() ?? throw new ArgumentNullException("ftp_command");
            string hostCertificateFingerprint = _configuration.GetSection("AppSettings:ftp_host_fingerprint").ToString() ?? throw new ArgumentNullException("ftp_command");

            return new FTPSDriver().DownloadFile(localfilePath, remotefilePath, host, port, username, password, command, hostCertificateFingerprint)!;
        }
    }
}
