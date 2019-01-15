using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE.bleClass
{
    /// <summary>
    /// 获取网关ip和mac地址
    /// </summary>
    public class PF_AP_IP_REQ_0x01_ : BLEData
    {
        public PF_AP_IP_REQ_0x01_() : base(BLEcommand.t11)
        {
        }
    }
}
