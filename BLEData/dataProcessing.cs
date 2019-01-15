using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE
{
    public class dataProcessing
    {
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

        #endregion
        #region 处理传入数据
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
                            writeRet  = ble.writeByte(d);
                            
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

        }
        #endregion
    }
}
