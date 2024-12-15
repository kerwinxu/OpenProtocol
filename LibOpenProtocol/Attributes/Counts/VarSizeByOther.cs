namespace Io.Github.KerwinXu.OpenProtocol.Attributes.Counts
{
    /// <summary>
    /// 在某个变量为某个值的情况下，某个变量的值是个数
    /// </summary>
    public class VarSizeByOther: BaseOnOtherSize
    {

        public string OtherSize { get; set; }

        public VarSizeByOther(string other_name, object other_value, string  other_size, bool isBytesCount)
        {
            this.OtherName = other_name;
            this.OtherValue = other_value;
            this.OtherSize = other_size;
            this.IsBytesCount = isBytesCount;
        }
    }
}
