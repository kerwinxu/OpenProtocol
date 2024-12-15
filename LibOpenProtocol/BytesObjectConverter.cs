using Io.Github.KerwinXu.OpenProtocol.Attributes;
using Io.Github.KerwinXu.OpenProtocol.Attributes.Counts;
using Io.Github.KerwinXu.OpenProtocol.Exceptions;
using Io.Github.KerwinXu.OpenProtocol.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Io.Github.KerwinXu.OpenProtocol
{
    /// <summary>
    /// 我自己写的方便字节转换的
    /// </summary>
    public  class BytesObjectConverter
    {

        #region 公共的函数


        private static Dictionary<string, int> SIZEOFS = new Dictionary<string, int>() {
            // 单个元素的
            { typeof(sbyte).Name, 1 },
            { typeof(byte).Name, 1 },
            //
            { typeof(short).Name,2 },
            { typeof(ushort).Name,2 },
            //
            { typeof(int).Name, 4 },
            { typeof(uint).Name, 4 },
            //
            { typeof(long).Name, 8 },
            { typeof(ulong).Name, 8 },
            //
            { typeof(float).Name, 4 },
            { typeof(double).Name, 8 },
            //
           //{ typeof(decimal).Name, 16 },
            //
            { typeof(char).Name,2 },
            //
            { typeof(bool).Name,1 },
            // 数组的形式
            { typeof(sbyte[]).Name, 1 },
            { typeof(byte[]).Name, 1 },
            //
            { typeof(short[]).Name,2 },
            { typeof(ushort[]).Name,2 },
            //
            { typeof(int[]).Name, 4 },
            { typeof(uint[]).Name, 4 },
            //
            { typeof(long[]).Name, 8 },
            { typeof(ulong[]).Name, 8 },
            //
            { typeof(float[]).Name, 4 },
            { typeof(double[]).Name, 8 },
            //
           //{ typeof(decimal[]).Name, 16 },
            //
            { typeof(char[]).Name,2 },
            //
            { typeof(bool[]).Name,1 },
        };


        /// <summary>
        ///  取得字节数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int SizeOfByType(Type type)
        {
            if (type == null) return 0;
            if (SIZEOFS.ContainsKey(type.Name))
            {
                return SIZEOFS[type.Name];
            }
            throw new UnsupportedTypeException(type);
        }

        /// <summary>
        /// 取得有DataItemde的成员
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<MemberInfo> GetHasDataItemMemberInfos(Type type)
        {
            List<MemberInfo> members = new List<MemberInfo>();
            members.AddRange(type.GetProperties().Where(x => x.GetCustomAttributes(false) != null && x.GetCustomAttributes(false).Where(y => y is DataItem).Any()));
            members.AddRange(type.GetFields().Where(x => x.GetCustomAttributes(false) != null && x.GetCustomAttributes(false).Where(y => y is DataItem).Any()));
            // 然后排序
            members.Sort((x, y) => getIndex(x).CompareTo(getIndex(y)));
            // 最后返回
            return members;
        }

        /// <summary>
        /// 取得序号
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private int getIndex(MemberInfo member)
        {
            return ((DataItem)member.GetCustomAttributes(false).Where(x => x is DataItem).First()).Index;
        }

        /// <summary>
        /// 取得某个成员的数据项目类注解
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private DataItem getDataItem(MemberInfo member)
        {
            return ((DataItem)member.GetCustomAttributes(false).Where(x => x is DataItem).FirstOrDefault());
        }


        /// <summary>
        /// 位置的类，DateItem中的序号跟实际字节中的序号的对应
        /// </summary>
        class Position
        {
            /// <summary>
            /// DataItem中的序号
            /// </summary>
            public int MemberIndex { get; set; }

            /// <summary>
            /// 字节数组中的序号
            /// </summary>
            public int BytesIndex { get; set; }

            /// <summary>
            /// 这个元素占用了几个字节。
            /// </summary>
            public int BytesCount { get; set; }

            /// <summary>
            /// 一个方便的构造函数
            /// </summary>
            /// <param name="memberIndex"></param>
            /// <param name="bytesIndex"></param>
            /// <param name="bytesCount"></param>
            public Position(int memberIndex, int bytesIndex, int bytesCount)
            {
                MemberIndex = memberIndex;
                BytesIndex = bytesIndex;
                BytesCount = bytesCount;
            }
        }


        private Type getMemberType(MemberInfo memberInfo)
        {
            if(memberInfo is FieldInfo)
            {
               return  ((FieldInfo)memberInfo).FieldType;
            }
            else 
            {
                return ((PropertyInfo)memberInfo).PropertyType;
            }
        }

        /// <summary>
        /// 取得一个数组元素的大小
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private int getArrayMemberElementSize(MemberInfo member)
        {
            var type = getMemberType(member);
            if (type.IsArray)
            {
                return SizeOfByType(type.GetElementType());
            }
            // 
            return 1;
        }

        /// <summary>
        /// 取得某个成员的字节数
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private int getMemberCount(object obj, MemberInfo memberInfo)
        {
            var _base_on_size = memberInfo.GetCustomAttributes(false).Where(x => x is BaseOnOtherSize).ToArray();// 可能存在多个条件的情况
            var _other_size = (OtherSize)memberInfo.GetCustomAttributes(false).Where(x => x is OtherSize).FirstOrDefault();
            var _count_element = getArrayMemberElementSize(memberInfo); // 计算单个元素的所占的字节数量
            var _default_size = memberInfo.GetCustomAttributes(false).Where(x => x is DefaultSize).FirstOrDefault();
            var count = 0;
            if(_other_size != null)            // 如果由其他项目指定个数
            {
                var _other_value = get_value_by_member_name(obj, _other_size.OtherName); // 取得某个成员的值
                count =  Convert.ToInt32( _other_value);
                // 要判断是否字节计数
                if (_other_size.IsBytesCount) { count = count / _count_element; }
                return count;
                
            }
            else if (_base_on_size.Length > 0) // 如果存在依照别的字段的值
            {
                foreach (BaseOnOtherSize item1 in _base_on_size)
                {
                    var _other_value = get_value_by_member_name(obj, item1.OtherName); // 取得某个成员的值
                    if (_other_value != null && _other_value.Equals(item1.OtherValue))    // 判断是否相等
                    {
                        // 这里表示相等，有两种情况
                        if (item1 is StaticSizeByOther) // 如果是固定的
                        {
                            count = item1.Value;

                        }
                        else if (item1 is VarSizeByOther)
                        {
                            // 如果是可变的，那么还得找另一个成员
                            var _other_size_value = get_value_by_member_name(obj, ((VarSizeByOther)item1).OtherSize);
                            // TODO 要加入异常，没有某个成员
                            count = (int)_other_size_value;

                        }
                        // 这里要判断是否是字节计数
                        if (item1.IsBytesCount) count /= _count_element; // 不是字节计数需要相乘。
                        return count; // 这里得到字节数了。
                    }

                }
            }
            // 这里判断是否是常量
            if (_default_size != null)
            {
                var item2 = _default_size as DefaultSize;
                count = item2.Value;
                if (item2.IsBytesCount) count /= _count_element;
                return count; // 常量的字节
            }

            // 最后按照成员自己的给出
            // 如果到这里还没找到，那么表示这里是要给字段了。
            if (memberInfo is FieldInfo && ((FieldInfo)memberInfo).FieldType.IsArray)  
            {
                var _array = (Array)get_value_by_member_name(obj, memberInfo.Name);  // 转成数组
                return _array.Length;                                   // 总的个数

            }
            // 默认是一个元素
            return 1;
        }

        /// <summary>
        /// 取得某个实例的某个成员名称的值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="member_name"></param>
        /// <returns></returns>
        private object? get_value_by_member_name(object obj, string member_name)
        {
            var _memberinfo = obj.GetType().GetMember(member_name).FirstOrDefault();// 或许不止一个，这里暴力当作取得第一个吧。
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
            throw new UnknownMember(member_name);
            // 
            // return null;

        }

        #endregion


        #region 反序列化

        /// <summary>
        /// 将字节数组转成成员
        /// </summary>
        /// <param name="data"></param>
        /// <param name="memberInfo"></param>
        /// <param name="count"></param>
        /// <param name="start_index"></param>
        /// <param name="end_index"></param>
        /// <returns></returns>
        public object BytesToMember(IList<byte> data, MemberInfo memberInfo, int count, int start_index, ref int end_index)
        {
            // 这里要取得字节序
            DataItem number_type = (DataItem)memberInfo.GetCustomAttributes(false).Where(x => x is DataItem).FirstOrDefault();
            string endianness = number_type != null ? number_type.Endianness : string.Empty;
            Type member_type = memberInfo is FieldInfo ? ((FieldInfo)memberInfo).FieldType : ((PropertyInfo)memberInfo).PropertyType;

            object member_value = null;          // 成员的值
            if (Judgement.IsBulitinType(member_type)) // 如果是基本类型
            {
                // 这里要截取数据
                member_value = BytesToBulitinType(data, start_index, ref end_index, member_type, endianness);
            }
            else if (member_type.IsArray)    // 这里判断是否是数组
            {
                member_value = Array.CreateInstance(member_type.GetElementType(), count); // 声明数组
                // 遍历这么多
                for (int i = 0; i < count; i++)
                {
                    object arr_item = null;
                    if (Judgement.IsBulitinType(member_type.GetElementType())) // 基本数据类型
                    {
                        arr_item = BytesToBulitinType(data, start_index, ref end_index, member_type.GetElementType(), endianness);
                    }
                    else
                    {
                        arr_item = BytesToObject(data, start_index, ref end_index, member_type.GetElementType());
                    }

                    ((Array)member_value).SetValue(arr_item, i);
                    start_index = end_index;
                }
                
            }
            else                                 // 递归其他类型
            {
                member_value = BytesToObject(data, start_index, ref end_index, member_type);
            }



            return member_value;

        }

        /// <summary>
        /// 字节转成对象
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object BytesToObject(IList<byte>data, int start_index, ref int end_index, Type type)
        {
            // 动态创建对象
            var result = Activator.CreateInstance(type);

            List<MemberInfo> members = GetHasDataItemMemberInfos(type); // 取得有注解的成员
            List<Position> positions = new List<Position>();             // 位置对照信息的,
            foreach (var member in members)                              // 对成员进行迭代
            {
                var _count = getMemberCount(result, member);             // 取得数量
                // 注意，如上的获得的是数组的数量，而不是单个元素字节的数量
                if (_count == 0) continue;                               // 不需要的化，直接下一个
                if (start_index + _count > data.Count) { throw new IncompleteException(); }  // 字节个数不够
                var _check = member.GetCustomAttributes(false).Where(x => x is Check).FirstOrDefault();                          // 检查是否是校验的
               
                // 要将如下的搞成要给函数。
                object member_value = BytesToMember(data, member, _count, start_index, ref end_index);          // 成员的值
                // 更新位置
                positions.Add(new Position(getIndex(member), start_index, end_index - start_index));
                // 这里要判断字段
                if (member is FieldInfo)           // 如果是字段
                {
                    // 这里要判断相等
                    var member_value_old = get_value_by_member_name(result, member.Name);
                    if(!Judgement.ObjectCompare(member_value_old, member_value))
                    {
                        throw new NotMatchException(member.Name);
                    }
                }else if (_check != null)
                {
                    var check_start = positions.Where(x => x.MemberIndex == ((Check)_check).StartIndex).FirstOrDefault();
                    var check_end = positions.Where(x => x.MemberIndex == ((Check)_check).EndIndex).FirstOrDefault();
                    // 
                    if (check_start == null) throw new CheckException(); // TODO 详细的错误
                    if (check_end == null) throw new CheckException(); // TODO 详细的错误
                                                                        // 取得字节数组，这个要做校验。
                    var data3 = data.Skip(check_start.MemberIndex).Take(check_end.BytesIndex + check_end.BytesCount - check_start.MemberIndex).ToArray();
                    // 计算出校验
                    var data4 = ((Check)_check).Compute(data3); // 
                    // 计算校验
                    if(!Judgement.ObjectCompare(data4, member_value))
                    {
                        throw new CheckException();
                    }

                }
                else if( member is  PropertyInfo)  // 如果是属性
                {
                    // 直接赋值吧。
                    ((PropertyInfo)member).SetValue(result, member_value, null); 
                }
                   
                
                // 下一位。
                start_index = end_index;

            }



            return result;

        }

        /// <summary>
        /// 字节数组转成基本的数据类型
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start_index"></param>
        /// <param name="end_index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object BytesToBulitinType(IList<byte> data, int start_index, ref int end_index, Type type, string? endianness = null) {

            // 占用的长度
            var count = SizeOfByType(type);
            end_index = start_index +  count;
            if (end_index > data.Count) throw new IncompleteException(); // 字节不够的异常

            var data2 = data.Skip(start_index).Take(count).ToArray();                     // 截取字节数组
            if (endianness != null) data2 = Utils.AdjustEndianness.ProtocolToMachine(data2, endianness); // 调整字节

            // 根据类型判断需要怎样转换
            if (type.Name == typeof(sbyte).Name)
            {
                return (sbyte)data2[0];
            }
            else if (type.Name == typeof(byte).Name)
            {
                return (byte)data2[0];
            }
            else if (type.Name == typeof(short).Name)
            {
                return BitConverter.ToInt16(data2, 0);
            }
            else if (type.Name == typeof(ushort).Name)
            {
                return BitConverter.ToUInt16(data2, 0);
            }
            else if (type.Name == typeof(int).Name)
            {
                return BitConverter.ToInt32(data2, 0);
            }
            else if (type.Name == typeof(uint).Name)
            {
                return BitConverter.ToUInt32(data2, 0);
            }
            else if (type.Name == typeof(long).Name)
            {
                return BitConverter.ToInt64(data2, 0);
            }
            else if (type.Name == typeof(ulong).Name)
            {
                return BitConverter.ToUInt64(data2, 0);
            }
            else if (type.Name == typeof(float).Name)
            {
                return BitConverter.ToSingle(data2, 0);
            }
            else if (type.Name == typeof(double).Name)
            {
                return BitConverter.ToDouble(data2, 0);
            }
            else if (type.Name == typeof(char).Name)
            {
                return BitConverter.ToChar(data2, 0);
            }
            else if (type.Name == typeof(bool).Name)
            {
                return BitConverter.ToBoolean(data2, 0);
            }
            // 字符串类型暂时当作字符类型的组合吧。
            else if (type.Name == typeof(string).Name)
            {
                return BitConverter.ToChar(data2, 0);
            }
            // 暂时不支持。
            //else if (type.Name == typeof(decimal).Name)
            //{
            //    return BitConverter.de(data2, 0);
            //}

            // TODO 以后加上异常，不支持的类型
            return null;

        }



        #endregion


        #region 序列化

        /// <summary>
        /// 将基本的数据类型转成字节
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="endianness"></param>
        /// <returns></returns>
        public byte[] BulitinTypeToBytes(object obj , string? endianness = null)
        {
            Type type = obj.GetType();           // 取得数据类型
            List<byte> bytes = new List<byte>(); // 缓冲区

            if (type == typeof(sbyte)) {
                bytes.Add((byte)obj);
            }else if (type == typeof(byte))
            {
                bytes.Add((byte)obj);
            }
            else if (type == typeof(ushort))
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)obj));
            }
            else if (type == typeof(short))
            {
                bytes.AddRange(BitConverter.GetBytes((short)obj));
            }
            else if (type == typeof(uint))
            {
                bytes.AddRange(BitConverter.GetBytes((uint)obj));
            }
            else if (type == typeof(int))
            {
                bytes.AddRange(BitConverter.GetBytes((int)obj));
            }
            else if (type == typeof(long))
            {
                bytes.AddRange(BitConverter.GetBytes((long)obj));
            }
            else if (type == typeof(ulong))
            {
                bytes.AddRange(BitConverter.GetBytes((ulong)obj));
            }
            else if (type == typeof(float))
            {
                bytes.AddRange(BitConverter.GetBytes((float)obj));
            }
            else if (type == typeof(double))
            {
                bytes.AddRange(BitConverter.GetBytes((double)obj));
            }
            else if (type == typeof(char))
            {
                bytes.AddRange(BitConverter.GetBytes((char)obj));
            }
            else if (type == typeof(bool))
            {
                bytes.AddRange(BitConverter.GetBytes((bool)obj));
            }
            var arr_bytes = bytes.ToArray();
            arr_bytes = AdjustEndianness.MachineToProtocol(arr_bytes, endianness);
            return arr_bytes;

        }


        /// <summary>
        /// 将一个对象转成字节数组
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public byte[]ObjectToBytes(object obj)
        {
            List<byte> bytes = new List<byte>();
            if(obj != null)
            {
                List<MemberInfo> members = GetHasDataItemMemberInfos(obj.GetType()); // 取得有注解的成员
                List<Position> positions = new List<Position>();                     // 位置对照信息的,
                foreach (MemberInfo member in members)                               // 遍历所有的成员
                {
                    // 首先要检查是否是校验
                    // 然后判断是否是基本数据类型
                    // 然后判断是否是数组
                    // 最后判断遍历其他的。
                    int old_index = bytes.Count();  // 这里是原先的下标
                    // 取得这个变量的类型
                    Type type = member is FieldInfo ? ((FieldInfo)member).FieldType : ((PropertyInfo)member).PropertyType;
                    var _check = (Check)member.GetCustomAttributes(false).Where(x => x is Check).FirstOrDefault();
                    var member_value = get_value_by_member_name(obj, member.Name);
                    var dataItem = getDataItem(member);
                    if (_check!=null) // 如果这个是校验
                    {
                        // 校验
                        var check_start = positions.Where(x => x.MemberIndex == ((Check)_check).StartIndex).FirstOrDefault();
                        var check_end = positions.Where(x => x.MemberIndex == ((Check)_check).EndIndex).FirstOrDefault();
                        // 
                        if (check_start == null) throw new CheckException(); // TODO 详细的错误
                        if (check_end == null) throw new CheckException(); // TODO 详细的错误
                                                                           // 取得字节数组，这个要做校验。
                        var data3 = bytes.Skip(check_start.MemberIndex).Take(check_end.BytesIndex + check_end.BytesCount - check_start.MemberIndex).ToArray();
                        // 计算出校验
                        var data4 = ((Check)_check).Compute(data3); // 
                        bytes.AddRange(data4);

                    }
                    else if(member_value == null)
                    {
                        // TODO 这里看看是否加一个异常。
                    }
                    else if (Judgement.IsBulitinType(type))
                    {
                        // 取得变量的值
                        var bytes2 = BulitinTypeToBytes(member_value, dataItem.Endianness);
                        bytes.AddRange(bytes2);
                    }else if (type.IsArray) // 如果是数组
                    {
                        // 这里只是区分基本类型和其他
                        Array arr = (Array)member_value;
                        for (int i = 0; i < arr.Length; i++)
                        {
                            object arr_item = arr.GetValue(i);
                            if (Judgement.IsBulitinType(arr_item.GetType()))
                            {
                                var bytes2 = BulitinTypeToBytes(arr_item, dataItem.Endianness);
                                bytes.AddRange(bytes2);
                            }
                            else
                            {
                                var bytes2 = ObjectToBytes(arr_item);
                                bytes.AddRange(bytes2);
                            }
                        }
                    }
                    else  // 其他情况,递归。
                    {
                        var tmp = ObjectToBytes(member_value);
                        bytes.AddRange(tmp);
                    }
                    // 记录位置的。
                    positions.Add(new Position(dataItem.Index, old_index, bytes.Count() - old_index));

                }
            }

            return bytes.ToArray();
        }



        #endregion

    }
}
