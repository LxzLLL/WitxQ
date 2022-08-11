using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WitxQ.Common
{
    /// <summary>
    /// 重写path的Combine
    /// </summary>
    public class PathCombine
    {
        public static string Combine(params string[] paths)
        {
            if (paths.Length == 0)
            {
                return string.Empty;
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                string spliter = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)?"\\":"/";
                string firstPath = paths[0];

                if (firstPath.StartsWith("HTTP", StringComparison.OrdinalIgnoreCase))
                {
                    spliter = "/";
                }

                if (!firstPath.EndsWith(spliter))
                {
                    firstPath = firstPath + spliter;
                }
                builder.Append(firstPath);

                for (int i = 1; i < paths.Length; i++)
                {
                    string nextPath = paths[i];
                    if (nextPath.StartsWith("/") || nextPath.StartsWith("\\"))
                    {
                        nextPath = nextPath.Substring(1);
                    }

                    if (i != paths.Length - 1)//not the last one
                    {
                        if (nextPath.EndsWith("/") || nextPath.EndsWith("\\"))
                        {
                            nextPath = nextPath.Substring(0, nextPath.Length - 1) + spliter;
                        }
                        else
                        {
                            nextPath = nextPath + spliter;
                        }
                    }

                    builder.Append(nextPath);
                }

                return builder.ToString();
            }
        }
    }
}
