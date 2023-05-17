using System.Threading;

namespace Net.Share
{
    public class ThreadGroup
    {
        public int Id;
        public Thread Thread;

        public override string ToString()
        {
            return $"线程组ID:{Id} 线程ID:{Thread.ManagedThreadId}";
        }
    }
}
