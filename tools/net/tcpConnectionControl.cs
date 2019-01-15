using BLE;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace tools.net
{
    /// <summary>
    /// 管理当前存在的连接 
    /// </summary>
    public class tcpConnectionControl
    {
        /// <summary>
        /// 管理所有连接的通信
        /// </summary>
        public ConcurrentDictionary<string, tcpDataCommunication> tcpList = new ConcurrentDictionary<string, tcpDataCommunication>();


        /// <summary>
        /// 连接建立事件  
        /// </summary>
        public event Action<tcpDataCommunication> addTcpEvent;

        /// <summary>
        /// 连接断开事件
        /// </summary>
        public event Action<tcpDataCommunication> removeTcpEvent;


        /// <summary>
        /// 新消息事件 
        /// </summary>
        public event Action<tcpDataCommunication, stringMsg> newMessageEvent;

        /// <summary>
        /// 停止现在正在运行的连接
        /// </summary>
        public void stop()
        {
            List<string> keyList = tcpList.Keys.ToList();

            foreach (var item in keyList)
            {
                tcpDataCommunication t = tcpList[item];
                removeTcp(t);
                t.stop();

            }
        }

        /// <summary>
        /// 添加连接到集合
        /// </summary>
        /// <param name="tcp"></param>
        public tcpDataCommunication addTcp(TcpClient tcp)
        {
            tcpDataCommunication tcpCommunication = new tcpDataCommunication(tcp);
            tcpCommunication.newBleMessageEvent += newBlemessageEventFun;

            if (this.tcpList.TryAdd(tcpCommunication.tcpClientId, tcpCommunication))
            {
                addTcpEvent?.Invoke(tcpCommunication);
            }
            return tcpCommunication;
        }

        /// <summary>
        /// 消息接收事件处理程序
        /// </summary>
        /// <param name="tcpComm"></param>
        /// <param name="ble"></param>
        void newBlemessageEventFun(tcpDataCommunication tcpComm, BLE.BLEData ble)
        {
            string s1 = ble.ToString();
            stringMsg m2 = stringMsg.jsonToModel(s1); 
            newMessageEvent?.Invoke(tcpComm, m2);
            
        }

        /// <summary>
        /// 从集合移除连接
        /// </summary>
        /// <param name="tcp"></param>
        public void removeTcp(tcpDataCommunication tcp)
        {
            tcpDataCommunication t;
            if (this.tcpList.TryRemove(tcp.tcpClientId, out t))
            {
                removeTcpEvent?.Invoke(tcp);

            }
        }

        #region 发送相关函数
        ///// <summary>
        ///// 发送数据,成功返回true
        ///// </summary>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //public bool sendData(dataModel.tcpData data)
        //{
        //    tcpDataCommunication Communication;
        //    if (tcpList.TryGetValue(data.tcpClientId, out Communication))
        //    {
        //        Communication.sendData(data.data);
        //        return true;
        //    }
        //    return false;
        //}

        #endregion


    }
}
