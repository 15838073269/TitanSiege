using Net.System;

namespace Net.Event
{
    public class PlayerLoopRunner
    {
        internal void Run()
        {
            ThreadManager.Run(ThreadManager.Interval);
        }
    }
}