using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Ninject;

namespace OHC.EAMI.Common
{
    public interface ILoggingService
    {
        bool IsDebugEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsTraceEnabled { get; }
        bool IsWarnEnabled { get; }


        void Debug(Exception exception);
        void Debug(IFormatProvider provider, string format, params object[] args);
        void Debug(string message);
        void Debug(Exception exception, string format, params object[] args);

        void Error(Exception exception);
        void Error(IFormatProvider provider, string format, params object[] args);
        void Error(string message);
        void Error(Exception exception, string format, params object[] args);

        void Fatal(Exception exception);
        void Fatal(IFormatProvider provider, string format, params object[] args);
        void Fatal(string message);
        void Fatal(Exception exception, string format, params object[] args);

        void Info(Exception exception);
        void Info(IFormatProvider provider, string format, params object[] args);
        void Info(string message);
        void Info(Exception exception, string format, params object[] args);

        void Trace(Exception exception);
        void Trace(IFormatProvider provider, string format, params object[] args);
        void Trace(string message);
        void Trace(Exception exception, string format, params object[] args);

        void Warn(Exception exception);
        void Warn(IFormatProvider provider, string format, params object[] args);
        void Warn(string message);
        void Warn(Exception exception, string format, params object[] args);

    }

    public sealed class EAMILogger
    {
        private static volatile ILoggingService instance;
        private static object syncRoot = new Object();
        private EAMILogger()
        {
        }

        private static IKernel _ninjectKernel;
        public static ILoggingService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            _ninjectKernel = new StandardKernel(new LoggingModule());
                            instance = _ninjectKernel.Get<ILoggingService>();
                        }
                    }
                }

                return instance;
            }
        }
    }

    public class LoggingModule : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            ILoggingService logger = LoggingService.GetLoggingService();
            Bind<ILoggingService>().ToConstant(logger);
        }

        private class LoggingService : NLog.Logger, ILoggingService
        {
            private const string _loggerName = "EAMINLogLogger";

            public static ILoggingService GetLoggingService()
            {
                ILoggingService logger = (ILoggingService)LogManager.GetLogger("EAMINLogLogger", typeof(LoggingService));
                return logger;
            }

            public void Debug(Exception exception, string format, params object[] args)
            {
                if (!base.IsDebugEnabled) return;
                var logEvent = GetLogEvent(_loggerName, LogLevel.Debug, exception, string.Format(format, args));
                base.Log(typeof(LoggingService), logEvent);
            }

            public void Error(Exception exception, string format, params object[] args)
            {
                if (!base.IsErrorEnabled) return;
                var logEvent = GetLogEvent(_loggerName, LogLevel.Error, exception, string.Format(format, args));
                base.Log(typeof(LoggingService), logEvent);
            }

            public void Fatal(Exception exception, string format, params object[] args)
            {
                if (!base.IsFatalEnabled) return;
                var logEvent = GetLogEvent(_loggerName, LogLevel.Fatal, exception, string.Format(format, args));
                base.Log(typeof(LoggingService), logEvent);
            }

            public void Info(Exception exception, string format, params object[] args)
            {
                if (!base.IsInfoEnabled) return;
                var logEvent = GetLogEvent(_loggerName, LogLevel.Info, exception, string.Format(format, args));
                base.Log(typeof(LoggingService), logEvent);
            }

            public void Trace(Exception exception, string format, params object[] args)
            {
                if (!base.IsTraceEnabled) return;
                var logEvent = GetLogEvent(_loggerName, LogLevel.Trace, exception, string.Format(format, args));
                base.Log(typeof(LoggingService), logEvent);
            }

            public void Warn(Exception exception, string format, params object[] args)
            {
                if (!base.IsWarnEnabled) return;
                var logEvent = GetLogEvent(_loggerName, LogLevel.Warn, exception, string.Format(format, args));
                base.Log(typeof(LoggingService), logEvent);
            }

            public void Debug(Exception exception)
            {
                this.Debug(exception, string.Empty);
            }

            public void Error(Exception exception)
            {
                this.Error(exception, string.Empty);
            }

            public void Fatal(Exception exception)
            {
                this.Fatal(exception, string.Empty);
            }

            public void Info(Exception exception)
            {
                this.Info(exception, string.Empty);
            }

            public void Trace(Exception exception)
            {
                this.Trace(exception, string.Empty);
            }

            public void Warn(Exception exception)
            {
                this.Warn(exception, string.Empty);
            }

            public void Debug(IFormatProvider provider, string format, params object[] args)
            {
                if (!base.IsDebugEnabled) return;
                var logEvent = GetLogEvent(_loggerName, LogLevel.Debug, null, string.Format(provider, format, args));
                base.Log(typeof(LoggingService), logEvent);
            }

            public void Error(IFormatProvider provider, string format, params object[] args)
            {
                if (!base.IsErrorEnabled) return;
                var logEvent = GetLogEvent(_loggerName, LogLevel.Error, null, string.Format(provider, format, args));
                base.Log(typeof(LoggingService), logEvent);
            }

            public void Fatal(IFormatProvider provider, string format, params object[] args)
            {
                if (!base.IsFatalEnabled) return;
                var logEvent = GetLogEvent(_loggerName, LogLevel.Fatal, null, string.Format(provider, format, args));
                base.Log(typeof(LoggingService), logEvent);
            }

            public void Info(IFormatProvider provider, string format, params object[] args)
            {
                if (!base.IsInfoEnabled) return;
                var logEvent = GetLogEvent(_loggerName, LogLevel.Info, null, string.Format(provider, format, args));
                base.Log(typeof(LoggingService), logEvent);
            }

            public void Trace(IFormatProvider provider, string format, params object[] args)
            {
                if (!base.IsTraceEnabled) return;
                var logEvent = GetLogEvent(_loggerName, LogLevel.Trace, null, string.Format(provider, format, args));
                base.Log(typeof(LoggingService), logEvent);
            }

            public void Warn(IFormatProvider provider, string format, params object[] args)
            {
                if (!base.IsWarnEnabled) return;
                var logEvent = GetLogEvent(_loggerName, LogLevel.Warn, null, string.Format(provider, format, args));
                base.Log(typeof(LoggingService), logEvent);
            }

            public void Debug(string message)
            {
                if (!base.IsDebugEnabled) return;
                var logEvent = GetLogEvent(_loggerName, LogLevel.Debug, null, message);
                base.Log(typeof(LoggingService), logEvent);
            }

            public void Error(string message)
            {
                if (!base.IsErrorEnabled) return;
                var logEvent = GetLogEvent(_loggerName, LogLevel.Error, null, message);
                base.Log(typeof(LoggingService), logEvent);
            }

            public void Fatal(string message)
            {
                if (!base.IsFatalEnabled) return;
                var logEvent = GetLogEvent(_loggerName, LogLevel.Fatal, null, message);
                base.Log(typeof(LoggingService), logEvent);
            }

            public void Info(string message)
            {
                if (!base.IsInfoEnabled) return;
                var logEvent = GetLogEvent(_loggerName, LogLevel.Info, null, message);
                base.Log(typeof(LoggingService), logEvent);
            }

            public void Trace(string message)
            {
                if (!base.IsTraceEnabled) return;
                var logEvent = GetLogEvent(_loggerName, LogLevel.Trace, null, message);
                base.Log(typeof(LoggingService), logEvent);
            }

            public void Warn(string message)
            {
                if (!base.IsWarnEnabled) return;
                var logEvent = GetLogEvent(_loggerName, LogLevel.Warn, null, message);
                base.Log(typeof(LoggingService), logEvent);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="message">Assumes message will nt be used if Exception is present</param>
            /// <param name="loggerName"></param>
            /// <param name="level"></param>
            /// <param name="exception"></param>
            /// <param name="format"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            private LogEventInfo GetLogEvent(string loggerName, LogLevel level, Exception exception, string formattedMessage)
            {
                string assemblyProp = string.Empty;
                string classProp = string.Empty;
                string methodProp = string.Empty;
                string innerMessageProp = string.Empty;
                string messageProp = formattedMessage;//by default, but can be overriden by exception message

                var logEvent = new LogEventInfo(level, loggerName, formattedMessage);

                if (exception != null)
                {
                    logEvent.Exception = exception;

                    assemblyProp = exception.Source;
                    classProp = exception.TargetSite.DeclaringType.FullName;
                    methodProp = exception.TargetSite.Name;
                    messageProp = exception.Message;

                    if (exception.InnerException != null)
                    {
                        innerMessageProp = exception.InnerException.Message;
                    }
                }

                logEvent.Properties["error-source"] = assemblyProp;
                logEvent.Properties["error-class"] = classProp;
                logEvent.Properties["error-method"] = methodProp;
                logEvent.Properties["inner-error-message"] = innerMessageProp;
                logEvent.Properties["error-message"] = messageProp;

                logEvent.Message = messageProp;

                return logEvent;
            }
        }
    }
}
