using Io.Github.KerwinXu.OpenProtocol.Attributes.Sizes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Io.Github.KerwinXu.OpenProtocol.Attributes
{
    /// <summary>
    /// 在某个变量为某个值的情况下，某个变量的值是个数
    /// </summary>
    public class VarSizeByOther: OtherSize
    {

        public string OtherSize { get; set; }

        public VarSizeByOther(string other_name, object other_value, string  other_size)
        {
            this.OtherName = other_name;
            this.OtherValue = other_value;
            this.OtherSize = other_size;
        }
    }
}
