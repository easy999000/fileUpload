using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tools
{
    public class message
    {
        #region 字段
        /// <summary>
        /// 数据头
        /// </summary>
        public readonly byte[] head = new byte[2] { 0x55, 0xAA };

        /// <summary>
        /// 消息类型
        /// </summary>
        public readonly byte command;
        
        /// <summary>
        /// 消息内容
        /// </summary>
        List<byte> text { get; }

        #endregion

        public message(byte _command)
        {
            this.command = _command;
        }
        /// <summary>
        /// 写入消息内容
        /// </summary>
        /// <param name="data"></param>
        public void writeByte( byte[] data)
        {
        }
         

        /// <summary>
        /// 获取命令描述
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string getCommandDescribe(byte command)
        {
            string describe = "";
        
            switch (command)
            {
                case 10:
                    describe = "文字消息|[头][内容长度][字符串]";
                    break;
                case 11:
                    describe = "上传文件|[头][完整路径长度][文件长度][完整路径][文件内容]";
                    break;
                case 12:
                    describe = "登入|[头][账号长度][密码长度][账号][密码]";
                    break;
                default:
                    describe = "未定义";
                    break;
            }
            return describe;
        }


    }



}
