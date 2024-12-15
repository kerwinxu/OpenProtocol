using System;
using System.Collections.Generic;
using System.Text;

namespace Io.Github.KerwinXu.OpenProtocol.Attributes
{
    /// <summary>
    /// 项目
    /// </summary>
    public  class DataItem:Attribute
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int Index { get; set; }


        /// <summary>
        /// 字节序，可以是null，如果是null，表示不做处理
        /// </summary>
        public string? Endianness { get; set; }


        public DataItem(int index, string? endianness = null)
        {
            Index = index; Endianness = endianness;
        }


    }
}
