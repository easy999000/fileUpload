﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE.bleClass
{
    /// <summary>
    /// 返回网关信息
    /// </summary>
    public class t12 : BLEData
    {
        public t12() : base(BLEcommand.t12)
        {
        }

        /// <summary>
        /// 接收文件完整信息stringMsg格式
        /// </summary>
        public string ReceiveFullMsg
        {
            get; set;
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
        /// 文件长度
        /// </summary>
        Int64 fileDataLength = 0;
        /// <summary>
        /// 文件长度字节
        /// </summary>
        List<byte> fileDataLengthByte = new List<byte>();

        /// <summary>
        /// msgByte
        /// </summary>
        List<byte> msgByte = new List<byte>();

        FileStream fileWrite;

        string filePath;

        protected override void initReaddata()
        {
            base.initReaddata();
            msgByteLength = 0;
            msgByteLengthByte.Clear();
            fileDataLength = 0;
            msgByte.Clear();
            if (fileWrite != null)
            {
                //fileWrite.Flush();
                fileWrite.Close();
            }
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
                case 3://///标识消息总长度8位
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
                case 15:////这8位代表文件的长度
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                    fileDataLengthByte.Add(b);
                    break;
                case 22:
                    fileDataLengthByte.Add(b);
                    fileDataLength = byteToInt64(fileDataLengthByte.ToArray());

                    break;

                default:
                    int beforIndex = 22;

                    if (currentPosition < beforIndex + msgByteLength)
                    {
                        ////当前位置小于消息长度,为消息内容
                        msgByte.Add(b);

                    }
                    else if (currentPosition == beforIndex + msgByteLength)
                    {
                        ////当前位置等于消息长度,为消息结尾
                        msgByte.Add(b);
                        string msgJson = getString(msgByte.ToArray());

                        // stringMsg sm = stringMsg.jsonToModel(pathJson);

                        this.ReceiveFullMsg = msgJson;

                        try
                        {
                            stringMsg FullMsg = stringMsg.jsonToModel(msgJson);
                            string rfmPath = FullMsg.value["saveFileFullPath"];
                            if (rfmPath.Trim()=="")
                            {
                                errorData();
                                return 0;
                            }
                            string dir = System.IO.Path.GetDirectoryName(rfmPath);
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }


                            fileWrite = System.IO.File.Create(rfmPath);//ReceiveFullMsg
                        }
                        #region 创建文件异常处理
                        catch (UnauthorizedAccessException ex1)
                        {
                            errorData();
                            return 0;
                        }
                        catch (ArgumentException ex1)
                        {
                            errorData();
                            return 0;

                        }
                        catch (PathTooLongException ex1)
                        {
                            errorData();
                            return 0;

                        }
                        catch (DirectoryNotFoundException ex1)
                        {
                            errorData();
                            return 0;
                        }
                        catch (IOException ex1)
                        {
                            errorData();
                            return 0;
                        }
                        catch (NotSupportedException ex1)
                        {
                            errorData();
                            return 0;
                        }
                        #endregion


                    }
                    else if (currentPosition < beforIndex + msgByteLength + fileDataLength)
                    {
                        ////当前位置小于文件长度,为文件内容
                        fileWrite.WriteByte(b);
                        fileWrite.Flush();
                    }
                    else if (currentPosition >= beforIndex + msgByteLength + fileDataLength)
                    {
                        ////消息结束
                        fileWrite.WriteByte(b);
                        fileWrite.Flush();
                        fileWrite.Close();
                        successData();
                        return 0;
                    }


                    break;
            }

            return 1;
        }

        public override string ToString()
        {
            //stringMsg sm = new stringMsg();
            //sm.name = msgEnum.fileUpload;
            //sm.value.Add("value", this.ReceiveFileFullPath);
            return this.ReceiveFullMsg;
        }


        #endregion

        #region 发送


        public override byte[] toBleByte()
        {
            return base.toBleByte();
        }
        public override void toBleStream(System.IO.Stream sr)
        {
            //try
            //{

            stringMsg Msg = stringMsg.jsonToModel(ReceiveFullMsg);
            string sendFileFullPath = Msg.value["sendFileFullPath"];
            
            byte[] FullMsg = getByte(ReceiveFullMsg);
            byte[] FullMsgLength = getByte(FullMsg.Length);

            Stream fileStream = System.IO.File.OpenRead(sendFileFullPath);
            byte[] streamLength = getByte(fileStream.Length);

            //////////////////////
            
            ////////////////
            allDataLength = FullMsgLength.LongLength + FullMsg.LongLength;///1

            messageData[0] = FullMsgLength;//2

            messageData[1] = streamLength;///3

            messageData[2] = FullMsg;///3

            List<byte> l1 = new List<byte>();
            l1.AddRange(head);
            l1.Add((byte)command);
            l1.AddRange(getByte(this.allDataLength));


            sr.Write(l1.ToArray(), 0, l1.Count);
            sr.Write(messageData[0], 0, messageData[0].Count());///报连接错误

            sr.Write(messageData[1], 0, messageData[1].Count());
            sr.Write(messageData[2], 0, messageData[2].Count());

            fileStream.CopyTo(sr);

            #region 手动流复制
            ////手动流复制
            //byte[] bytes = new byte[128];
            //float num = 0;
            //while (true)
            //{
            //    // byte[] srByte = StreamToBytes(sr, num);

            //    int n = fileStream.Read(bytes, 0, 128);
            //    if (n == 0)
            //    {
            //        break;
            //    }
            //    sr.Write(bytes, 0, n);
            //    num += n;

            //    currProgress = int.Parse(Math.Ceiling((num / fileStream.Length)*100).ToString());
            //}
            /////
            #endregion

            fileStream.Close();
            //}
            //catch (Exception ex)
            //{

            //    string x = string.Format(@"[Message]:{0},[StackTrace]:{1},[Source]:{2},[TargetSite]:{3},[HResult]:{4}", ex.Message, ex.StackTrace, ex.Source, ex.TargetSite, ex.HResult);
            //    string y = x;
            //}
        }


        //void setStream(object o)
        //{
        //    MemoryStream ms = o as MemoryStream;
        //    if (ms == null)
        //    {
        //        return;
        //    }

        //    byte[] path = getByte(ReceiveFullMsg);
        //    byte[] pathLength = getByte(path.Length);

        //    Stream fileStream = System.IO.File.OpenRead(sendFileFullPath);
        //    byte[] streamLength = getByte(fileStream.Length);


        //    List<byte> l1 = new List<byte>();
        //    l1.AddRange(head);
        //    l1.Add((byte)command);
        //    l1.AddRange(getByte(this.allDataLength));

        //    allDataLength = pathLength.LongLength + path.LongLength;///1

        //    messageData[0] = pathLength;//2

        //    messageData[1] = streamLength;///3

        //    messageData[2] = path;///3

        //    ms.Write(l1.ToArray(), 0, l1.Count);
        //    ms.Write(messageData[0], 0, messageData[0].Count());
        //    ms.Write(messageData[1], 0, messageData[1].Count());
        //    ms.Write(messageData[2], 0, messageData[2].Count());

        //    fileStream.CopyTo(ms);

        //    fileStream.Close();
        //    //  ms.Close();


        //}
        #endregion


    }
}
