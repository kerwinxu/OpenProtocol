using System;
using System.Collections.Generic;
using System.Text;

namespace Io.Github.KerwinXu.OpenProtocol.Attributes.Checks
{
    public  class CheckCRC8: Check
    {
        /// <summary>
        /// 多项式
        /// </summary>
        public byte POLY { get; set; }


        /// <summary>
        /// 初始值
        /// </summary>
        public byte INITValue { get; set; }

        /// <summary>
        /// 异或值
        /// </summary>
        public byte XOROUT { get; set; }

        /// <summary>
        /// 输入反转
        /// </summary>
        public bool InputReverse { get; set; }

        /// <summary>
        /// 输出反转
        /// </summary>
        public bool OutputReverse { get; set; }

        public CheckCRC8(int start, int end, byte poly, byte initvalue, byte xorout, bool inputReverse = true, bool outputReverse = true)
        {
            this.StartIndex = start;
            this.EndIndex = end;
            this.POLY = poly;
            this.INITValue = initvalue;
            this.XOROUT = xorout;
            this.InputReverse = inputReverse;
            this.OutputReverse = outputReverse;
        }


        public ushort reverse16(ushort data)
        {
            ushort result = 0;
            for (int i = 0; i < 16; i++)
            {
                result |= (ushort)(((data >> i) & 0x01) << (15 - i));
            }
            return result;
        }

        public byte reverse8(byte data)
        {
            byte result = 0;
            for (int i = 0; i < 8; i++)
            {
                result |= (byte)(((data >> i) & 0x01) << (7 - i));
            }
            return result;
        }


        public override object Compute(byte[] datas)
        {
            byte crc = INITValue;
            for (int i = 0; i < datas.Length; i++)
            {
                var data = datas[i];
                if (InputReverse) data = reverse8(data);
                crc = (byte)(crc ^ (data << 0));
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x80) == 0x80)
                    {

                        crc = (byte)((crc << 1) ^ POLY);
                    }
                    else
                    {
                        crc <<= 1;
                    }
                }
            }
            if (OutputReverse) crc = reverse8(crc);
            crc = (byte)(crc ^ XOROUT);
            return crc;

        }
    }
}
