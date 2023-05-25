using Io.Github.KerwinXu.OpenProtocol.Attributes;
using Io.Github.KerwinXu.OpenProtocol.Attributes.Size;
using Io.Github.KerwinXu.OpenProtocol.Attributes.Sizes;
using Io.Github.KerwinXu.OpenProtocol.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Io.Github.KerwinXu.OpenProtocol
{
    /// <summary>
    /// 二进制序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public  class BytesSerializer<T>  :IProtocol<T> where T : class,  new() 
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public   T Deserialize(byte[] data)
        {
            // 首先创建一个对象
            var result = new T();
            // 取得类型
            var type = typeof(T);
            // 然后查看有多少个有相关注解的
            List<MemberInfo> members = new List<MemberInfo>();
            members.AddRange(type.GetProperties().Where(x => x.GetCustomAttributes() != null && x.GetCustomAttributes(false).Where(y => y is DataItem).Any()));
            members.AddRange(type.GetFields().Where(x => x.GetCustomAttributes() != null && x.GetCustomAttributes(false).Where(y => y is DataItem).Any()));
            // 然后排序
            members.Sort((x, y) => getIndex(x).CompareTo(getIndex(y)));
            // 这里遍历
            int i_start = 0; // 当前位置
            // 如下3个是一体的。
            List<int> member_indexs = new List<int>();// 有数据的序号。
            List<int> indexs = new List<int>();       // 报错每一个index
            List<int> counts = new List<int>();       // 保存每一个count
            foreach (var item in members)
            {
                if(item is FieldInfo) // 如果是字段，那意味着这个要判断相等，如果不相等，就返回null
                {
                    /// 请注意，这个字段只支持字节类型或者字节数组类型。
                    /// 并且，这个不用关心字节序。
                    FieldInfo fieldInfo = item as FieldInfo;

                    int count = 0;
                    if (fieldInfo.FieldType.IsArray) // 是否是数组
                    {
                        var arr = fieldInfo.GetValue(result) as byte[];                           // 数组
                        count = arr.Length *  sizeOfByType(fieldInfo.FieldType.GetElementType()); // 这个才是真正的长度
                        if (i_start + count > data.Length) throw new IncompleteException(data, item.Name, i_start, count); // 这里判断长度是否足够
                        byte[] data2 = data.Skip(i_start).Take(count).ToArray(); // 然后这里要取得这个值
                        if (arr.Length != data2.Length) throw new NotMatchException(item.Name);
                        for (int i = 0; i < arr.Length; i++)
                        {
                            // 每一项都判断。
                            if (arr[i] != data2[i]) throw new NotMatchException(item.Name); 
                        }
                    }
                    else
                    {   // 暂时这个字段只支持字节类型。
                        count = sizeOfByType(fieldInfo.FieldType); //  不是数组，
                        if (count != 1) throw new UnsupportedTypeException(fieldInfo.FieldType); // 固定是byte或者ubyte类型
                        if (i_start + count > data.Length) throw new IncompleteException(data, item.Name, i_start, count); // 判断是否有足够的空间
                        byte b = (byte)fieldInfo.GetValue(result);
                        if(b != data[i_start]) throw new NotMatchException(item.Name);
                       
                    }
                    // 先保存
                    member_indexs.Add(getIndex(item));
                    indexs.Add(i_start);
                    counts.Add(count);
                    // 然后再增加。
                    i_start += count;

                }
                else if (item is  PropertyInfo) // 如果是属性
                {
                    // 属性部分，需要注意的地方：
                    // 1. 数据类型问题，有特别的指定数据就按照指定的，没有就按照属性的数据类型
                    // 2. 占用的字节数问题，通常是由于这个属性的数据长度是由其他属性的值决定的。
                    // 3. 真正的取得值
                    // 4. 要判断是否校验，

                    // 1. 这里要判断数据类型问题
                    PropertyInfo propertyInfo = item as PropertyInfo;
                    bool isBigEndian = false;     // 字节序大小端
                    double scale = 0;             // 放大倍数
                    Type type1 = getPropertyType(propertyInfo, ref isBigEndian, ref scale);  // 类型。

                    // 2. 占用的字节数，有时候指定的类型不一定是属性中的类型。
                    int count = 0;
                    bool isByteCount = false;
                   // 如果配置中没有取得字节数量
                    if (!getPropertyBytesCount(propertyInfo, type, result, ref count))
                    {
                        // 这里要看类型
                        if (propertyInfo.PropertyType.IsArray)
                        {
                            // 这里要判断是否有初始化
                            var arr = propertyInfo.GetValue(result) as Array;
                            if(arr == null)
                            {
                                // TODO 异常，数组不知道长度
                            }
                            else
                            {
                                count = arr.Length * sizeOfByType(propertyInfo.PropertyType.GetElementType());
                            }
                        }
                        else
                        {
                            // 如果不是数组类型
                            count = sizeOfByType(propertyInfo.PropertyType);  // 这个类型的长度
                        }
                    }

                    // 这里要判断一下这个长度
                    if (count == 0) continue; // 下一个循环。
                    // 3. 赋值
                    if (i_start + count > data.Length) throw new IncompleteException(data, item.Name, i_start, count); // 不完整
                    byte[] data2 = data.Skip(i_start).Take(count).ToArray(); // 取得数组
                    // 要给这个变量设置值。
                    // 这里是分是否是数组的
                    if (propertyInfo.PropertyType.IsArray)
                    {
                        // 数组的话，要循环
                        var arr = propertyInfo.GetValue(result) as Array; // 转换成数组
                        var persize = sizeOfByType(type1);                // 每个占用的长度
                        // 这里要看看这个数组是否有空间
                        if (arr == null)
                        {
                            arr = Array.CreateInstance(type1, count / persize);  // 动态创建数组
                            propertyInfo.SetValue(result, arr);                  // 赋值
                        }
                        // 遍历赋值
                        for (int i = 0; i < arr.Length; i++)
                        {
                            // 计算
                            var obj = bytesToObject(data2.Skip(i * persize).Take(persize).ToArray(), type1, isBigEndian);
                            // 然后赋值
                            arr.SetValue(obj, i);  
                        }

                    }
                    else
                    {
                        // 这里设置单个值
                        propertyInfo.SetValue(result, bytesToObject(data2, type1, isBigEndian));

                    }
                    // 下一个。
                    // 先保存
                    member_indexs.Add(getIndex(item));
                    indexs.Add(i_start);
                    counts.Add(count);
                    // 然后再增加。
                    i_start += count;
                    // 4. 这里要判断是否是校验，不符合校验就返回null
                    // 这里判断一下是否有校验
                    var check = propertyInfo.GetCustomAttributes(false).Where(x => x is Check).FirstOrDefault();
                    if (check != null) // 表示有校验。
                    {
                        // 取得要计算校验的
                        var check_start = indexs[member_indexs.IndexOf(((Check)check).StartIndex)];
                        var check_end = indexs[member_indexs.IndexOf(((Check)check).EndIndex)];
                        var check_count = counts[member_indexs.IndexOf(((Check)check).EndIndex)];
                        // 取得字节数组，这个要做校验。
                        var data3 = data.Skip(check_start).Take(check_end + check_count - check_start).ToArray();
                        // 计算出校验
                        var data4 = ((Check)check).Compute(data3); // 
                        // TODO 然后比较，请注意，这里比较的是字节数组，data4先转成字节数组。
                        var data5 = objectToBytes(data4);
                        if (BitConverter.IsLittleEndian ^ isBigEndian) Array.Reverse(data5);
                        // 然后判断
                        if(!Enumerable.SequenceEqual(data5, data2))
                        {
                            // 异常，校验不符合。
                            throw new CheckException();
                        }

                    }

                }
            }
            return result;
            //throw new NotImplementedException();
        }

        



        private byte[] objectToBytes(object obj )
        {
            if (obj == null) return null;
            if (obj is byte[]) return obj as byte[];    
            if (obj is byte) return new byte[] { (byte)obj };
            if (obj is sbyte) return new byte []{ (byte)obj};
            if (obj is short) return BitConverter.GetBytes((short)obj);
            if (obj is ushort) return BitConverter.GetBytes((ushort)obj);
            if (obj is int) return BitConverter.GetBytes((int)obj);
            if (obj is uint) return BitConverter.GetBytes((uint)obj);
            if (obj is long) return BitConverter.GetBytes((long)obj);
            if (obj is ulong) return BitConverter.GetBytes((ulong)obj);
            if (obj is float) return BitConverter.GetBytes((float)obj);
            if (obj is double) return BitConverter.GetBytes((double)obj);
            return null;

        }

        public object bytesToObject(byte[]data, Type type, bool isBigEndian)
        {
            if (isBigEndian ^ BitConverter.IsLittleEndian) { Array.Reverse(data); } // 如果是大端，就反转
            // 然后根据类型返回相关的值
            if (type == typeof(byte)) return data[0];
            if (type == typeof(sbyte)) return data[0];
            if (type == typeof(ushort)) return BitConverter.ToUInt16(data);
            if (type == typeof(short)) return BitConverter.ToInt16(data);
            if (type == typeof(uint)) return BitConverter.ToUInt32(data);
            if (type == typeof(int)) return BitConverter.ToInt32(data);
            if (type == typeof(ulong)) return BitConverter.ToUInt64(data);
            if (type == typeof(long)) return BitConverter.ToInt64(data);
            if (type == typeof(float)) return BitConverter.ToSingle(data);
            

            return null;
        }

        /// <summary>
        /// 取得这个属性占用的字节数量
        /// </summary>
        /// <param name="propertyInfo">属性</param>
        /// <param name="type">类型</param>
        /// <param name="t">这是一个实例</param>
        /// <param name="count">占用的字节数。</param>
        /// <returns></returns>
        /// <exception cref="NoAttributeExistsException"></exception>
        private bool getPropertyBytesCount(PropertyInfo propertyInfo, Type type, T t, ref int count)
        {
            // 取得基本的数据类型。
            Type elementType = propertyInfo.PropertyType;
            if (propertyInfo.PropertyType.IsArray) elementType = propertyInfo.PropertyType.GetElementType(); // 如果是数组，就取得数组的项目类型
            //
            var defaultSize = propertyInfo.GetCustomAttributes(false).Where(x => x is DefaultSize).FirstOrDefault(); // 默认的
            var otherSize = propertyInfo.GetCustomAttributes(false).Where(x => x is OtherSize).ToArray();            // 根据别的确定大小
            if (defaultSize != null) { 
                count = ((DefaultSize)defaultSize).Value;                                         // 默认的数量
                if (!((DefaultSize)defaultSize).IsByteCount) count *= sizeOfByType(elementType);  // 如果不是按照字节计数，就乘以
            }   
            if (otherSize != null && otherSize.Length > 0)
            {
                foreach (OtherSize item2 in otherSize) // 判断每一个，因为存在多种情况
                {
                    // !!! 这里注意一下，如果OtherName为空，那么当作这条成立。
                    // 这里比较一下值,这里要取得item2中OtherName属性的值
                    var propertyInfo2 = type.GetProperties().Where(x => x.Name == item2.OtherName).FirstOrDefault();
                    if (!string.IsNullOrEmpty(item2.OtherName) && propertyInfo2 == null)
                    {
                        throw new NoAttributeExistsException(item2.OtherName);
                    }
                    var other_value = propertyInfo2 != null ? propertyInfo2.GetValue(t) : int.MinValue;                 // 通常情况下，这个在前面已经赋值了
                    if (string.IsNullOrEmpty(item2.OtherName) || other_value.ToString() == item2.OtherValue.ToString()) // 如果没有名称或者这里相等，因为牵涉到对象比较，这里直接比较字符串吧。
                    {
                        if (item2 is StaticSizeByOther) // 如果是固定的
                        {
                            count = ((StaticSizeByOther)item2).Value;
                            if (!((StaticSizeByOther)item2).IsByteCount) count *= sizeOfByType(elementType);  // 这里要判断是否按照自己计数。
                        }
                        else if (item2 is VarSizeByOther) // 如果是变量
                        {
                            // 这里要取得另一个属性的值
                            var propertyInfo3 = type.GetProperties().Where(x => x.Name == ((VarSizeByOther)item2).OtherSize).FirstOrDefault();
                            count =int.Parse(propertyInfo3.GetValue(t).ToString()); // 这里以后得做一个判断，是否是数字。                                 
                            if (!((VarSizeByOther)item2).IsByteCount) count *= sizeOfByType(elementType); // 这里要判断一下是否是按照字节计数的。
                        }
                    }
                }
            }
            return defaultSize != null || (otherSize != null && otherSize.Length > 0);
        }

        /// <summary>
        /// 取得这个属性的类型
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="isBigEndian"></param>
        /// <returns></returns>
        private Type getPropertyType(PropertyInfo propertyInfo, ref bool isBigEndian , ref double scale)
        {
            Type type_result = null;       // 这个是属性的数据类型，如果是数组，就数组元素的类型
            var int_type = propertyInfo.GetCustomAttributes(false).Where(x => x is IntType).FirstOrDefault();
            var float_type = propertyInfo.GetCustomAttributes(false).Where(x => x is FloatType).FirstOrDefault();
            var big_endian = propertyInfo.GetCustomAttributes(false).Where(x => x is DataItem).FirstOrDefault(); // 这个是肯定有的
            if (int_type != null) { 
                IntType intType = int_type as IntType;  
                type_result = getIntType(intType.BytesCount, intType.IsSigned);
                scale = intType.Scale;
            }
            if (scale == 0) scale = 1;
            if (float_type != null) { type_result = typeof(float);  }
            isBigEndian = ((DataItem)big_endian).BigEndian;
            if (int_type == null && float_type == null) // 没有指定数据类型，
            {
                // 这里分两种情况，是否数组的。
                if (propertyInfo.PropertyType.IsArray)
                {
                    type_result = propertyInfo.PropertyType.GetElementType();
                }
                else
                {
                    type_result = propertyInfo.PropertyType;
                }
            }
            return type_result;
        }


        private Type getIntType(int byte_count, bool isSigned)
        {
            if (byte_count == 1 && !isSigned) return typeof(byte);
            if (byte_count == 1 && isSigned) return typeof(sbyte);
            if (byte_count == 2 && !isSigned) return typeof(ushort);
            if (byte_count == 2 && isSigned) return typeof(short);
            if (byte_count == 4 && !isSigned) return typeof(uint);
            if (byte_count == 4 && isSigned) return typeof(int);
            if (byte_count == 8 && !isSigned) return typeof(long);
            if (byte_count == 8 && isSigned) return typeof(ulong);
            // TODO 这里要发出错误
            return null;
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

        // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/sizeof
        private Dictionary<Type, int>sizeofs = new Dictionary<Type, int>() {
            { typeof(sbyte), 1 },
            { typeof(byte), 1 },
            { typeof(short),2 },
            { typeof(ushort),2 },
            { typeof(int), 4 },
            { typeof(uint), 4 },
            { typeof(long), 8 },
            { typeof(ulong), 8 },
            { typeof(float), 4 },
            { typeof(double), 8 },
            { typeof(decimal), 16 },
            { typeof(char),2 },
            { typeof(bool),1 },

        };

        /// <summary>
        ///  取得字节数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private int sizeOfByType(Type type)
        {
            if (type == null) return 0;
            if (sizeofs.ContainsKey(type))
            {
                return sizeofs[type];
            }
            throw new UnsupportedTypeException(type);
        }


        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public  byte[] Serialize(T t)
        {

            // 首先创建一个对象
            List<byte> result = new List<byte>();
            // 取得类型
            var type = typeof(T);
            // 然后查看有多少个有相关注解的
            List<MemberInfo> members = new List<MemberInfo>();
            members.AddRange(type.GetProperties().Where(x => x.GetCustomAttributes() != null && x.GetCustomAttributes(false).Where(y => y is DataItem).Any()));
            members.AddRange(type.GetFields().Where(x => x.GetCustomAttributes() != null && x.GetCustomAttributes(false).Where(y => y is DataItem).Any()));
            // 然后排序
            members.Sort((x, y) => getIndex(x).CompareTo(getIndex(y)));
            int i_start = 0; // 当前位置
            // 如下3个是一体的。
            List<int> member_indexs = new List<int>();// 有数据的序号。
            List<int> indexs = new List<int>();       // 报错每一个index
            List<int> counts = new List<int>();       // 保存每一个count
            // 循环每一个
            foreach (var item in members)
            {
                // 记录操作
                member_indexs.Add(getIndex(item));
                indexs.Add(i_start);

                if (item is FieldInfo) // 如果是字段
                {
                    FieldInfo fieldInfo = item as FieldInfo;
                    // 字段只判断2种情况，一种是单个字节，一种是字节数组
                    
                    if (fieldInfo.FieldType.IsArray)
                    {
                        var arr = fieldInfo.GetValue(t) as byte[];
                        result.AddRange(arr);
                        counts.Add(arr.Length);
                        i_start += arr.Length;
                    }
                    else
                    {
                        // 这里表示是单个字节
                        var arr = (byte)fieldInfo.GetValue(t);
                        result.Add(arr);
                        counts.Add(1);
                        i_start += 1;
                    }
                }else if (item is PropertyInfo) // 如果是属性，
                {
                    // 思路，
                    // 1. 首先判断类型
                    // 2. 然后转换这个类型到字节数组，注意字节顺序。特例是校验

                    // 1. 这里要判断数据类型问题
                    PropertyInfo propertyInfo = item as PropertyInfo;
                    bool isBigEndian = false;                                     // 字节序大小端
                    double scale = 0;
                    Type type1 = getPropertyType(propertyInfo, ref isBigEndian, ref scale);  // 类型
                    // 2. 
                    // 这里判断是否是校验
                    var check = propertyInfo.GetCustomAttributes(false).Where(x => x is Check).FirstOrDefault();
                    if(check != null) // 如果是校验，就先计算这个校验。
                    {
                        // 取得要计算校验的
                        var check_start = indexs[member_indexs.IndexOf(((Check)check).StartIndex)];
                        var check_end = indexs[member_indexs.IndexOf(((Check)check).EndIndex)];
                        var check_count = counts[member_indexs.IndexOf(((Check)check).EndIndex)];
                        // 取得字节数组，这个要做校验。
                        var data3 = result.Skip(check_start).Take(check_end + check_count - check_start).ToArray();
                        // 计算出校验
                        var data4 = ((Check)check).Compute(data3); // 
                        // TODO 然后比较，请注意，这里比较的是字节数组，data4先转成字节数组。
                        var data5 = objectToBytes(data4);
                        if (BitConverter.IsLittleEndian ^ isBigEndian) Array.Reverse(data5);
                        // 
                        counts.Add(data5.Length);
                        result.AddRange(data5);
                        i_start += data5.Length;
                    }
                    else
                    {
                        var data6 = propertyInfo.GetValue(t); // 取得这个值
                        // 查看是否是数组
                        if (propertyInfo.PropertyType.IsArray) // 数组的话，每一个都添加。
                        {
                            var data8 = data6 as Array;
                            for (int i = 0; i < data8.Length; i++)
                            {
                                var data9 = objectToBytes(data8.GetValue(i));  // 转成字节数组
                                if (BitConverter.IsLittleEndian ^ isBigEndian) Array.Reverse(data9); // 字节序
                                result.AddRange(data9);
                            }
                            counts.Add(sizeOfByType(type1) * data8.Length);
                            i_start += counts.Last();
                        }
                        else
                        {
                            counts.Add(sizeOfByType(type1));   // 只有一个
                            var data7 = objectToBytes(data6);  // 转成字节数组
                            if (BitConverter.IsLittleEndian ^ isBigEndian) Array.Reverse(data7); // 字节序
                            result.AddRange(data7);
                            i_start += data7.Length;
                        }
                    }
                   
                }

            }
            return result.ToArray();
            //throw new NotImplementedException();
        }

        

    }
}
