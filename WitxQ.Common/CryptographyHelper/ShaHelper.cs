using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WitxQ.Common.CryptographyHelper
{
    /// <summary>
    /// sha加密算法
    /// </summary>
    public class ShaHelper
    {
        /// <summary>
        /// sha256加密算法
        /// </summary>
        /// <param name="data">需要加密的字符串</param>
        /// <returns></returns>
        public static string Sha256EncryptToString(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256Managed.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString();
        }

        /// <summary>
        /// sha256加密算法
        /// </summary>
        /// <param name="data">需要加密的字符串</param>
        /// <returns></returns>
        public static Byte[] Sha256EncryptToByte(string data)
        {
            var sha256 = new SHA256Managed();
            var Asc = new ASCIIEncoding();
            var tmpByte = Asc.GetBytes(data);
            var EncryptBytes = sha256.ComputeHash(tmpByte);
            sha256.Clear();
            return EncryptBytes;
        }
    }
}
