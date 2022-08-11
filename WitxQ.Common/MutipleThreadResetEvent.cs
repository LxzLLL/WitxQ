using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

/**********************************************************************************
 原理：封装一个ManualResetEvent对象，一个计数器current，提供SetOne和WaitAll方法；
      主线程调用WaitAll方法使ManualResetEvent对象等待唤醒信号；
      各个子线程调用setOne方法 ，setOne每执行一次current减1，直到current等于0时表示所有子线程执行完毕 ,
      调用ManualResetEvent的set方法，这时主线程可以执行WaitAll之后的步骤。

 目标：减少ManualResetEvent对象的大量产生和使用的简单性。
***********************************************************************************/
namespace WitxQ.Common
{
    /// <summary>
    /// 封装 ManualResetEvent ，该类允许一次等待N（N>64）个事件执行完毕
    /// </summary>
    public class MutipleThreadResetEvent:IDisposable
    {
        private readonly ManualResetEvent _done;
        private readonly int _total;
        private long _current;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="total">需要等待执行的线程总数</param>
        public MutipleThreadResetEvent(int total)
        {
            this._total = total;
            _current = total;
            _done = new ManualResetEvent(false);
        }

        /// <summary>
        /// 唤醒一个等待的线程
        /// </summary>
        public void SetOne()
        {
            // Interlocked 原子操作类 ,此处将计数器减1
            if (Interlocked.Decrement(ref _current) == 0)
            {
                //当所以等待线程执行完毕时，唤醒等待的线程
                _done.Set();
            }
        }

        /// <summary>
        /// 等待所以线程执行完毕
        /// </summary>
        public void WaitAll()
        {
            _done.WaitOne();
        }

        /// <summary>
        /// 释放对象占用的空间
        /// </summary>
        public void Dispose()
        {
            ((IDisposable)_done).Dispose();
        }
    }
}
