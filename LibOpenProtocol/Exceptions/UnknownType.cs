using System;
using System.Collections.Generic;
using System.Text;

namespace Io.Github.KerwinXu.OpenProtocol.Exceptions
{
    public  class UnknownType:Exception
    {
        public int ByteCount { get; set; }

        public bool IsSigned { get; set; }

        public UnknownType(int byteCount, bool isSigned)
        {
            ByteCount = byteCount;
            IsSigned = isSigned;
        }
    }
}
