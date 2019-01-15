using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE.bleClass
{
    /// <summary>
    /// 返回白名单
    /// </summary>
    public class AP_PF_PRINT_WHITELIST_0x18_ : BLEData
    {
        public AP_PF_PRINT_WHITELIST_0x18_() : base(BLEcommand.t11)
        {
        }
        string maclist = "";

        /// <summary>
        /// 白名单列表 以逗号(,)分隔 
        /// </summary>
        public string WhiteAddressList
        {
            get
            {
                if (maclist == "")
                {
                    for (int i = 0; i < this.messageData[0].Length; i += 6)
                    {
                        byte[] b1 = new byte[6];

                        Array.Copy(this.messageData[0], i, b1, 0, 6);

                        maclist += BLEData.byteToHexString(b1) + ",";
                    }
                }
                return maclist;
            }
        }



    }
}
