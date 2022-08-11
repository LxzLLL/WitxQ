using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WitxQ.Common.Logger
{
    public interface ILogger
    {
        /// <summary>
        ///   Log debug message
        /// </summary>
        /// <param name="message"> The debug message </param>
        /// <param name="loggerName">the logger name</param>
        void Debug(string message, string loggerName);

        /// <summary>
        ///   Log debug message
        /// </summary>
        /// <param name="message"> The message </param>
        /// <param name="exception"> Exception to write in debug message </param>
        /// <param name="loggerName">the logger name</param>
        void Debug(string message, Exception exception, string loggerName);

        /// <summary>
        ///   Log FATAL error
        /// </summary>
        /// <param name="message"> The message of fatal error </param>
        /// <param name="loggerName">the logger name</param>
        void Fatal(string message, string loggerName);

        /// <summary>
        ///   log FATAL error
        /// </summary>
        /// <param name="message"> The message of fatal error </param>
        /// <param name="exception"> The exception to write in this fatal message </param>
        /// <param name="loggerName">the logger name</param>
        void Fatal(string message, Exception exception, string loggerName);

        /// <summary>
        ///   Log message information
        /// </summary>
        /// <param name="message"> The information message to write </param>
        /// <param name="loggerName">the logger name</param>
        void Info(string message, string loggerName);

        /// <summary>
        ///   Log warning message
        /// </summary>
        /// <param name="message"> The warning message to write </param>
        /// <param name="loggerName">the logger name</param>
        void Warning(string message, string loggerName);

        /// <summary>
        ///   Log error message
        /// </summary>
        /// <param name="message"> The error message to write </param>
        /// <param name="loggerName">the logger name</param>
        void Error(string message, string loggerName);

        /// <summary>
        ///   Log error message
        /// </summary>
        /// <param name="message"> The error message to write </param>
        /// <param name="exception"> The exception associated with this error </param>
        /// <param name="loggerName">the logger name</param>
        void Error(string message, Exception exception, string loggerName);
    }
}
