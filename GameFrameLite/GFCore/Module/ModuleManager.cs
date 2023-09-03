using GF.Msg;
using GF.Utils;
using System;
using System.Collections.Generic;

namespace GF.Module {
    /// <summary>
    /// module管理模块，ModuleManager属于服务层，单例 ，处理模块的创建加载，销毁和消息通讯
    /// 继承ILogTag的类，使用日志时可以直接应用this.log,否则只能使用Debuger.log
    /// </summary>
    public class ModuleManager : Singleton<ModuleManager> {

        /// <summary>
        /// 存储所有模块的字典
        /// </summary>
        private Dictionary<string, GeneralModule> m_mapModules;

        /// <summary>
        /// 注册模块的创建器
        /// </summary>
        private List<IModuleActivator> m_listModuleActivator;
        /// <summary>
        /// 无参构造
        /// </summary>
        public ModuleManager() {
            m_mapModules = new Dictionary<string, GeneralModule>();
            m_listModuleActivator = new List<IModuleActivator>();
        }
        /// <summary>
        /// 加载模块，我们需要通过Type.GetType来获取模块，但这个模块需要全称，方便后期传参，把域名默认出来
        /// </summary>
        /// <param name="domian">域名</param>
        public void Init() {
            CheckSingleton();//检查以下单例
        }
        /// <summary>
        /// 释放模块的方法
        /// </summary>
        /// <param name="module">模块</param>
        public void ReleaseModdule(GeneralModule module) {
            if (module != null) {
                //属于模块管理器创建的模块，才由模块管理器释放
                if (m_mapModules.ContainsKey(module.Name)) {
                    m_mapModules.Remove(module.Name);
                    module.Release();
                }
            } else {
                //非模块管理器创建的模块，后期拓展
                //todo 一般不会有
            }
        }
        /// <summary>
        /// 清理缓存消息和现有模块的方法
        /// </summary>
        public void ReleaseAll() {
            m_listModuleActivator.Clear();
            foreach (KeyValuePair<string, GeneralModule> module in m_mapModules) {
                module.Value.Release();
            }
            m_mapModules.Clear();
        }
        /// <summary>
        /// 注册创建模块的方法 //原模块管理器使用Activator.CreateInstance(type) as GeneralModule创建，但这种创建是不可控的，所以需要自己写一个自己管理的模块创建器
        /// </summary>
        /// <param name="activator">模块创建器</param>
        public void RegisterModuleActivator(IModuleActivator activator) {
            if (!m_listModuleActivator.Contains(activator)) {
                m_listModuleActivator.Add(activator);
            }
        }
        /// <summary>
        /// 通过模块创建器创建模块
        /// </summary>
        /// <param name="modulename"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public GeneralModule CreateModule<T>(string modulename, T args) {
            Debuger.Log("modulename=" + modulename + ",args=" + args);
            if (HasModule(modulename)) {
                Debuger.Log("模块{0}已存在！", modulename);
                return null;
            }
            GeneralModule module = null;
            //遍历所有已有的模块创建器，能创建成功就跳出遍历
            for (int i = 0; i < m_listModuleActivator.Count; i++) {
                module = m_listModuleActivator[i].CreateInstance(modulename);
                if (module != null) {
                    break;
                }
            }
            if (module == null) {
                Debuger.LogError("模块{0}实例化失败，没有对应的模块创建器！", modulename);
                return null;
            }
            m_mapModules.Add(modulename, module);
            module.m_ID = MsgCenter.GetModuleId(modulename);
            module.Create(args);
            return module;
        }
        public GeneralModule CreateModule(string modulename) {
            if (HasModule(modulename)) {
                Debuger.Log("模块{0}已存在！", modulename);
                return null;
            }
            GeneralModule module = null;
            //遍历所有已有的模块创建器，能创建成功就跳出遍历
            for (int i = 0; i < m_listModuleActivator.Count; i++) {
                module = m_listModuleActivator[i].CreateInstance(modulename);
                if (module != null) {
                    break;
                }
            }
            if (module == null) {
                Debuger.LogError("模块{0}实例化失败，没有对应的模块创建器！", modulename);
                return null;
            }
            m_mapModules.Add(modulename, module);
            module.Create();
            return module;
        }
        /// <summary>
        /// 判断模块是否已经存在
        /// </summary>
        /// <param name="modulename"></param>
        /// <returns></returns>
        public bool HasModule(string modulename) {
            return m_mapModules.ContainsKey(modulename);
        }
        /// <summary>
        /// 通过模块名称获取模块
        /// </summary>
        /// <param name="modulename">模块名称</param>
        /// <returns></returns>
        public GeneralModule GetModule(string modulename) {
            GeneralModule module = null;
            m_mapModules.TryGetValue(modulename, out module);
            return module;
        }

    }
   
}
