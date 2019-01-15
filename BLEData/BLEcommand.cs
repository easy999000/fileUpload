using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE
{
    /// <summary>
    /// 标识通信的命令代码,位置和值对应,位置不可改变.
    /// </summary>
    public enum BLEcommand: byte
    {

        /// <summary>
        /// 用户登入
        /// </summary>
        t10 = 10,
        /// <summary>
        ///  发送字符串消息
        /// </summary>
        t11,
        /// <summary>
        /// 发送文件
        /// </summary>
        t12, 




    }
}
