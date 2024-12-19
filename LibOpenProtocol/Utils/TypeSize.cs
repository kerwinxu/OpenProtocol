using Io.Github.KerwinXu.OpenProtocol.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Io.Github.KerwinXu.OpenProtocol.Utils
{
    /// <summary>
    /// 类型
    /// </summary>
    public class TypeSize
    {
        private static Dictionary<string, int> SIZEOFS = new Dictionary<string, int>() {
            // 单个元素的
            { typeof(sbyte).Name, 1 },
            { typeof(byte).Name, 1 },
            //
            { typeof(short).Name,2 },
            { typeof(ushort).Name,2 },
            //
            { typeof(int).Name, 4 },
            { typeof(uint).Name, 4 },
            //
            { typeof(long).Name, 8 },
            { typeof(ulong).Name, 8 },
            //
            { typeof(float).Name, 4 },
            { typeof(double).Name, 8 },
            //
           //{ typeof(decimal).Name, 16 },
            //
            { typeof(char).Name,2 },
            //
            { typeof(bool).Name,1 },
            // 数组的形式
            { typeof(sbyte[]).Name, 1 },
            { typeof(byte[]).Name, 1 },
            //
            { typeof(short[]).Name,2 },
            { typeof(ushort[]).Name,2 },
            //
            { typeof(int[]).Name, 4 },
            { typeof(uint[]).Name, 4 },
            //
            { typeof(long[]).Name, 8 },
            { typeof(ulong[]).Name, 8 },
            //
            { typeof(float[]).Name, 4 },
            { typeof(double[]).Name, 8 },
            //
           //{ typeof(decimal[]).Name, 16 },
            //
            { typeof(char[]).Name,2 },
            //
            { typeof(bool[]).Name,1 },
        };


        /// <summary>
        ///  取得字节数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int SizeOfByType(Type type)
        {
            if (type == null) return 0;
            if (SIZEOFS.ContainsKey(type.Name))
            {
                return SIZEOFS[type.Name];
            }
            throw new UnsupportedTypeException(type);
        }

    }
}
