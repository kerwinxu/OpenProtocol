using System;

namespace Io.Github.KerwinXu.OpenProtocol.Attributes.Counts
{
    /// <summary>
    /// 数组的元素个数。
    /// </summary>
    public abstract  class Count:Attribute
    {
        public int Value { get; set; }

        /// <summary>
        /// 是否是字节计数
        /// </summary>
        public bool IsBytesCount { get; set; }

    }
}
