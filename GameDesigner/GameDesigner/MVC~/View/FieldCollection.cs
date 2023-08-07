#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
namespace MVC.View
{
    using Net.Helper;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class FieldCollection : MonoBehaviour
    {
        [Serializable]
        public class Field
        {
            public string name;
            public string typeName;
            public Object target;
#if UNITY_EDITOR
            public int componentIndex;
            public string[] typeNames;
#endif
            private Type type;
            public Type Type
            {
                get
                {
                    if (type == null)
                        type = AssemblyHelper.GetType(typeName);
                    return type;
                }
                set { type = value; }
            }
            public T To<T>() where T : Object
            {
                return target as T;
            }

#if UNITY_EDITOR
            public void Update()
            {
                type = null; //要清空, 后面修改的类型才生效
                if (target == null)
                {
                    typeName = "UnityEngine.Object";
                    typeNames = new string[] { "UnityEngine.Object" };
                    type = typeof(Object);
                    componentIndex = 0;
                    return;
                }
                GameObject gameObject;
                if (target is Component component)
                    gameObject = component.gameObject;
                else
                    gameObject = target as GameObject;
                var components = gameObject.GetComponents<Component>();
                var objects = new List<Object>() { gameObject };
                objects.AddRange(components);
                typeNames = new string[objects.Count];
                for (int a = 0; a < objects.Count; a++)
                    typeNames[a] = objects[a].GetType().ToString();
                if (Type == typeof(GameObject) | Type == typeof(Object))
                {
                    target = gameObject;
                    componentIndex = 0;
                }
                else
                {
                    for (int i = 0; i < components.Length; i++)
                    {
                        if (components[i].GetType() == Type) 
                        {
                            target = components[i];
                            componentIndex = i + 1; // +1是前面有gameobject
                            break;
                        }
                    }
                }
            }
#endif
        }
        public string fieldName;
        public List<Field> fields = new List<Field>();
        private readonly Dictionary<string, Object> fieldsDic = new Dictionary<string, Object>();
#if UNITY_EDITOR
        public int nameIndex;
        public bool compiling;
        public int savePathInx;
        public int savePathExtInx;
        public bool genericType;
        public int inheritTypeInx;
#endif
        private bool init;
        public Field this[int index]
        {
            get { return fields[index]; }
            set
            {
                fields[index].target = value.target;
                fields[index].typeName = value.typeName;
                fields[index].name = value.name;
            }
        }

        public Field this[string name]
        {
            get
            {
                foreach (Field f in fields)
                {
                    if (f.name == name)
                    {
                        return f;
                    }
                }
                return null;
            }
            set
            {
                foreach (Field f in fields)
                {
                    if (f.name == name)
                    {
                        f.name = value.name;
                        f.typeName = value.typeName;
                        f.Type = value.Type;
                        f.target = value.target;
                        return;
                    }
                }
            }
        }

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            if (init)
                return;
            init = true;
            for (int i = 0; i < fields.Count; i++)
            {
                if (string.IsNullOrEmpty(fields[i].name))
                    fieldsDic.Add(i.ToString(), fields[i].target);
                else
                    fieldsDic.Add(fields[i].name, fields[i].target);
            }
        }

        public T GetField<T>(string name) where T : Object
        {
            return fieldsDic[name] as T;
        }

        public T GetField<T>(int index) where T : Object
        {
            return fields[index].target as T;
        }

        public T Get<T>(string name) where T : Object
        {
            return fieldsDic[name] as T;
        }

        public T Get<T>(int index) where T : Object
        {
            return fields[index].target as T;
        }
    }
}
#endif