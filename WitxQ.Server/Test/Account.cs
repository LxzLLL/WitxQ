using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Server.Test
{
    public class Account
    {
        /// <summary>
        /// 账号ID
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// 账号的地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 账号的ApiKey
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// 账号的PublicKeyX
        /// </summary>
        public string PublicKeyX { get; set; }

        /// <summary>
        /// 账号的PublicKeyY
        /// </summary>
        public string PublicKeyY { get; set; }

        /// <summary>
        /// 账号的SecretKey
        /// </summary>
        public string SecretKey { get; set; }
    }
}
