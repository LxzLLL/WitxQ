using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WitxQ.Common.Logger
{
    public interface ILoggerFactory
    {
        /// <summary>
        ///  创建logger处理器
        /// </summary>
        /// <returns> The ILog created </returns>
        ILogger Create();
    }
}
