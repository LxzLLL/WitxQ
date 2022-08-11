using System;

namespace WitxQ.Common.Logger
{
    public class Logger4Factory : ILoggerFactory
    {

        public Logger4Factory() { }

        #region ILoggerFactory

        public ILogger Create()
        {
            return new Logger4Helper();
        }

        #endregion
    }
}
