using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tools.net
{
    /// <summary>
    /// 用户tcp对象的数据缓存
    /// </summary>
    /// <typeparam name="tData"></typeparam>
    public class tcpDataQueue<tData> where tData : class
    {
        /// <summary>
        /// 数据集合,线程安全形,先进先出集合
        /// </summary>
        ConcurrentQueue<tData> tcpData = new ConcurrentQueue<tData>();
        /// <summary>
        /// 线程同步锁
        /// </summary>
        ManualResetEventSlim dataLock = new ManualResetEventSlim(false, 100);

        /// <summary>
        /// 向缓存中添加数据.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Enqueue(tData data)
        {
            tcpData.Enqueue(data);
            dataLock.Set();
            return true;
        }

        public List<tData> GetSendFileList()
        {
            return tcpData.ToList();
        }

        /// <summary>
        /// 包含元素的数量
        /// </summary>
        public int Count
        {
            get
            {
                return tcpData.Count;
            }
        }
        /// <summary>
        /// 获取数据,如果当前没有数据,会照成线程阻塞,直到有数据为止.
        /// </summary>
        /// <returns></returns>
        public tData Dequeue()
        {

            tData data;
            bool returnValue = tcpData.TryDequeue(out data);
            if (!returnValue)
            {
                while (true)
                {
                    try
                    {
#if DEBUG
                        //    tools.log.writeLog("tcpDataQueue.Dequeue 线程:{0}",System.Threading.Thread.CurrentThread.ManagedThreadId);
#endif
                        dataLock.Wait(500);
                        returnValue = tcpData.TryDequeue(out data);
                        if (returnValue)
                        {
                            dataLock.Set();
                            return data;
                        }
                        else
                        {
                            dataLock.Reset();
                        }
                    }
                    catch (Exception ex1)
                    {
                        tools.log.writeLog(ex1);
                    }
                }
            }
            return data;
        }


    }
}
