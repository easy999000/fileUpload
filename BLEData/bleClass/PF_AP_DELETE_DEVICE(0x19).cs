using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE.bleClass
{
    /// <summary>
    /// 从白名单中删除某一个设备或者清空白名单列表
    /// </summary>
    public class PF_AP_DELETE_DEVICE_0x19_ : BLEData
    {
        public PF_AP_DELETE_DEVICE_0x19_( ) : base(BLEcommand.t11)
        {
        }


        public string BLEMAC
        {
            get
            {
                return BLEData.byteToHexString(this.messageData[0]);
            }
            set
            {

                byte[] byteArray = HexToByte(value,'-');
                if (byteArray.Length > 6)
                {
                    byteArray = byteArray.Take(6).ToArray();
                }
                else if (byteArray.Length < 6)
                {
                    byteArray = byteArray.Concat(new byte[6 - byteArray.Length]).ToArray();
                }
                this.messageData[0] = byteArray;
                 
            }
        }

    }
}
