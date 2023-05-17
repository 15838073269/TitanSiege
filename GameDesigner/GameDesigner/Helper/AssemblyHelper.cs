using System;
using System.Collections.Generic;

namespace Net.Helper
{
    public class AssemblyHelper
    {
        private static readonly Dictionary<string, Type> TypeDict = new Dictionary<string, Type>();
        private static readonly HashSet<string> NotTypes = new HashSet<string>();

        /// <summary>
        /// 添加后续要查找的类型
        /// </summary>
        /// <param name="type"></param>
        public static void AddFindType(Type type) 
        {
            TypeDict[type.ToString()] = type;
        }

        /// <summary>
        /// 获取类型， 如果类型已经获取过一次则直接取，否则或查找所有程序集获取类型，如果查找到则会添加到缓存字典中，下次不需要再遍历所有程序集
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetType(string typeName)
        {
            if (TypeDict.TryGetValue(typeName, out var type))
                return type;
            if (NotTypes.Contains(typeName))
                goto JMP;
            type = Type.GetType(typeName);
            if (type != null)
            {
                TypeDict.Add(typeName, type);
                return type;
            }
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                type = assembly.GetType(typeName);
                if (type != null)
                {
                    TypeDict[typeName] = type;
                    return type;
                }
            }
            if (typeName.IndexOf("[") >= 0) //虽然支持了泛型和字典检测, 但是不要搞的太复杂花里胡哨的会影响性能和产生bug, 这里还有一个问题, 就是当编译il2cpp后可能也会获取不到
            {
                var index = typeName.IndexOf("[");
                var itemTypeName = typeName.Substring(0, index);
                var genericType = GetType(itemTypeName);
                if (genericType == null)
                    goto JMP;
                itemTypeName = typeName.Remove(0, index + 1);
                index = StringHelper.FindHitCount(itemTypeName, '[');
                StringHelper.RemoveHit(ref itemTypeName, ']', index);
                var itemTypeList = new List<Type>();
                if (genericType.GetGenericArguments().Length == 1)
                {
                    var itemType = GetType(itemTypeName);
                    if (itemType == null)
                        goto JMP;
                    itemTypeList.Add(itemType);
                }
                else
                {
                    var itemTypeNames = itemTypeName.Split(',');
                    foreach (var itemTypeNameN in itemTypeNames)
                    {
                        var itemType = GetType(itemTypeNameN);
                        if (itemType == null)
                            goto JMP;
                        itemTypeList.Add(itemType);
                    }
                }
                type = genericType.MakeGenericType(itemTypeList.ToArray());
                TypeDict[typeName] = type;
                return type;
            }
            NotTypes.Add(typeName);
        JMP: Event.NDebug.LogError($"找不到类型:{typeName}, 类型太复杂时需要使用 AssemblyHelper.AddFindType(type) 标记后面要查找的类");
            return null;
        }

        public static Type GetTypeNotOptimized(string typeName) 
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }
    }
}