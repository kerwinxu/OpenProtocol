using System;
using System.Collections.Generic;
using System.Text;

namespace Io.Github.KerwinXu.OpenProtocol.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class IncompleteException:Exception
    {
        public byte[] Data { get; set; }

        public string MemberName { get; set; }

        public int StartIndex { get; set; }

        public int Count { get; set; }

        public IncompleteException(byte[] data, string memberName, int startIndex, int count) { 
            Data = data;
            MemberName = memberName;
            StartIndex = startIndex;
            Count = count;
        }
    }
}
