#if UNITY_EDITOR
using UnityEditor;

namespace Net.MMORPG
{
    [CustomEditor(typeof(LineOfSight))]
    [CanEditMultipleObjects]
    public class LineOfSightEdit : Editor
    {
        private LineOfSight self;

        private void OnEnable()
        {
            self = target as LineOfSight;
        }

        void OnSceneGUI()
        {
            Handles.color = new Color(0.8f, 0.7f, 0.2f, 0.3f);
            Handles.DrawSolidArc(self.transform.position + self.offset, self.transform.up, self.transform.forward, self.viewAngle, self.detectionRadius);
            Handles.DrawSolidArc(self.transform.position + self.offset, self.transform.up, self.transform.forward, -self.viewAngle, self.detectionRadius);
        }
    }
}
#endif