using Net.Serialize;
using Net.Share;
using Net.System;
using System;

namespace Net.Adapter
{
    /// <summary>
    /// 快速序列化适配器
    /// </summary>
    public class SerializeAdapter : ISerializeAdapter
    {
        [Obsolete("此处加密会影响性能,所以可以在ServerBase.PackageAdapter和ClientBase.PackageAdapter设置数据加密适配器")]
        public bool IsEncrypt { get; set; }
        public int Password { get; set; } = 758426581;

        public byte[] OnSerializeRpc(RPCModel model)
        {
            var buffer = NetConvertBinary.SerializeModel(model);
            //if (IsEncrypt)
            //    Helper.EncryptHelper.ToEncrypt(Password, buffer);
            return buffer;
        }

        public FuncData OnDeserializeRpc(byte[] buffer, int index, int count)
        {
            //if (IsEncrypt)
            //    Helper.EncryptHelper.ToDecrypt(Password, buffer, index, count);
            return NetConvertBinary.DeserializeModel(buffer, index, count);
        }

        public byte[] OnSerializeOpt(OperationList list)
        {
            return NetConvertFast2.SerializeObject(list).ToArray(true);
        }

        public OperationList OnDeserializeOpt(byte[] buffer, int index, int count)
        {
            var segment = new Segment(buffer, index, count, false);
            return NetConvertFast2.DeserializeObject<OperationList>(segment);
        }
    }

    /// <summary>
    /// 通用升级版适配器
    /// </summary>
    public class SerializeFastAdapter : ISerializeAdapter
    {
        [Obsolete("此处加密会影响性能,所以可以在ServerBase.PackageAdapter和ClientBase.PackageAdapter设置数据加密适配器")]
        public bool IsEncrypt { get; set; }
        public int Password { get; set; } = 758426581;

        public byte[] OnSerializeRpc(RPCModel model)
        {
            var buffer = NetConvertFast.Serialize(model);
            //if (IsEncrypt)
            //    Helper.EncryptHelper.ToEncrypt(Password, buffer);
            return buffer;
        }

        public FuncData OnDeserializeRpc(byte[] buffer, int index, int count)
        {
            //if (IsEncrypt)
            //    Helper.EncryptHelper.ToDecrypt(Password, buffer, index, count);
            return NetConvertFast.Deserialize(buffer, index, count);
        }

        public byte[] OnSerializeOpt(OperationList list)
        {
            return NetConvertFast2.SerializeObject(list).ToArray(true);
        }

        public OperationList OnDeserializeOpt(byte[] buffer, int index, int count)
        {
            var segment = new Segment(buffer, index, count, false);
            return NetConvertFast2.DeserializeObject<OperationList>(segment);
        }
    }

    /// <summary>
    /// 快速序列化2适配器
    /// </summary>
    public class SerializeAdapter2 : ISerializeAdapter
    {
        [Obsolete("此处加密会影响性能,所以可以在ServerBase.PackageAdapter和ClientBase.PackageAdapter设置数据加密适配器")]
        public bool IsEncrypt { get; set; }
        public int Password { get; set; } = 758426581;

        public byte[] OnSerializeRpc(RPCModel model)
        {
            var buffer = NetConvertBinary.SerializeModel(model);
            //if (IsEncrypt)
            //    Helper.EncryptHelper.ToEncrypt(Password, buffer);
            return buffer;
        }

        public FuncData OnDeserializeRpc(byte[] buffer, int index, int count)
        {
            //if (IsEncrypt)
            //    Helper.EncryptHelper.ToDecrypt(Password, buffer, index, count);
            return NetConvertBinary.DeserializeModel(buffer, index, count);
        }

        public byte[] OnSerializeOpt(OperationList list)
        {
            return NetConvertFast2.SerializeObject(list).ToArray(true);
        }

        public OperationList OnDeserializeOpt(byte[] buffer, int index, int count)
        {
            var segment = new Segment(buffer, index, count, false);
            return NetConvertFast2.DeserializeObject<OperationList>(segment);
        }
    }

    /// <summary>
    /// 极速序列化3适配器
    /// </summary>
    public class SerializeAdapter3 : ISerializeAdapter
    {
        [Obsolete("此处加密会影响性能,所以可以在ServerBase.PackageAdapter和ClientBase.PackageAdapter设置数据加密适配器")]
        public bool IsEncrypt { get; set; }
        public int Password { get; set; } = 758426581;

        public byte[] OnSerializeRpc(RPCModel model)
        {
            var buffer = NetConvertFast2.SerializeModel(model);
            return buffer;
        }

        public FuncData OnDeserializeRpc(byte[] buffer, int index, int count)
        {
            var segment = new Segment(buffer, index, count, false);
            return NetConvertFast2.DeserializeModel(segment);
        }

        public byte[] OnSerializeOpt(OperationList list)
        {
            return NetConvertFast2.SerializeObject(list).ToArray(true);
        }

        public OperationList OnDeserializeOpt(byte[] buffer, int index, int count)
        {
            var segment = new Segment(buffer, index, count, false);
            return NetConvertFast2.DeserializeObject<OperationList>(segment);
        }
    }
}