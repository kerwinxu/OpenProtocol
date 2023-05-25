using System;
using System.Collections.Generic;
using System.Text;

namespace Io.Github.KerwinXu.OpenProtocol.Attributes.Sizes
{
    /// <summary>
    /// 根据别的判断大小
    /// </summary>
    public class OtherSize: Count
    {
        public string OtherName { get; set; }

        public object OtherValue { get; set; }
    }
}
