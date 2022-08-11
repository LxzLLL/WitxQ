using System;
using System.Collections.Generic;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models
{
    /// <summary>
    /// 签名模型
    /// </summary>
    public class SignatureModel:BaseModel
    {
        public string Rx { get; set; }
        public string Ry { get; set; }
        public string s { get; set; }
    }
}
