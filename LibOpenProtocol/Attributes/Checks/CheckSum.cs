using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Io.Github.KerwinXu.OpenProtocol.Attributes;


namespace Io.Github.KerwinXu.OpenProtocol.Attributes.Checks
{
    public class CheckSum : Check
    {

        public override byte[] Compute(byte[] data)
        {
            byte[] result = new byte[1]; //只有一个
            var sum = data.Select(x => (int)x).Sum();  // 计算和
            sum = sum % 256;                           // 只保留低位
            sum = ~sum + 1;                            // 补码
            result[0] = (byte)sum;                     // 只是取得一位
            return result;

            //throw new NotImplementedException();
        }

        public CheckSum(int start, int end)
        {
            this.StartIndex = start;
            this.EndIndex = end;
        }
    }
}
