using System;
using System.Collections.Generic;


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

        public byte[] Serialize<T>(T t) where T : new()
        {
            return bytesObjectConverter.ObjectToBytes(t);
            //throw new NotImplementedException();
        }
    }
}
