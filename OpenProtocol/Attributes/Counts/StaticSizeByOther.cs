using Io.Github.KerwinXu.OpenProtocol.Attributes.Sizes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Io.Github.KerwinXu.OpenProtocol.Attributes.Size
{
    /// <summary>
    /// 在某个变量为某个值的情况下，固定多少个
    /// </summary>
    public class StaticSizeByOther: OtherSize
    {
        public StaticSizeByOther(string other_name, object other_value, int size, bool isByteCount=false) { 
            this.OtherName = other_name;
            this.OtherValue = other_value;
            this.Value = size;
            this.IsByteCount = isByteCount;
        }


    }
}
