using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;
using WitxQ.Exchange.Loopring.Models;
using WitxQ.Exchange.Loopring.Operation;
using WitxQ.Exchange.Loopring.Sys;

namespace WitxQ.Exchange.Loopring.Tools
{
    /// <summary>
    /// websocket的客户端类
    /// </summary>
    public class WSClient
    {
        /// <summary>
        /// websocket服务路径
        /// </summary>
        private string _serverPath;

        #region 内部处理事件
        /// <summary>
        /// ws opoened时的委托
        /// </summary>
        public event EventHandler OpenedHandler;

        /// <summary>
        /// ws opoened时的委托
        /// </summary>
        public event Action<string> MessageReceivedHandler;
        #endregion

        /// <summary>
        /// WebSocket客户端
        /// </summary>
        private WebSocket _webSocketClient;

        /// <summary>
        /// 检查重连线程
        /// </summary>
        private Thread _threadCheckConnection;
        private bool _isRunning = true;

        /// <summary>
        /// 是否正在重连
        /// </summary>
        public bool IsReConnecting { get; set; } = false;

        private AccountModel _account;

        #region 单例

        /// <summary>
        /// 私有构造
        /// </summary>
        private WSClient()
        {

        }

        /// <summary>
        /// 获取WsManager单例
        /// </summary>
        /// <returns></returns>
        public static WSClient GetInstance()
        {
            return InnerInstance.instance;
        }

        private class InnerInstance
        {
            /// <summary>
            /// 当一个类有静态构造函数时，它的静态成员变量不会被beforefieldinit修饰
            /// 就会确保在被引用的时候才会实例化，而不是程序启动的时候实例化
            /// </summary>
            static InnerInstance() { }
            internal static WSClient instance = new WSClient();
        }
        #endregion

        /// <summary>
        /// 设置websocket的服务器路径
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="serverPath">ws的url</param>
        public WSClient SetServerPath(AccountModel account,string serverPath)
        {
            this._serverPath = serverPath;
            this._account = account;

            ExLoopring.WS_API_KEY = new WSOperation(this._account).GetWSApiKeyByApi();
            this._webSocketClient = new WebSocket($"{this._serverPath}?wsApiKey={ExLoopring.WS_API_KEY}");

            // 打开事件
            this._webSocketClient.Opened += WebSocket_Opened;
            // 错误事件
            this._webSocketClient.Error += WebSocket_Error;
            // 关闭事件
            this._webSocketClient.Closed += WebSocket_Closed;
            // 消息接收事件
            this._webSocketClient.MessageReceived += WebSocket_MessageReceived;
            return this;
        }


        /// <summary>
        /// 连接
        /// <returns></returns>
        public bool Start()
        {
            bool result = false;
            try
            {
                this._webSocketClient.Open();

                this._isRunning = true;
                // 检查重连线程
                this._threadCheckConnection = new Thread(new ThreadStart(this.CheckConnection));
                this._threadCheckConnection.Start();
                result = true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"WSClient--Start:{ex.Message}");
                ExLoopring.LOGGER.Error($"WSClient--Start:{ex.Message}", ex);
            }
            return result;
        }


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message"></param>
        public void SendMessage(string Message)
        {
            Task.Factory.StartNew(() =>
            {
                int timeout = 60;   //一分钟
                // 如果未连接时，等待一分钟
                while (timeout >0)
                {
                    if (this._webSocketClient == null || this._webSocketClient.State != WebSocketState.Open)
                        ExLoopring.LOGGER.Error($"WSClient--SendMessage: Send message error，websocket state:{(this._webSocketClient != null ? this._webSocketClient.State.ToString() : "null")},waitting for connecting! Left time:{timeout}s");
                    else
                        break;
                    timeout--;
                    Task.Delay(1000).Wait();
                }

                if (this._webSocketClient != null && this._webSocketClient.State == WebSocketState.Open)
                {
                    this._webSocketClient.Send(Message);
                }
                else
                {
                    ExLoopring.LOGGER.Error($"WSClient--SendMessage: Send message error，websocket state:{this._webSocketClient.State},TimeOut!");
                }
            });
        }

        /// <summary>
        /// 消息收到事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            //Console.WriteLine(" Received:" + e.Message);
            // 心跳回复
            // WebSocket链接建立后，中继会每30秒会发送“ping”消息给客户端做心跳检测。
            // 如果客户端在最近2分钟内都没有任何“pong”消息，中继会断开WebSocket链接。
            // 如果客户端的“pong”消息数量超过服务端发送的“ping”消息数量，中继也会断开WebSocket链接。
            if (e.Message.Equals("ping"))
            {
                Console.WriteLine(" Received HeartBeat:" + e.Message);
                ExLoopring.LOGGER.Info("WSClient--WebSocket_MessageReceived:Received HeartBeat:" + e.Message);
                this.SendMessage("pong");
                Console.WriteLine(" Send HeartBeat:pong");
                ExLoopring.LOGGER.Info("WSClient--WebSocket_MessageReceived:Send HeartBeat:pong");
            }
            else
            {
                Task.Run(() => MessageReceivedHandler?.Invoke(e.Message));
            }
            
        }
        /// <summary>
        /// Socket关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Closed(object sender, EventArgs e)
        {
            //Console.WriteLine("websocket_Closed:");
            ExLoopring.LOGGER.Info("WSClient--WebSocket_Closed:websocket_Closed");
            //_Logger.Info("websocket_Closed");
        }
        /// <summary>
        /// Socket报错事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Error(object sender, ErrorEventArgs e)
        {
            //Console.WriteLine("websocket_Error:" + e.Exception.ToString());
            ExLoopring.LOGGER.Info("WSClient--WebSocket_Error:" + e.Exception.ToString());
        }
        /// <summary>
        /// Socket打开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Opened(object sender, EventArgs e)
        {
            //Console.WriteLine("websocket_Opened");
            Task.Run(() =>
            {
                this.OpenedHandler?.Invoke(sender, e);

                this.IsReConnecting = false;
            });
            
            ExLoopring.LOGGER.Info("WSClient--WebSocket_Opened");
        }


        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            this._isRunning = false;
            try
            {
                this._threadCheckConnection.Abort();
            }
            catch
            {

            }
            this._webSocketClient.Close();
            this._webSocketClient.Dispose();
            this._webSocketClient = null;
        }


        /// <summary>
        /// 检查重连线程，5s检测一次
        /// </summary>
        private void CheckConnection()
        {
            do
            {
                //Thread.Sleep(5000);
                Task.Delay(5000).Wait();   // 先等待，避免第一次连接时间过长，而进入此检查连接
                try
                {
                    // 不在 打开状态
                    if (this._webSocketClient.State != WebSocketState.Open)
                    {
                        ExLoopring.LOGGER.Info($"WSClient--CheckConnection:Websocket正在重连......，WebSocketState：{this._webSocketClient.State}");
                        this.IsReConnecting = true;

                        // 正在连接状态，需要关闭连接
                        if(this._webSocketClient.State == WebSocketState.Connecting)
                            this._webSocketClient.Close();

                        // 如果是关闭状态 则重新创建客户单连接
                        if(this._webSocketClient.State ==WebSocketState.Closed)
                        {
                            ExLoopring.WS_API_KEY = new WSOperation(this._account).GetWSApiKeyByApi();
                            this._webSocketClient = new WebSocket($"{this._serverPath}?wsApiKey={ExLoopring.WS_API_KEY}");
                            ExLoopring.LOGGER.Info($"WSClient--CheckConnection:连接：{this._serverPath}?wsApiKey={ExLoopring.WS_API_KEY}");

                            // 打开事件
                            this._webSocketClient.Opened += WebSocket_Opened;
                            // 错误事件
                            this._webSocketClient.Error += WebSocket_Error;
                            // 关闭事件
                            this._webSocketClient.Closed += WebSocket_Closed;
                            // 消息接收事件
                            this._webSocketClient.MessageReceived += WebSocket_MessageReceived;
                        }

                        this._webSocketClient.Open();
                        ExLoopring.LOGGER.Info($"WSClient--CheckConnection:已执行webSocketClient.Open");
                    }
                }
                catch (Exception ex)
                {
                    ExLoopring.LOGGER.Error($"WSClient--CheckConnection:{ex.Message}",ex);
                }
                
            } while (this._isRunning);
        }
    }
}
