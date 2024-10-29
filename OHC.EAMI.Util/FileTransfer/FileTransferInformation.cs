using System;
using System.IO;

namespace OHC.EAMI.Util.FileTransfer
{
    public class FileTransferInformation
    {
        public string LocalFileName { get; set; }
        public string LocalFolder { get; set; }
        public long FileSize { get; set; }

        public string RemoteFileName { get; set; }
        public string RemoteFolder { get; set; }

        public DateTime UploadDate { get; set; }
        public string UploadStatus { get; set; }

        public FileTransferInformation(string localFolder, string localFile, string remoteFolder)
        {
            LocalFolder = localFolder;
            LocalFileName = localFile;
            RemoteFolder = remoteFolder;
        }

        public FileTransferInformation(string remoteFileName, long size, DateTime lastModifiedDate)
        {
            if (!string.IsNullOrEmpty(remoteFileName))
            {
                RemoteFileName = Path.GetFileName(remoteFileName);
                RemoteFolder = Path.GetDirectoryName(remoteFileName);
            }
            FileSize = size;
            UploadDate = lastModifiedDate;
        }
    }
}
