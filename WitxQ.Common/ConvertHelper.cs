using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WitxQ.Common
{
    /// <summary>
    /// 类型转换公用类
    /// </summary>
    public static class ConvertHelper
    {
        #region 实体

        /// <summary>
        /// 实体转List
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T2> ConvertToList<T1, T2>(List<T1> source)
        {
            List<T2> t2List = new List<T2>();
            if (source == null || source.Count == 0)
                return t2List;

            T2 model = default(T2);
            PropertyInfo[] pi = typeof(T2).GetProperties();
            PropertyInfo[] pi1 = typeof(T1).GetProperties();
            foreach (T1 t1Model in source)
            {
                model = Activator.CreateInstance<T2>();
                foreach (var p in pi)
                {
                    foreach (var p1 in pi1)
                    {
                        if (p.Name == p1.Name)
                        {
                            p.SetValue(model, p1.GetValue(t1Model, null), null);
                            break;
                        }
                    }
                }
                t2List.Add(model);
            }
            return t2List;
        }

        /// <summary>
        /// 实体间转换
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T2 ConvertToModel<T1, T2>(T1 source)
        {
            T2 model = default(T2);
            PropertyInfo[] pi = typeof(T2).GetProperties();
            PropertyInfo[] pi1 = typeof(T1).GetProperties();
            model = Activator.CreateInstance<T2>();
            foreach (var p in pi)
            {
                foreach (var p1 in pi1)
                {
                    if (p.Name == p1.Name)
                    {
                        p.SetValue(model, p1.GetValue(source, null), null);
                        break;
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// 从实体转为key和value都为string类型的字典
        /// </summary>
        /// <param name="data">实体</param>
        /// <returns></returns>
        public static Dictionary<string, string> ConvertModelToDic<T>(T data)
            where T:class
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (PropertyInfo pro in typeof(T).GetProperties())
            {
                string value = pro.GetValue(data, null)?.ToString();
                string name = pro.Name;
                if (string.IsNullOrEmpty(value)) continue;
                if (name == "Type") continue;
                dic.Add(name, value);
            }
            return dic;
        }

        #endregion

        #region 基本类型转换

        /// <summary>
        /// 字符串转Double
        /// </summary>
        /// <param name="strNumber"></param>
        /// <param name="def">默认值0.0d</param>
        /// <returns></returns>
        public static double StringToDouble(string strNumber, double def = 0.0d)
        {
            double d;
            if (!double.TryParse(strNumber, out d))
            {
                d = def;
            }
            return d;
        }

        /// <summary>
        /// 字符串转Int
        /// </summary>
        /// <param name="strNumber"></param>
        /// <param name="def">默认值0</param>
        /// <returns></returns>
        public static int StringToInt(string strNumber, int def = 0)
        {
            int d;
            if (!int.TryParse(strNumber, out d))
            {
                d = def;
            }
            return d;
        }

        /// <summary>
        /// 字符串转long
        /// </summary>
        /// <param name="strNumber"></param>
        /// <param name="def">默认值0</param>
        /// <returns></returns>
        public static long StringToLong(string strNumber, long def = 0)
        {
            long d;
            if (!long.TryParse(strNumber, out d))
            {
                d = def;
            }
            return d;
        }

        /// <summary>
        /// 字符串转decimal
        /// </summary>
        /// <param name="strNumber"></param>
        /// <param name="def">默认值0.0M</param>
        /// <returns></returns>
        public static decimal StringToDecimal(string strNumber, decimal def = 0.0M)
        {
            decimal d;
            if (!decimal.TryParse(strNumber, out d))
            {
                d = def;
            }
            return d;
        }

        /// <summary>
        /// Decimal截取位数，不进行四舍五入
        /// </summary>
        /// <param name="d"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static decimal CutDecimalWithN(decimal d, int n)
        {
            string strDecimal = d.ToString();
            int index = strDecimal.IndexOf(".");
            if (index == -1 || strDecimal.Length < index + n + 1)
            {
                strDecimal = string.Format("{0:F" + n + "}", d);
            }
            else
            {
                int length = index;
                if (n != 0)
                {
                    length = index + n + 1;
                }
                strDecimal = strDecimal.Substring(0, length);
            }
            return Decimal.Parse(strDecimal);
        }


        #endregion

        #region 时间戳
        /// <summary>
        /// 将Unix时间戳转换为dateTime格式
        /// </summary>
        /// <param name="ticks">从utc基准时间（1970-01-01）以来的毫秒数</param>
        /// <returns></returns>
        public static DateTime UnixTimeToDateTime(long ticks)
        {
            DateTime dt;
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = start.AddMilliseconds(ticks).ToLocalTime();

            return dt;
        }


        /// <summary>
        /// 将dateTime格式转换为Unix时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>返回utc基准时间（1970-01-01）以来的秒数</returns>
        public static long DateTimeToUnixTime(DateTime dateTime)
        {
            return (long)(dateTime - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        #endregion


        #region 进制转换

        /// <summary>
        /// 16进制字符集
        /// </summary>
        public static string CHS_STR16 = "0123456789abcdef";

        /// <summary>
        /// 10进制字符集
        /// </summary>
        public static string CHS_STR10 = "0123456789";

        /// <summary>
        /// 将一个大数字符串从M进制转换成N进制
        /// </summary>
        /// <param name="sourceValue">M进制数字字符串</param>
        /// <param name="sourceBaseChars">M进制字符集（例如：CHS_STR16）</param>
        /// <param name="newBaseChars">N进制字符集（例如：CHS_STR16）</param>
        /// <returns>N进制数字字符串</returns>
        public static string BaseConvert(string sourceValue, string sourceBaseChars, string newBaseChars)
        {
            //M进制
            var sBase = sourceBaseChars.Length;
            //N进制
            var tBase = newBaseChars.Length;
            //M进制数字字符串合法性判断（判断M进制数字字符串中是否有不包含在M进制字符集中的字符）
            if (sourceValue.Any(s => !sourceBaseChars.Contains(s))) return null;

            //将M进制数字字符串的每一位字符转为十进制数字依次存入到LIST中
            var intSource = new List<int>();
            intSource.AddRange(sourceValue.Select(c => sourceBaseChars.IndexOf(c)));

            //余数列表
            var res = new List<int>();

            //开始转换（判断十进制LIST是否为空或只剩一位且这个数字小于N进制）
            while (!((intSource.Count == 1 && intSource[0] < tBase) || intSource.Count == 0))
            {

                //每一轮的商值集合
                var ans = new List<int>();

                var y = 0;
                //十进制LIST中的数字逐一除以N进制（注意：需要加上上一位计算后的余数乘以M进制）
                foreach (var t in intSource)
                {
                    //当前位的数值加上上一位计算后的余数乘以M进制
                    y = y * sBase + t;
                    //保存当前位与N进制的商值
                    ans.Add(y / tBase);
                    //计算余值
                    y %= tBase;
                }
                //将此轮的余数添加到余数列表
                res.Add(y);

                //将此轮的商值（去除0开头的数字）存入十进制LIST做为下一轮的被除数
                var flag = false;
                intSource.Clear();
                foreach (var a in ans.Where(a => a != 0 || flag))
                {
                    flag = true;
                    intSource.Add(a);
                }
            }
            //如果十进制LIST还有数字，需将此数字添加到余数列表后
            if (intSource.Count > 0) res.Add(intSource[0]);

            //将余数列表反转，并逐位转换为N进制字符
            var nValue = string.Empty;
            for (var i = res.Count - 1; i >= 0; i--)
            {
                nValue += newBaseChars[res[i]].ToString();
            }

            return nValue;
        }


        #endregion
    }
}
