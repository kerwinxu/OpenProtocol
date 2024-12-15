namespace Io.Github.KerwinXu.OpenProtocol.Attributes.Counts
{
    /// <summary>
    /// 默认长度
    /// </summary>
    public class DefaultSize : Count
    {
        public DefaultSize(int size, bool isBytesCount = false) {
            this.Value = size;
            this.IsBytesCount = isBytesCount;
        }
    }
}
