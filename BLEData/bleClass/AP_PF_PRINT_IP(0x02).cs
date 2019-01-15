using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE.bleClass
{
    public class AP_PF_PRINT_IP_0x02_ : BLEData
    {
        public AP_PF_PRINT_IP_0x02_() : base(BLEcommand.t11)
        {
        }

        /// <summary>
        /// ip
        /// </summary>
        public string ip
        {
            get
            {
                return BLEData.byteToHexString(this.messageData[0]);
            }
        }
        /// <summary>
        /// 网关mac地址
        /// </summary>
        public string Mac
        {
            get
            {
                return BLEData.byteToHexString(this.messageData[1]);
            }
        }





    }
}
