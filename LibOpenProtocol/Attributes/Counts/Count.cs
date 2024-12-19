using Io.Github.KerwinXu.OpenProtocol.Utils;
using System;
using System.Reflection;

namespace Io.Github.KerwinXu.OpenProtocol.Attributes.Counts
{
    /// <summary>
    /// 数组的元素个数。
    /// </summary>
    public abstract  class Count:Attribute,ICount
    {
        public int Value { get; set; }

        /// <summary>
        /// 是否是字节计数
        /// </summary>
        public bool IsBytesCount { get; set; }

        public virtual int getCount(object obj, MemberInfo memberInfo)
        {
            // 如果是字节的，那么
            if (IsBytesCount)
            {
                var _type = Member.GetMemberType(memberInfo);
                int _count = TypeSize.SizeOfByType(_type);
                return Value / _count;
            }
            else
            {
                return Value;
            }

            
            //throw new NotImplementedException();
        }

        public virtual bool IsAvailable(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
