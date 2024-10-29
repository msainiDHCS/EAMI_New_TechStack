using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace EAMI.Util
{
    public class SFTPDriver
    {
        private SftpClient sc;
        private const string TempExtension = ".tmp";
        private int IsSftpDisposed = 0; //values - 0,1,2

        public SFTPDriver(string hostName, int port, string userId, string password)
        {
            sc = new SftpClient(hostName, port, userId, password);
        }

        ~SFTPDriver()
        {
            Close();
        }

        public List<FileTransferInformation> DownloadFiles(FileDownloadRequest request)
        {
            List<FileTransferInformation> returnData = new List<FileTransferInformation>();
            if (request == null)
            {
                throw new Exception("Empty download request.");
            }

            if (string.IsNullOrEmpty(request.DownloadFromFolder))
            {
                throw new Exception("Unknown remote directory.");
            }

            if (string.IsNullOrEmpty(request.DownloadToFolder))
            {
                throw new Exception("Unknown local directory.");
            }

            if (!Directory.Exists(request.DownloadToFolder))
            {
                Directory.CreateDirectory(request.DownloadToFolder);
            }

            Connect();
            if (!sc.Exists(request.DownloadFromFolder))
            {
                Close();
                throw new Exception("Remote directory does not exist.");
            }

            foreach (SftpFile file in sc.ListDirectory(request.DownloadFromFolder))
            {
                if (file.IsRegularFile)
                {
                    string extension = Path.GetExtension(file.Name);
                    if (string.IsNullOrEmpty(request.AllowedFileExtension) || (!string.IsNullOrEmpty(extension) && extension.Trim().ToLower() == request.AllowedFileExtension.ToLower()))
                    {
                        string outputFileName = Path.Combine(request.DownloadToFolder, file.Name + TempExtension);
                        if (File.Exists(outputFileName) || File.Exists(Path.Combine(request.DownloadToFolder, file.Name)))
                        {
                            outputFileName = Path.Combine(request.DownloadToFolder, Path.GetFileNameWithoutExtension(file.Name) + "_" + Guid.NewGuid() + extension + TempExtension);
                        }
                        try
                        {
                            using (StreamWriter sw = new StreamWriter(outputFileName))
                            {
                                sc.DownloadFile(file.FullName, sw.BaseStream);
                                sw.Close();
                            }

                            System.IO.File.Move(outputFileName, Path.ChangeExtension(outputFileName, null));
                            // if everything succeeds, use the filename without the tmp extension
                            outputFileName = Path.ChangeExtension(outputFileName, null);
                        }
                        catch
                        {
                            //In case of any exception continue with the next file.
                            if (File.Exists(outputFileName))
                            {
                                File.Delete(outputFileName);
                            }
                            continue;
                        }

                        returnData.Add(new FileTransferInformation(file.FullName, file.Length, file.LastWriteTime));
                        returnData[returnData.Count - 1].LocalFileName = Path.GetFileName(outputFileName);
                        returnData[returnData.Count - 1].LocalFolder = request.DownloadToFolder;

                        if (request.DeleteRemoteFile)
                        {
                            sc.DeleteFile(file.FullName);
                        }
                    }
                }
            }
            Close();
            return returnData;
        }

        public List<FileTransferInformation> DownloadSingleFile(FileDownloadRequest request)
        {
            List<FileTransferInformation> returnData = new List<FileTransferInformation>();
            if (request == null)
            {
                throw new Exception("Empty download request.");
            }

            if (string.IsNullOrEmpty(request.DownloadFromFolder))
            {
                throw new Exception("Unknown remote directory.");
            }

            if (string.IsNullOrEmpty(request.DownloadToFolder))
            {
                throw new Exception("Unknown local directory.");
            }

            if (string.IsNullOrEmpty(request.DownloadFileName))
            {
                throw new Exception("Download File Name is not provided.");
            }

            if (!Directory.Exists(request.DownloadToFolder))
            {
                Directory.CreateDirectory(request.DownloadToFolder);
            }

            if (File.Exists(Path.Combine(request.DownloadToFolder, request.DownloadFileName)))
            {
                throw new Exception("File already exists in local directory.");
            }

            Connect();
            if (!sc.Exists(request.DownloadFromFolder))
            {
                Close();
                throw new Exception("Remote directory does not exist.");
            }

            foreach (SftpFile file in sc.ListDirectory(request.DownloadFromFolder))
            {
                if (file.IsRegularFile)
                {
                    if (file.Name.ToLower() == request.DownloadFileName.ToLower())
                    {
                        string outputFileName = Path.Combine(request.DownloadToFolder, file.Name + TempExtension);

                        if (File.Exists(outputFileName))
                        {
                            File.Delete(outputFileName);
                        }

                        try
                        {
                            using (StreamWriter sw = new StreamWriter(outputFileName))
                            {
                                sc.DownloadFile(file.FullName, sw.BaseStream);
                                sw.Close();
                            }

                            System.IO.File.Move(outputFileName, Path.ChangeExtension(outputFileName, null));
                            // if everything succeeds, use the filename without the tmp extension
                            outputFileName = Path.ChangeExtension(outputFileName, null);
                        }
                        catch
                        {
                            if (File.Exists(outputFileName))
                            {
                                File.Delete(outputFileName);
                            }
                        }

                        returnData.Add(new FileTransferInformation(file.FullName, file.Length, file.LastWriteTime));
                        returnData[returnData.Count - 1].LocalFileName = Path.GetFileName(outputFileName);
                        returnData[returnData.Count - 1].LocalFolder = request.DownloadToFolder;

                        if (request.DeleteRemoteFile)
                        {
                            sc.DeleteFile(file.FullName);
                        }

                        break;
                    }
                }
            }
            Close();
            return returnData;
        }

        public void UploadFile(List<FileTransferInformation> filesToUpload)
        {
            if (filesToUpload == null || filesToUpload.Count == 0)
            {
                throw new Exception("No files to upload.");
            }

            Connect();
            foreach (FileTransferInformation file in filesToUpload)
            {
                if (file == null || string.IsNullOrEmpty(file.LocalFolder) || !Directory.Exists(file.LocalFolder))
                {
                    throw new Exception(string.Format("Local folder \"{0}\" is not valid.", file.LocalFolder));
                }

                if (string.IsNullOrEmpty(file.LocalFileName))
                {
                    throw new Exception(string.Format("Local file \"{0}\" is not valid.", file.LocalFileName));
                }

                if (!File.Exists(Path.Combine(file.LocalFolder, Path.GetFileName(file.LocalFileName))))
                {
                    throw new Exception(string.Format("Local file \"{0}\" not found.", Path.Combine(file.LocalFolder, Path.GetFileName(file.LocalFileName))));
                }

                if (string.IsNullOrEmpty(file.RemoteFolder))
                {
                    throw new Exception(string.Format("Remote folder \"{0}\" is not valid.", file.RemoteFolder));
                }

                if (!sc.Exists(file.RemoteFolder))
                {
                    sc.CreateDirectory(file.RemoteFolder);
                }

                if (string.IsNullOrEmpty(file.RemoteFileName))
                {
                    file.RemoteFileName = Path.GetFileName(file.LocalFileName);
                }
            }

            foreach (FileTransferInformation file in filesToUpload)
            {
                string localfilePath = Path.Combine(file.LocalFolder, Path.GetFileName(file.LocalFileName));
                string remotefilePath = string.Format("{0}/{1}", file.RemoteFolder.TrimEnd('/'), Path.GetFileName(file.RemoteFileName));

                try
                {
                    using (StreamReader sr = new StreamReader(localfilePath))
                    {
                        sc.UploadFile(sr.BaseStream, remotefilePath);
                        sr.Close();
                    }

                    file.UploadDate = DateTime.Now;
                }
                catch (Exception ex)
                {
                    file.UploadDate = DateTime.MinValue;
                    file.UploadStatus = ex.Message;
                }
            }
            Close();
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        private void Connect()
        {
            IsSftpDisposed = 0;
            if (sc != null && !sc.IsConnected)
            {
                sc.Connect();
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        private void Close()
        {
            if (IsSftpDisposed == 0)
            {
                int status = Interlocked.CompareExchange(ref IsSftpDisposed, 1, 0);

                if (status == 0) //this is the first thread that is setting the value
                {
                    if (sc != null && sc.IsConnected)
                    {
                        sc.Disconnect();
                    }

                    IsSftpDisposed = 2;
                }
            }
        }

        private void DownloadFile()
        {

        }

        public List<FTPFile> ListFiles(string remoteFolderPath, string allowedFileExtension)
        {
            List<FTPFile> returnData = new List<FTPFile>();

            if (string.IsNullOrEmpty(remoteFolderPath))
            {
                throw new Exception("Unknown remote directory.");
            }

            Connect();
            if (!sc.Exists(remoteFolderPath))
            {
                Close();
                throw new Exception("Remote directory does not exist.");
            }

            IEnumerable<SftpFile> sftpFileList = sc.ListDirectory(remoteFolderPath);

            foreach (SftpFile file in sftpFileList)
            {
                if (file.IsRegularFile)
                {
                    string extension = Path.GetExtension(file.Name);

                    if (string.IsNullOrEmpty(allowedFileExtension) || (!string.IsNullOrEmpty(extension) && extension.Trim().ToLower() == allowedFileExtension.ToLower()))
                    {
                        returnData.Add(new FTPFile { FileName = file.Name, FileSize = file.Attributes.Size });
                    }
                }
            }

            Close();
            return returnData;
        }

        public void MoveFile(string source, string destination, string fileName)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new Exception("Unknown source directory.");
            }

            if (string.IsNullOrEmpty(destination))
            {
                throw new Exception("Unknown destination directory.");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new Exception("Empty File Name.");
            }

            Connect();

            if (!sc.Exists(Path.Combine(source, fileName)))
            {
                Close();
                throw new Exception("File does not exist at source directory.");
            }

            if (sc.Exists(Path.Combine(destination, fileName)))
            {
                Close();
                throw new Exception("File already exists at destination directory.");
            }

            try
            {
                SftpFile file = sc.Get(Path.Combine(source, fileName));
                file.MoveTo(Path.Combine(destination, fileName));
                Close();
            }
            catch (Exception ex)
            {
                Close();
                throw new Exception(string.Format("Error moving sftp file from {0} to {1}. {2} ", Path.Combine(source, fileName), Path.Combine(destination, fileName), ex.Message));
            }
        }
    }
}
