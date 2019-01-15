using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE
{
    /// <summary>
    /// 这个函数 实现功能 没有写完,按照这个逻辑,完成功能后删掉这个注释.
    /// 消息描述类,描述指定命令的消息的格式及说明.
    /// </summary>
    public class messageDescribe
    {
        //  public readonly BLEcommand command;
        /// <summary>
        /// 描述
        /// </summary>
        public readonly string Describe;



        /// <summary>
        /// 每段消息的长度,占用的字节数
        /// </summary>
        public readonly short[] messageLength;

        /// <summary>
        /// 消息的最短有效长度
        /// </summary>
        public short MessageSumLength_min
        {
            get
            {
                short b1 =0;
                foreach (var item in messageLength)
                {
                    if (item>0)
                    {
                        b1 += item;
                    }

                }
                return b1;
            }
        }


        messageDescribe(string Describe1, params short[] MessageArrayLength1)
        {
            this.Describe = Describe1;
            this.messageLength = MessageArrayLength1;
        }


        /// <summary>
        /// 这个函数 实现功能 没有写完,按照这个逻辑,完成功能后删掉这个注释.
        /// 根据数据类型,创建命令帮助类型. 
        /// </summary>
        /// <param name="command1"></param>
        /// <returns></returns>
        public static messageDescribe CreateBLEDataHelper(BLEcommand command1)
        {
            //这个函数 实现功能 没有写完,按照这个逻辑,完成功能后删掉这个注释.

            messageDescribe h1;
            switch (command1)
            {
                //case BLEcommand.t10: 
                //    h1 = new messageDescribe(@"登入", 2, 2);
                //    break; 
                case BLEcommand.t11:
                    h1 = new messageDescribe(@"发送文字消息[int32|string]", 4,-1);
                    break;
                case BLEcommand.t12:
                    h1 = new messageDescribe(@"发送文件[int32|int64|string|file]", 4,8,-1,-1);
                    break;
                default:
                    h1 = new messageDescribe("无", 0);
                    break;
            } 
            return h1;

        }

    }
    /// <summary>
    ///  BLEcommand辅助类,提供一些工具
    /// </summary>
    public class BLEcommandHelper
    {
        static readonly Dictionary<BLEcommand, messageDescribe> messageDic = new Dictionary<BLEcommand, messageDescribe>();
        

        //public BLEDataHelper(BLEcommand comm)
        //{
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Describe1"></param>
        /// <param name="MessageArrayLength1">-1代表不固定长度</param>
        BLEcommandHelper(string Describe1, params short[] MessageArrayLength1)
        {
        }

        static BLEcommandHelper()
        {
            foreach (BLEcommand item in Enum.GetValues(typeof(BLEcommand)))
            { 
                messageDescribe msg1 = messageDescribe.CreateBLEDataHelper(item);
                messageDic.Add(item, msg1);
            }
        }

        /// <summary>
        /// 获取指定命令的消息描述
        /// </summary>
        public static messageDescribe getBLEcommandMessageDescribe(BLEcommand comm)
        { 
            return messageDic[comm];
        }



    }
}
