using EAMI.Common;
using System;
using System.Collections.Generic;

namespace EAMI.Util
{
    public interface IFileTransferManager
    {
        void SFTP_SetClient();
        void SFTP_SetClient(string host, int port);
        void SFTP_SetClient(string host, int port, string username, string password);
        void SFTP_Upload(List<FileTransferInformation> files);
        void SFTP_MoveFile(string source, string destination, string fileName);
        List<FTPFile> SFTP_ListFiles(string remoteFolderPath, string allowedFileExtension);
        List<FileTransferInformation> SFTP_Download(FileDownloadRequest request);
        List<FileTransferInformation> SFTP_Download_SingleFile(FileDownloadRequest request);
        CommonStatus FTPS_Upload(string localfilePath, string remotefilePath);
        CommonStatus FTPS_Download(string localfilePath, string remotefilePath);
    }
}
