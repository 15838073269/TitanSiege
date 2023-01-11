
namespace GF.Module {
    public interface IModuleActivator {
        GeneralModule CreateInstance(string ModuleName);
    }
}
