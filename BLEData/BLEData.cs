using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE
{
    /// <summary>
    /// 通信数据 消息格式"头|命令|数据[长度标识|长度标识|内容|内容]"
    /// </summary>
    public class BLEData
    {
        /// <summary>
        /// 消息头长度 头,命令,消息长度int64
        /// </summary>
        public const int headLength = 11;
        /// <summary>
        /// 客户端,是否是低字节.
        /// </summary>
        public const bool IsLittleEndian = true;

        /// <summary>
        /// 接收数据的tcpClient标识id
        /// </summary>
        public string tcpClientId;

        /// <summary>
        /// 数据头
        /// </summary>
        public readonly byte[] head = new byte[2] { 0x55, 0xAA };


        /// <summary>
        /// 命令
        /// </summary>
        readonly BLEcommand _command;



        /// <summary>
        /// 命令
        /// </summary>
        public BLEcommand command { get { return _command; } }

        /// <summary>
        /// 全部数据内容的实际长度
        /// </summary>
        public Int64 allDataLength;


        /// <summary>
        /// 固定长度有效数据载荷长度
        /// </summary>
        public Int16 messageLength
        {
            get
            {
                int? i6 = messageData.Sum(o => o?.Length);
                return (Int16)i6.Value;
                //return (byte)(message.Length);
            }
        }





        /// <summary>
        /// 数据内容
        /// </summary>
        public byte[][] messageData;

        /// <summary>
        /// 消息描述.
        /// </summary>
        public messageDescribe messageDescribe
        {
            get { return BLEcommandHelper.getBLEcommandMessageDescribe(command); }
        }

        public BLEData(BLEcommand comm)
        {
            this._command = comm;

            byte[][] msg1 = new byte[messageDescribe.messageLength.Length][];
            msg1.Initialize();
            this.messageData = msg1;


        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="comm"></param>
        /// <returns></returns>
        public static BLEData CreateBle(BLEcommand comm)
        {
            BLEData b = null;
            switch (comm)
            {
                case BLEcommand.t11:
                    b = new bleClass.t11();
                    break;
                case BLEcommand.t12:
                    b = new bleClass.t12();
                    break;
                default:
                    b = new BLEData(comm);
                    break;
            }
            return b;
        }


        /// <summary>
        /// 检测数据和命令是否一致.
        /// 功能未实现,实现后,删掉这个注释
        /// </summary>
        /// <param name="comm"></param>
        /// <returns></returns>
        public bool checkBLE(BLEcommand comm)
        {
            return true;
        }


        /// <summary>
        /// 返回字符数组形式
        /// </summary>
        /// <returns></returns>
        public virtual byte[] toBleByte()
        {
            List<byte> l1 = new List<byte>();
            l1.AddRange(head);
            // l1.AddRange(getByte(messageLength));
            l1.Add((byte)command);
            l1.AddRange(getByte(this.allDataLength));

            foreach (var item in messageData)
            {
                if (item != null)
                {
                    l1.AddRange(item);
                }
            }
            return l1.ToArray();


            //List<byte> l1 = new List<byte>();
            //l1.AddRange(head);
            //l1.Add((byte)command);
            //for (int i = 0; i < messageLength.Length; i++)
            //{
            //    l1.AddRange(getByte(messageLength[i]));

            //}

            //foreach (var item in messageData)
            //{
            //    l1.AddRange(item);

            //}
            //return l1.ToArray();
        }


        #region 静态转换方法

        /// <summary>
        /// 获取相符的字符顺序
        /// </summary>
        /// <returns></returns>
        public static byte[] getEndianByte(byte[] b)
        {
            if (BLEData.IsLittleEndian == BitConverter.IsLittleEndian)
            {
                return b;
            }
            else
            {
                return b.Reverse().ToArray();
            }
        }

        public static string getString(byte[] b)
        {
            return System.Text.Encoding.UTF8.GetString(b);
        }
        public static string getString(byte[] b, System.Text.Encoding E)
        {
            return E.GetString(b);
        }
        /// <summary>
        /// 获取字节十六进制字符串标识形式
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string byteToHexString(byte[] b)
        {
            if (b == null)
            {
                return "";
            }

            return BitConverter.ToString(b);
        }
        /// <summary>
        /// 获取字节十六进制字符串标识形式
        /// </summary>
        /// <param name="b"></param>
        /// <param name="Split"></param>
        /// <param name="befor">16进制的前缀</param>
        /// <returns></returns>
        public static string byteToHexString(byte[] b, string Split, string befor)
        {
            if (b == null)
            {
                return "";
            }
            string s1 = BitConverter.ToString(b);
            string[] s2 = s1.Split('-').Select(o => { return befor + o; }).ToArray();


            return string.Join(Split, s2); ;
        }

        /// <summary>
        /// 32位int
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int byteToInt32(params byte[] b)
        {

            return BitConverter.ToInt32(getEndianByte(b), 0);
        }
        /// <summary>
        /// 64位int
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Int64 byteToInt64(params byte[] b)
        {

            return BitConverter.ToInt64(getEndianByte(b), 0);
        }
        /// <summary>
        /// 32位int
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int byteToInt16(params byte[] b)
        {

            return BitConverter.ToInt16(getEndianByte(b), 0);
        }
        /// <summary>
        ///  直接把字符串按字符以文字的形式转换成byte
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static byte[] getByte(string value)
        {
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(value);
            if (IsLittleEndian != BitConverter.IsLittleEndian)
            {
                return byteArray.Reverse().ToArray();

            }
            return byteArray;
        }
        /// <summary>
        /// 把字符分割成16进制字符,在转换
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static byte[] HexToByte(string value, char SplitChar)
        {
            string[] s1 = value.Split(SplitChar);
            List<byte> b1 = new List<byte>();
            foreach (var item in s1)
            {
                b1.Add(Convert.ToByte(item, 16));
            }

            if (IsLittleEndian != BitConverter.IsLittleEndian)
            {
                b1.Reverse();

            }
            return b1.ToArray();
        }
        /// <summary>
        ///  
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static byte[] getByte(Int16 value)
        {
            byte[] byteArray = BitConverter.GetBytes(value);
            if (IsLittleEndian != BitConverter.IsLittleEndian)
            {
                return byteArray.Reverse().ToArray();

            }
            return byteArray;
        }
        /// <summary>
        ///  
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static byte[] getByte(int value)
        {
            byte[] byteArray = BitConverter.GetBytes(value);

            if (IsLittleEndian != BitConverter.IsLittleEndian)
            {
                return byteArray.Reverse().ToArray();

            }
            return byteArray;
        }
        /// <summary>
        ///  
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static byte[] getByte(Int64 value)
        {
            byte[] byteArray = BitConverter.GetBytes(value);

            if (IsLittleEndian != BitConverter.IsLittleEndian)
            {
                return byteArray.Reverse().ToArray();

            }
            return byteArray;
        }



        #endregion

        #region 处理接收数据
        /// <summary>
        /// 当前准备读取数据的位置.
        /// </summary>
        protected Int64 currentPosition = -1;

        /// <summary>
        /// 收到的字节
        /// </summary>
        protected List<byte> headByte = new List<byte>();
         
        /// <summary>
        /// 初始化接收参数
        /// </summary>
        protected virtual void initReaddata()
        {
            headByte.Clear();
            allDataLength = 0;
            currentPosition = -1;
        }
        /// <summary>
        /// 成功收到格式正确的消息
        /// </summary>
        protected virtual void successData()
        {
            initReaddata();
            //byte[] data1 = data.ToArray();
            //tcpData data2 = new tcpData();
            //data2.tcpClientId = this.tcpClientId;
            //data2.data = data1;

            //mainService1.tcpDataCatch1.Enqueue(data2);

            //initReaddata();

        }
        /// <summary>
        ///如果数据格式不正确,执行此方法
        /// </summary>
        protected virtual void errorData()
        {
            initReaddata();
        }

        /// <summary>
        /// 返回1代表继续接收,返回0代表接收结束
        /// </summary>
        /// <param name="b"></param>
        /// <returns></re
        public virtual int writeByte(byte b)
        {
            return 0;
        }
        #endregion




    }
}
