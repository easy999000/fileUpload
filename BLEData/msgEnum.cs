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
        /// <summary>
        /// 请求用文件列表
        /// </summary>
        getUserFileList,
        /// <summary>
        /// 返回用文件列表
        /// </summary>
        returnUserFileList,
        /// <summary>
        /// 心跳连接
        /// </summary>
        xintiao,

    }
}
