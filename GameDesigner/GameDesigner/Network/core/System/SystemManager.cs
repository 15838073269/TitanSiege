using System;

namespace Net.System
{
    public interface ISystemManager
    {
        void Update();
    }

    public class SystemManager
    {
        public static SystemManager Instance = new SystemManager();
        private FastList<ISystemManager> managers = new FastList<ISystemManager>();

        public void AddManager(ISystemManager manager)
        {
            managers.Add(manager);
        }

        public void RemoveManager(ISystemManager manager) 
        {
            managers.Remove(manager);
        }

        public void Run() 
        {
            for (int i = 0; i < managers.Count; i++)
            {
                managers[i].Update();
            }
        }
    }
}
