using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE.bleClass
{
    /// <summary>
    /// 返回扫描结果
    /// </summary>
    public class AP_PF_PRINT_DEVICE_0x0D_ : BLEData
    {
        public AP_PF_PRINT_DEVICE_0x0D_() : base(BLEcommand.t11)
        {
        }
         

        /// <summary>
        /// BLE模块地址
        /// </summary>
        public string Address_Mac
        {
            get
            {
                return BLEData.byteToHexString(this.messageData[0]);
            }
        }
        /// <summary>
        /// 设备时间(1970-1-1 00:00:00以来经过的秒数)
        /// </summary>
        public int bleTime
        {
            get
            {
                return BLEData.byteToInt32(this.messageData[1]);
            }
        }
        /// <summary>
        /// 传感器1数据
        /// </summary>
        public int Value1
        {
            get
            {
                return BLEData.byteToInt16(this.messageData[2]);
            }
        }
        public int Value2
        {
            get
            {
                return BLEData.byteToInt16(this.messageData[3]);
            }
        }
        public int Value3
        {
            get
            {
                return BLEData.byteToInt16(this.messageData[4]);
            }
        }
        public int Value4
        {
            get
            {
                return BLEData.byteToInt16(this.messageData[5]);
            }
        }
        /// <summary>
        /// 有效数据位
        /// </summary>
        public byte Parity
        {
            get
            {
                return  this.messageData[6][0] ;
            }
        }
        /// <summary>
        /// 更新时间间隔
        /// </summary>
        public byte updateInterval
        {
            get
            {
                return this.messageData[7][0];
            }
        }





    }
}
