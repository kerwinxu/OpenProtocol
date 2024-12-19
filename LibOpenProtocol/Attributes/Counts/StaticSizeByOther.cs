using System.Reflection;

namespace Io.Github.KerwinXu.OpenProtocol.Attributes.Counts
{
    /// <summary>
    /// 在某个变量为某个值的情况下，固定多少个
    /// </summary>
    public class StaticSizeByOther: BaseOnOtherSize
    {
        public StaticSizeByOther(string other_name, object other_value, int size, bool isBytesCount=false) { 
            this.OtherName = other_name;
            this.OtherValue = other_value;
            this.Value = size;
            this.IsBytesCount = isBytesCount;
        }

    }
}
