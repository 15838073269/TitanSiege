using UnityEngine;

namespace Framework
{
    public class Logger : MonoBehaviour
    {
        public bool EnableLog = true;

        public void Log(object message)
        {
            if (!EnableLog)
                return;
            Debug.Log(message);
        }

        public void LogWarning(object message)
        {
            if (!EnableLog)
                return;
            Debug.LogWarning(message);
        }

        public void LogError(object message)
        {
            if (!EnableLog)
                return;
            Debug.LogError(message);
        }
    }
}