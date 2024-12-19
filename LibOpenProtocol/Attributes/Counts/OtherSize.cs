using Io.Github.KerwinXu.OpenProtocol.Utils;
using System.Reflection;

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

        public override int getCount(object obj, MemberInfo memberInfo)
        {
            var _other_size_value = (int)Member.getValue(obj, OtherName);
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

        public override bool IsAvailable(object obj)
        {
            return true;
            return base.IsAvailable(obj);
        }
    }
}
