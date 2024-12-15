using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Io.Github.KerwinXu.OpenProtocol.Exceptions
{
    public  class UnknownMember:Exception
    {
        public string MemberName { get; set; }
        public UnknownMember(string memberName):base($"不识别的成员:{memberName}") {
            MemberName = memberName;
        }
    }
}
