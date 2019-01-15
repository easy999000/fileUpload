using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BLE;

namespace tools.net
{
    /// <summary>
    /// 处理tcp数据通信的
    /// </summary>
    public class tcpDataCommunication
    {
        /// <summary>　
        /// tcpClient标识符
        /// </summary>
        public readonly string tcpClientId;

        public readonly TcpClient tcpClient1;

        #region 接收相关字段
        List<byte> data = new List<byte>();
        /// <summary>
        /// 当前准备读取数据的位置.
        /// </summary>
        Int64 currentPosition = 0;

        /// <summary>
        /// 当前数据有效载荷长度.　 
        /// </summary>
        Int64 dataLength = 0;

        /// <summary>
        /// 数据对象
        /// </summary>
        BLEData ble = null;


        /// <summary>
        /// 新消息事件 
        /// </summary>
        public event Action<tcpDataCommunication, BLEData> newBleMessageEvent;



        /// <summary>
        /// 
        /// </summary>
        System.Threading.Thread thReading;
        #endregion
        #region 发送相关字段

        #endregion

        /////要测试的几个问题,可不可以同时进行读写.可不可以一个tcp类,get多个流进行读写.多个流是不是一个实例.,可不可以多线程读写.
        ////测试结果,tcp不管调用多少次GetStream,获取到的都是同一个stream对象. 一个流可以同时进行读取和写入(可能是有2个缓冲区).
        ////连接断开后,设备会不会自动重新连接服务器.

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Service1"></param>
        /// <param name="tcp"></param>
        public tcpDataCommunication(TcpClient tcp)
        {
            tcpClient1 = tcp;
            //this.tcpName = tcpName1;

            tcpClientId = tcpClient1.GetHashCode().ToString() + "&" + DateTime.Now.ToFileTime() + "&" + tcpClient1.Client.RemoteEndPoint.ToString();

            //readByte(null);

            thReading = new System.Threading.Thread(readByte);

            thReading.IsBackground = true;

            thReading.Start();
        }

        /// <summary>
        /// 停止运行
        /// </summary>
        public void stop()
        {
            this.tcpClient1.Close();
            this.thReading.Abort();
        }

        #region 接收相关函数
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="o"></param>
        void readByte(object o)
        {

            NetworkStream stream1 = tcpClient1.GetStream();

            // stream1.

#if DEBUG

            //    tools.log.writeLog("tcpListenerControl.readByte 线程:{0},数据等待", System.Threading.Thread.CurrentThread.ManagedThreadId);
#endif
            while (true)
            {
                try
                {
                    byte[] bs = new byte[256];
                    int count = stream1.Read(bs, 0, bs.Length);

                    //  dataPro.readData(bs, count);
                    readData(bs, count);

                }
                catch (NotSupportedException ex1)
                {
                    connectionDisconnection();
                }
                catch (ObjectDisposedException ex2)
                {
                    connectionDisconnection();
                }
                catch (IOException ex3)
                {
                    connectionDisconnection();
                }
                catch (System.Threading.ThreadAbortException ex)
                {
                    ///线程终止
                    connectionDisconnection();
                }
            }
        }

        /// <summary>
        /// 连接断开
        /// </summary>
        void connectionDisconnection()
        {
            this.tcpClient1.Close();
            this.thReading.Abort();
        }
        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="b"></param>
        public void readData(byte[] tcpByte, int count)
        {
            ////写入返回值
            int writeRet = -1;
            for (int i = 0; i < count; i++)
            {
                byte b = tcpByte[i];

                switch (currentPosition)
                {
                    case 0:
                        if (b == 0x55)
                        {
                            data.Add(b);
                            currentPosition++;

                        }
                        else
                        {
                        }
                        break;

                    case 1:
                        if (b == 0xAA)
                        {
                            data.Add(b);
                            currentPosition++;

                        }
                        else
                        {
                            errorData();
                        }
                        break;

                    case 2:

                        BLEcommand b1;
                        if (!Enum.TryParse(b.ToString(), out b1))
                        {
                            errorData();
                            break;
                        }
                        data.Add(b);
                        ble = BLEData.CreateBle(b1);
                        currentPosition++;

                        break;
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        data.Add(b);
                        currentPosition++;

                        break;
                    case 10:
                        data.Add(b);
                        try
                        {

                            dataLength = BLEData.byteToInt64(data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10]);
                        }
                        catch
                        {
                            errorData();
                            break;
                        }

                        foreach (var d in data)
                        {
                            writeRet = ble.writeByte(d);

                        }
                        currentPosition++;
                        //  dataLength = BLE.BLEData.getInt16(data[2], data[3]);
                        break;


                    default:
                        writeRet = ble.writeByte(b);
                        currentPosition++;

                        break;
                }
                if (writeRet == 0)
                {
                    successData();
                    writeRet = -1;
                }
            }
        }

        /// <summary>
        /// 初始化接收参数
        /// </summary>
        void initReaddata()
        {
            data.Clear();
            dataLength = 0;
            currentPosition = 0;
        }

        /// <summary>
        ///如果数据格式不正确,执行此方法
        /// </summary>
        void errorData()
        {
            initReaddata();
        }

        /// <summary>
        /// 成功收到格式正确的消息
        /// </summary>
        void successData()
        {
#if DEBUG
            Console.WriteLine(ble.ToString());
#endif
            initReaddata();

            newBleMessageEvent?.Invoke(this, ble);



        }
        #endregion

        #region 发送相关函数
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public System.Net.Sockets.NetworkStream sendDataGetStream()
        {
            return tcpClient1.GetStream();
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public void sendData(Stream sr)
        {

            System.Net.Sockets.NetworkStream ns = tcpClient1.GetStream();
            byte[] bs = new byte[128];
            int num = 0;
          //  sr.Position = 0;
            while (sr.CanRead)
            {
                num = sr.Read(bs, 0, bs.Length);
                ns.Write(bs, 0, num);
            }
            //  sr.CopyTo(ns);


            ns.Flush();

        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public void sendData(byte[] data)
        {
            lock (this)
            {
                try
                {
                    System.Net.Sockets.NetworkStream ns = tcpClient1.GetStream();
                    ns.Write(data, 0, data.Length);
                    ns.Flush();
                }
                catch (ObjectDisposedException ex2)
                {
                    connectionDisconnection();
                }
                catch (IOException ex3)
                {
                    connectionDisconnection();
                }
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public void sendData(BLE.BLEData b1)
        {
            lock (this)
            {
                try
                {
                    //BLE.bleClass.t11 t11 = new BLE.bleClass.t11();
                    //t11.msg = this.richTextBox2.Text;

                    sendData(b1.toBleByte());
                }
                catch (ObjectDisposedException ex2)
                {
                    connectionDisconnection();
                }
                catch (IOException ex3)
                {
                    connectionDisconnection();
                }
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public void sendData(BLE.stringMsg m1)
        {
            lock (this)
            {
                try
                {
                    BLE.bleClass.t11 t11 = new BLE.bleClass.t11();

                    t11.msg = m1.modelToJson();

                    sendData(t11);
                }
                catch (ObjectDisposedException ex2)
                {
                    connectionDisconnection();
                }
                catch (IOException ex3)
                {
                    connectionDisconnection();
                }
            }
        }

        #endregion


    }
}
