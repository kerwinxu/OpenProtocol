using System;
using System.Collections.Generic;
using System.Text;

namespace Io.Github.KerwinXu.OpenProtocol.Exceptions
{
    public  class NotMatchException:Exception
    {
        public string MemberName { get; set; }

        public NotMatchException(string  memberName)
        {
            this.MemberName = memberName;
        }
    }
}
