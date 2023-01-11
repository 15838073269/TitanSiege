/****************************************************
    文件：BinarySerialize.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/10/27 17:11:37
	功能：xml与二进制之间的转换工具类
*****************************************************/
using GF.Unity.AB;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;
namespace GF.ConfigTable {
    public class BinarySerializeOpt {
        /// <summary>
        /// 类序列化成xml
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <returns>是否序列化成功</returns>
        public static bool Xmlserialize(string path, System.Object obj) {
            try {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)) {
                    using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8)) {
                        //删除xml的命名空间,删不删都行
                        //XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                        //namespaces.Add(string.Empty, string.Empty);
                        //序列化
                        XmlSerializer xs = new XmlSerializer(obj.GetType());
                        xs.Serialize(sw, obj);
                    }
                }
                return true;
                ;
            } catch (Exception e) {
                Debuger.LogError("此类无法转换成xml " + obj.GetType() + "," + e);
                //出现异常的文件可以在这里通过代码删除，不过也可以不删，看需要，先不做了

            }
            return false;
        }

        /// <summary>
        /// xml转成二进制，直接用文件流读取xml，编辑器读取xml使用的
        /// 游戏运行时，不会这样直接读取xml文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string path) where T : class {
            T t = default(T);
            try {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)) {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    t = (T)xs.Deserialize(fs);
                }
            } catch (Exception e) {
                Debuger.LogError("此xml无法转成二进制: " + path + "," + e);
            }
            return t;
        }
        /// <summary>
        /// 这个是游戏运行时使用的读取xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T XmlDeserializeRun<T>(string path) where T : class {
            T t = default(T);
            TextAsset textAsset = ResourceManager.GetInstance.LoadResource<TextAsset>(path);

            if (textAsset == null) {
                UnityEngine.Debug.LogError("发生错误，没有该文件: " + path);
                return null;
            }

            try {
                using (MemoryStream stream = new MemoryStream(textAsset.bytes)) {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    t = (T)xs.Deserialize(stream);
                }
                ResourceManager.GetInstance.DestoryResource(path, true);
            } catch (Exception e) {
                Debuger.LogError("读取文件错误: " + path + "," + e);
            }
            return t;
        }
        /// <summary>
        /// Xml的反序列化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static System.Object XmlDeserialize(string path, Type type) {
            System.Object obj = null;
            try {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)) {
                    XmlSerializer xs = new XmlSerializer(type);
                    obj = xs.Deserialize(fs);
                }
            } catch (Exception e) {
                Debuger.LogError("此xml无法转成二进制: " + path + "," + e);
            }
            return obj;
        }



        /// <summary>
        /// 类转换成二进制
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool BinarySerilize(string path, System.Object obj) {
            try {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)) {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, obj);
                }
                return true;
            } catch (Exception e) {
                Debuger.LogError($"此类无法转换成二进制 {obj.GetType()}, {e}");
            }
            return false;
        }

        /// <summary>
        /// 读取二进制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T BinaryDeserilize<T>(string path) where T : class {
            T t = default(T);
            TextAsset textAsset = ResourceManager.GetInstance.LoadResource<TextAsset>(path);

            if (textAsset == null) {
                UnityEngine.Debug.LogError("该二进制文件不存在: " + path);
                return null;
            }

            try {
                using (MemoryStream stream = new MemoryStream(textAsset.bytes)) {
                    BinaryFormatter bf = new BinaryFormatter();
                    t = (T)bf.Deserialize(stream);
                }
                ResourceManager.GetInstance.DestoryResource(path, true);
            } catch (Exception e) {
                Debuger.LogError($"加载转换二进制出错: {path},{e.Message},{e.StackTrace}");
            }
            return t;
        }
    }
}
