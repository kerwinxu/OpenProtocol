using Io.Github.KerwinXu.OpenProtocol.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Io.Github.KerwinXu.OpenProtocol.Attributes.Checks.Tests
{
    [TestClass()]
    public class CheckModbusTests
    {
        [TestMethod()]
        public void CheckModbusTest()
        {
            byte[] data = { 0x01, 0x03, 0x00, 0x00, 0x00, 0x01 };
            CheckModbus checkModbus = new CheckModbus(0,0);
            var data2 = checkModbus.Compute(data);
            byte[] data3 = { 0x84, 0x0a };
            Assert.IsTrue(Judgement.ObjectCompare(data2, data3));
            //Assert.Fail();
        }
    }
}