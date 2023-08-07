#if UNITY_EDITOR
using Net.UnityComponent;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NetworkTransform))]
[CanEditMultipleObjects]
public class NetworkTransformEdit : Editor
{
    private NetworkTransform nt;

    private void OnEnable()
    {
        nt = target as NetworkTransform;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUI.enabled = false;
        EditorGUILayout.LabelField("mode", nt.currMode.ToString());
        GUI.enabled = true;
    }
}
#endif