using System.Reflection;

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

        //public override int getCount(object obj, MemberInfo memberInfo)
        //{
        //    return base.getCount(obj, memberInfo);
        //}

        public override bool IsAvailable(object obj)
        {
            return true;// 都可用
            return base.IsAvailable(obj);
        }
    }
}
