using System;
using System.Threading;
using Net.Server;
using Net.System;

namespace Net.Share
{
    public class ThreadGroup
    {
        public int Id;
        public Thread Thread;

        public virtual void Add(NetPlayer client)
        {
        }

        public virtual void Remove(NetPlayer client)
        {
        }

        public override string ToString()
        {
            return $"线程组ID:{Id} 线程ID:{Thread.ManagedThreadId}";
        }
    }

    public class ThreadGroup<Player> : ThreadGroup where Player : NetPlayer
    {
        public FastListSafe<Player> Clients = new FastListSafe<Player>();

        public override void Add(NetPlayer client)
        {
            Clients.Add(client as Player);
        }

        public override void Remove(NetPlayer client)
        {
            Clients.Remove(client as Player);
        }

        public override string ToString()
        {
            return $"线程组ID:{Id} 线程ID:{Thread.ManagedThreadId} 玩家数量:{Clients.Count}";
        }
    }
}
