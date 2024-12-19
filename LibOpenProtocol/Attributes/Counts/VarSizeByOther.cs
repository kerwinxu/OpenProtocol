using Io.Github.KerwinXu.OpenProtocol.Utils;
using System.Reflection;

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

        public override int getCount(object obj, MemberInfo memberInfo)
        {
            // 取得其他变量的值
            var _other_size_value =(int) Member.getValue(obj, OtherSize);
            // 还要判断是否是字节的
            if (IsBytesCount)
            {
                var _type = Member.GetMemberType(memberInfo);
                var _count = TypeSize.SizeOfByType(_type);
                return _other_size_value / _count;
            }
            else
            {
                return _other_size_value;
            }
            return base.getCount(obj, memberInfo);
        }

    }
}
