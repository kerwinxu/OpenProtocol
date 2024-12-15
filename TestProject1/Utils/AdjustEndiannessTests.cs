using Microsoft.VisualStudio.TestTools.UnitTesting;
using Io.Github.KerwinXu.OpenProtocol.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Io.Github.KerwinXu.OpenProtocol.Utils.Tests
{
    [TestClass()]
    public class AdjustEndiannessTests
    {
        [TestMethod()]
        public void ProtocolToMachineTest()
        {
            // 一个4字节整数的字节数组
            byte[] data = { 0x12, 0x34, 0x56, 0x78 };

            byte[] tmp1 = { 0x02, 0x00, 0x00, 0x00 };
            var tmp2 = BitConverter.ToInt32(tmp1, 0);
            //
            var data1 = AdjustEndianness.ProtocolToMachine(data, "ABCD");
            var i1 = BitConverter.ToInt32(data1, 0);
            Assert.AreEqual(305419896, i1);
            //
            var data2 = AdjustEndianness.ProtocolToMachine(data, "DCBA");
            var i2 = BitConverter.ToInt32(data2, 0);
            Assert.AreEqual(2018915346, i2);
            //Assert.Fail();
        }
    }
}