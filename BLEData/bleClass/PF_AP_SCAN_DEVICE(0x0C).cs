using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE.bleClass
{
    /// <summary>
    /// 开始扫描ble设备数据
    /// 
    /// </summary>
    public class PF_AP_SCAN_DEVICE_0x0C_ : BLEData
    {
        public PF_AP_SCAN_DEVICE_0x0C_() : base(BLEcommand.t11)
        {
        }
    }
}
