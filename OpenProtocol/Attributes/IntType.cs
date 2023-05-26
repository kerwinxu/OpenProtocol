using System;
using System.Collections.Generic;
using System.Text;

namespace Io.Github.KerwinXu.OpenProtocol.Attributes
{
    /// <summary>
    /// 整数类型
    /// </summary>
    public  class IntType:BasicType
    {
        /// <summary>
        /// 字节数，1，2，4
        /// </summary>
        public int BytesCount { get; set; }

        /// <summary>
        /// 是否有负号
        /// </summary>
        public bool IsSigned { get; set; }


        /// <summary>
        /// 加一个倍数的
        /// </summary>
        public double Scale { get; set; }


        public IntType(int bytesCount, bool isSigned, double scale=1)
        {
            BytesCount = bytesCount;
            IsSigned = isSigned;
            Scale = scale;
        }
    }
}
