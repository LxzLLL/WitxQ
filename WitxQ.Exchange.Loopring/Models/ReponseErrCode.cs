using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WitxQ.Exchange.Loopring.Models
{
    /// <summary>
    /// 异常信息的code
    /// </summary>
    public enum ReponseErrCode
    {
        /// <summary>
        /// 未知错误
        /// </summary>
        [Description("成功")]
        Success = 0,

        /// <summary>
        /// 未知错误
        /// </summary>
        [Description("未知错误")]
        UnkownErr = 100000,

        /// <summary>
        /// 参数非法
        /// </summary>
        [Description("参数非法")]
        InvalidParam = 100001,

        /// <summary>
        /// 请求超时
        /// </summary>
        [Description("请求超时")]
        RequestTimeOut = 100002,

        /// <summary>
        /// 更新失败
        /// </summary>
        [Description("更新失败")]
        UpdateFailed = 100202,

        /// <summary>
        /// 内部存储错误
        /// </summary>
        [Description("内部存储错误")]
        InternalStorageError = 100203,

        /// <summary>
        /// 重复提交
        /// </summary>
        [Description("重复提交")]
        DuplicateSubmission = 100204,

        /// <summary>
        /// 交易所ID不正确
        /// </summary>
        [Description("交易所ID不正确")]
        ExIDIncorrect = 102001,

        /// <summary>
        /// 订单中存在不支持的TokenId
        /// </summary>
        [Description("订单中存在不支持的TokenId")]
        UnsupportedTokenId = 102002,

        /// <summary>
        /// 无效的账户ID
        /// </summary>
        [Description("无效的账户ID")]
        InvalidAccountId = 102003,

        /// <summary>
        /// 无效的订单ID
        /// </summary>
        [Description("无效的订单ID")]
        InvalidOrderId = 102004,

        /// <summary>
        /// 市场对不支持
        /// </summary>
        [Description("市场对不支持")]
        UnsupportedMarketPair = 102005,

        /// <summary>
        /// 不合法的费率字段
        /// </summary>
        [Description("不合法的费率字段")]
        InvalidOrderFee = 102006,

        /// <summary>
        /// 订单已经存在
        /// </summary>
        [Description("订单已经存在")]
        OrderExisted = 102007,

        /// <summary>
        /// 订单已经过期
        /// </summary>
        [Description("订单已经过期")]
        OrderExpired = 102008,

        /// <summary>
        /// 订单缺少签名信息
        /// </summary>
        [Description("订单缺少签名信息")]
        OrderMissingSignature = 102010,

        /// <summary>
        /// 用户余额不足
        /// </summary>
        [Description("用户余额不足")]
        InsufficientBalance = 102011,

        /// <summary>
        /// 下单金额太小
        /// </summary>
        [Description("下单金额太小")]
        OrderAmountSmall = 102012,

        /// <summary>
        /// 冻结金额失败，请稍后重试
        /// </summary>
        [Description("冻结金额失败，请稍后重试")]
        FreezeAmountFailed = 102014,

        /// <summary>
        /// 超过下单最大金额
        /// </summary>
        [Description("超过下单最大金额")]
        OrderAmountExceedingMaximum = 102020,

        /// <summary>
        /// 订单未生效
        /// </summary>
        [Description("订单未生效")]
        OrderIneffective = 102120,

        /// <summary>
        /// APIKEY不能为空
        /// </summary>
        [Description("APIKEY不能为空")]
        ApiKeyNull = 104001,

        /// <summary>
        /// APIKEY验证失败
        /// </summary>
        [Description("APIKEY验证失败")]
        ApiKeyVerifyFailure = 104002,

        /// <summary>
        /// 用户不存在
        /// </summary>
        [Description("用户不存在")]
        AccountNonExistent = 104003,

        /// <summary>
        /// 未提供签名信息
        /// </summary>
        [Description("未提供签名信息")]
        NoSignature = 104004,

        /// <summary>
        /// 错误的签名信息
        /// </summary>
        [Description("错误的签名信息")]
        SignatureError = 104005,

        /// <summary>
        /// 批量操作部分失败
        /// </summary>
        [Description("批量操作部分失败")]
        OrderBatchPartFailed = 104209,

        /// <summary>
        /// 不支持的市场
        /// </summary>
        [Description("不支持的市场")]
        UnsupportedMarket = 108000,

        /// <summary>
        /// 不支持的深度等级
        /// </summary>
        [Description("不支持的深度等级")]
        UnsupportedMarketLevel = 108001,
    }
}
