using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BLE.bleClass
{
    /// <summary>
    /// udp广播,通知服务器ip和端口
    /// </summary>
    public class PF_AP_TCP_SRV_CFG_0x06_ : BLEData
    {
        public PF_AP_TCP_SRV_CFG_0x06_() : base(BLEcommand.t11)
        {
        }
        /// <summary>
        /// ip地址
        /// </summary>
        public string ip
        {
            get
            {
                return BLEData.byteToHexString(this.messageData[0]);
            }
            set
            {
                IPAddress ip1 = IPAddress.Parse(value);

                this.messageData[0] = ip1.GetAddressBytes();
            }
        }

        /// <summary>
        /// 端口
        /// </summary>
        public int PORT
        {
            get
            {
                return BLEData.byteToInt32(this.messageData[1]);
            }
            set
            {
                this.messageData[1] = BLEData.getByte(Convert.ToInt16(value));
            }
        }
    }
}
