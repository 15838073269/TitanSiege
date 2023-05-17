#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using System;

namespace Net.Component
{
    public enum SerializeAdapterType
    {
        Default,//默认序列化, protobuff + json
        PB_JSON_FAST,//快速序列化 protobuff + json
        Binary,//快速序列化 需要注册远程类型
        Binary2,//极速序列化 Binary + Binary2 需要生成序列化类型, 菜单GameDesigner/Netowrk/Fast2BuildTools
        Binary3//极速序列化 需要生成序列化类型, 菜单GameDesigner/Netowrk/Fast2BuildTools
    }

    [Obsolete("此组件已弃用, 内部已自动匹配适配器, 无需手动处理")]
    public class SerializeAdapter : SingleCase<SerializeAdapter>
    {
        public SerializeAdapterType type;
        public bool isEncrypt = false;//数据加密?
        public int password = 758426581;

        protected override void Awake()
        {
            base.Awake();
            Init();
        }

        public void Init()
        {
            var cm = GetComponent<ClientManager>();
            switch (type) {
                case SerializeAdapterType.Default:
                    break;
                case SerializeAdapterType.PB_JSON_FAST:
                    cm.client.AddAdapter(new Adapter.SerializeFastAdapter() { IsEncrypt = isEncrypt, Password = password });
                    break;
                case SerializeAdapterType.Binary:
                    cm.client.AddAdapter(new Adapter.SerializeAdapter() { IsEncrypt = isEncrypt, Password = password });
                    break;
                case SerializeAdapterType.Binary2:
                    cm.client.AddAdapter(new Adapter.SerializeAdapter2() { IsEncrypt = isEncrypt, Password = password });
                    break;
                case SerializeAdapterType.Binary3:
                    cm.client.AddAdapter(new Adapter.SerializeAdapter3() { IsEncrypt = isEncrypt, Password = password });
                    break;
            }
        }
    }
}
#endif