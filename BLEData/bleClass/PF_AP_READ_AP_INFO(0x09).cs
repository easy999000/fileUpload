using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE.bleClass
{
    /// <summary>
    /// 查询网关信息
    /// </summary>
    public class PF_AP_READ_AP_INFO_0x09_ : BLEData
    {
        public PF_AP_READ_AP_INFO_0x09_() : base(BLEcommand.t11)
        {
        }
    }
}
