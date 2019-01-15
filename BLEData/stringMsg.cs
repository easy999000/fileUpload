using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BLE
{
    /// <summary>
    /// 文字消息载体
    /// </summary>
    public class stringMsg
    {
        /// <summary>
        /// 消息名字 liaotian(聊天)
        /// </summary>
        public msgEnum name;
        /// <summary>
        /// 消息内容
        /// </summary>
        public Dictionary<string, string> value=new Dictionary<string, string>();

        public string modelToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static stringMsg jsonToModel(string json)
        {
            return JsonConvert.DeserializeObject<stringMsg>(json);
        }


    }


}
