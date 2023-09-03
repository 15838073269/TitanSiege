
using System;

namespace GF.Module {
    /// <summary>
    /// 常规模块的创建器，这个常规模块一般指的就是非Lua模块
    /// </summary>
    public class NativeModuleActivator : IModuleActivator {
        //之所以定义下面的两个变量，是因为一般传递创建类实例时，需要通过这两个变量补全类的路径
        /// <summary>
        /// 命名空间名称
        /// </summary>
        private string m_namespace = "";
        /// <summary>
        /// 程序集名称
        /// </summary>
        private string m_assemblyName = "";
        //private Assembly m_assembly;
        public GeneralModule CreateInstance(string ModuleName) {
            string fullname = m_namespace + "." + ModuleName;
            //这种操作用到了反射，性能可能有问题
            //if (m_assembly != null) {
            //    return m_assembly.CreateInstance(fullname) as GeneralModule;
            //} else {
            //    //通过type类来获取Assembly，通过Assembly创建模块类的实例,但这么做性能有问题，不建议
            //    Type type = Type.GetType(fullname +","+m_assemblyName);
            //    if (type!=null) {
            //        m_assembly = type.Assembly;
            //       return m_assembly.CreateInstance(type) as GeneralModule;
            //    }
            //    return null;
            //}
            //通过Activator创建模块类的实例
            Type type = Type.GetType(fullname + "," + m_assemblyName);
            if (type != null) {
                return Activator.CreateInstance(type) as GeneralModule;
            }
            return null;
        }
        /// <summary>
        /// 构造获取命名空间名称和程序集名称
        /// </summary>
        /// <param name="spacename">命名空间名称</param>
        /// <param name="assemblyName">程序集名称</param>
        public NativeModuleActivator(string spacename,string assemblyName) {
            m_namespace = spacename;
            m_assemblyName = assemblyName;
        }
    }
}
