using Io.Github.KerwinXu.OpenProtocol.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Io.Github.KerwinXu.OpenProtocol.Utils
{
    /// <summary>
    /// 调节字节序的
    /// </summary>
    public  class AdjustEndianness
    {
        /// <summary>
        /// 协议字节序转成本机字节序
        /// </summary>
        /// <param name="data"></param>
        /// <param name="endianness"></param>
        /// <returns></returns>
        public static byte[] ProtocolToMachine(byte[] data, string? endianness = null)
        {
            if (string.IsNullOrWhiteSpace(endianness)) return data;    // 如果没有调整，就返回原址
            endianness = endianness.ToUpper();                         // 都转成大写。
            var endianness2 = endianness.ToArray();                    // 转成字节数组。
            var endianness_sort = endianness.Select(x => x).ToArray(); // 转成字符数组
            Array.Sort(endianness_sort);                               // 排序，从小到到
            Array.Reverse(endianness_sort);                            // 这里转成从大到小。因为A表示最大的。
            // 排序后的，是小端，如果机器也是小端，就不与用更改了。
            if (!BitConverter.IsLittleEndian) Array.Reverse(endianness_sort); // 如果不是小端，就反转。
            if (data.Length != endianness.Length) throw new EndiannessException();  // 如果个数不对，也调整
            byte[] result = new byte[data.Length];  // 动态申请一个控件。
            // 因为暂时我了解的电脑上，要么是ABCD，要么是DCBA，只是传输的数据存在其他的方式，
            for (int i = 0; i < endianness2.Length; i++)
            {
                // 其实就是将endianness转成endianness_sort
                var tmp1 = data[i];           // 数据
                var tmp2 = endianness2[i];    // 协议序中的字母
                var tmp3 = Array.IndexOf(endianness_sort, tmp2); // 这个字母在机器字节序中的位置
                result[tmp3] = tmp1;
            }
            return result;
        }

        /// <summary>
        /// 本机字节序转成协议字节序。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="endianness"></param>
        /// <returns></returns>
        public static byte[]MachineToProtocol(byte[] data, string? endianness = null)
        {
            if (string.IsNullOrWhiteSpace(endianness)) return data;    // 如果没有调整，就返回原址
            endianness = endianness.ToUpper();                         // 都转成大写。
            var endianness2 = endianness.ToArray();                    // 转成字节数组。
            var endianness_sort = endianness.Select(x => x).ToArray(); // 转成字符数组
            Array.Sort(endianness_sort);                               // 排序，从小到到
            Array.Reverse(endianness_sort);                            // 这里转成从大到小。因为A表示最大的。
            // 排序后的，是小端，如果机器也是小端，就不与用更改了。
            if (!BitConverter.IsLittleEndian) Array.Reverse(endianness_sort); // 如果不是小端，就反转。
            if (data.Length != endianness.Length) throw new EndiannessException();  // 如果个数不对，也调整
            byte[] result = new byte[data.Length];  // 动态申请一个控件。
            // 因为暂时我了解的电脑上，要么是ABCD，要么是DCBA，只是传输的数据存在其他的方式，
            for (int i = 0; i < endianness2.Length; i++)
            {
                // 其实就是将endianness转成endianness_sort
                var tmp1 = data[i];               // 数据
                var tmp2 = endianness_sort[i];    // 本机序中的字母
                var tmp3 = Array.IndexOf(endianness2, tmp2); // 这个字母在协议字节序中的位置
                result[tmp3] = tmp1;
            }
            return result;

        }



    }
}
