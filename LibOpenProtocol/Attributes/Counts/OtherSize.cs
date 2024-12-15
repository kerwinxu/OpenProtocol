namespace Io.Github.KerwinXu.OpenProtocol.Attributes.Counts
{
    /// <summary>
    /// 个数就是别的项目的值。
    /// </summary>
    public class OtherSize : Count
    {
        public string OtherName { get; set; }

        public OtherSize(string other_name, bool isBytesCount=false) 
        { 
            this.OtherName = other_name; 
            this.IsBytesCount = isBytesCount;
        }

    }
}
