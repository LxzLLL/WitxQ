using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WitxQ.Common
{
    /// <summary>
    /// 通过映射生成实体帮助类
    /// </summary>
    public class MapHelper
    {
        /// <summary>
        /// 通过映射生成实例
        /// </summary>
        /// <typeparam name="T">源类</typeparam>
        /// <typeparam name="U">目标类</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="target">目标实例</param>
        /// <returns></returns>
        public static U Map<T, U>(T source, U target) where U:new ()
        {
            if (target == null)
            {
                target = new U();
            }
            foreach (var i in typeof(U).GetProperties())
            {
                if (typeof(T).GetProperty(i.Name) != null)
                {
                    var item = typeof(T).GetProperty(i.Name).GetValue(source);
                    typeof(U).GetProperty(i.Name).SetValue(target, item);
                }
            }
            return target;
        }
    }
}
