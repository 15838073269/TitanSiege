
namespace GF.Module {
    /// <summary>
    /// 模块类的基类，只有一个释放得方法，方便扩展
    /// </summary>
    public abstract  class ModuleBase {
        
        public virtual void Release() {
            
        }
    }
}
