/****************************************************
    文件：ClassObjectPool.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/8/18 8:52:21
	功能：ObjectManager管理所有需要实例化的对象资源，同时包括类对象池的管理
*****************************************************/
//使用过程中一定要注意，没有实例化前，不要对对象进行任何操作
//使用时需要在场景内添加空物体RecyclePoolTrs和空物体SceneTransform,并RecyclePoolTrs设置为不显示
using System;
using System.Collections.Generic;
using GF.Utils;
using UnityEngine;
using GF.Pool;
namespace GF.Unity.AB {
    public class ObjectManager : Singleton<ObjectManager> {
        public bool IsEditor = false;
        #region 类对象池的操作函数
        /// <summary>
        /// 存储所有对象池的字典,object存储内容为ClassObjectPool<T>的对象
        /// </summary>
        public Dictionary<Type, object> m_ClassPoolDic = new Dictionary<Type, object>();
        /// <summary>
        /// 获取指定类型的对象池对象，如果不存在或者对象意外销毁，就再创建一个，获取后可以调用对应对象池操作方法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="maxcount">创建对象池的最大个数，默认0,0为不限制个数</param>
        /// <returns></returns>
        public ClassObjectPool<T> GetClassObjectPool<T>(int maxcount= 0) where T : class, new() {
            Type t = typeof(T);
            object poolobj = null;
            if (!m_ClassPoolDic.TryGetValue(t,out poolobj) || poolobj == null) {
                ClassObjectPool<T> newpool = new ClassObjectPool<T>(maxcount);
                m_ClassPoolDic.Add(t,newpool);
                return newpool;
            }
            return poolobj as ClassObjectPool<T>;
        }
        #endregion
       
        /// <summary>
        /// 实例化对象的对象池，key为CRC,value为对象list
        /// </summary>
        public Dictionary<uint, List<ResourceObj>> m_ObjectPoolDic = new Dictionary<uint, List<ResourceObj>>();
        /// <summary>
        /// Resourceobj的类对象池,用的比较多，配到1000
        /// </summary>
        protected ClassObjectPool<ResourceObj> m_ResourceObjPool;
        /// <summary>
        /// 缓存ResourceObj的Dic，key为guid
        /// </summary>
        protected Dictionary<int, ResourceObj> m_ResourceObjCacheDic = new Dictionary<int, ResourceObj>();
        /// <summary>
        /// 正在进行的异步加载的Dic，key为AsyncId
        /// </summary>
        protected Dictionary<long, ResourceObj> m_AsyncResourceObjDic = new Dictionary<long, ResourceObj>();
        /// <summary>
        /// 暂时不用的对象添加的父物体
        /// </summary>
        public Transform RecyclePoolTrs;
        /// <summary>
        /// 实例化对象默认放在这个物体下
        /// </summary>
        public Transform SceneTransform;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="recyclepool">不用的对象的父物体</param>
        /// <param name="recyclepool">实例化对象默认放在这个物体下</param>
        public void Init(Transform recyclepool,Transform SceneTrs) {
            RecyclePoolTrs = recyclepool;
            SceneTransform = SceneTrs;
            m_ResourceObjPool = ObjectManager.GetInstance.GetClassObjectPool<ResourceObj>(1000);
        }
        /// <summary>
        /// 从对象池中取对象
        /// </summary>
        /// <param name="crc">路径的crc</param>
        /// <returns>ResourceObj 对象</returns>
        protected ResourceObj GetObjectFromPool(uint crc) {
            List<ResourceObj> st = null;
            if (m_ObjectPoolDic.TryGetValue(crc,out st)&&st!=null&&st.Count>0) {
                //引用计数
                ResourceManager.GetInstance.InCreaseResourceRef(crc);
                ResourceObj tmpro = st[0];
                st.RemoveAt(0);
                tmpro.m_Already = false;
                //获取的时候还原一下对象参数,因为还原也要消耗一部分性能，不用的就不用还原
                if (!System.Object.ReferenceEquals(tmpro.m_OfflineData, null)) {
                    tmpro.m_OfflineData.ResetProp();
                }
                //编辑器下，为了方便观察对象池运转，加上从对象池获取到后改名的机制，实际运行时，频繁修改名称会造成GC
                if (IsEditor) {
                    GameObject obj = tmpro.m_CloneObj;
                    if (!System.Object.ReferenceEquals(obj, null)) { //判断是否为空的方法，效率较高
                        if (obj.name.EndsWith("(Recycle)")) { //对象池对象的名称都会加上(Recycle)，通过这个来判断
                            obj.name = obj.name.Replace("(Recycle)", "");
                        }
                    }
                }

                return tmpro;
            }
            return null;
        }
        #region 同步资源加载
        /// <summary>
        /// 同步对象资源加载
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="bClear">跳场景是否删除</param>
        /// <param name="isfullpath">是否是完整路径</param>
        ///  <param name="setTranform">是否要指定父级，这里是true,father才起作用，如果为true，但father参数悟空，则默认加载到SceneTransform</param>
        /// <param name="father">加载UI或者需要指定父级时使用的参数</param>
        public GameObject InstanceObject(string path, bool setTranform = true, bool bClear = true, bool isfullpath = false, Transform father=null) {
            if (!isfullpath) {
                path = ResourceManager.GetInstance.ResPrePath + path;
            }
            uint crc = Crc32.GetCRC32(path);
            ResourceObj resobj = GetObjectFromPool(crc);
            if (resobj == null) {
                resobj = m_ResourceObjPool.GetObj(true);
                resobj.m_Crc = crc;
                //通过ResourceManager获取加载resitem
                resobj = ResourceManager.GetInstance.GetResourceItemInObj(path,resobj);
                resobj.m_ResItem.m_Clear = bClear;
                if (resobj.m_ResItem.m_Obj != null) {
                    //if (setTranform) {
                    //    if (father == null) {
                    //        //大量setparent会造成性能消耗
                    //        resobj.m_CloneObj = GameObject.Instantiate(resobj.m_ResItem.m_Obj, SceneTransform) as GameObject;
                    //    } else {
                    //        resobj.m_CloneObj = GameObject.Instantiate(resobj.m_ResItem.m_Obj, father) as GameObject;
                    //    }
                    //} else {
                    //    resobj.m_CloneObj = GameObject.Instantiate(resobj.m_ResItem.m_Obj) as GameObject;
                    //}
                    resobj.m_CloneObj = GameObject.Instantiate(resobj.m_ResItem.m_Obj) as GameObject;
                    resobj.m_OfflineData = resobj.m_CloneObj.GetComponent<OfflineDatabase>();
                }
            }
            //进行判定
            if (setTranform) {
                if (father == null) {
                    resobj.m_CloneObj.transform.SetParent(SceneTransform, false);
                } else {
                    resobj.m_CloneObj.transform.SetParent(father, false);
                }
            }
            resobj.m_Clear = bClear;
            //缓存，不然resobj就没用了
            int tmpid = resobj.m_CloneObj.GetInstanceID();
            if (!m_ResourceObjCacheDic.ContainsKey(tmpid)) {
                m_ResourceObjCacheDic.Add(tmpid, resobj);
            }
            
            return resobj.m_CloneObj;
        }
        #endregion
        #region 异步资源加载
        /// <summary>
        /// 实例化资源的异步加载
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="ObjDealFinish">这个是OnsyncLoadObjFinish的回调</param>
        /// <param name="priority">资源优先级</param>
        /// <param name="scenetransform">是否将资源实例化资源到场景父节点中，这里是true,father才起作用，如果为true，但father参数悟空，则默认加载到SceneTransform</param>
        /// <param name="bClear">跳场景删除</param>
        /// <param name="isfullpath">path是否是完整路径</param>
        /// <param name="objarr">可变参数</param>
        /// <param name="father">加载UI或者需要指定父级时使用的参数</param>
        public long AsyncInstanceObject(string path, OnAsyncLoadObjFinish ObjDealFinish, LoadResPriority priority,  bool scenetransform = true,bool bClear=true, bool isfullpath = false,Transform father=null,params object[] objarr) {
            if (!isfullpath) {
                path = ResourceManager.GetInstance.ResPrePath + path;
            }
            if (string.IsNullOrEmpty(path)) {
                Debuger.LogError("异步加载实例化资源时路径为空");
                return -1;
            }
            uint crc = Crc32.GetCRC32(path);
            ResourceObj resobj = GetObjectFromPool(crc);
            if (resobj!=null) {
                resobj.m_SceneTransform = scenetransform;
                resobj.m_Father = father;
                if (scenetransform) {
                    if (father == null) {
                        resobj.m_CloneObj.transform.SetParent(SceneTransform);
                    } else {
                        resobj.m_CloneObj.transform.SetParent(father);
                    }
                }
                if (ObjDealFinish != null) {
                    ObjDealFinish(path, resobj.m_CloneObj, objarr);
                }
                return resobj.m_AsyncId;
            }
            long asyncId = ResourceManager.GetInstance.GetAsyncId();
            resobj = m_ResourceObjPool.GetObj(true);
            resobj.m_AsyncId = asyncId;
            resobj.m_Crc = crc;
            resobj.m_Clear = bClear;
            resobj.m_SceneTransform = scenetransform;
            resobj.m_Father = father;
            resobj.m_FinishCallBack = ObjDealFinish;
            resobj.m_Objarr = objarr;
            //调用ResourceManager的异步加载接口，需要注意，这方法里有两个回调，一个是objectmanger管理器的回调（OnAsyncLoadResFinish），另一个是调用方的回调（OnAsyncLoadObjFinish）,此处是objectmanger管理器的回调，也就是实例化资源加载完成后，先返回给objectmanger来处理缓存或加载到场景等操作，操作结束后，才会回调调用方的回调方法，容易绕，多理解一下
            ResourceManager.GetInstance.AsyncLoadResObj(path,resobj,OnResLoadFinish,priority);
            return asyncId;
        }
        /// <summary>
        /// 实例化异步加载完成后的回调
        /// 注意这个回调时 OnLoadResFinish，跟静态资源加载回调OnLoadObjFinish不同，
        /// </summary>
        /// <param name="path">路径，只会是完整路径</param>
        /// <param name="resobj">实例化的资源块</param>
        /// <param name="objarr">可变参数</param>
        void OnResLoadFinish(string path,ResourceObj resobj,params object[] objarr) {
            if (resobj == null) {
                return;
            }
            if (resobj.m_ResItem.m_Obj == null) {
                Debuger.LogError($"{path}资源加载错误，异步加载返回空");
            } else {
                if (resobj.m_SceneTransform) {
                    if (resobj.m_Father == null) {
                        resobj.m_CloneObj = GameObject.Instantiate(resobj.m_ResItem.m_Obj, SceneTransform) as GameObject;
                    } else {
                        resobj.m_CloneObj = GameObject.Instantiate(resobj.m_ResItem.m_Obj, resobj.m_Father) as GameObject;
                    }
                }else{
                    resobj.m_CloneObj = GameObject.Instantiate(resobj.m_ResItem.m_Obj) as GameObject;
                }
                resobj.m_OfflineData = resobj.m_CloneObj.GetComponent<OfflineDatabase>();
                if (m_AsyncResourceObjDic.ContainsKey(resobj.m_AsyncId)) {
                    m_AsyncResourceObjDic.Remove(resobj.m_AsyncId);
                }
                if (resobj.m_FinishCallBack != null) {
                    int tempid = resobj.m_CloneObj.GetInstanceID();
                    if (!m_ResourceObjCacheDic.ContainsKey(tempid)) {
                        m_ResourceObjCacheDic.Add(tempid, resobj);
                    }
                    resobj.m_FinishCallBack(path, resobj.m_CloneObj, objarr);
                }
            }
        }
#endregion
#region 实例化资源预加载
        /// <summary>
        /// 实例化资源预加载
        /// 有了异步之后为什么还要预加载，是因为异步加载实例化时（instance），还是同步操作的,大量资源同时instance还是会造成卡顿，所以需要预加载。
        /// 预加载是同步的，需要在加载进度条时提前把需要的资源预加载出来
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="count">预加载的数量</param>
        /// <param name="bClear">跳场景是否清除</param>
        public void PreLoadGameObject(string path, int count = 1, bool bClear = false, bool isfullpath = false) {
            List<GameObject> GOList = new List<GameObject>();
            for (int i = 0; i < count; i++) {
                GameObject obj = InstanceObject(path, false, bClear: bClear, isfullpath: isfullpath);
                GOList.Add(obj);
            }

            for (int i = 0; i < GOList.Count; i++) {
                GameObject obj = GOList[i];
                ReleaseObject(obj, isrecycle: true);
                obj = null;
            }
            GOList.Clear();
        }
#endregion
        /// <summary>
        /// 释放对象资源
        /// </summary>
        /// <param name="obj">释放对象</param>
        /// <param name="maxCacheCount">该对象对象池中最大的缓存数量，-1为无限制</param>
        /// <param name="destoryCache">是否销毁缓存</param>
        /// <param name="isrecycle">是否回收到对象池，因为回收对象池会进行父物体的切换，大量setparent会造成性能消耗会造成性能消耗，所以非必要资源少回收对象池</param>
        public void ReleaseObject(GameObject obj, int maxCacheCount = -1, bool destoryCache = false,bool isrecycle = true) {
            if (obj == null) {
                return;
            }
            ResourceObj resobj = null;
            int objid = obj.GetInstanceID();
            if (!m_ResourceObjCacheDic.TryGetValue(objid,out resobj)) {
                Debuger.Log("创建对象与回收的对象不一致");
                return;
            }
            if (resobj == null) {
                Debuger.LogError("发生错误，缓存已经被清除");
                return;
            }
            if (resobj.m_Already) {
                Debuger.LogError("该对象已经清除或者放回对象池，请检查是否没有清除引用");
                return;
            }
            //编辑下修改名称
            if (IsEditor) {
                obj.name += "(Recycle)";
            }
           
            List<ResourceObj> tmp = null;
            if (maxCacheCount == 0||destoryCache) { //不缓存就是不放回对象池
                m_ResourceObjCacheDic.Remove(objid);
                ResourceManager.GetInstance.DestoryResource(resobj, destoryCache);
                GameObject.Destroy(resobj.m_CloneObj);
                resobj.Reset();
                m_ResourceObjPool.Recycle(resobj);
            } else { //回收到对象池
                if (!m_ObjectPoolDic.TryGetValue(resobj.m_Crc, out tmp) || tmp == null) {
                    tmp = new List<ResourceObj>();
                    m_ObjectPoolDic.Add(resobj.m_Crc, tmp);
                }
                if (resobj.m_CloneObj!=null) {
                    if (isrecycle) {//如果需要回收对象池
                        resobj.m_CloneObj.transform.SetParent(RecyclePoolTrs);
                    } else {//不需要就直接不显示
                        resobj.m_CloneObj.SetActive(false);
                    }
                }
                if (maxCacheCount < 0 || tmp.Count < maxCacheCount) {
                    tmp.Add(resobj);
                    resobj.m_Already = true;
                    //引用计数减少
                    //只在这里减少，是因为只有从对象池中取引用计数才会增加，因此只有回到池子的对象引用计数才需要减少，其他情况，引用计数没增加过，也不需要减少
                    ResourceManager.GetInstance.DeCreaseResourceRef(resobj);
                } else {
                    m_ResourceObjCacheDic.Remove(resobj.m_CloneObj.GetInstanceID());
                    ResourceManager.GetInstance.DestoryResource(resobj, destoryCache);
                    GameObject.Destroy(resobj.m_CloneObj);
                    resobj.Reset();
                    m_ResourceObjPool.Recycle(resobj);
                }
            }
        }
        /// <summary>
        /// 取消实例化异步加载
        /// </summary>
        /// <param name="asyncid"></param>
        public void CancelLoad(long asyncid) {
            ResourceObj resobj = null;
            if(m_AsyncResourceObjDic.TryGetValue(asyncid,out resobj)&&ResourceManager.GetInstance.CancelLoad(resobj)){
                m_AsyncResourceObjDic.Remove(asyncid);
                resobj.Reset();
                m_ResourceObjPool.Recycle(resobj);
            }
        }
        /// <summary>
        /// 是否正在进行异步加载
        /// </summary>
        /// <param name="asyncid">异步加载唯一ID</param>
        /// <returns></returns>
        public bool IsingAsyncLoad(long asyncid) {
            return m_AsyncResourceObjDic[asyncid] != null;
        }
        /// <summary>
        /// 该对象是否是ObjectManager创建的
        /// </summary>
        /// <returns></returns>
        public bool IsObjectManagerCreate(GameObject obj) {
            ResourceObj resobj = m_ResourceObjCacheDic[obj.GetInstanceID()];
            return (resobj != null);
        }
        /// <summary>
        /// 通过对象获取离线数据
        /// 这里可以通过离线数据做很多拓展，例如需要获取游戏物体的某个组件，可以提前将组件写到离线数据里，然后获取离线数据即可拿到组件，这样效率比较高，直接GetComponent有性能损耗
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public OfflineDatabase FindOfflineData(GameObject obj) {
            OfflineDatabase data = null;
            ResourceObj resobj=null;
            m_ResourceObjCacheDic.TryGetValue(obj.GetInstanceID(),out resobj);
            if (resobj!=null) {
                data = resobj.m_OfflineData; 
            }
            return data;
        }
        /// <summary>
        /// 清空实例化对象池及缓存，一般跳场景时使用
        /// 注意有些不能清空的，根据ResourceObj中m_Clear判断
        /// </summary>
        public void ClearCache() {
            List<uint> tmpcrclist = new List<uint>();//待删除列表，用来删除的，不能再foreach中直接删除
            foreach (uint crc in m_ObjectPoolDic.Keys) {
                List<ResourceObj> resobjlist = m_ObjectPoolDic[crc];
                for (int i = resobjlist.Count-1; i>=0; i--) {
                    ResourceObj res = resobjlist[i];
                    if (!System.Object.ReferenceEquals(res.m_CloneObj,null)&&res.m_Clear) {
                        //清除缓存
                        m_ResourceObjCacheDic.Remove(res.m_CloneObj.GetInstanceID());
                        //销毁克隆对象，并回收ResourceObj
                        GameObject.Destroy(res.m_CloneObj);
                        res.Reset();
                        m_ResourceObjPool.Recycle(res);
                        resobjlist.Remove(res);
                    }
                }
                if (resobjlist.Count<=0) {//如果循环后，对象池没有东西了，就把crc添加到待删除列表
                    tmpcrclist.Add(crc);
                }
            }
            //根据待删除列表清除对象池，因为不能再foreach中直接删除
            for (int i = 0; i < tmpcrclist.Count; i++) {
                if (m_ObjectPoolDic.ContainsKey(tmpcrclist[i])) {
                    m_ObjectPoolDic.Remove(tmpcrclist[i]);
                }
            }
            tmpcrclist.Clear();
        }
        /// <summary>
        /// 根据crc删除某个对象的对象池内所有对象
        /// 这个方法一般在releaseitem时调用
        /// </summary>
        /// <param name="crc">crc</param>
        public void ClearPoolObject(uint crc) {
            List<ResourceObj> resobjlist = null;
            if (!m_ObjectPoolDic.TryGetValue(crc,out resobjlist)|| resobjlist==null) {
                return;
            }
            for (int i = resobjlist.Count-1; i>=0; i--) {
                ResourceObj res = resobjlist[i];
                if ( res.m_Clear&&!System.Object.ReferenceEquals(res.m_CloneObj, null) ) {
                    //清除缓存
                    m_ResourceObjCacheDic.Remove(res.m_CloneObj.GetInstanceID());
                    //销毁克隆对象，并回收ResourceObj
                    GameObject.Destroy(res.m_CloneObj);
                    res.Reset();
                    m_ResourceObjPool.Recycle(res);
                }
            }
            if (resobjlist.Count <= 0) {//如果循环后，对象池没有东西了，就把crc添加到待删除列表
                m_ObjectPoolDic.Remove(crc);
            }
        }

        public void RealseAll() {
            m_ClassPoolDic.Clear();
            m_ObjectPoolDic.Clear();
            m_ResourceObjPool = null;
            m_ResourceObjCacheDic.Clear();
            m_AsyncResourceObjDic.Clear();
        }
    }
}
