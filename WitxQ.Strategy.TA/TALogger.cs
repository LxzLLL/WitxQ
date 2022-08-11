using System;
using System.IO;
using System.Text;

namespace WitxQ.Strategy.TA
{
    /// <summary>
    /// 三角套利的日志类
    /// </summary>
    public class TALogger
    {
        /// <summary>
        /// 日志路径
        /// </summary>
        private string _path;

        /// <summary>
        /// 日志文件夹
        /// </summary>
        private string _folderName;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="path">日志路径，上层调用传入路径</param>
        /// <param name="folderName">日志文件夹</param>
        public TALogger(string path,string folderName)
        {
            this._path = string.IsNullOrWhiteSpace(path)? "Logs/TA":path;
            this._folderName = string.IsNullOrWhiteSpace(path) ? "Log":folderName;
        }

        /// <summary>
        /// 写入错误日志
        /// </summary>
        /// <param name="ex">Exception对象</param>
        /// <param name="title">日志标题</param>
        public void Error(Exception ex, string title = "")
        {
            string content = string.Format("错误信息：{1}{0}错误来源：{2}{0}堆栈信息：{0}{3}", Environment.NewLine, ex.Message, ex.Source, ex.StackTrace);
            this.Write(content, title, this._folderName, "Exception");
        }

        /// <summary>
        /// 写入交易日志
        /// </summary>
        /// <param name="content">日志内容</param>
        /// <param name="title">日志标题</param>
        public void Tran(string content, string title = "")
        {
            //string content = string.Format("错误信息：{1}{0}错误来源：{2}{0}堆栈信息：{0}{3}", Environment.NewLine, ex.Message, ex.Source, ex.StackTrace);
            this.Write(content, title, this._folderName, "Tran");
        }

        /// <summary>
        /// 写入信息日志
        /// </summary>
        /// <param name="content"></param>
        /// <param name="title"></param>
        public void Info(string content, string title = "")
        {
            this.Write(content, title, this._folderName, "Info");
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="format">符合格式字符串</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        //public void WriteFormat(string format, params object[] args)
        //{
        //    string content = string.Format(format, args);
        //    this.Write(content, "", "Log", "Log");
        //}

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="content">日志内容</param>
        /// <param name="title">日志标题</param>
        /// <param name="folderName">文件夹名称</param>
        /// <param name="filePrefixName">文件前缀名</param>
        private void Write(string content, string title = "", string folderName = "Log", string filePrefixName = "Log")
        {
            try
            {
                lock (typeof(TALogger))
                {
                    DateTime dateTimeNow = DateTime.Now;
                    string logDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this._path, folderName);
                    if (!Directory.Exists(logDirPath))
                    {
                        Directory.CreateDirectory(logDirPath);
                    }

                    // 按小时日志
                    string logFilePath = string.Format("{0}/{1}-{2}.txt", logDirPath, filePrefixName, dateTimeNow.ToString("yyyy-MM-dd HH"));
                    using (StreamWriter writer = new StreamWriter(logFilePath, true, Encoding.UTF8))
                    {
                        try
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("------------------------------------------------------------------------------------------"+Environment.NewLine);
                            sb.Append(title + Environment.NewLine);
                            sb.Append("日志时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + Environment.NewLine);
                            sb.Append(content + Environment.NewLine);
                            sb.Append("------------------------------------------------------------------------------------------" + Environment.NewLine);
                            writer.WriteLine(sb.ToString());
                            //writer.WriteLine("------------------------------------------------------------------------------------------");
                            //writer.WriteLine(title);
                            //writer.WriteLine("日志时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            //writer.WriteLine(content);
                            //writer.WriteLine("------------------------------------------------------------------------------------------");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("TALogger.cs " + ex.Message);
                        }

                        writer.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("TALogger.cs " + ex);
                //throw new Exception("无法将日志写入文件,请查看安装目录是否有权限!");
            }
        }

    }
}
