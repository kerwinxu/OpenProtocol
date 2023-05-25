using Io.Github.KerwinXu.OpenProtocol.Attributes.Checks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}