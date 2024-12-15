using System;
using System.Collections.Generic;
using System.Text;
using Io.Github.KerwinXu.OpenProtocol.Attributes;

namespace Io.Github.KerwinXu.OpenProtocol.Attributes.Checks
{
    public class CheckCRC16 : Check
    {
        /// <summary>
        /// 多项式
        /// </summary>
        public ushort POLY { get; set; }

     
        /// <summary>
        /// 初始值
        /// </summary>
        public ushort INITValue { get; set; }

        /// <summary>
        /// 字节序，可以是null，如果是null或者空，表示不做处理
        /// </summary>
        public string? Endianness { get; set; }

        public CheckCRC16(int start, int end, ushort poly, ushort initvalue,string endianness="")
        {
            this.StartIndex = start;
            this.EndIndex = end;
            this.POLY = poly;
            this.INITValue = initvalue;
            this.Endianness = endianness;
        }


     

        public override byte[] Compute(byte[] datas)
        {
            ushort crc = INITValue;
            for (int i = 0; i < datas.Length; i++)
            {
                crc ^= datas[StartIndex + i];   // 取出一个8字节跟crc做异或，结果放在crc中
                for (int j = 0; j < 8; j++)     // 8位，循环8次
                {
                    if((crc &0x0001)!=0)
                    {
                        crc >>= 1;
                        crc ^= POLY;
                    }
                    else
                    {
                        crc >>= 1;
                    }

                } 
            }
            // 
            var tmp = BitConverter.GetBytes(crc);           // 转成字节
            return Utils.AdjustEndianness.MachineToProtocol(tmp, Endianness); // 调整字节序

        }

    }
}
