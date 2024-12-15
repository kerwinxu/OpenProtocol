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
        public IncompleteException():base ("格式不完整") { 
        }
    }
}
