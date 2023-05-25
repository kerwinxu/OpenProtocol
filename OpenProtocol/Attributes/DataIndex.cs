using System;
using System.Collections.Generic;
using System.Text;

namespace Io.Github.KerwinXu.OpenProtocol.Attributes
{
    /// <summary>
    /// 序号
    /// </summary>
    public  class DataIndex:Attribute
    {
        public int Value { get; set; }
        public DataIndex(int v) { Value = v; }
   
    }
}
