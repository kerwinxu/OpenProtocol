using Io.Github.KerwinXu.OpenProtocol.Utils;

namespace Io.Github.KerwinXu.OpenProtocol.Attributes.Counts
{
    /// <summary>
    /// 根据别的判断大小
    /// </summary>
    public class BaseOnOtherSize: Count
    {
        public string OtherName { get; set; }

        public object OtherValue { get; set; }


        public override bool IsAvailable(object obj)
        {
            var _other_value = Member.getValue(obj, OtherName);  // 取得某个实际的值
            return _other_value == OtherValue;                   // 判断是否相同
            return base.IsAvailable(obj);
        }
    }
}
