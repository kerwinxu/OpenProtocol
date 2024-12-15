using System;
using System.Collections.Generic;
using System.Text;

namespace Io.Github.KerwinXu.OpenProtocol.Attributes
{
    /// <summary>
    /// 校验方法的
    /// </summary>
    public abstract  class Check:Attribute
    {
        // 这里写要哪种校验方法
        /// <summary>
        /// 起始的序号
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// 结束的序号
        /// </summary>
        public int EndIndex { get; set; }


        public abstract byte[] Compute(byte[] data);

    }
}
