/****************************************************
    文件：ResourceManager.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/8/14 16:43:43
	功能：基础资源管理器，例如图片，声音等不需要实例化直接加载的资源，不涉及实例化操作，实例化操作由ObjectManager管理，资源池基于双向链表
*****************************************************/
///之所以使用双向链表，是因为资源使用频率，资源频繁使用放在最顶端，不常用的就逐步到底部，清空时，就可以从最底端清空不常用的，避免影响资源展示。
using UnityEngine;
using GF.Utils;
using GF.Pool;
using System.Collections.Generic;
using System.Collections;

namespace GF.Unity.AB {
    /// <summary>
    /// 实例化的对象资源块
    /// </summary>
    public class ResourceObj {
        /// <summary>
        /// 路径Crc
        /// </summary>
        public uint m_Crc = 0;
        /// <summary>
        /// 克隆的物体
        /// </summary>
        public GameObject m_CloneObj = null;
        /// <summary>
        /// 跳场景是否删除
        /// </summary>
        public bool m_Clear = true;
        ///// <summary>
        ///// 这个是对象的Guid
        ///// </summary>
        //public int m_Guid = 0;
        /// <summary>
        /// 此处guid是自定义的guid,是每一个异步加载的唯一标识，用来取消异步加载
        /// </summary>
        public long m_AsyncId = 0;
        /// <summary>
        /// 原资源（非克隆或者实例化的）的资源块
        /// </summary>
        public ResourceItem m_ResItem = null;
        /// <summary>
        /// 是否已经清除/放回对象池
        /// </summary>
        public bool m_Already = false;
        public OfflineDatabase m_OfflineData = null;
        #region 以下参数异步使用
        /// <summary>
        /// 是否放在场景的SceneTransform下,主要异步加载使用，同步时，直接就判断了，用不上
        /// </summary>
        public bool m_SceneTransform = false;
        /// <summary>
        /// 加载ui或者需要制定父级的物体时使用的参数
        /// </summary>
        public Transform m_Father = null;
        /// <summary>
        /// 异步加载的回调
        /// </summary>
        public OnAsyncLoadObjFinish m_FinishCallBack = null;
        /// <summary>
        /// 回调的参数
        /// </summary>
        public object[] m_Objarr = null;
        #endregion
        public void Reset() {
            m_Crc = 0;
            m_CloneObj = null;
            m_Clear = true;
            m_ResItem = null;
            m_SceneTransform = false;
            m_Father = null;
            m_FinishCallBack = null;
            m_Objarr = null;
            m_AsyncId = 0;
            m_OfflineData = null;
        }
    }
    #region 静态资源和实例化资源异步加载所需要的中间类
    /// <summary>
    /// 静态资源异步加载资源完成后的回调
    /// </summary>
    public delegate void OnAsyncLoadObjFinish(string path,UnityEngine.Object obj, params object[] objarr);
    /// <summary>
    /// 实例化资源异步加载资源完成后的回调，回调给ObjectManager管理器
    /// </summary>
    public delegate void OnAsyncLoadResFinish(string path, ResourceObj resobj, params object[] objarr);
    /// <summary>
    /// 异步资源加载的优先级枚举
    /// </summary>
    public enum LoadResPriority {
        RES_HIGHT = 0,//最高优先级
        RES_MIDDLE = 1,//中等优先级
        RES_SLOW = 2,//低优先级
        RES_NUM = 3,//优先级的数量，一共三个优先级
    }
    /// <summary>
    /// 异步加载参数类
    /// </summary>
    public class AsyncLoadParam {
        //主要解决通过一个资源多处同时加载的情况，此时需要将同一个资源回调多次，所以需要通过list记录下来，依次调用
        public List<AsyncCallBack> m_CallBackList = new List<AsyncCallBack>();
        public uint m_Crc;
        public string m_Path;
        //public bool m_IsSprite = false;//(应该没有这个问题，先注释了)加这个变量是因为Ab包异步加载的asset是无法转换成图片的，需要加载时加上spite类型才行。因此区分一下sprite图片类型
        public LoadResPriority m_Priority = LoadResPriority.RES_SLOW;
        public bool m_Clear;
        //要使用类对象池，写个重置方法
        public void Reset() {
            m_CallBackList.Clear();
            m_CallBackList = null;
            m_Path = string.Empty;
            //m_IsSprite = false;
            m_Crc = 0;
            m_Priority = LoadResPriority.RES_SLOW;
            m_Clear = true;
        }

    }
    /// <summary>
    /// 回调类，主要解决通过一个资源多处同时加载的情况，此时需要将同一个资源回调多次，所以需要通过list记录下来，依次调用
    /// </summary>
    public class AsyncCallBack {
        /// <summary>
        /// 静态资源的回调的方法
        /// </summary>
        public OnAsyncLoadObjFinish m_ObjDealFinish = null;
        //----------------------------------针对ObjectManager的----------------------------------------
        /// <summary>
        /// 实例化资源的回调的方法，ObjectManager管理器的回调
        /// </summary>
        public OnAsyncLoadResFinish m_ResDealFinish = null;

        public ResourceObj resobj = null;
        //----------------------------------针对ObjectManager的----------------------------------------//
        public object[] objarr = null;//回调的参数
        //要使用类对象池，写个重置方法
        public void Reset() {
            m_ObjDealFinish = null;
            m_ResDealFinish = null;
            objarr = null;
            resobj = null;
        }
    }
    #endregion
    public class ResourceManager : Singleton<ResourceManager> {
        /// <summary>
        /// 资源前置路径，这里是为了简化开发使用的。
        /// 资源完整的路径是：Assets/Art/audio/skill/fight/Fashu_1.wav 每次加载都需要写全路径，很烦，所以将Assets/Art/提取出来
        /// </summary>
        public string ResPrePath = "Assets/Art/";
        /// <summary>
        /// 通过变量判断是否通过ab包加载
        /// </summary>
        public bool m_LoadFormAssetBundle = false;
        /// <summary>
        /// 缓存使用的资源列表，包含完整资源
        /// 属性直接new是.net framework4.0的特性，需要切换目标框架为.net framework4.0，unity也需要切换
        /// </summary>
        public Dictionary<uint, ResourceItem> m_AssetCacheDic { get; set; } = new Dictionary<uint, ResourceItem>();
        /// <summary>
        /// 这里存储的是没有使用的资源（引用次数为0的），比如图片材质音频等等
        /// 这里缓存的内容并没有从m_AssetCacheDic字典中清理掉，m_AssetCacheDic字典中也有还有
        /// 存它是防止我们不使用某些游离资源时，gc自动清除，再次加载还得从硬盘加载
        /// 当然也不是无限制缓存，达到最大缓存数量后，释放资源池中最早没用的资源
        /// </summary>
        protected CMapList<ResourceItem> m_NoRefrenceMapList = new CMapList<ResourceItem>();

        //--------------------------------------异步资源加载----------------------------------
        #region 异步资源加载代码块
        /// <summary>
        /// 异步加载时最大卡着连续异步加载的时间。，单位是微秒，默认20微妙
        /// </summary>
        public long MAXLOADINGTIME = 200000;
        /// <summary>
        /// 异步加载参数类对象池,默认50个，根据需要自行调整
        /// </summary>
        protected ClassObjectPool<AsyncLoadParam> m_AsyncLoadParamPool = ObjectManager.GetInstance.GetClassObjectPool<AsyncLoadParam>(50);
        /// <summary>
        /// 加载回调类的对象池，默认100个，根据需要自行调整
        /// </summary>
        protected ClassObjectPool<AsyncCallBack> m_AsyncCallBackPool = ObjectManager.GetInstance.GetClassObjectPool<AsyncCallBack>(100);
        /// <summary>
        /// 异步加载队列list数组，按优先等级分配
        /// </summary>
        protected List<AsyncLoadParam>[] m_AsyncLoadingAssetList = new List<AsyncLoadParam>[(int)LoadResPriority.RES_NUM];
        /// <summary>
        /// 正在加载的资源字典，因为是异步，所以要避免重复加载，通过这个字段来判断是否正在加载，如果正在加载，就不再重复加载资源
        /// 之所以不使用上面的异步加载队列list数组来做重复判断，是因为list数组结构判定不方便，其实从效率上来说，可能直接用list数组效率更好些
        /// </summary>
        protected Dictionary<uint, AsyncLoadParam> m_AsyncLoadingAssetDic = new Dictionary<uint, AsyncLoadParam>();
        /// <summary>
        /// //异步加载初始化，主要初始化协程，使用协程，需要使用monobehiour
        /// </summary>
        protected MonoBehaviour m_Startmono;
        public void Init(MonoBehaviour mono) {
            for (int i = 0; i < m_AsyncLoadingAssetList.Length; i++) {
                m_AsyncLoadingAssetList[i] = new List<AsyncLoadParam>();
            }
            m_Startmono = mono;
            m_Startmono.StartCoroutine(AsyncLoadCor());//开启协程
        }
        /// <summary>
        /// 异步加载资源，加载不需要实例化的资源。例如图片和音频
        /// 这个方法并没有真正加载，只是把资源信息封装，放到了m_AsyncLoadingAssetList待加载
        /// 真正的加载是协程中处理的
        /// 异步加载这里还没法设置跳场景是否删除，需要完善。todo
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="deleOnFinish">资源加载完成后的委托</param>
        ///  <param name="crc">上层调用时可以传递crc，也可以不传递</param>
        /// <param name="objarr">可变参数数组，为了方便上层调用传递参数</param>
        ///  <param name="isfullpath">是否是完整路径</param>
        public void AsyncLoadResource(string path, OnAsyncLoadObjFinish deleOnFinish, LoadResPriority priority, uint crc = 0, bool bclear = true, bool isfullpath = false, params object[] objarr) {
            if (!isfullpath) {
                path = ResPrePath + path;
            }
            if (crc == 0) {
                crc = Crc32.GetCRC32(path);
            }
            //如果缓存里有，就直接返回
            ResourceItem item = GetCacheResourceItem(crc);
            if (item != null) {
                if (deleOnFinish != null)
                    deleOnFinish(path, item.m_Obj, objarr);
                return;
            }
            //缓存没有，继续加载
            AsyncLoadParam param = null;
            //先判断是否已经正在加载了，没有的话，才加载
            if (!m_AsyncLoadingAssetDic.TryGetValue(crc, out param) || param == null) {
                param = m_AsyncLoadParamPool.GetObj(true);//通过对象池获取，并赋值
                param.m_Crc = crc;
                param.m_Path = path;
                param.m_Priority = priority;
                param.m_Clear = bclear;
                m_AsyncLoadingAssetDic.Add(param.m_Crc, param);//加入到正在加载的资源队列中，方便判定重复加载
                m_AsyncLoadingAssetList[(int)priority].Add(param);//按优先级加入到加载的资源队列中
            }
            //往回调列表中添加回调
            if (deleOnFinish != null) {
                AsyncCallBack tempcallback = m_AsyncCallBackPool.GetObj(true);
                tempcallback.m_ObjDealFinish = deleOnFinish;
                tempcallback.objarr = objarr;
                param.m_CallBackList.Add(tempcallback);
            } else {
                Debuger.LogError(param.m_Path + "异步加载资源回调为空,请检查！");
            }
        }
        /// <summary>
        /// 异步加载使用地协程，一般在游戏运行时开启，然后不停的循环加载待加载列表里的资源
        /// 每次循环只加载一个资源，从优先级最高的开始加载
        /// </summary>
        /// <returns></returns>
        IEnumerator AsyncLoadCor() {
            //记录一下上次加载的时间
            long lastyeildtime = System.DateTime.Now.Ticks;
            //回调
            List<AsyncCallBack> CallBackList = null;
            while (true) {
                bool isyield = false;
                if (m_AsyncLoadingAssetDic.Count <= 0) {/// 解决bug，因为wile true 协程永远不结束，并且优先级列表加载完后不清除，会一直不停的加载，虽然不会重复执行加载具体资源，但循环执行也是浪费资源，这里设置如果没有资源需要加载，就等待一针
                    yield return null;
                    continue;
                }
                for (int i = 0; i < m_AsyncLoadingAssetList.Length; i++) {
                    if (m_AsyncLoadingAssetList[(int)LoadResPriority.RES_HIGHT].Count > 0) {
                        i = (int)LoadResPriority.RES_HIGHT;
                    } else if (m_AsyncLoadingAssetList[(int)LoadResPriority.RES_MIDDLE].Count>0) {
                        i = (int)LoadResPriority.RES_MIDDLE;
                    }
                    List<AsyncLoadParam> loadinglist = m_AsyncLoadingAssetList[i];
                    if (loadinglist.Count <= 0) {
                        continue;
                    }
                    AsyncLoadParam loadingparam = loadinglist[0];
                    loadinglist.RemoveAt(0);
                    CallBackList = loadingparam.m_CallBackList;

                    UnityEngine.Object obj = null;
                    ResourceItem item = null;
//#if UNITY_EDITOR   //编辑器加载  打包到dll中时，要记上#if UNITY_EDITOR ，因为dll中不认，并且还会导致打包时必须放dll进入editor
                    if (!m_LoadFormAssetBundle) {
                        item = AssetBundleManager.GetInstance.FindResourceItem(loadingparam.m_Crc);
                        if (item != null && item.m_Obj != null) {
                            obj = item.m_Obj;
                            item.m_Clear = loadingparam.m_Clear;
                        } else {
                            if (item == null) {//这种情况是一般是因为编辑器下没有打包，配置表中没有这个东西导致的
                                item = new ResourceItem();
                                item.m_Crc = loadingparam.m_Crc;
                                item.m_Clear = loadingparam.m_Clear;
                            }
                            obj = LoadAssetByEditor<UnityEngine.Object>(loadingparam.m_Path);
                            //从编辑器加载是没有异步的，我们模拟一下...仅仅为了和同步区分开
                            yield return new WaitForSeconds(0.5f);
                        }
                        Debuger.Log("异步从编辑器加载" + obj.name);
                    }
//#endif

                    if (obj == null) {
                        item = AssetBundleManager.GetInstance.GetResourceItem(loadingparam.m_Crc);
                        if (item != null && item.m_LoadedAB != null) {
                            if (item.m_Obj != null) {
                                obj = item.m_Obj;
                            } else {
                                #region asset转sprite没有问题，这里应该不用，先注释，使用看看
                                //AssetBundleRequest abrequest = null;
                                //if (loadingparam.m_IsSprite) {//加这个变量是因为Ab包异步加载的asset是无法转换成图片的，需要加载时加上spite类型才行。因此区分一下sprite图片类型
                                //    abrequest = item.m_LoadedAB.LoadAssetAsync<Sprite>(item.m_AssetName);
                                //} else {
                                //    abrequest = item.m_LoadedAB.LoadAssetAsync(item.m_AssetName);
                                //}
                                #endregion
                                AssetBundleRequest abrequest = item.m_LoadedAB.LoadAssetAsync(item.m_AssetName);
                                ;
                                yield return abrequest;
                                if (abrequest.isDone) {
                                    obj = abrequest.asset;//如果LoadAssetAsync不写类型，加载出来的asset Object无法转换成Sprite.  
                                }
                                lastyeildtime = System.DateTime.Now.Ticks;
                            }
                            item.m_Clear = loadingparam.m_Clear;
                        }
                        Debuger.Log("从ab包加载" + obj.name);
                    }
                    //缓存资源,有多少个callback，相当于有多少个引用
                    CacheResource(loadingparam.m_Path, loadingparam.m_Crc, ref item, obj, CallBackList.Count);
                    //执行回调
                    for (int j = 0; j < CallBackList.Count; j++) {
                        //基础资源异步加载的回调
                        if (CallBackList[j] != null && CallBackList[j].m_ObjDealFinish != null) {
                            CallBackList[j].m_ObjDealFinish(loadingparam.m_Path, obj, CallBackList[j].objarr);
                            CallBackList[j].m_ObjDealFinish = null;
                        }
                        //ObjectManager实例化资源异步加载的回调
                        if (CallBackList[j] != null && CallBackList[j].m_ResDealFinish != null&& CallBackList[j].resobj!=null) {
                            CallBackList[j].resobj.m_ResItem = item;
                            CallBackList[j].m_ResDealFinish(loadingparam.m_Path, CallBackList[j].resobj, CallBackList[j].resobj.m_Objarr);
                            CallBackList[j].m_ResDealFinish = null;
                        }
                        CallBackList[j].Reset();
                        m_AsyncCallBackPool.Recycle(CallBackList[j]);
                    }
                    //把不用的资源清理重置一下
                    obj = null;
                    CallBackList.Clear();
                    m_AsyncLoadingAssetDic.Remove(loadingparam.m_Crc);//加载完成就从正在加载的列表中移除

                    loadingparam.Reset();
                    m_AsyncLoadParamPool.Recycle(loadingparam);
                    //如果m_AsyncLoadingAssetList内容过多，可能循环时就已经超时了，所以循环时也判断一下
                    if (System.DateTime.Now.Ticks - lastyeildtime > MAXLOADINGTIME) {
                        yield return null;
                        lastyeildtime = System.DateTime.Now.Ticks;
                        isyield = true;
                    }
                }
                //不直接yield return null,因为那样会导致每帧只加载一个资源，太慢了，我们应该根据情况设计一个加载间隔，不能太大，也要保证速度，默认20万微秒
                if (!isyield || System.DateTime.Now.Ticks - lastyeildtime > MAXLOADINGTIME) {
                    lastyeildtime = System.DateTime.Now.Ticks;
                    yield return null;
                }
            }
        }
        #endregion
        //--------------------------------------同步资源加载----------------------------------
        #region 同步资源加载
        /// <summary>
        ///  获取资源的方法，同步资源加载，外部直接调用，仅加载不需要实例化的资源，例如图片，音频等资源
        ///这个方法时上层最经常调用的，上层基本不会调用AssetBundleManger的方法，都是由ResourceManager调用的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T LoadResource<T>(string path, bool bClear = true, bool isfullpath = false) where T : UnityEngine.Object {
            
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
            if (!isfullpath) {
                path = ResPrePath + path;
            }
            uint crc = Crc32.GetCRC32(path);
            ResourceItem item = GetCacheResourceItem(crc);
            if (item != null) {//缓存中有，直接返回
                return item.m_Obj as T;
            }
            //缓存中没有这个资源，需要重新加载
            T obj = null;
// #if UNITY_EDITOR   //编辑器加载  打包到dll中时，要取消#if UNITY_EDITOR ，因为dll中不认，并且还会导致打包时必须放dll进入editor
            //编辑器加载
            if (!m_LoadFormAssetBundle) {
                item = AssetBundleManager.GetInstance.FindResourceItem(crc);
                if (item != null && item.m_LoadedAB != null) {
                    if (item.m_Obj!=null) {
                        obj = item.m_Obj as T;
                    } else {
                        obj = item.m_LoadedAB.LoadAsset<T>(item.m_AssetName);
                    }
                } else {
                    if (item == null) {//这种情况是一般是因为编辑器下没有打包，配置表中没有这个东西导致的
                        item = new ResourceItem();
                        item.m_Crc = crc;
                    }
                    obj = LoadAssetByEditor<T>(path);
                }
                Debuger.Log($"从编辑器加载{path}");
            }
//#endif
            //从ab包加载
            if (obj == null&& m_LoadFormAssetBundle) {
                item = AssetBundleManager.GetInstance.GetResourceItem(crc);
                if (item != null && item.m_LoadedAB != null) {
                    if (item.m_Obj != null) {
                        obj = item.m_Obj as T;
                    } else {
                        obj = item.m_LoadedAB.LoadAsset<T>(item.m_AssetName);
                    }
                }
                Debuger.Log("从ab包加载" + obj.name);
            }
            item.m_Clear = bClear;
            CacheResource(path, crc, ref item, obj);
            return obj;
        }
//#if UNITY_EDITOR   //编辑器加载  打包到dll中时，要取消#if UNITY_EDITOR ，因为dll中不认
        protected T LoadAssetByEditor<T>(string path) where T : UnityEngine.Object {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        }
//#endif
        #endregion

        //--------------------------------------预加载资源----------------------------------
        #region 预加载资源
        /// <summary>
        /// 资源预加载
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="isfullpath">是不是完整路径，默认false,就意味着不需要写Assets/Art/了</param>
        /// <param name="bclear">跳场景是否清除</param>

        public void PreLoadResource(string path, bool isfullpath = false,bool bClear = false) {
            if (string.IsNullOrEmpty(path)) {
                Debuger.LogError("预加载资源错误，路径为空");
                return;
            }
            if (!isfullpath) {
                path = ResPrePath + path;
            }
            uint crc = Crc32.GetCRC32(path);
            ResourceItem item = GetCacheResourceItem(crc, 0);//预加载，不需要计算引用计数，送以给0
            if (item != null) {//缓存中有，直接返回
                return;
            }
            //缓存中没有这个资源，需要重新加载
            Object obj = null;
//#if UNITY_EDITOR   //编辑器加载  打包到dll中时，要取消#if UNITY_EDITOR ，因为dll中不认,因为dll中不认，并且还会导致打包时必须放dll进入editor
            if (!m_LoadFormAssetBundle) {
                item = AssetBundleManager.GetInstance.FindResourceItem(crc);
                if (item != null && item.m_Obj != null) {
                    obj = item.m_Obj as Object;
                } else {
                    if (item == null) {//这种情况是一般是因为编辑器下没有打包，配置表中没有这个东西导致的
                        item = new ResourceItem();
                        item.m_Crc = crc;
                    }
                    obj = LoadAssetByEditor<Object>(path);
                }
                Debuger.Log($"从编辑器加载{obj.name}");
            }
//#endif
            //从ab包加载
            if (obj == null) {
                item = AssetBundleManager.GetInstance.GetResourceItem(crc);
                if (item != null && item.m_LoadedAB != null) {
                    if (item.m_Obj != null) {
                        obj = item.m_Obj as Object;
                    } else {
                        obj = item.m_LoadedAB.LoadAsset<Object>(item.m_AssetName);
                    }
                }
                Debuger.Log("从ab包加载" + obj.name);
            }
            CacheResource(path, crc, ref item, obj);
            //因为是预加载，所以加载缓存后，就直接扔到不用的双向链表中
            item.m_Clear = bClear;
            DestoryResource(item, false);

        }
        #endregion

        //--------------------------------------缓存销毁方法---------------------------------
        /// <summary>
        /// 缓存这个资源
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="crc">资源路径crc</param>
        /// <param name="item">资源对应资源块,ref引用，直接修改原变量</param>
        /// <param name="obj">对应资源对象</param>
        /// <param name="addrefcount">引用次数，这里的应用次数，同步永远是1，异步加载时，可能存在多个同时加载使用，异步传递回调list的count</param>
        private void CacheResource(string path, uint crc, ref ResourceItem item, UnityEngine.Object obj, int addrefcount = 1) {
            //缓存之前判断一下缓存是否太多，是否超过内存80%，超过了就清理最就不用的资源
            WashOut();
            if (item == null) {
                Debuger.LogError($"ResourceItem资源块为空，资源路径为：{path}");
                return;
            }
            if (obj == null) {
                Debuger.LogError($"Resource资源加载失败，资源路径为：{path},Crc:{crc}");
                return;
            }
            item.m_Obj = obj;
            item.m_Guid = obj.GetInstanceID();
            item.m_LastUseTime = Time.realtimeSinceStartup;
            item.RefCount += addrefcount;
            //if (m_AssetCacheDic.ContainsKey(crc)) { //根据测试，TryGetValue的性能远比ContainsKey强的多，可参考网上测试的博文
            ResourceItem olditem = null;
            //如果缓存中已经存在这个，在目前的环境内部不会发生换个情况，已经存在在上部分代码判断中已经跳出，这里是为了容错，防止其他模块触发调用这个方法报错
            if (m_AssetCacheDic.TryGetValue(crc, out olditem)) {
                m_AssetCacheDic[crc] = item;
            } else {
                m_AssetCacheDic.Add(crc, item);
            }
        }
        /// <summary>
        /// 从缓存字典中获取资源，并添加引用次数
        /// </summary>
        /// <param name="crc">crc</param>
        /// <param name="addcount">引用次数，默认1，因为有可能一次地方引用该资源</param>
        /// <returns></returns>
        private ResourceItem GetCacheResourceItem(uint crc, int addcount = 1) {
            ResourceItem item = null;
            if (m_AssetCacheDic.TryGetValue(crc, out item)) {
                if (item != null) {
                    item.RefCount += addcount;
                    item.m_LastUseTime = Time.realtimeSinceStartup;
                    //item.m_LastUseTime = Timer.GFTime.GetTimeSinceStartup();
                    //if (item.RefCount<=1) {//这一步理论上进不来，只是容错判断
                    //    m_NoRefrenceMapList.Remove(item);
                    //}
                }
            }
            return item;
        }
        /// <summary>
        /// 自定义的高中低配置制定m_NoRefrenceMapList的大小
        /// </summary>
        public int MAXMAPLISTSIZE = 300;
        /// <summary>
        /// 清理内存的方法
        /// 这里只是简单粗糙的处理，根据自定义的高中低配置制定m_NoRefrenceMapList的大小，目前是高配1000，中配500，低配300，超过这个个数就自动清理一半的内容
        /// 正经的内存清理需要考虑每个资源的大小和当前设备的内存使用情况，处理起来很复杂，设计各系统的底层调用，先不考虑了
        /// </summary>
        protected void WashOut() {
            //超过这个个数就自动清理一半的内容
            while (m_NoRefrenceMapList.Size() >= MAXMAPLISTSIZE) {
                for (int i = 0; i < MAXMAPLISTSIZE/2; i++) {
                    ResourceItem item = m_NoRefrenceMapList.BackTail();
                    DestoryResource(item, true);
                }
            }
        }
        #region 清理或者删除的方法多态
        /// <summary>
        /// 清理或回收资源的方法，第二个参数true就清理缓存，真删除。false其实就是将它添加到m_NoRefrenceMapList，不是真的删除
        /// </summary>
        /// <param name="item">资源块变量</param>
        /// <param name="destorycache">true就清理缓存，删除。false其实就是将它添加到m_NoRefrenceMapList，不是真的删除</param>
        public void DestoryResource(ResourceItem item, bool destorycache = false) {
            if (item == null || item.RefCount > 0) {
                return;
            }
            item.m_Clear = destorycache;
            //如果不删除缓存，添加到m_NoRefrenceMapList
            if (!destorycache) {
                m_NoRefrenceMapList.MoveToHead(item);
                return;
            }
            //此处注意，如果真的删除，才会从m_AssetCacheDic字典中移除，否则不移除，所以，字典里装的是完整的缓存资源
            if (!m_AssetCacheDic.Remove(item.m_Crc)) { //通过是否成功从缓存中移除成功判断,item变量是否有问题，如果移除失败，就直接返回
                return;
            }
            //假如一个资源第一次删除时为fales,第二次为true，会出现m_NoRefrenceMapList中没有清理它的问题
            m_NoRefrenceMapList.Remove(item);//删除函数里已经做了包含判定，不用自己判定直接用就行了。
            //执行到这里才是真的删除
            AssetBundleManager.GetInstance.ReleaseAsset(item);
            ObjectManager.GetInstance.ClearPoolObject(item.m_Crc);
            if (item.m_Obj != null) {//引用置空
                item.m_Obj = null;
            }
        }
        /// <summary>
        /// 不需要实例化的资源的卸载，例如图片或者音频
        /// </summary>
        /// <param name="obj">不需要实例化的资源</param>
        /// <param name="destorycache">true就清理缓存，删除。false其实就是将它添加到m_NoRefrenceMapList，不是真的删除</param>
        /// <returns></returns>
        public bool DestoryResource(UnityEngine.Object obj, bool destorycache = false) {
            if (obj == null) {
                Debuger.Log("销毁的obj资源为空");
                return false;
            }
            ResourceItem item = null;
            foreach (ResourceItem tmpitem in m_AssetCacheDic.Values) {
                if (tmpitem.m_Guid == obj.GetInstanceID()) {
                    item = tmpitem;
                }
            }
            if (item == null) {
                Debuger.LogError($"obj资源{obj.name}可能被释放了多次");
                return false;
            } else {
                item.RefCount--;
                DestoryResource(item, destorycache);
                return true;
            }
        }
        /// <summary>
        /// 实例化ResObj的资源的卸载
        /// </summary>
        /// <param name="obj">实例化的ResObj资源</param>
        /// <param name="destorycache">true就清理缓存，删除。false其实就是将它添加到m_NoRefrenceMapList，不是真的删除</param>
        /// <returns></returns>
        public bool DestoryResource(ResourceObj resobj, bool destorycache = false) {
            if (resobj == null) {
                Debuger.Log("销毁的resobj资源为空");
                return false;
            }
            ResourceItem item = resobj.m_ResItem;

            if (item == null) {
                Debuger.LogError($"obj资源{resobj.m_CloneObj.name}可能被释放了多次");
                return false;
            } else {
                item.RefCount--;
                DestoryResource(item, destorycache);
                return true;
            }
        }
        /// <summary>
        /// 不需要实例化的资源的卸载，例如图片或者音频,同上方法，只是通过路径卸载
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="destorycache">销毁缓存</param>
        /// <param name="isfullpath">是否是全路径，全路径就不需要加ResPrePath路径头了</param>
        /// <returns></returns>
        public bool DestoryResource(string path, bool destorycache = false, bool isfullpath = false) {
            if (string.IsNullOrEmpty(path)) {
                Debuger.Log("销毁的路径为空");
                return false;
            }
            if (!isfullpath) {
                path = ResPrePath + path;
            }
            uint crc = Crc32.GetCRC32(path);
            ResourceItem item = null;
            if (!m_AssetCacheDic.TryGetValue(crc, out item) || item == null) {
                Debuger.LogError($"资源{path}可能被释放了多次");
                return false;
            }
            item.RefCount--;
            DestoryResource(item, destorycache);
            return true;
        }
        #endregion
        /// <summary>
        /// 根据是否删除，清除缓存，经常在跳场景时使用
        /// item.m_Clear为false的资源一般都是需要全局使用的例如全局UI，或者预加载的资源
        /// </summary>
        public void ClearCache() {
            //建一个临时的list是因为，DestoryResourceItem中会进行m_AssetCacheDic的remove操作，导致foreach出问题，不能直接remove
            List<ResourceItem> tmpitemlist = new List<ResourceItem>();
            foreach (ResourceItem item in m_AssetCacheDic.Values) {
                if (item.m_Clear) {
                    Debuger.Log(item.m_AssetName+"-"+ item.RefCount);
                    tmpitemlist.Add(item);
                }
            }
            for (int i = 0; i < tmpitemlist.Count; i++) {
                // tmpitemlist[i].RefCount = 0;//跳场景时，如果m_Clear = true的话，避免忘记清除资源，无法删除该资源。正常是不需要清理的，因为正常资源不使用要手动清除，或者放到双向链表中，由双向链表根据内存清除，但如果忘记手动清除这个资源，一旦跳场景，这个资源就无法清除，只能等gc去清除。所以，但凡是忘记清除的资源，跳场景时，强制清除
                if (tmpitemlist[i].RefCount>0) {
                    Debuger.LogError($"静态资源{tmpitemlist[i].m_ABName}中的{tmpitemlist[i].m_AssetName}的m_Clear=true,但存在引用未卸载，请检查");
                }
                DestoryResource(tmpitemlist[i], true);
            }
            tmpitemlist.Clear();
        }
        /// <summary>
        /// 释放所有资源
        /// </summary>
        public void ReleaseAll() {
            foreach (ResourceItem item in m_AssetCacheDic.Values) {
                AssetBundleManager.GetInstance.ReleaseAsset(item);
                item.m_Obj = null;
            }
            m_AssetCacheDic.Clear();
            //m_NoRefrenceMapList.Clear(); //应该不需要主动调用，链表的析构方法已经调用过了
            m_Startmono.StopCoroutine(AsyncLoadCor());
            for (int i = 0; i < m_AsyncLoadingAssetList.Length; i++) {
                m_AsyncLoadingAssetList[i].Clear();
            }
            m_AsyncLoadingAssetList = null;
            m_AsyncLoadingAssetDic.Clear();
            if (!m_LoadFormAssetBundle) {
                Resources.UnloadUnusedAssets();
            }
        }

        #region ObjectManager所需要的接口类
        /// <summary>
        /// 获取ResourceObj对应的ResourceItem
        /// 针对ObjectManager中同步加载资源InstanceObject的方法
        /// </summary>
        /// <param name="path"></param>
        /// <param name="resobj"></param>
        /// <returns></returns>
        public ResourceObj GetResourceItemInObj(string path, ResourceObj resobj) {
            if (resobj == null) {
                return null;
            }
            uint crc = resobj.m_Crc == 0 ? Crc32.GetCRC32(path) : resobj.m_Crc;
            ResourceItem item = GetCacheResourceItem(crc);
            if (item != null) {
                resobj.m_ResItem = item;
                return resobj;
            }
            GameObject obj = null;
//#if UNITY_EDITOR   //编辑器加载  打包到dll中时，要取消#if UNITY_EDITOR ，因为dll中不认，并且还会导致打包时必须放dll进入editor
            if (!m_LoadFormAssetBundle) {
                item = AssetBundleManager.GetInstance.FindResourceItem(crc);
                Debuger.Log(path);
                if (item != null && item.m_Obj != null) {
                    obj = item.m_Obj as GameObject;
                } else {
                    if (item == null) {//这种情况是一般是因为编辑器下没有打包，配置表中没有这个东西导致的
                        item = new ResourceItem();
                        item.m_Crc = crc;
                    }
                    obj = LoadAssetByEditor<GameObject>(path);
                }
                Debuger.Log($"从编辑器加载{obj.name}");
            }
//#endif
            //从ab包加载
            if (obj == null) {
                item = AssetBundleManager.GetInstance.GetResourceItem(crc);
                if (item != null && item.m_LoadedAB != null) {
                    if (item.m_Obj != null) {
                        obj = item.m_Obj as GameObject;
                    } else {
                        obj = item.m_LoadedAB.LoadAsset<GameObject>(item.m_AssetName);
                    }
                }
                Debuger.Log($"从ab包加载{item.m_AssetName}");
            }
            CacheResource(path, crc, ref item, obj);
            item.m_Clear = resobj.m_Clear;
            resobj.m_ResItem = item;
            return resobj;
        }
        /// <summary>
        /// 针对ObjecManager管理器的实例化资源的异步加载接口
        /// </summary>
        /// <param name="path">资源路径，此处路径一定为完整路径</param>
        /// <param name="resobj">实例化资源块</param>
        /// <param name="ResdealFinsh">ObjectMaanger管理器的回调</param>
        /// <param name="priority">资源又相继</param>
        public void AsyncLoadResObj(string path,ResourceObj resobj,OnAsyncLoadResFinish ResdealFinsh,LoadResPriority priority) {
            ResourceItem item = GetCacheResourceItem(resobj.m_Crc);
            if (item!=null) {//如果缓存存在
                item.m_Clear = resobj.m_Clear;
                resobj.m_ResItem = item;
                if (ResdealFinsh != null) {
                    ResdealFinsh(path,resobj);
                }
                return;
            }
            //没有缓存
            AsyncLoadParam param = null; 
            //先判断是否已经正在加载了，没有的话，才加载
            if (!m_AsyncLoadingAssetDic.TryGetValue(resobj.m_Crc, out param) || param == null) {
                param = m_AsyncLoadParamPool.GetObj(true);//通过对象池获取，并赋值
                param.m_Crc = resobj.m_Crc;
                param.m_Path = path;
                param.m_Priority = priority;
                param.m_Clear = resobj.m_Clear;
                m_AsyncLoadingAssetDic.Add(param.m_Crc, param);//加入到正在加载的资源队列中，方便判定重复加载
                m_AsyncLoadingAssetList[(int)priority].Add(param);//按优先级加入到加载的资源队列中
            }
            //往回调列表中添加回调
            if (ResdealFinsh != null) {
                AsyncCallBack tempcallback = m_AsyncCallBackPool.GetObj(true);
                tempcallback.m_ResDealFinish = ResdealFinsh;
                tempcallback.resobj = resobj;
                param.m_CallBackList.Add(tempcallback);
            } else {
                Debuger.LogError($"{param.m_Path}异步加载资源回调为空,请检查！");
            }
        }
        /// <summary>
        /// objectManager使用的取消异步加载接口
        /// </summary>
        /// <param name="resobj">实例化资源块</param>
        /// <returns>是否成功</returns>
        public bool CancelLoad(ResourceObj resobj) {
            AsyncLoadParam param = null;
            //通过正在异步加载的dic和异步加载对应优先级的队列来判断此异步加载是否正在进行
            if (m_AsyncLoadingAssetDic.TryGetValue(resobj.m_Crc,out param)&&m_AsyncLoadingAssetList[(int)param.m_Priority].Contains(param)) {
                //循环将属于这个实例化资源的回调统统都移除
                for (int i = 0; i < param.m_CallBackList.Count; i++) {
                    AsyncCallBack tmpcallback = param.m_CallBackList[i];
                    if (tmpcallback!=null&& (resobj == tmpcallback.resobj)) {
                        param.m_CallBackList.Remove(tmpcallback);
                        tmpcallback.Reset();
                        m_AsyncCallBackPool.Recycle(tmpcallback);
                    }
                }
                if (param.m_CallBackList.Count<=0) {
                    m_AsyncLoadingAssetList[(int)param.m_Priority].Remove(param);
                    param.Reset();
                    m_AsyncLoadParamPool.Recycle(param);
                    m_AsyncLoadingAssetDic.Remove(param.m_Crc);
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region 引用计数的处理方法
        /// <summary>
        /// 通过crc获取并增加一个引用计数
        /// </summary>
        /// <param name="crc"></param>
        /// <param name="count"></param>
        /// <returns>引用计数</returns>
        public int InCreaseResourceRef(uint crc, int count = 1) {
            ResourceItem item = null;
            if (!m_AssetCacheDic.TryGetValue(crc, out item) || item == null)
                return 0;//如果没有这个资源就返回0
            item.RefCount += count;
            item.m_LastUseTime = Time.realtimeSinceStartup;
            return item.RefCount;
        }
        /// <summary>
        /// 通过ResourceObj获取并增加一个引用计数
        /// </summary>
        /// <param name="crc"></param>
        /// <param name="count"></param>
        /// <returns>引用计数</returns>
        public int InCreaseResourceRef(ResourceObj resobj, int count = 1) {
            return resobj != null ? InCreaseResourceRef(resobj.m_Crc, count) : 0;
        }
        /// <summary>
        /// 通过crc获取并减少一个引用计数
        /// </summary>
        /// <param name="crc"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int DeCreaseResourceRef(uint crc, int count = 1) {
            ResourceItem item = null;
            if (!m_AssetCacheDic.TryGetValue(crc, out item) || item == null)
                return 0;//如果没有这个资源就返回0
            item.RefCount -= count;
            return item.RefCount;
        }
        /// <summary>
        /// 通过ResourceObj获取并减少一个引用计数
        /// </summary>
        /// <param name="resobj"></param>
        /// <param name="count"></param>
        /// <returns></returns>z
        public int DeCreaseResourceRef(ResourceObj resobj, int count = 1) {
            return resobj != null ? DeCreaseResourceRef(resobj.m_Crc, count) : 0;
        }
        #endregion
        protected long AsyncId = 0;
        public long GetAsyncId() {
            return AsyncId++;
        }
    }
}