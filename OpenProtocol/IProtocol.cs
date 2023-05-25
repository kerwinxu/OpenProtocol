using System;
using System.Collections.Generic;
using System.Text;

namespace Io.Github.KerwinXu.OpenProtocol
{
    /// <summary>
    /// 协议的基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public  interface IProtocol<T> where T : class,  new ()
    {

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        T Deserialize(byte[] data);

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
          byte[] Serialize(T t);
    }
}
