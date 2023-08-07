using Net.Helper;
using Net.Share;
using System;
using System.Runtime.CompilerServices;

namespace Net.Common
{
    /// <summary>
    /// 属性观察接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPropertyObserver<T>
    {
        /// <summary>
        /// 属性值
        /// </summary>
        T Value { get; set; }
        /// <summary>
        /// 当属性被修改事件
        /// </summary>
        Action<T> OnValueChanged { get; set; }
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <returns></returns>
        T GetValue();
        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="value">新的属性值</param>
        /// <param name="isNotify">是否通知事件</param>
        void SetValue(T value, bool isNotify = true);
    }

    /// <summary>
    /// 属性观察类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyObserver<T> : IPropertyObserver<T>
    {
        protected T value;
        public T Value { get => GetValue(); set => SetValue(value); }
        public Action<T> OnValueChanged { get; set; }
        public PropertyObserver() { }
        public PropertyObserver(T value) : this(value, null) { }
        public PropertyObserver(T value, Action<T> onValueChanged)
        {
            this.value = value;
            OnValueChanged = onValueChanged;
        }
        
        public virtual T GetValue()
        {
            return value;
        }

        public virtual void SetValue(T value, bool isNotify = true)
        {
            if (Equals(this.value, value))
                return;
            this.value = value;
            if (isNotify) OnValueChanged?.Invoke(value);
        }

        public override string ToString()
        {
            return $"{Value}";
        }

        public static implicit operator PropertyObserver<T>(T value)
        {
            return new PropertyObserver<T>(value, null);
        }

        public static implicit operator T(PropertyObserver<T> value)
        {
            return value.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is PropertyObserver<T> value)
                return Equals(value);
            return false;
        }

        public bool Equals(PropertyObserver<T> obj)
        {
            return Value.Equals(obj.Value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// 模糊属性观察类, 此类只支持byte, sbyte, short, ushort, char, int, uint, float, long, ulong, double
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObscuredPropertyObserver<T> : IPropertyObserver<T>
    {
        private readonly string name;
        private long valueAtk;
        private readonly long valueAtkKey;
        private byte crcValue;
        public T Value { get => GetValue(); set => SetValue(value); }
        public Action<T> OnValueChanged { get; set; }

        public ObscuredPropertyObserver(T value) : this(null, value) { }
        public ObscuredPropertyObserver(string name, T value) : this(name, value, null) { }
        public ObscuredPropertyObserver(string name, T value, Action<T> onValueChanged)
        {
            this.name = name;
            valueAtkKey = RandomHelper.Range(0, int.MaxValue);
            SetValue(value);
            OnValueChanged = onValueChanged;
        }

        public unsafe T GetValue()
        {
            var value = valueAtk ^ valueAtkKey;
            var ptr = (byte*)&value;
            var crcIndex = (byte)(valueAtk % 247);
            var crcValue = CRCHelper.CRC8(ptr, 0, 8, crcIndex);
            var value1 = Unsafe.As<long, T>(ref value);
            if (this.crcValue != crcValue)
            {
                AntiCheatHelper.OnDetected?.Invoke(name, value1, value1); //因为原值不做记录, 所以原值没有了, 需要在数据库找到原值
                throw new Exception();
            }
            return value1;
        }

        public unsafe void SetValue(T value, bool isNotify = true)
        {
            var value1 = Unsafe.As<T, long>(ref value);
            valueAtk = value1 ^ valueAtkKey;
            var ptr = (byte*)&value1;
            var crcIndex = (byte)(valueAtk % 247);
            crcValue = CRCHelper.CRC8(ptr, 0, 8, crcIndex);
            if (isNotify) OnValueChanged?.Invoke(value);
        }

        public override string ToString()
        {
            return $"{Value}";
        }

        public static implicit operator ObscuredPropertyObserver<T>(T value)
        {
            return new ObscuredPropertyObserver<T>(string.Empty, value, null);
        }

        public static implicit operator T(ObscuredPropertyObserver<T> value)
        {
            return value.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ObscuredPropertyObserver<T> value)
                return Equals(value);
            return false;
        }

        public bool Equals(ObscuredPropertyObserver<T> obj)
        {
            return Value.Equals(obj.Value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// 属性观察自动类, 可模糊,不模糊
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyObserverAuto<T> : IPropertyObserver<T>
    {
        private readonly IPropertyObserver<T> binding;
        public T Value { get => GetValue(); set => SetValue(value); }
        public Action<T> OnValueChanged { get => binding.OnValueChanged; set => binding.OnValueChanged = value; }

        public PropertyObserverAuto() { }
        /// <summary>
        /// 属性观察自动类构造
        /// </summary>
        /// <param name="name">当属性被发现修改时提示名称</param>
        /// <param name="available">使用模糊属性?</param>
        /// <param name="onValueChanged">当属性被修改事件</param>
        public PropertyObserverAuto(string name, bool available, Action<T> onValueChanged) : this(name, available, default, onValueChanged)
        {
        }

        public PropertyObserverAuto(string name, bool available, T value, Action<T> onValueChanged)
        {
            if (!AntiCheatHelper.IsActive | !available)
                binding = new PropertyObserver<T>(value, onValueChanged);
            else
                binding = new ObscuredPropertyObserver<T>(name, value, onValueChanged);
        }

        public T GetValue()
        {
            return binding.GetValue();
        }

        public void SetValue(T value, bool isNotify = true)
        {
            binding.SetValue(value, isNotify);
        }

        public override string ToString()
        {
            return $"{Value}";
        }

        public static implicit operator PropertyObserverAuto<T>(T value)
        {
            return new PropertyObserverAuto<T>(string.Empty, true, value, null);
        }

        public static implicit operator T(PropertyObserverAuto<T> value)
        {
            return value.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is PropertyObserverAuto<T> value)
                return Equals(value);
            return false;
        }

        public bool Equals(PropertyObserverAuto<T> obj)
        {
            return Value.Equals(obj.Value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
