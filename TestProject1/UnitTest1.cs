using Io.Github.KerwinXu.OpenProtocol;
using Io.Github.KerwinXu.OpenProtocol.Attributes;
using Io.Github.KerwinXu.OpenProtocol.Attributes.Checks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Runtime.Intrinsics.Arm;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCRC16()
        {
            // modbus crc :  x16 + x15 + x2 + 1 
            CheckCRC16 crc16 = new CheckCRC16(0, 1, 0x8005, 0xffff, 0x0000);
            byte[] datas = { 1, 2, 3, 4 };
            var crc16_result = crc16.Compute(datas);
            Assert.AreEqual((ushort)0x2ba1, (ushort)crc16_result);

        }


        [TestMethod]
        public void TestCRC32()
        {
            // crc32 mpeg-2 , 
            CheckCRC32 crc32 = new CheckCRC32(0, 1, 0x04c11db7, 0xffffffff, 0,false, false);
            byte[] datas = { 1, 2, 3, 4 };
            var crc32_result = crc32.Compute(datas);
            Assert.AreEqual((uint)0x793737cd, (uint)crc32_result);
        }


        class TestScale1
        {
            [DataItem(0)]
            [IntType(2,true,10)]
            public float data { get; set; }
        }

        [TestMethod]
        public void TestScale()
        {
            byte[] datas = { 01, 02 };
            TestScale1 testClass = (new BytesSerializer<TestScale1>()).Deserialize(datas);
            Assert.AreEqual(25.8f, testClass.data, 0.01);
            byte[] datas2 = (new BytesSerializer<TestScale1>()).Serialize(testClass);
            Assert.IsTrue(Enumerable.SequenceEqual(datas, datas2));

        }
    }
}