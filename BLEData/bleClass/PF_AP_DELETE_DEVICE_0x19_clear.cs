using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE.bleClass
{
    /// <summary>
    /// 清空白名单
    /// </summary>
    public class PF_AP_DELETE_DEVICE_0x19_clear : BLEData
    {
        public PF_AP_DELETE_DEVICE_0x19_clear() : base(BLEcommand.t11)
        {
        }
    }
}
