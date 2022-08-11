using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Server.SysFrame
{
    /// <summary>
    /// 服务加载器，用于获取注入容器中的服务实例
    /// </summary>
    public static class ServiceLocator
    {
        public static IServiceProvider Instance { get; set; }
    }
}
