// See https://aka.ms/new-console-template for more information

using Io.Github.KerwinXu.OpenProtocol;
using Io.Github.KerwinXu.OpenProtocol.Attributes;
using Io.Github.KerwinXu.OpenProtocol.Attributes.Checks;
using Io.Github.KerwinXu.OpenProtocol.Attributes.Counts;

namespace Demo
{
    /// <summary>
    /// modbus的请求
    /// </summary>
    class ModbusRequest
    {

        [DataItem(0)]   // DataItem是数据项目，0表示第一项。
        public byte SlaveId { get; set; }

        [DataItem(1)]
        public byte FunCode { get; set; }

        [DataItem(2, "AB")]  // AB表示大端。
        public ushort RegAddress { get; set; }

        [DataItem(3, "AB")]
        public ushort RegCount { get; set; }

        [DataItem(4)]
        [DefaultSize(2)]     // 固定的2个元素。可以
        [CheckModbus(0, 3)]
        public byte[] Sum { get; set; }
    }

    /// <summary>
    /// modbus的响应
    /// </summary>
    class ModbusResponse
    {

        [DataItem(0)]   // DataItem是数据项目，0表示第一项。
        public byte SlaveId { get; set; }

        [DataItem(1)]
        public byte FunCode { get; set; }

        [DataItem(2)]
        public byte BytesCount { get; set; }

        [DataItem(3, "AB")]
        [OtherSize("BytesCount", true)]   // 由BytesCount指定了字节数，是以字节计数的。
        public ushort[] Data { get; set; }

        [DataItem(4)]
        [DefaultSize(2)]     // 固定的2个元素。
        [CheckModbus(0, 3)]
        public byte[] Sum { get; set; }
    }




    class Program
    {
        static void Main(string[] args)
        {
            // 一个简单的modbus
            // 请求命令
            byte[] data1 = { 0x01, 0x03, 0x00, 0x00, 0x00, 0x01, 0x84, 0x0a };
            BytesSerializer bytesSerializer = new BytesSerializer();
            int end_index = 0;
            // 转成对象
            var obj = bytesSerializer.Deserialize<ModbusRequest>(data1, 0, ref end_index);
            // 再转回去
            var data2 = bytesSerializer.Serialize(obj);
            Console.WriteLine($"{data1.SequenceEqual(data2)}");
            // 回复的
            byte[] data10 = { 0x01, 0x03, 0x02, 0x19, 0x98, 0xb2, 0x7e};
            var obj2 = bytesSerializer.Deserialize<ModbusResponse>(data10,0, ref end_index);
            Console.WriteLine($"{Convert.ToString(obj2.Data[0], 16)}");

            // 这里添加一个更多的数组
            byte[] data20 = { 0x01,
                0x01, 0x03, 0x02, 0x19, 0x98, 0xb2, 0x7e ,
                0x01, 0x03, 0x02, 0x19, 0x98, 0xb2, 0x7e ,
                0x7e
            };
            var obj3 = bytesSerializer.Deserializes<ModbusResponse>(data20, 0, ref end_index);
            Console.WriteLine($"count; {obj3.Count}, end_index:{end_index}");



        }
    }
}


