using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;
using OHC.EAMI.Common;

namespace OHC.EAMI.Util.FileTransfer
{
    public class FTPSDriver
    {
        public FTPSDriver()
        {

        }

        public CommonStatus UploadFile(string localfilePath, string remotefilePath, string hostName, int port, string userName, string password, string command, string hostCertificateFingerprint, bool useAsciiMode)
        {
            CommonStatus commonStatus = new CommonStatus(false);
            
            try
            {
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Ftp,
                    HostName = hostName,
                    UserName = userName,
                    Password = password,
                    PortNumber = port,
                    FtpSecure = WinSCP.FtpSecure.Explicit,
                    TlsHostCertificateFingerprint = hostCertificateFingerprint
                };

                using (Session session = new Session())
                {
                    TransferOptions transferOptions = new TransferOptions
                    {
                        TransferMode = useAsciiMode ? TransferMode.Ascii : TransferMode.Automatic
                    };

                    //open session
                    session.Open(sessionOptions);

                    //execute command
                    CommandExecutionResult commandExecutionResult = session.ExecuteCommand(command);

                    if (commandExecutionResult.IsSuccess)
                    {
                        //MOVE FILES
                        TransferOperationResult transferOperationResult = session.PutFiles(string.Format(@"{0}", localfilePath), string.Format(@"/'{0}'", remotefilePath), false, transferOptions);
                        transferOperationResult.Check();

                        if (transferOperationResult.IsSuccess)
                        {
                            //SUCCESS
                            commonStatus.Status = true;
                        }
                        else
                        {
                            //CAPTURE TRANSFER ERRORS
                            commonStatus.AddMessageDetails(transferOperationResult.Failures.Select(_ => _.Message).ToList());
                        }
                    }
                    else
                    {
                        //CAPTURE COMMAND ERRORS
                        commonStatus.AddMessageDetail(commandExecutionResult.ErrorOutput);
                    }
                }
            }
            catch(Exception ex)
            {
                commonStatus.AddMessageDetail(ex.Message);
            }

            return commonStatus;
        }

        public CommonStatus DownloadFile(string localfilePath, string remotefilePath, string hostName, int port, string userName, string password, string command, string hostCertificateFingerprint)
        {
            CommonStatus commonStatus = new CommonStatus(false);
            
            //THE FUNCTIONALITY IS BEING IMPLEMENTED

            return commonStatus;
        }
    }
}
