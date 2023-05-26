using System;
using System.Collections.Generic;
using System.Text;

namespace Io.Github.KerwinXu.OpenProtocol.Attributes.Sizes
{

    public  class Count:Attribute
    {
        public int Value { get; set; }

        public bool IsByteCount { get; set; }

    }
}
