using Io.Github.KerwinXu.OpenProtocol.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Io.Github.KerwinXu.OpenProtocol.Utils
{
    /// <summary>
    /// 关于成员的一堆小工具
    /// </summary>
    public class Member
    {

        /// <summary>
        /// 取得成员的类型
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static Type GetMemberType(MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo)
            {
                return ((FieldInfo)memberInfo).FieldType;
            }
            else
            {
                return ((PropertyInfo)memberInfo).PropertyType;
            }
        }

        /// <summary>
        /// 返回某个对象某个成员的只
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static object getValue(object obj , string memberName)
        {
            var _memberinfo = obj.GetType().GetMember(memberName).FirstOrDefault();// 或许不止一个，这里暴力当作取得第一个吧。
            if (_memberinfo == null) return null; // 没有发现成员
            if (_memberinfo is FieldInfo) // 如果是字段
            {
                return ((FieldInfo)_memberinfo).GetValue(obj);

            }
            else if (_memberinfo is PropertyInfo)
            {
                return ((PropertyInfo)_memberinfo).GetValue(obj, null);
            }

            // 这里发出异常，不识别。
            throw new UnknownMember(memberName);

        }

    }
}
