using System;
using System.Collections.Generic;
using System.Text;

namespace Io.Github.KerwinXu.OpenProtocol.Exceptions
{
    /// <summary>
    /// 不支持的类型
    /// </summary>
    public  class UnsupportedTypeException:Exception
    {
        public Type Type { get; set; }

        public UnsupportedTypeException(Type type)
        {
            Type = type;
        }

        public override string ToString()
        {
            return $"Unsupported Type:{Type}";
            //return base.ToString();
        }
    }
}
