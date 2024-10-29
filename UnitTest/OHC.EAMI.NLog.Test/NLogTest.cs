using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using System;
using System.IO;

namespace OHC.EAMI.NLog.Test
{
    [TestClass]
    public class NLogTest
    {
        private enum LogFileType
        {
            Main ,
            Error,
            Trace 
        }

        [TestMethod]
        public void NLog_LogInfoTest()
        {
            //arrange
            Logger logger = LogManager.GetCurrentClassLogger();
            string messageKey = Guid.NewGuid().ToString();
            string logFile = GetMainFilePath(LogFileType.Main);
            string logLevel = LogLevel.Info.ToString().ToUpper();

            //act
            logger.Info(string.Format("Unit test log message. key={0}", messageKey));

            //assert
            Assert.IsTrue(LogFileExistinLogsFolder(logFile));
            Assert.IsTrue(LogEntryExistInLogFile(logFile, logLevel, messageKey));
        }

        [TestMethod]
        public void NLog_LogDebugTest()
        { 
            //arrange
            Logger logger = LogManager.GetCurrentClassLogger();
            string messageKey = Guid.NewGuid().ToString();
            string logFile = GetMainFilePath(LogFileType.Main);
            string logLevel = LogLevel.Debug.ToString().ToUpper();

            //act
            logger.Debug(string.Format("Unit test log message. key={0}", messageKey));

            //assert
            Assert.IsTrue(LogFileExistinLogsFolder(logFile));
            Assert.IsTrue(LogEntryExistInLogFile(logFile, logLevel, messageKey));
        }

        [TestMethod]
        public void NLog_LogErrorTest()
        {
            //arrange
            Logger logger = LogManager.GetCurrentClassLogger();
            string messageKey = Guid.NewGuid().ToString();
            string logFile = GetMainFilePath(LogFileType.Error);
            string logLevel = LogLevel.Error.ToString().ToUpper();
            
            //act
            try
            {
                throw new Exception(string.Format("Unit test log message. key={0}", messageKey));
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }

            //assert
            Assert.IsTrue(LogFileExistinLogsFolder(logFile));
            Assert.IsTrue(LogEntryExistInLogFile(logFile, logLevel, messageKey));
        }

        [TestMethod]
        public void NLog_LogWarnTest()
        {
            //arrange
            Logger logger = LogManager.GetCurrentClassLogger();
            string messageKey = Guid.NewGuid().ToString();
            string logFile = GetMainFilePath(LogFileType.Main);
            string logLevel = LogLevel.Warn.ToString().ToUpper();
            
            //act
            logger.Warn(string.Format("Unit test log message. key={0}", messageKey));

            //assert
            Assert.IsTrue(LogFileExistinLogsFolder(logFile));
            Assert.IsTrue(LogEntryExistInLogFile(logFile, logLevel, messageKey));
        }

        [TestMethod]
        public void NLog_LogTraceTest()
        {
            //arrange
            Logger logger = LogManager.GetCurrentClassLogger();
            string messageKey = Guid.NewGuid().ToString();
            string logFileTrace = GetMainFilePath(LogFileType.Trace);
            string logLevel = LogLevel.Trace.ToString().ToUpper(); 

            //act
            logger.Trace(string.Format("Unit test log message. key={0}", messageKey));

            //assert
            Assert.IsTrue(LogFileExistinLogsFolder(logFileTrace));
            Assert.IsTrue(LogEntryExistInLogFile(logFileTrace, logLevel, messageKey));
        }

        [TestMethod]
        public void NLog_LogTraceInTwoTargetLogsTest()
        {
            //arrange
            Logger logger = LogManager.GetCurrentClassLogger();
            string messageKey = Guid.NewGuid().ToString();
            string logFileTrace = GetMainFilePath(LogFileType.Trace);
            string logFileMain = GetMainFilePath(LogFileType.Main);
            string logLevel = LogLevel.Trace.ToString().ToUpper();

            //act
            logger.Trace(string.Format("Unit test log message. key={0}", messageKey));

            //assert
            Assert.IsTrue(LogEntryExistInLogFile(logFileTrace, logLevel, messageKey) 
                && LogEntryExistInLogFile(logFileMain, logLevel, messageKey));
        }
        
        [TestMethod]
        public void NLog_LogTraceDoesNotExistInErrorLogTest()
        {
            //arrange
            Logger logger = LogManager.GetCurrentClassLogger();
            string messageKey = Guid.NewGuid().ToString();
            string logFileMain = GetMainFilePath(LogFileType.Main);
            string logFileError = GetMainFilePath(LogFileType.Error);
            string logLevel = LogLevel.Trace.ToString().ToUpper();

            //act
            logger.Trace(string.Format("Unit test log message. key={0}", messageKey));

            //assert
            Assert.IsTrue(!LogEntryExistInLogFile(logFileError, logLevel, messageKey) && LogEntryExistInLogFile(logFileMain, logLevel, messageKey));
        }

        [TestMethod]
        public void NLog_LogFatalTest()
        {
            //arrange
            Logger logger = LogManager.GetCurrentClassLogger();
            string messageKey = Guid.NewGuid().ToString();
            string logFile = GetMainFilePath(LogFileType.Error);
            string logLevel = LogLevel.Fatal.ToString().ToUpper(); 

            //act
            logger.Fatal(string.Format("Unit test log message. key={0}", messageKey));

            //assert
            Assert.IsTrue(LogFileExistinLogsFolder(logFile));
            Assert.IsTrue(LogEntryExistInLogFile(logFile, logLevel, messageKey));
        }

        private string GetMainFilePath(LogFileType logFileType)
        {
            string fileType = string.Empty;
            string dateFormatted = DateTime.Now.ToString("yyyy.MM.dd");
            
            if(logFileType == LogFileType.Error)
            {
                fileType = "error.";
            }
            else if(logFileType == LogFileType.Trace)
            {
                fileType = "nLogTrace.";
            }

            return string.Format("{0}\\logs\\EAMI.{1}.{2}log", AppDomain.CurrentDomain.BaseDirectory, dateFormatted, fileType);
        }

        private bool LogFileExistinLogsFolder(string logFilePath)
        {
            return File.Exists(logFilePath);
        }

        private bool LogEntryExistInLogFile(string logFilePath, string logLevel, string logKey)
        {
            using (var reader = new StreamReader(logFilePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.Contains(logLevel) && line.Contains(logKey))
                        {
                            return true;
                        }
                }
                return false;
            }
        }

    }
}
