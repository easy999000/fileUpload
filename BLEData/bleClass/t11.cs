using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE.bleClass
{
    /// <summary>
    /// 发送字符串消息
    /// </summary>
    public class t11 : BLEData
    {
        public t11() : base(BLEcommand.t11)
        {
        }

        ////// t11  ----[ 0xaa 0x55 comand longLength] [msgLength   msg]
        ////// t12  ----[ 0xaa 0x55 comand longLength] [pathStrLength fileLengthf pathStr File]

        ///// <summary>
        ///// 消息长度(字节)
        ///// </summary>
        //public int msgLength
        //{
        //    get
        //    {
        //        return BLEData.byteToInt32(this.messageData[0]);
        //    }
        //}

        /// <summary>
        /// 消息内容
        /// </summary>
        public string msg
        {
            get; set;
        }

        public override string ToString()
        {
            return msg;
        }

        #region 接收数据变量
        /// <summary>
        /// 消息字节长度
        /// </summary>
        int msgByteLength = 0;
        /// <summary>
        /// 收到的字节
        /// </summary>
        List<byte> msgByteLengthByte = new List<byte>();


        /// <summary>
        /// 消息内容字节
        /// </summary>
        List<byte> msgDataByte = new List<byte>();
        /// <summary>
        /// 文件长度
        /// </summary>
        Int64 fileDataLength = 0;
        /// <summary>
        /// 文件长度字节
        /// </summary>
        List<byte> fileDataLengthByte = new List<byte>();

        protected override void initReaddata()
        {
            base.initReaddata();
            msgByteLength = 0;
            msgByteLengthByte.Clear();
        }
        protected override void successData()
        {
            base.successData();
            initReaddata();
            this.msg = getString(msgDataByte.ToArray());
        }



        /// <summary>
        /// 返回1代表继续接收,返回0代表接收结束
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public override int writeByte(byte b)
        {
            ////写入返回值
            int writeRet = -1;
            currentPosition++;

            switch (currentPosition)
            {
                case 0:
                case 1:
                case 2:
                case 3://///标识消息总长度
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    headByte.Add(b);

                    break;
                case 10:
                    headByte.Add(b);
                    try
                    {

                        allDataLength = BLEData.byteToInt64(headByte[3], headByte[4], headByte[5], headByte[6], headByte[7], headByte[8], headByte[9], headByte[10]);
                    }
                    catch
                    {
                        errorData();
                        return 0;
                    }


                    //  dataLength = BLE.BLEData.getInt16(data[2], data[3]);
                    break;

                case 11:////这4位代表消息内容的长度
                case 12:
                case 13:
                    msgByteLengthByte.Add(b);
                    break;
                case 14:
                    msgByteLengthByte.Add(b);
                    msgByteLength = byteToInt32(msgByteLengthByte.ToArray());
                    break;

                default:
                    msgDataByte.Add(b);
                    if (currentPosition + 1 >= allDataLength + BLE.BLEData.headLength)
                    {
                        successData();
                        return 0;
                    }

                    break;
            }

            return 1;
        }


        #endregion

        #region 发送功能
        public override byte[] toBleByte()
        {
            byte[] bs = getByte(msg);
            byte[] bsLength = getByte(bs.Length);
            messageData[0] = bsLength;
            messageData[1] = bs;
            allDataLength = bsLength.LongLength + bs.LongLength;
            return base.toBleByte();
        }
        #endregion
    }
}
