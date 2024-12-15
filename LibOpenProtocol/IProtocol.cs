using System;
using System.Collections.Generic;
using System.Text;

namespace Io.Github.KerwinXu.OpenProtocol
{
    /// <summary>
    /// 协议的基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public  interface IProtocol
    {

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="data">字节数组或者队列</param>
        /// <param name="start_index">起始下标</param>
        /// <param name="end_index">结束下标</param>
        /// <returns></returns>
        T Deserialize<T>(IList<byte> data, int start_index, ref int end_index) where T : new();

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        byte[] Serialize<T>(T t) where T : new();
    }
}
