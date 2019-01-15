using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE
{
    public enum msgEnum
    {
        /// <summary>
        /// 文字消息
        /// </summary>
        liaotian,

        dengru,
        /// <summary>
        /// 上传文件
        /// </summary>
        fileUpload,
        /// <summary>
        /// 获取目录
        /// </summary>
        getDir,

    }
}
