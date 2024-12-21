using System;
using System.Collections.Generic;
using System.Linq;

namespace Io.Github.KerwinXu.OpenProtocol
{
    /// <summary>
    /// 二进制序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public class BytesSerializer : IProtocol
    {
        /// <summary>
        /// 私有的。
        /// </summary>
        private BytesObjectConverter bytesObjectConverter = new BytesObjectConverter();

        public T Deserialize<T>(IList<byte> data, int start_index, ref int end_index) where T : new()
        {
            return  (T)bytesObjectConverter.BytesToObject(data,start_index,ref end_index,typeof(T));
            //throw new NotImplementedException();
        }

        public IList<T> Deserializes<T>(IList<byte> data, int start_index, ref int end_index) where T : new()
        {
            // 返回数据
            List<T> lst = new List<T>();
            int end_index_old = 0; ;
            if(start_index < data.Count)
            {
                for (int i = start_index; i < data.Count; i++)
                {
                    try
                    {
                        // 这里是遍历所有的情况
                        T t = Deserialize<T>(data, i, ref end_index);
                        // 如果找到一个合适的
                        if (t != null)
                        {
                            lst.Add(t);               // 添加到列表
                            i = end_index - 1;        // 下一个接着来
                            end_index_old = end_index;// 保存这个，

                        }
                    }
                    catch (Exception e)
                    {
                        // 取消所有的异常
                        //throw;
                    }
                }
            }
            end_index = end_index_old; // 这个真正寻找的最后。
            return lst;
            //throw new NotImplementedException();
        }

        public byte[] Serialize<T>(T t) where T : new()
        {
            return bytesObjectConverter.ObjectToBytes(t);
            //throw new NotImplementedException();
        }
    }
}
