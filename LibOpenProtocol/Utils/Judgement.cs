using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Io.Github.KerwinXu.OpenProtocol.Utils
{
    /// <summary>
    /// 表示判断的小工具
    /// </summary>
    public class Judgement
    {
        /// <summary>
        /// 2个对象比较是否相等
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool ObjectCompare(object obj1, object obj2)
        {
            if (obj1 == null && obj2 == null)   // 如果都是空置
            {
                return true;
            }
            else if (obj1 == null)           // 只要一个是空值
            {
                return false;
            }
            else if (obj2 == null)           // 只要一个是空值
            {
                return false;
            }
            else if (obj1.GetType() != obj2.GetType())   // 如果类型不同，就不用比较了。
            {
                return false;
            }
            else if (IsBulitinType(obj1.GetType()) && IsBulitinType(obj2.GetType())) // 如果都是基本元素
            {
                return obj1.Equals(obj2);
            }
            else if (obj1.GetType().IsArray && obj2.GetType().IsArray)
            {           // 如果是数组
                Array arr1 = obj1 as Array;
                Array arr2 = obj2 as Array;
                if (arr1.Length != arr2.Length) return false; // 长度不同
                for (int i = 0; i < arr1.Length; i++)
                {
                    if (!ObjectCompare(arr1.GetValue(i), arr2.GetValue(i))) return false;

                }
                // 可以认为是相等的
                return true;
            }

            return obj1 == obj2;

        }

        /// <summary>
        /// 是否是基本数据类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsBulitinType(Type type)
        {
            return (type == typeof(object) || Type.GetTypeCode(type) != TypeCode.Object);
        }

    }
}
