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
        /// 字节序是否是大端
        /// </summary>
        public bool BigEndian { get; set; }

        

        public DataItem(int index) { Index = index; }

        public DataItem(int index, bool bigEndian) : this(index)
        {
            BigEndian = bigEndian;
        }
    }
}
