using Microsoft.VisualStudio.TestTools.UnitTesting;
using tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tools.Tests
{
    [TestClass()]
    public class messageTests
    {
        [TestMethod()]
        public void getCommandDescribeTest()
        {
            List<byte> bs = new List<byte>();
            for (int i = 0; i < 128; i++)
            {
                bs.Add((byte)i);
            }
            string s2 = "一二三四五六七八九十abcde123456789";

            byte[] bs2 = System.Text.Encoding.UTF8.GetBytes(s2);

            string s1 = System.Text.Encoding.UTF8.GetString(bs.ToArray());
            Assert.Fail();
        }
    }
}