using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace tools.net
{
    public class tcpDataQueueControl  
    {
        tcpDataQueue<BLE.BLEData> DataQueue;

        public tcpDataQueueControl( ) 
        {
            DataQueue = new tcpDataQueue<BLE.BLEData>();
        }


        public List<BLE.BLEData> GetSendFileList()
        {
            return null;
        }
        /// <summary>
        /// 包含元素的数量
        /// </summary> 
        public int Count
        {
            get
            { 
                return DataQueue.Count;
            }
        }

        /// <summary>
        /// 向缓存中添加数据.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void Enqueue(BLE.BLEData data)
        {
            DataQueue.Enqueue(data);
        }


        /// <summary>
        /// 获取数据,如果当前没有数据,会造成线程阻塞,直到有数据为止.建议使用异步线程,
        /// </summary>
        /// <returns></returns>
        public BLE.BLEData Dequeue()
        {
            return DataQueue.Dequeue();
        }

    }
}
