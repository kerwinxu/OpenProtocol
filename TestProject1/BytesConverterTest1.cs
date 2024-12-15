using Io.Github.KerwinXu.OpenProtocol.Attributes;
using Io.Github.KerwinXu.OpenProtocol.Attributes.Checks;
using Io.Github.KerwinXu.OpenProtocol.Attributes.Counts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Io.Github.KerwinXu.OpenProtocol.Tests
{
    [TestClass()]
    public class BytesConverterTest1
    {
        [TestMethod()]
        public void BytesToBulitinTypeTest()
        {

            BytesObjectConverter bytesObjectConverter = new BytesObjectConverter();

            // 一个字节数组
            var bytes = new byte[] { 0x00, 0x01 };
            int end_index = 0;
            var obj = bytesObjectConverter.BytesToBulitinType(
                bytes,
                0,
                ref end_index, typeof(ushort),
                "AB");  // 这里表示高位在前
            Assert.AreEqual(2, end_index);
            Assert.AreEqual((ushort)1, obj);


            //Assert.Fail();
        }

        class T1
        {
            [DataItem(0)]
            public byte Start = 0x00;

            [DataItem(1)]
            public short Data { get; set; }

            [DataItem(2)]
            public byte End = 0xff;
        }

        [TestMethod()]
        public void BytesToObjectTest()
        {
            // 测试是否可以通过bytes转成对象
            BytesObjectConverter bytesObjectConverter = new BytesObjectConverter();
            var data1 = new byte[] { 0x00, 0x01, 0x00, 0xff };
            int end_index = 0;
            var obj = (T1)bytesObjectConverter.BytesToObject(
                data1,
                0,
                ref end_index,
                typeof(T1)
                );
            Assert.AreEqual(4, end_index);
            Assert.IsTrue(obj != null);
            Assert.AreEqual((short)1, obj.Data);
            //Assert.Fail();
        }


        class Modbus
        {
            [DataItem(0)]
            public byte SlaveId { get; set; }

            [DataItem(1)]
            public byte FunCode { get; set; }

            [DataItem(2, "AB")]
            public ushort RegAddress { get; set; }

            [DataItem(3, "AB")]
            public ushort RegCount { get; set; }

            [DataItem(4)]
            [DefaultSize(2)]
            [CheckModbus(0, 3)]
            public byte[] Sum { get; set; }
        }


        [TestMethod()]
        public void BytesToObjectModbusTest()
        {
            byte[] data1 = { 0x01, 0x03, 0x00, 0x00, 0x00, 0x01, 0x84, 0x0a };
            BytesObjectConverter bytesObjectConverter = new BytesObjectConverter();
            int end_index = 0;
            var obj = (Modbus)bytesObjectConverter.BytesToObject(data1, 0, ref end_index, typeof(Modbus));
            Assert.IsNotNull(obj);
            Assert.AreEqual(1, obj.SlaveId);
            Assert.AreEqual(3, obj.FunCode);
            Assert.AreEqual(0, obj.RegAddress);
            Assert.AreEqual(1, obj.RegCount);
        }

        [TestMethod()]
        public void BulitinTypeToBytesTest()
        {
            int i1 = 0x12345678;
            BytesObjectConverter bytesObjectConverter = new BytesObjectConverter();
            //
            var d1 = bytesObjectConverter.BulitinTypeToBytes(i1, "ABCD");
            var d2 = new byte[] { 0x12, 0x34, 0x56, 0x78 };
            Assert.IsTrue(d1.SequenceEqual(d2));
            //
            var d3 = bytesObjectConverter.BulitinTypeToBytes(i1, "DCBA");
            var d4 = new byte[] { 0x78, 0x56, 0x34, 0x12 };
            Assert.IsTrue(d3.SequenceEqual(d4));


            //Assert.Fail();
        }

        [TestMethod()]
        public void ObjectToBytesTest()
        {
            byte[] data1 = { 0x01, 0x03, 0x00, 0x00, 0x00, 0x01, 0x84, 0x0a };
            BytesObjectConverter bytesObjectConverter = new BytesObjectConverter();
            int end_index = 0;
            var obj = (Modbus)bytesObjectConverter.BytesToObject(data1, 0, ref end_index, typeof(Modbus));
            // 再转回去
            var data2 = bytesObjectConverter.ObjectToBytes(obj);
            Assert.IsTrue(data1.SequenceEqual(data2));

            //Assert.Fail();
        }
    }
}

