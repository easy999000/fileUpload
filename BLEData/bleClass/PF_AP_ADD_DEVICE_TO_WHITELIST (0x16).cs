using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE.bleClass
{
    /// <summary>
    /// 添加指定设备到白名单列表
    /// </summary>
    public class PF_AP_ADD_DEVICE_TO_WHITELIST__0x16_ : BLEData
    {
        public PF_AP_ADD_DEVICE_TO_WHITELIST__0x16_() : base(BLEcommand.t11)
        {
        }
        /// <summary>
        /// mac格式01-01-01-00-10-21
        /// </summary>
        public string BLEMAC
        {
            get
            {
                return BLEData.byteToHexString(this.messageData[0]);
            }
            set
            {

                byte[] byteArray = HexToByte(value,'-' );



                if (byteArray.Length>6)
                {
                    byteArray = byteArray.Take(6).ToArray();
                }
                else if (byteArray.Length < 6)
                {
                    byteArray= byteArray.Concat(new byte[6 - byteArray.Length]).ToArray();
                }


                this.messageData[0] = byteArray;
            }
        }
               
    }
}
