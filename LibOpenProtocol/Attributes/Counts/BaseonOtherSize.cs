namespace Io.Github.KerwinXu.OpenProtocol.Attributes.Counts
{
    /// <summary>
    /// 根据别的判断大小
    /// </summary>
    public class BaseOnOtherSize: Count
    {
        public string OtherName { get; set; }

        public object OtherValue { get; set; }
    }
}
