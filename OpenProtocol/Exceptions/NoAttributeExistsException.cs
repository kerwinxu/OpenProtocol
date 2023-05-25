using System;
using System.Collections.Generic;
using System.Text;

namespace Io.Github.KerwinXu.OpenProtocol.Exceptions
{
    public class NoAttributeExistsException : Exception
    {
        public string PropertyName { get; set; }

        public NoAttributeExistsException(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
