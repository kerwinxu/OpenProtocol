using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Io.Github.KerwinXu.OpenProtocol.Attributes
{
    /// <summary>
    /// 个数的接口
    /// </summary>
    interface ICount
    {
        /// <summary>
        /// 取得某个对象，某个成员，如果是数组，那么取得个数，其他为1.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        int getCount(object obj, MemberInfo memberInfo);

        /// <summary>
        /// 这个接口现在可用
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool IsAvailable(object obj);
    }
}
