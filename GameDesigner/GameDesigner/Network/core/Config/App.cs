using Net.Share;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Net.Config
{
    public static class App
    {
        [STAThread]
        public static void Setup() => Init();

        public static void Init()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assemblie in assemblies)
            {
                foreach (var type in assemblie.GetTypes())
                {
                    var members = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    foreach (var member in members)
                    {
                        var runtimeInitialize = member.GetCustomAttribute<RuntimeInitializeOnLoadMethod>();
                        if (runtimeInitialize == null)
                            continue;
                        member.Invoke(null, null);
                    }
                }
            }
        }
    }
}
