using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMI.Util
{
    public class FileDownloadRequest
    {
        public string DownloadFromFolder { get; set; }
        public string DownloadToFolder { get; set; }
        public bool DeleteRemoteFile { get; set; }
        public string AllowedFileExtension { get; set; }
        public string DownloadFileName { get; set; }
    }
}
