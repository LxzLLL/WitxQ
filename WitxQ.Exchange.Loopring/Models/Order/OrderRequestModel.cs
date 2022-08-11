using System;
using System.Collections.Generic;
using System.Text;
using WitxQ.Exchange.Loopring.Sys;

namespace WitxQ.Exchange.Loopring.Models.Order
{


    /// <summary>
    /// 下单请求实体
    /// </summary>
    public class OrderRequestModel : BaseModel
    {

        /// <summary>
        /// 交易所地址，exchange.loopring.io
        /// </summary>
        public string exchange { get; set; } = SysConst.EXCHANGE_ADDRESS;

        /// <summary>
        /// 订单ID
        /// </summary>
        public long orderId { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public long storageId { get; set; }

        /// <summary>
        /// 账户ID
        /// </summary>
        public int accountId { get; set; }

        /// <summary>
        /// 需要卖出的Token ID
        /// </summary>
        public int tokenSId { get; set; }

        /// <summary>
        ///需要买入的Token ID
        /// </summary>
        public int tokenBId { get; set; }

        /// <summary>
        /// 需要卖出的Token数量
        /// </summary>
        public string amountS { get; set; }

        /// <summary>
        /// 需要买入的Token数量
        /// </summary>
        public string amountB { get; set; }

        /// <summary>
        /// 订单是否只能完全成交或者0成交, 目前只支持false，不是必须，例如："false"
        /// </summary>
        public bool allOrNone { get; set; } = false;

        /// <summary>
        /// 订单失效时间, 精确到秒（有效的UNIX时间戳）（建议下单时间戳+1个月），例如：1567053142
        /// </summary>
        public long validUntil { get; set; }

        /// <summary>
        /// 用户可以接受的最大订单费用, 取值范围(万分之)1~63，例如：20
        /// </summary>
        public int maxFeeBips { get; set; } = 50;

        /// <summary>
        /// 订单标签,用来标示订单属性或者来源等, 参与订单签名, 取值范围0~65535，例如：20
        /// </summary>
        public int label { get; set; }

        /// <summary>
        /// 订单渠道号, 用来标示订单从哪个渠道提交，不是必须，例如："hebao::subchannel::0001"
        /// </summary>
        public string channelId { get; set; } = "hebao::subchannel::0001";

        /// <summary>
        /// 费用 20
        /// </summary>
        public int feeBips { get; set; } = 50;

        /// <summary>
        /// fillAmountBOrS，应该买卖方向
        /// <para>
        /// 1、卖的时候设为false，买的话设为true
        /// 2、So far AMM swap order does NOT support fillAmountBOrS, so, please set fillAmountBOrS to false in swap request.
        ///    当orderType=AMM时，设置为false
        /// </para>
        /// </summary>
        public bool fillAmountBOrS { get; set; }

        /// <summary>
        /// 订单类型（LIMIT_ORDER：订单薄，AMM：Swap闪兑）
        /// </summary>
        public string orderType { get; set; }

        /// <summary>
        /// 账户的以太坊地址
        /// </summary>
        public string owner { get; set; }

        /// <summary>
        /// sell的代币地址
        /// </summary>
        public string tokenS { get; set; }
        /// <summary>
        /// buy的代币地址
        /// </summary>
        public string tokenB { get; set; }

        /// <summary>
        /// sell的总数量，同amountS
        /// </summary>
        public string amountSInBN { get; set; }

        /// <summary>
        /// buy的总数量，同amountB
        /// </summary>
        public string amountBInBN { get; set; }

        /// <summary>
        /// 签名信息（）
        /// </summary>
        public string eddsaSig { get; set; }

        /// <summary>
        /// 由用户指定的P2P订单使用的taker，当前使用0x0000000000000000000000
        /// </summary>
        //public string taker { get; set; } = SysConst.EXCHANGE_TAKER;


        ///// <summary>
        ///// 回扣 0
        ///// </summary>
        //public int rebateBips { get; set; } = 0;

        ///// <summary>
        ///// hash
        ///// </summary>
        //public string hash { get; set; }

        ///// <summary>
        ///// 签名
        ///// </summary>
        //public SignatureModel signature { get; set; }

    }


    #region 废弃
    /// <summary>
    /// 下单请求实体
    /// </summary>
    public class OrderRequestModelAbandoned : BaseModel
    {

        /// <summary>
        /// 交易所ID，默认为2，表示为loopring.io
        /// </summary>
        public int exchangeId { get; set; } = 2;

        /// <summary>
        /// 订单ID
        /// </summary>
        public long orderId { get; set; }

        /// <summary>
        /// 账户ID
        /// </summary>
        public int accountId { get; set; }

        /// <summary>
        /// 需要卖出的Token ID
        /// </summary>
        public int tokenSId { get; set; }

        /// <summary>
        ///需要买入的Token ID
        /// </summary>
        public int tokenBId { get; set; }

        /// <summary>
        /// 需要卖出的Token数量
        /// </summary>
        public string amountS { get; set; }

        /// <summary>
        /// 需要买入的Token数量
        /// </summary>
        public string amountB { get; set; }

        /// <summary>
        /// 订单是否只能完全成交或者0成交, 目前只支持false，不是必须，例如："false"
        /// </summary>
        public bool allOrNone { get; set; } = false;

        /// <summary>
        /// 买到为止或者卖出为止，例如："true"
        /// </summary>
        public bool buy { get; set; }

        /// <summary>
        /// 订单生效时间, 精确到秒（下单的UNIX时间戳），例如：1567053142
        /// </summary>
        public long validSince { get; set; }

        /// <summary>
        /// 订单失效时间, 精确到秒（有效的UNIX时间戳）（建议下单时间戳+1个月），例如：1567053142
        /// </summary>
        public long validUntil { get; set; }

        /// <summary>
        /// 用户可以接受的最大订单费用, 取值范围(万分之)1~63，例如：20
        /// </summary>
        public int maxFeeBips { get; set; } = 63;

        /// <summary>
        /// 订单标签,用来标示订单属性或者来源等, 参与订单签名, 取值范围0~65535，例如：20
        /// </summary>
        public int label { get; set; }

        /// <summary>
        /// 订单签名结果Rx部分，signatureRx,同Rx
        /// </summary>
        public string signatureRx { get; set; }
        /// <summary>
        /// 订单签名结果Ry部分，signatureRy,同Ry
        /// </summary>
        public string signatureRy { get; set; }
        /// <summary>
        /// 订单签名结果S部分，signatureS,同s
        /// </summary>
        public string signatureS { get; set; }

        /// <summary>
        /// 客户端标识的订单唯一ID，不是必须，例如："1"
        /// </summary>
        public string clientOrderId { get; set; }

        /// <summary>
        /// 订单渠道号, 用来标示订单从哪个渠道提交，不是必须，例如："hebao::subchannel::0001"
        /// </summary>
        public string channelId { get; set; } = "hebao::subchannel::0001";


        ///// <summary>
        ///// 账户的以太坊地址
        ///// </summary>
        //public string owner { get; set; }

        ///// <summary>
        ///// sell的代币地址
        ///// </summary>
        //public string tokenS { get; set; }
        ///// <summary>
        ///// buy的代币地址
        ///// </summary>
        //public string tokenB { get; set; }

        ///// <summary>
        ///// sell的总数量，同amountS
        ///// </summary>
        //public string amountSInBN { get; set; }

        ///// <summary>
        ///// buy的总数量，同amountB
        ///// </summary>
        //public string amountBInBN { get; set; }

        ///// <summary>
        ///// 费用 20
        ///// </summary>
        //public int feeBips { get; set; } = 50;

        ///// <summary>
        ///// 回扣 0
        ///// </summary>
        //public int rebateBips { get; set; } = 0;

        ///// <summary>
        ///// hash
        ///// </summary>
        //public string hash { get; set; }

        ///// <summary>
        ///// 签名
        ///// </summary>
        //public SignatureModel signature { get; set; }

    }
    #endregion
}
