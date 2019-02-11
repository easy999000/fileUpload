using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BLE;
using DB;

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

        public UserInfo user;

        tcpDataQueueControl DataQueue = new tcpDataQueueControl();

        /// <summary>
        /// 待发送队列
        /// </summary>
        public List<BLE.BLEData> waitSendList { get { return this.DataQueue.GetSendFileList(); } }

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
        /// 连接断开事件
        /// </summary>
        public event Action<tcpDataCommunication> connectionDisconnectionEvent;

        /// <summary>
        /// 正在接受数据事件
        /// </summary>
        public event Action<tcpDataCommunication, bool> isReading;

        /// <summary>
        /// 
        /// </summary>
        System.Threading.Thread thReading;
        #endregion
        #region 发送相关字段

        /// <summary>
        /// 心跳
        /// </summary>
        public System.Threading.Timer sendXintiao;

        /// <summary>
        /// 发送线程
        /// </summary>
        System.Threading.Thread thSending;

        /// <summary>
        /// 当前正在发送的对象。
        /// </summary>
        public BLEData currentSendBleData;
        #endregion

        /////要测试的几个问题,可不可以同时进行读写.可不可以一个tcp类,get多个流进行读写.多个流是不是一个实例.,可不可以多线程读写.
        ////测试结果,tcp不管调用多少次GetStream,获取到的都是同一个stream对象. 一个流可以同时进行读取和写入(可能是有2个缓冲区).
        ////连接断开后,设备会不会自动重新连接服务器.


        void xintiao(object o)
        {
            if (this.waitSendList.Count < 1 && this.currentSendBleData == null)
            {
                BLE.stringMsg s1 = new BLE.stringMsg();
                s1.name = BLE.msgEnum.xintiao;
                this.addSendBle(s1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Service1"></param>
        /// <param name="tcp"></param>
        public tcpDataCommunication(TcpClient tcp)
        {
            tcpClient1 = tcp;
            tcpClient1.ReceiveTimeout = 20 * 1000;
            ///  tcpClient1.SendTimeout = 20 * 1000;
            //this.tcpName = tcpName1;

            sendXintiao = new System.Threading.Timer(new System.Threading.TimerCallback(xintiao), null, 0, 9 * 1000);

            tcpClientId = tcpClient1.GetHashCode().ToString() + "&" + DateTime.Now.ToFileTime() + "&" + tcpClient1.Client.RemoteEndPoint.ToString();

            //readByte(null);

            thSending = new System.Threading.Thread(thSend);

            thSending.IsBackground = true;

            thSending.Start();

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
            this.thSending.Abort();
        }

        #region 接收相关函数
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="o"></param>
        void readByte(object o)
        {
            bool whileSwitch = true;
            NetworkStream stream1 = tcpClient1.GetStream();
            int c = 0;
            // stream1.

#if DEBUG

            //    tools.log.writeLog("tcpListenerControl.readByte 线程:{0},数据等待", System.Threading.Thread.CurrentThread.ManagedThreadId);
#endif
            while (whileSwitch)
            {
                c++;
                try
                {
                    byte[] bs = new byte[256];
                    int count = stream1.Read(bs, 0, bs.Length);

                    //  dataPro.readData(bs, count);
                    readData(bs, count);
#if DEBUG
                    //tools.log.writeLog("try:第{0}次",  c.ToString());
#endif
                }
                catch (NotSupportedException ex1)
                {
                    whileSwitch = false;
                    // tools.log.writeLog("NotSupportedException:{0},第{1}次", ex1.Message, c.ToString());
                    connectionDisconnection();
                }
                catch (ObjectDisposedException ex2)
                {
                    whileSwitch = false;
                    //   tools.log.writeLog("ObjectDisposedException:{0},第{1}次", ex2.Message, c.ToString());
                    connectionDisconnection();
                }
                catch (IOException ex3)
                {
                    whileSwitch = false;
                    //     tools.log.writeLog("IOException:{0},第{1}次", ex3.Message, c.ToString());
                    connectionDisconnection();
                }
                catch (System.Threading.ThreadAbortException ex)
                {
                    whileSwitch = false;
                    //   tools.log.writeLog("ThreadAbortException:{0},第{1}次", ex.Message, c.ToString());
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
            sendXintiao.Dispose();
            this.tcpClient1.Close();
            connectionDisconnectionEvent?.Invoke(this);
            this.thReading.Abort();
            this.thSending.Abort();
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
                            isReading?.Invoke(this, true);
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
                        if (!b.ToString().Equals("11") && !b.ToString().Equals("12"))
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
                        catch (Exception ex)
                        {
                            // tools.log.writeLog("readData:第{0}次,错误:{1}", i.ToString(), ex.Message);
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
            isReading?.Invoke(this, false);
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
        /// <summary>
        /// 添加发送队列
        /// </summary>
        /// <param name="ble"></param>

        public void addSendBle(BLEData ble)
        {
            this.DataQueue.Enqueue(ble);
            //  waitSendList = this.DataQueue.GetSendFileList();
        }

        /// <summary>
        /// 添加发送队列
        /// </summary>
        /// <param name="m1"></param>
        public void addSendBle(BLE.stringMsg m1)
        {
            BLEData ble1;
            if (m1.name == msgEnum.fileUpload)
            {
                string sendFileFullPath = m1.value["sendFileFullPath"];
                string saveFileFullPath = m1.value["saveFileFullPath"];

                BLE.bleClass.t12 t12 = new BLE.bleClass.t12();

                //  t12.sendFileFullPath = sendFileFullPath;

                //  string fileName = System.IO.Path.GetFileName(sendFileFullPath);

                //  ConfigInfo config = user.GetSave();
                //  string reviced = System.IO.Path.Combine(CurrUser.config.Path + "\\" + CurrUser.currUser.ID, fileName);//d:\\
                //stringMsg sm = new stringMsg();
                //sm.name = msgEnum.fileUpload;
                //sm.value.Add("sendFileFullPath", sendFileFullPath); 
                //sm.value.Add("saveFileFullPath", saveFileFullPath); 

                //   sm.value.Add("fileDirFullPath", CurrUser.config.Path + "\\" + CurrUser.currUser.ID);//文件存储路径


                t12.ReceiveFullMsg = m1.modelToJson();



                ble1 = t12;
            }
            else
            {
                BLE.bleClass.t11 t11 = new BLE.bleClass.t11();

                t11.msg = m1.modelToJson();

                ble1 = t11;
            }

            addSendBle(ble1);

        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="o"></param>
        void thSend(object o)
        {
            bool whileSwitch = true;
#if DEBUG

            //    tools.log.writeLog("tcpListenerControl.readByte 线程:{0},数据等待", System.Threading.Thread.CurrentThread.ManagedThreadId);
#endif
            while (whileSwitch)
            {
                try
                {

                    currentSendBleData = this.DataQueue.Dequeue();
                    if (currentSendBleData == null)
                    {
                        continue;
                    }
#if DEBUG
                    //   tools.log.writeLog("tcpDataProcessingControl.tcpDataProcessing 线程:{0},取到了一条数据", System.Threading.Thread.CurrentThread.ManagedThreadId);
#endif
                    if (currentSendBleData.command == BLEcommand.t12)
                    {
                        currentSendBleData.toBleStream(this.sendDataGetStream());

                    }
                    else
                    {
                        sendData(currentSendBleData);
                    }
                    currentSendBleData = null;

                }
                catch (NotSupportedException ex1)
                {
                    whileSwitch = false;
                    tools.log.writeLog("NotSupportedException:{0}", ex1.Message);
                    connectionDisconnection();
                }
                catch (ObjectDisposedException ex2)
                {
                    whileSwitch = false;
                    tools.log.writeLog("ObjectDisposedException:{0}", ex2.Message);
                    connectionDisconnection();
                }
                catch (IOException ex3)
                {
                    whileSwitch = false;
                    tools.log.writeLog("IOException:{0}", ex3.Message);
                    connectionDisconnection();
                }
                catch (System.Threading.ThreadAbortException ex)
                {
                    whileSwitch = false;
                    tools.log.writeLog("ThreadAbortException:{0}", ex.Message);
                    ///线程终止
                    connectionDisconnection();
                }
            }
        }
        #endregion


    }
}
