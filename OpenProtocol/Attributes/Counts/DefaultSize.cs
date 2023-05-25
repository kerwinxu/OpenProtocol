using Io.Github.KerwinXu.OpenProtocol.Attributes.Sizes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Io.Github.KerwinXu.OpenProtocol.Attributes.Size
{
    /// <summary>
    /// 默认长度
    /// </summary>
    public class DefaultSize : Count
    {
        public DefaultSize(int size, bool isByteCount=false) {
            this.Value = size;
            this.IsByteCount = isByteCount;
        }
    }
}
