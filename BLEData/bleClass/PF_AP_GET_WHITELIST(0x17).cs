using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE.bleClass
{
    /// <summary>
    /// 查询网关白名单列表
    /// </summary>
    public class PF_AP_GET_WHITELIST_0x17_ : BLEData
    {
        public PF_AP_GET_WHITELIST_0x17_( ) : base(BLEcommand.t11)
        {
        }
    }
}
