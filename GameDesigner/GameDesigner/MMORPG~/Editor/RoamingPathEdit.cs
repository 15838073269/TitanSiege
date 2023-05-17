#if UNITY_EDITOR
namespace Net.MMORPG
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(RoamingPath))]
    [CanEditMultipleObjects]
    public class RoamingPathEdit : Editor
    {
        private RoamingPath self;
        private SerializedProperty WaypointsFoldout;

        private void OnEnable()
        {
            self = target as RoamingPath;
            WaypointsFoldout = serializedObject.FindProperty("waypointsFoldout");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (GUILayout.Button("添加路点"))
            {
                Vector3 newPoint = new Vector3(0, 0, 0);
                if (self.waypointsList.Count == 0)
                {
                    newPoint = new Vector3(0, 0, 5);
                }
                else if (self.waypointsList.Count > 0)
                {
                    newPoint = new Vector3(self.localWaypoints[self.localWaypoints.Count - 1].x, self.localWaypoints[self.localWaypoints.Count - 1].y, self.localWaypoints[self.localWaypoints.Count - 1].z + 5);
                }
                self.waypointsList.Add(newPoint);
                self.localWaypoints.Add(newPoint);
                EditorUtility.SetDirty(self);
            }
            if (GUILayout.Button("清除所有路点", GUI.skin.button) && EditorUtility.DisplayDialog("清除所有路点?", "确定清除所有路点?", "确定", "取消"))
            {
                self.waypointsList.Clear();
                self.localWaypoints.Clear();
                EditorUtility.SetDirty(self);
            }
            WaypointsFoldout.boolValue = Foldout(WaypointsFoldout.boolValue, "所有路点", true, EditorStyles.foldout);
            if (WaypointsFoldout.boolValue)
            {
                EditorGUILayout.BeginVertical("Box");
                if (self.waypointsList.Count > 0)
                {
                    for (int j = 0; j < self.waypointsList.Count; ++j)
                    {
                        GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
                        EditorGUILayout.LabelField("路点 " + (j + 1), EditorStyles.toolbarButton);
                        GUI.backgroundColor = Color.white;
                        self.waypointsList[j] = EditorGUILayout.Vector3Field("世界坐标", self.waypointsList[j]);
                        self.localWaypoints[j] = EditorGUILayout.Vector3Field("局部坐标", self.localWaypoints[j]);
                        if (GUILayout.Button("移除", EditorStyles.miniButton, GUILayout.Height(18)))
                        {
                            self.waypointsList.RemoveAt(j);
                            self.localWaypoints.RemoveAt(j);
                            --j;
                            EditorUtility.SetDirty(self);
                        }
                        GUILayout.Space(10);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            serializedObject.ApplyModifiedProperties();
        }

        public static bool Foldout(bool foldout, GUIContent content, bool toggleOnLabelClick, GUIStyle style)
        {
            Rect position = GUILayoutUtility.GetRect(40f, 40f, 16f, 16f, style);
            return EditorGUI.Foldout(position, foldout, content, toggleOnLabelClick, style);
        }
        public static bool Foldout(bool foldout, string content, bool toggleOnLabelClick, GUIStyle style)
        {
            return Foldout(foldout, new GUIContent(content), toggleOnLabelClick, style);
        }

        private void OnSceneGUI()
        {
            if (self.waypointsList.Count == 0)
                return;
            Handles.color = Color.blue;
            Handles.DrawLine(self.transform.position, self.waypointsList[0]);
            Handles.color = Color.white;

            Handles.color = Color.green;
            for (int i = 0; i < self.waypointsList.Count - 1; i++)
            {
                Handles.DrawLine(self.waypointsList[i], self.waypointsList[i + 1]);
            }
            if (self.waypointsList.Count > 1)
                Handles.DrawLine(self.waypointsList[0], self.waypointsList[self.waypointsList.Count - 1]);
            Handles.color = Color.white;

            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            for (int i = 0; i < self.waypointsList.Count; i++)
            {
                Handles.SphereHandleCap(0, self.waypointsList[i], Quaternion.identity, 0.5f, EventType.Repaint);
                DrawString("路点 " + (i + 1), self.waypointsList[i] + Vector3.up, Color.white);
            }

            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
            for (int i = 0; i < self.waypointsList.Count; i++)
            {
                self.waypointsList[i] = Handles.PositionHandle(self.transform.position + self.localWaypoints[i], Quaternion.identity);
                self.localWaypoints[i] = self.waypointsList[i] - self.transform.position;
            }
        }

        public static void DrawString(string text, Vector3 worldPos, Color? colour = null)
        {
            GUIStyle style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;

            Handles.BeginGUI();

            var restoreColor = GUI.color;

            if (colour.HasValue) GUI.color = colour.Value;
            var view = SceneView.currentDrawingSceneView;
            Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);

            if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0)
            {
                GUI.color = restoreColor;
                Handles.EndGUI();
                return;
            }

            Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
            GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text, style);
            GUI.color = restoreColor;
            Handles.EndGUI();
        }
    }
}
#endif