#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameDesigner
{
    [CustomEditor(typeof(StateManager))]
    [CanEditMultipleObjects]
    public class StateManagerEditor : Editor
    {
        private static StateManager stateManager = null;
        public static string createScriptName = "NewStateBehaviour";
		public static string stateActionScriptPath = "/Actions/StateActions";
		public static string stateBehaviourScriptPath = "/Actions/StateBehaviours";
		public static string transitionScriptPath = "/Actions/Transitions";
        private static bool findBehaviours;
        private static bool findBehaviours1;
        private static bool findBehaviours2;
        private static bool compiling;
        private static List<Type> findBehaviourTypes;
        private static List<Type> findBehaviourTypes1;
        private static List<Type> findBehaviourTypes2;

        void OnEnable()
        {
            stateManager = target as StateManager;
            var stateMachine = stateManager.stateMachine;
            if (stateMachine != null) 
            {
                if (stateMachine.animation == null)
                    stateMachine.animation = stateManager.GetComponentInChildren<Animation>();
                if (stateMachine.animation != null)
                {
                    var clips = AnimationUtility.GetAnimationClips(stateMachine.animation.gameObject);
                    if (stateMachine.clipNames.Count != clips.Length)
                    {
                        stateMachine.clipNames.Clear();
                        foreach (var clip in clips)
                        {
                            stateMachine.clipNames.Add(clip.name);
                        }
                    }
                }
                if (stateMachine.animator == null)
                    stateMachine.animator = stateManager.GetComponentInChildren<Animator>();
                if (stateMachine.animator != null)
                {
                    var clips = stateMachine.animator.runtimeAnimatorController.animationClips;
                    if (stateMachine.clipNames.Count != clips.Length)
                    {
                        stateMachine.clipNames.Clear();
                        foreach (var clip in clips)
                        {
                            stateMachine.clipNames.Add(clip.name);
                        }
                    }
                }
            }
            StateMachineWindow.stateMachine = stateManager.stateMachine;
            if (findBehaviourTypes == null) 
            {
                findBehaviourTypes = new List<Type>();
                AddBehaviourTypes(findBehaviourTypes, typeof(StateBehaviour));
            }
            if (findBehaviourTypes1 == null)
            {
                findBehaviourTypes1 = new List<Type>();
                AddBehaviourTypes(findBehaviourTypes1, typeof(ActionBehaviour));
            }
            if (findBehaviourTypes2 == null)
            {
                findBehaviourTypes2 = new List<Type>();
                AddBehaviourTypes(findBehaviourTypes2, typeof(TransitionBehaviour));
            }
        }

        private void AddBehaviourTypes(List<Type> types, Type type) 
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types1 = assembly.GetTypes().Where(t => t.IsSubclassOf(type)).ToArray();
                types.AddRange(types1);
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stateMachine"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[0]));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("initMode"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[114]));
            if (GUILayout.Button(BlueprintSetting.Instance.LANGUAGE[1], GUI.skin.GetStyle("LargeButtonMid"), GUILayout.ExpandWidth(true)))
                StateMachineWindow.Init(stateManager.stateMachine);
            if (stateManager.stateMachine == null)
                goto J;
            if (stateManager.stateMachine.selectState != null)
            {
                DrawState(stateManager.stateMachine.selectState, stateManager);
                EditorGUILayout.Space();
                for (int i = 0; i < stateManager.stateMachine.selectState.transitions.Count; ++i)
                    DrawTransition(stateManager.stateMachine.selectState.transitions[i]);
            }
            else if (StateMachineWindow.selectTransition != null)
            {
                DrawTransition(StateMachineWindow.selectTransition);
            }
            EditorGUILayout.Space();
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(stateManager.stateMachine);
            Repaint();
            J: serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 绘制状态监视面板属性
        /// </summary>
        public static void DrawState(State s, StateManager sm)
        {
            var serializedObject = new SerializedObject(sm.stateMachine);
            var serializedProperty = serializedObject.FindProperty("states").GetArrayElementAtIndex(s.ID);
            serializedObject.Update();
            GUILayout.Button(BlueprintGUILayout.Instance.LANGUAGE[2], GUI.skin.GetStyle("dragtabdropwindow"));
            EditorGUILayout.BeginVertical("ProgressBarBack");
            EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("name"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[3], "name"));
            EditorGUILayout.IntField(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[4], "stateID"), s.ID);
            EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("actionSystem"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[5], "actionSystem  专为玩家角色AI其怪物AI所设计的一套AI系统！"));
            if (s.actionSystem)
            {
                sm.stateMachine.animMode = (AnimationMode)EditorGUILayout.EnumPopup(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[6], "animMode"), sm.stateMachine.animMode);
                if (sm.stateMachine.animMode == AnimationMode.Animation)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("animation"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[7], "animation"));
                else
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("animator"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[8], "animator"));
                s.animPlayMode = (AnimPlayMode)EditorGUILayout.Popup(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[9], "animPlayMode"), (int)s.animPlayMode, new GUIContent[]{
                    new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[10],"Random"),
                    new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[11],"Sequence") }
                );
                EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("animSpeed"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[12], "animSpeed"), true);
                EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("animLoop"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[13], "animLoop"), true);
                s.isExitState = EditorGUILayout.Toggle(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[14], "isExitState"), s.isExitState);
                if (s.isExitState)
                    s.DstStateID = EditorGUILayout.Popup(BlueprintGUILayout.Instance.LANGUAGE[15], s.DstStateID, Array.ConvertAll(s.transitions.ToArray(), new Converter<Transition, string>(delegate (Transition t) { return t.currState.name + " -> " + t.nextState.name + "   ID:" + t.nextState.ID; })));
                BlueprintGUILayout.BeginStyleVertical(BlueprintGUILayout.Instance.LANGUAGE[16], "ProgressBarBack");
                EditorGUI.indentLevel = 1;
                Rect actRect = EditorGUILayout.GetControlRect();
                s.foldout = EditorGUI.Foldout(new Rect(actRect.position, new Vector2(actRect.size.x - 120f, 15)), s.foldout, BlueprintGUILayout.Instance.LANGUAGE[17], true);

                if (GUI.Button(new Rect(new Vector2(actRect.size.x - 40f, actRect.position.y), new Vector2(60, 16)), BlueprintGUILayout.Instance.LANGUAGE[18]))
                {
                    s.actions.Add(new StateAction() { ID = s.ID, stateMachine = s.stateMachine });
                }
                if (GUI.Button(new Rect(new Vector2(actRect.size.x - 100, actRect.position.y), new Vector2(60, 16)), BlueprintGUILayout.Instance.LANGUAGE[19]))
                {
                    if (s.actions.Count > 1)
                    {
                        s.actions.RemoveAt(s.actions.Count - 1);
                    }
                }

                if (s.foldout)
                {
                    var actionsProperty = serializedProperty.FindPropertyRelative("actions");
                    EditorGUI.indentLevel = 2;
                    for (int a = 0; a < s.actions.Count; ++a)
                    {
                        var actionProperty = actionsProperty.GetArrayElementAtIndex(a);
                        StateAction act = s.actions[a];
                        Rect foldoutRect = EditorGUILayout.GetControlRect();
                        act.foldout = EditorGUI.Foldout(foldoutRect, act.foldout, new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[20] + a, "actions[" + a + "]"), true);
                        if (foldoutRect.Contains(Event.current.mousePosition) & Event.current.button == 1)
                        {
                            var menu = new GenericMenu();
                            menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[21]), false, (obj) =>
                            {
                                s.actions.RemoveAt((int)obj);
                            }, a);
                            menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[22]), false, (obj) =>
                            {
                                StateSystem.Component = s.actions[(int)obj];
                            }, a);
                            menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[23]), StateSystem.CopyComponent != null, () =>
                            {
                                if (StateSystem.Component is StateAction stateAction)
                                    s.actions.Add(Net.CloneHelper.DeepCopy<StateAction>(stateAction));
                            });
                            menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[24]), StateSystem.CopyComponent != null, (obj) =>
                            {
                                if (StateSystem.Component is StateAction stateAction)
                                {
                                    var index = (int)obj;
                                    if (stateAction == s.actions[index])//如果要黏贴的动作是复制的动作则返回
                                        return;
                                    s.actions[index] = Net.CloneHelper.DeepCopy<StateAction>(stateAction);
                                }
                            }, a);
                            menu.ShowAsContext();
                        }
                        if (act.foldout)
                        {
                            EditorGUI.indentLevel = 3;
                            try {
                                act.clipIndex = EditorGUILayout.Popup(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[25], "clipIndex"), act.clipIndex, Array.ConvertAll(s.stateMachine.clipNames.ToArray(), input => new GUIContent(input)));
                                act.clipName = s.stateMachine.clipNames[act.clipIndex];
                            } catch { }
                            EditorGUILayout.PropertyField(actionProperty.FindPropertyRelative("animTime"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[32], "animTime"));
                            EditorGUILayout.PropertyField(actionProperty.FindPropertyRelative("animTimeMax"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[33], "animTimeMax"));
                            EditorGUILayout.PropertyField(actionProperty.FindPropertyRelative("rewind"), new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[34], "rewind"));
                            for (int i = 0; i < act.behaviours.Count; ++i)
                            {
                                EditorGUILayout.BeginHorizontal();
                                Rect rect = EditorGUILayout.GetControlRect();
                                act.behaviours[i].show = EditorGUI.Foldout(new Rect(rect.x, rect.y, 50, rect.height), act.behaviours[i].show, GUIContent.none);
                                act.behaviours[i].Active = EditorGUI.ToggleLeft(new Rect(rect.x + 5, rect.y, 70, rect.height), GUIContent.none, act.behaviours[i].Active);
                                EditorGUI.LabelField(new Rect(rect.x + 20, rect.y, rect.width - 15, rect.height), act.behaviours[i].name, GUI.skin.GetStyle("BoldLabel"));
                                if (GUI.Button(new Rect(rect.x + rect.width - 15, rect.y, rect.width, rect.height), GUIContent.none, GUI.skin.GetStyle("ToggleMixed")))
                                {
                                    act.behaviours[i].OnDestroyComponent();
                                    act.behaviours.RemoveAt(i);
                                    continue;
                                }
                                if (rect.Contains(Event.current.mousePosition) & Event.current.button == 1)
                                {
                                    var menu = new GenericMenu();
                                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[47]), false, (obj) =>
                                    {
                                        var index = (int)obj;
                                        act.behaviours[index].OnDestroyComponent();
                                        act.behaviours.RemoveAt(index);
                                        return;
                                    }, i);
                                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[48]), false, (obj) =>
                                    {
                                        var index = (int)obj;
                                        StateSystem.CopyComponent = act.behaviours[index];
                                    }, i);
                                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[49]), StateSystem.CopyComponent != null, ()=>
                                    {
                                        if (StateSystem.CopyComponent is ActionBehaviour behaviour)
                                        {
                                            ActionBehaviour ab = (ActionBehaviour)Net.CloneHelper.DeepCopy(behaviour);
                                            act.behaviours.Add(ab);
                                        }
                                    });
                                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[50]), StateSystem.CopyComponent != null, (obj) =>
                                    {
                                        if (StateSystem.CopyComponent is ActionBehaviour behaviour)
                                        {
                                            var index = (int)obj;
                                            if (behaviour.name == act.behaviours[index].name)
                                                act.behaviours[index] = (ActionBehaviour)Net.CloneHelper.DeepCopy(StateSystem.CopyComponent);
                                        }
                                    }, i);
                                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[78]), false, (obj) =>
                                    {
                                        var index = (int)obj;
                                        var scriptName = act.behaviours[index].name;
                                        if (!Net.Helper.ScriptHelper.Cache.TryGetValue(scriptName, out var sequence))
                                            sequence = new Net.Helper.SequencePoint();
                                        InternalEditorUtility.OpenFileAtLineExternal(sequence.FilePath, sequence.StartLine, 0);
                                    }, i);
                                    menu.ShowAsContext();
                                }
                                EditorGUILayout.EndHorizontal();
                                if (act.behaviours[i].show)
                                {
                                    EditorGUI.indentLevel = 4;
                                    if (!act.behaviours[i].OnInspectorGUI(s))
                                        foreach (var metadata in act.behaviours[i].metadatas)
                                            PropertyField(metadata, 60f, 5, 4);
                                    GUILayout.Space(4);
                                    GUILayout.Box("", BlueprintSetting.Instance.HorSpaceStyle, GUILayout.Height(1), GUILayout.ExpandWidth(true));
                                    GUILayout.Space(4);
                                    EditorGUI.indentLevel = 3;
                                }
                            }
                            Rect r = EditorGUILayout.GetControlRect();
                            Rect rr = new Rect(new Vector2(r.x + (r.size.x / 4f), r.y), new Vector2(r.size.x / 2f, 20));
                            if (GUI.Button(rr, BlueprintGUILayout.Instance.LANGUAGE[51]))
                                findBehaviours1 = true;
                            if (findBehaviours1)
                            {
                                EditorGUILayout.Space();
                                try
                                {
                                    foreach (var type in findBehaviourTypes1)
                                    {
                                        if (GUILayout.Button(type.Name))
                                        {
                                            var stb = (ActionBehaviour)Activator.CreateInstance(type);
                                            stb.InitMetadatas(act.stateMachine);
                                            stb.ID = s.ID;
                                            act.behaviours.Add(stb);
                                            findBehaviours1 = false;
                                            EditorUtility.SetDirty(act.stateMachine);
                                        }
                                        if (compiling & type.Name == createScriptName)
                                        {
                                            var stb = (ActionBehaviour)Activator.CreateInstance(type);
                                            stb.InitMetadatas(sm.stateMachine);
                                            stb.ID = s.ID;
                                            act.behaviours.Add(stb);
                                            findBehaviours1 = false;
                                            compiling = false;
                                            EditorUtility.SetDirty(act.stateMachine);
                                        }
                                    }
                                }
                                catch { }
                                EditorGUILayout.Space();
                                EditorGUI.indentLevel = 0;
                                EditorGUILayout.LabelField(BlueprintGUILayout.Instance.LANGUAGE[52]);
                                stateActionScriptPath = EditorGUILayout.TextField(stateActionScriptPath);
                                Rect addRect = EditorGUILayout.GetControlRect();
                                createScriptName = EditorGUI.TextField(new Rect(addRect.position, new Vector2(addRect.size.x - 125f, 18)), createScriptName);
                                if (GUI.Button(new Rect(new Vector2(addRect.size.x - 100f, addRect.position.y), new Vector2(120, 18)), BlueprintGUILayout.Instance.LANGUAGE[53]))
                                {
                                    var text = Resources.Load<TextAsset>("ActionBehaviourScript");
                                    var scriptCode = text.text.Split(new string[] { "\r\n" }, 0);
                                    scriptCode[7] = scriptCode[7].Replace("ActionBehaviourScript", createScriptName);
                                    ScriptTools.CreateScript(Application.dataPath + stateActionScriptPath, createScriptName, scriptCode);
                                    compiling = true;
                                }
                                if (GUILayout.Button(BlueprintGUILayout.Instance.LANGUAGE[54]))
                                    findBehaviours1 = false;
                            }
                            EditorGUILayout.Space();
                        }
                        EditorGUI.indentLevel = 2;
                    }
                }
                BlueprintGUILayout.EndStyleVertical();
            }
            EditorGUILayout.Space();
            DrawBehaviours(s);
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 绘制状态行为
        /// </summary>
        public static void DrawBehaviours(State s)
        {
            GUILayout.Space(10);
            GUILayout.Box("", BlueprintSetting.Instance.HorSpaceStyle, GUILayout.Height(1), GUILayout.ExpandWidth(true));
            GUILayout.Space(5);
            for (int i = 0; i < s.behaviours.Count; ++i)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.BeginHorizontal();
                Rect rect = EditorGUILayout.GetControlRect();
                s.behaviours[i].show = EditorGUI.Foldout(new Rect(rect.x, rect.y, 20, rect.height), s.behaviours[i].show, GUIContent.none);
                s.behaviours[i].Active = EditorGUI.ToggleLeft(new Rect(rect.x + 5, rect.y, 30, rect.height), GUIContent.none, s.behaviours[i].Active);
                EditorGUI.LabelField(new Rect(rect.x + 20, rect.y, rect.width - 15, rect.height), s.behaviours[i].name, GUI.skin.GetStyle("BoldLabel"));
                if (GUI.Button(new Rect(rect.x + rect.width - 15, rect.y, rect.width, rect.height), GUIContent.none, GUI.skin.GetStyle("ToggleMixed")))
                {
                    s.behaviours[i].OnDestroyComponent();
                    s.behaviours.RemoveAt(i);
                    continue;
                }
                if (rect.Contains(Event.current.mousePosition) & Event.current.button == 1)
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[55]), false, (obj) =>
                    {
                        var index = (int)obj;
                        s.behaviours[index].OnDestroyComponent();
                        s.behaviours.RemoveAt(index);
                        return;
                    }, i);
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[56]), false, (obj) =>
                    {
                        var index = (int)obj;
                        StateSystem.CopyComponent = s.behaviours[index];
                    }, i);
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[57]), StateSystem.CopyComponent != null, delegate ()
                    {
                        if (StateSystem.CopyComponent is StateBehaviour behaviour)
                        {
                            var ab = (StateBehaviour)Net.CloneHelper.DeepCopy(behaviour);
                            s.behaviours.Add(ab);
                        }
                    });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[58]), StateSystem.CopyComponent != null, (obj) =>
                    {
                        if (StateSystem.CopyComponent is StateBehaviour behaviour)
                        {
                            var index = (int)obj;
                            if (behaviour.name == s.behaviours[index].name)
                                s.behaviours[index] = (StateBehaviour)Net.CloneHelper.DeepCopy(behaviour);
                        }
                    }, i);
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[79]), false, (obj) =>
                    {
                        var index = (int)obj;
                        var scriptName = s.behaviours[index].name;
                        if (!Net.Helper.ScriptHelper.Cache.TryGetValue(scriptName, out var sequence))
                            sequence = new Net.Helper.SequencePoint();
                        InternalEditorUtility.OpenFileAtLineExternal(sequence.FilePath, sequence.StartLine, 0);
                    }, i);
                    menu.ShowAsContext();
                }
                EditorGUILayout.EndHorizontal();
                if (s.behaviours[i].show)
                {
                    EditorGUI.indentLevel = 2;
                    if (!s.behaviours[i].OnInspectorGUI(s))
                    {
                        foreach (var metadata in s.behaviours[i].metadatas)
                        {
                            PropertyField(metadata);
                        }
                    }
                    EditorGUI.indentLevel = 1;
                    GUILayout.Space(4);
                    GUILayout.Box("", BlueprintSetting.Instance.HorSpaceStyle, GUILayout.Height(1), GUILayout.ExpandWidth(true));
                }
            }

            Rect r = EditorGUILayout.GetControlRect();
            Rect rr = new Rect(new Vector2(r.x + (r.size.x / 4f), r.y), new Vector2(r.size.x / 2f, 20));
            if (GUI.Button(rr, BlueprintGUILayout.Instance.LANGUAGE[59]))
                findBehaviours = true;
            if (findBehaviours)
            {
                try
                {
                    EditorGUILayout.Space();
                    foreach (var type in findBehaviourTypes)
                    {
                        if (GUILayout.Button(type.Name))
                        {
                            var stb = (StateBehaviour)Activator.CreateInstance(type);
                            stb.InitMetadatas(s.stateMachine);
                            stb.ID = s.ID;
                            s.behaviours.Add(stb);
                            findBehaviours = false;
                            EditorUtility.SetDirty(s.stateMachine);
                        }
                        if (compiling & type.Name == createScriptName)
                        {
                            var stb = (StateBehaviour)Activator.CreateInstance(type);
                            stb.InitMetadatas(s.stateMachine);
                            stb.ID = s.ID;
                            s.behaviours.Add(stb);
                            findBehaviours = false;
                            compiling = false;
                            EditorUtility.SetDirty(s.stateMachine);
                        }
                    }
                }
                catch { }
                EditorGUILayout.Space();
                EditorGUI.indentLevel = 0;
                EditorGUILayout.LabelField(BlueprintGUILayout.Instance.LANGUAGE[60]);
                stateBehaviourScriptPath = EditorGUILayout.TextField(stateBehaviourScriptPath);
                Rect addRect = EditorGUILayout.GetControlRect();
                createScriptName = EditorGUI.TextField(new Rect(addRect.position, new Vector2(addRect.size.x - 125f, 18)), createScriptName);
                if (GUI.Button(new Rect(new Vector2(addRect.size.x - 105f, addRect.position.y), new Vector2(120, 18)), BlueprintGUILayout.Instance.LANGUAGE[61]))
                {
                    var text = Resources.Load<TextAsset>("StateBehaviourScript");
                    var scriptCode = text.text.Split(new string[] { "\r\n" }, 0);
                    scriptCode[7] = scriptCode[7].Replace("StateBehaviourScript", createScriptName);
                    ScriptTools.CreateScript(Application.dataPath + stateBehaviourScriptPath, createScriptName, scriptCode);
                    compiling = true;
                }
                if (GUILayout.Button(BlueprintGUILayout.Instance.LANGUAGE[62]))
                    findBehaviours = false;
            }
        }

        private static void PropertyField(Metadata metadata, float width = 40f, int arrayBeginSpace = 3, int arrayEndSpace = 2)
        {
            if (metadata.type == TypeCode.Byte)
                metadata.value = (byte)EditorGUILayout.IntField(metadata.name, (byte)metadata.value);
            else if (metadata.type == TypeCode.SByte)
                metadata.value = (sbyte)EditorGUILayout.IntField(metadata.name, (sbyte)metadata.value);
            else if (metadata.type == TypeCode.Boolean)
                metadata.value = EditorGUILayout.Toggle(metadata.name, (bool)metadata.value);
            else if (metadata.type == TypeCode.Int16)
                metadata.value = (short)EditorGUILayout.IntField(metadata.name, (short)metadata.value);
            else if (metadata.type == TypeCode.UInt16)
                metadata.value = (ushort)EditorGUILayout.IntField(metadata.name, (ushort)metadata.value);
            else if (metadata.type == TypeCode.Char)
                metadata.value = EditorGUILayout.TextField(metadata.name, metadata.value.ToString()).ToCharArray();
            else if (metadata.type == TypeCode.Int32)
                metadata.value = EditorGUILayout.IntField(metadata.name, (int)metadata.value);
            else if (metadata.type == TypeCode.UInt32)
                metadata.value = (uint)EditorGUILayout.IntField(metadata.name, (int)metadata.value);
            else if (metadata.type == TypeCode.Single)
                metadata.value = EditorGUILayout.FloatField(metadata.name, (float)metadata.value);
            else if (metadata.type == TypeCode.Int64)
                metadata.value = EditorGUILayout.LongField(metadata.name, (long)metadata.value);
            else if (metadata.type == TypeCode.UInt64)
                metadata.value = (ulong)EditorGUILayout.LongField(metadata.name, (long)metadata.value);
            else if (metadata.type == TypeCode.Double)
                metadata.value = EditorGUILayout.DoubleField(metadata.name, (double)metadata.value);
            else if (metadata.type == TypeCode.String)
                metadata.value = EditorGUILayout.TextField(metadata.name, metadata.value.ToString());
            else if (metadata.type == TypeCode.Enum)
                metadata.value = EditorGUILayout.EnumPopup(metadata.name, (Enum)metadata.value);
            else if (metadata.type == TypeCode.Vector2)
                metadata.value = EditorGUILayout.Vector2Field(metadata.name, (Vector2)metadata.value);
            else if (metadata.type == TypeCode.Vector3)
                metadata.value = EditorGUILayout.Vector3Field(metadata.name, (Vector3)metadata.value);
            else if (metadata.type == TypeCode.Vector4)
                metadata.value = EditorGUILayout.Vector4Field(metadata.name, (Vector4)metadata.value);
            else if (metadata.type == TypeCode.Quaternion)
            {
                Quaternion q = (Quaternion)metadata.value;
                var value = EditorGUILayout.Vector4Field(metadata.name, new Vector4(q.x, q.y, q.z, q.w));
                Quaternion q1 = new Quaternion(value.x, value.y, value.z, value.w);
                metadata.value = q1;
            }
            else if (metadata.type == TypeCode.Rect)
                metadata.value = EditorGUILayout.RectField(metadata.name, (Rect)metadata.value);
            else if (metadata.type == TypeCode.Color)
                metadata.value = EditorGUILayout.ColorField(metadata.name, (Color)metadata.value);
            else if (metadata.type == TypeCode.Color32)
                metadata.value = (Color32)EditorGUILayout.ColorField(metadata.name, (Color32)metadata.value);
            else if (metadata.type == TypeCode.AnimationCurve)
                metadata.value = EditorGUILayout.CurveField(metadata.name, (AnimationCurve)metadata.value);
            else if (metadata.type == TypeCode.Object)
                metadata.value = EditorGUILayout.ObjectField(metadata.name, (Object)metadata.value, metadata.Type, true);
            else if (metadata.type == TypeCode.GenericType | metadata.type == TypeCode.Array) 
            {
                var rect = EditorGUILayout.GetControlRect();
                rect.x += width;
                metadata.foldout = EditorGUI.BeginFoldoutHeaderGroup(rect, metadata.foldout, metadata.name);
                if (metadata.foldout) 
                {
                    EditorGUI.indentLevel = arrayBeginSpace;
                    EditorGUI.BeginChangeCheck();
                    var arraySize = EditorGUILayout.DelayedIntField("Size", metadata.arraySize);
                    bool flag8 = EditorGUI.EndChangeCheck();
                    IList list = (IList)metadata.value;
                    if (flag8 | list.Count != metadata.arraySize)
                    {
                        metadata.arraySize = arraySize;
                        IList list1 = Array.CreateInstance(metadata.itemType, arraySize);
                        for (int i = 0; i < list1.Count; i++)
                            if (i < list.Count)
                                list1[i] = list[i];
                        if (metadata.type == TypeCode.GenericType)
                        {
                            IList list2 = (IList)Activator.CreateInstance(metadata.Type);
                            for (int i = 0; i < list1.Count; i++)
                                list2.Add(list1[i]);
                            list = list2; 
                        }
                        else list = list1;
                    }
                    for (int i = 0; i < list.Count; i++)
                        list[i] = PropertyField("Element " + i, list[i], metadata.itemType);
                    metadata.value = list;
                    EditorGUI.indentLevel = arrayEndSpace;
                }
                EditorGUI.EndFoldoutHeaderGroup();
            }
        }

        private static object PropertyField(string name, object obj, Type type)
        {
            var typeCode = (TypeCode)Type.GetTypeCode(type);
            if (typeCode == TypeCode.Byte)
                obj = (byte)EditorGUILayout.IntField(name, (byte)obj);
            else if (typeCode == TypeCode.SByte)
                obj = (sbyte)EditorGUILayout.IntField(name, (sbyte)obj);
            else if (typeCode == TypeCode.Boolean)
                obj = EditorGUILayout.Toggle(name, (bool)obj);
            else if (typeCode == TypeCode.Int16)
                obj = (short)EditorGUILayout.IntField(name, (short)obj);
            else if (typeCode == TypeCode.UInt16)
                obj = (ushort)EditorGUILayout.IntField(name, (ushort)obj);
            else if (typeCode == TypeCode.Char)
                obj = EditorGUILayout.TextField(name, (string)obj).ToCharArray();
            else if (typeCode == TypeCode.Int32)
                obj = EditorGUILayout.IntField(name, (int)obj);
            else if (typeCode == TypeCode.UInt32)
                obj = (uint)EditorGUILayout.IntField(name, (int)obj);
            else if (typeCode == TypeCode.Single)
                obj = EditorGUILayout.FloatField(name, (float)obj);
            else if (typeCode == TypeCode.Int64)
                obj = EditorGUILayout.LongField(name, (long)obj);
            else if (typeCode == TypeCode.UInt64)
                obj = (ulong)EditorGUILayout.LongField(name, (long)obj);
            else if (typeCode == TypeCode.Double)
                obj = EditorGUILayout.DoubleField(name, (double)obj);
            else if (typeCode == TypeCode.String)
                obj = EditorGUILayout.TextField(name, (string)obj);
            else if (type == typeof(Vector2))
                obj = EditorGUILayout.Vector2Field(name, (Vector2)obj);
            else if (type == typeof(Vector3))
                obj = EditorGUILayout.Vector3Field(name, (Vector3)obj);
            else if (type == typeof(Vector4))
                obj = EditorGUILayout.Vector4Field(name, (Vector4)obj);
            else if (type == typeof(Quaternion))
            {
                var value = EditorGUILayout.Vector4Field(name, (Vector4)obj);
                Quaternion quaternion = new Quaternion(value.x, value.y, value.z, value.w);
                obj = quaternion;
            }
            else if (type == typeof(Rect))
                obj = EditorGUILayout.RectField(name, (Rect)obj);
            else if (type == typeof(Color))
                obj = EditorGUILayout.ColorField(name, (Color)obj);
            else if (type == typeof(Color32))
                obj = EditorGUILayout.ColorField(name, (Color32)obj);
            else if (type == typeof(AnimationCurve))
                obj = EditorGUILayout.CurveField(name, (AnimationCurve)obj);
            else if (type.IsSubclassOf(typeof(Object)) | type == typeof(Object))
                obj = EditorGUILayout.ObjectField(name, (Object)obj, type, true);
            return obj;
        }

        /// <summary>
        /// 绘制状态连接行为
        /// </summary>
        public static void DrawTransition(Transition tr)
        {
            EditorGUI.indentLevel = 0;
            GUIStyle style = GUI.skin.GetStyle("dragtabdropwindow");
            style.fontStyle = FontStyle.Bold;
            style.font = Resources.Load<Font>("Arial");
            style.normal.textColor = Color.red;
            GUILayout.Button(BlueprintGUILayout.Instance.LANGUAGE[63] + tr.currState.name + " -> " + tr.nextState.name, style);
            tr.name = tr.currState.name + " -> " + tr.nextState.name;
            EditorGUILayout.BeginVertical("ProgressBarBack");

            EditorGUILayout.Space();

            tr.model = (TransitionModel)EditorGUILayout.Popup(BlueprintGUILayout.Instance.LANGUAGE[64], (int)tr.model, Enum.GetNames(typeof(TransitionModel)), GUI.skin.GetStyle("PreDropDown"));
            switch (tr.model)
            {
                case TransitionModel.ExitTime:
                    tr.time = EditorGUILayout.FloatField(BlueprintGUILayout.Instance.LANGUAGE[65], tr.time, GUI.skin.GetStyle("PreDropDown"));
                    tr.exitTime = EditorGUILayout.FloatField(BlueprintGUILayout.Instance.LANGUAGE[66], tr.exitTime, GUI.skin.GetStyle("PreDropDown"));
                    EditorGUILayout.HelpBox(BlueprintGUILayout.Instance.LANGUAGE[67], MessageType.Info);
                    break;
            }

            GUILayout.Space(10);
            GUILayout.Box("", BlueprintSetting.Instance.HorSpaceStyle, GUILayout.Height(1), GUILayout.ExpandWidth(true));
            GUILayout.Space(10);

            tr.isEnterNextState = EditorGUILayout.Toggle(BlueprintGUILayout.Instance.LANGUAGE[68], tr.isEnterNextState);

            GUILayout.Space(10);
            GUILayout.Box("", BlueprintSetting.Instance.HorSpaceStyle, GUILayout.Height(1), GUILayout.ExpandWidth(true));

            for (int i = 0; i < tr.behaviours.Count; ++i)
            {
                if (tr.behaviours[i] == null)
                {
                    tr.behaviours.RemoveAt(i);
                    continue;
                }
                EditorGUI.indentLevel = 1;
                EditorGUILayout.BeginHorizontal();
                Rect rect = EditorGUILayout.GetControlRect();
                EditorGUI.LabelField(new Rect(rect.x + 20, rect.y, rect.width - 15, 20), tr.behaviours[i].GetType().Name, GUI.skin.GetStyle("BoldLabel"));
                tr.behaviours[i].show = EditorGUI.Foldout(new Rect(rect.x, rect.y, 20, 20), tr.behaviours[i].show, GUIContent.none, true);
                tr.behaviours[i].Active = EditorGUI.ToggleLeft(new Rect(rect.x + 5, rect.y, 30, 20), GUIContent.none, tr.behaviours[i].Active);
                if (GUI.Button(new Rect(rect.x + rect.width - 15, rect.y, rect.width, rect.height), GUIContent.none, GUI.skin.GetStyle("ToggleMixed")))
                {
                    tr.behaviours[i].OnDestroyComponent();
                    tr.behaviours.RemoveAt(i);
                    continue;
                }
                if (rect.Contains(Event.current.mousePosition) & Event.current.button == 1)
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[69]), false, (obj) =>
                    {
                        var index = (int)obj;
                        tr.behaviours[index].OnDestroyComponent();
                        tr.behaviours.RemoveAt(index);
                        return;
                    }, i);
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[70]), false, (obj) =>
                    {
                        var index = (int)obj;
                        StateSystem.CopyComponent = tr.behaviours[index];
                    }, i);
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[71]), StateSystem.CopyComponent != null, () =>
                    {
                        if (StateSystem.CopyComponent is TransitionBehaviour behaviour)
                        {
                            TransitionBehaviour ab = (TransitionBehaviour)Net.CloneHelper.DeepCopy(behaviour);
                            tr.behaviours.Add(ab);
                        }
                    });
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[72]), StateSystem.CopyComponent != null, (obj) =>
                    {
                        var index = (int)obj;
                        if (StateSystem.CopyComponent is TransitionBehaviour behaviour)
                            if (behaviour.name == tr.behaviours[index].name)
                                tr.behaviours[index] = (TransitionBehaviour)Net.CloneHelper.DeepCopy(behaviour);
                    }, i);
                    menu.AddItem(new GUIContent(BlueprintGUILayout.Instance.LANGUAGE[80]), false, (obj) =>
                    {
                        var index = (int)obj;
                        var scriptName = tr.behaviours[index].name;
                        if (!Net.Helper.ScriptHelper.Cache.TryGetValue(scriptName, out var sequence))
                            sequence = new Net.Helper.SequencePoint();
                        InternalEditorUtility.OpenFileAtLineExternal(sequence.FilePath, sequence.StartLine, 0);
                    }, i);
                    menu.ShowAsContext();
                }
                EditorGUILayout.EndHorizontal();
                if (tr.behaviours[i].show)
                {
                    EditorGUI.indentLevel = 2;
                    if (!tr.behaviours[i].OnInspectorGUI(tr.currState))
                    {
                        foreach (var metadata in tr.behaviours[i].metadatas)
                        {
                            PropertyField(metadata);
                        }
                    }
                    EditorGUI.indentLevel = 1;
                    GUILayout.Space(10);
                    GUILayout.Box("", BlueprintSetting.Instance.HorSpaceStyle, GUILayout.Height(1), GUILayout.ExpandWidth(true));
                }
            }

            GUILayout.Space(5);

            Rect r = EditorGUILayout.GetControlRect();
            Rect rr = new Rect(new Vector2(r.x + (r.size.x / 4f), r.y), new Vector2(r.size.x / 2f, 20));
            if (GUI.Button(rr, BlueprintGUILayout.Instance.LANGUAGE[73]))
                findBehaviours2 = true;
            if (findBehaviours2)
            {
                EditorGUILayout.Space();
                foreach (var type in findBehaviourTypes2)
                {
                    if (GUILayout.Button(type.Name))
                    {
                        var stb = (TransitionBehaviour)Activator.CreateInstance(type);
                        stb.InitMetadatas(tr.stateMachine);
                        tr.behaviours.Add(stb);
                        findBehaviours2 = false;
                    }
                    if (compiling & type.Name == createScriptName)
                    {
                        var stb = (TransitionBehaviour)Activator.CreateInstance(type);
                        stb.InitMetadatas(tr.stateMachine);
                        tr.behaviours.Add(stb);
                        findBehaviours2 = false;
                        compiling = false;
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(BlueprintGUILayout.Instance.LANGUAGE[74]);
                transitionScriptPath = EditorGUILayout.TextField(transitionScriptPath);
                Rect addRect = EditorGUILayout.GetControlRect();
                createScriptName = EditorGUI.TextField(new Rect(addRect.position, new Vector2(addRect.size.x - 125f, 18)), createScriptName);
                if (GUI.Button(new Rect(new Vector2(addRect.size.x - 105f, addRect.position.y), new Vector2(120, 18)), BlueprintGUILayout.Instance.LANGUAGE[75]))
                {
                    var text = Resources.Load<TextAsset>("TransitionBehaviorScript");
                    var scriptCode = text.text.Split(new string[] { "\r\n" }, 0);
                    scriptCode[7] = scriptCode[7].Replace("TransitionBehaviorScript", createScriptName);
                    ScriptTools.CreateScript(Application.dataPath + transitionScriptPath, createScriptName, scriptCode);
                    compiling = true;
                }
                if (GUILayout.Button(BlueprintGUILayout.Instance.LANGUAGE[76]))
                    findBehaviours2 = false;
            }
            GUILayout.Space(10);
            EditorGUILayout.HelpBox(BlueprintGUILayout.Instance.LANGUAGE[77], MessageType.Info);
            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();
        }

        [UnityEditor.Callbacks.DidReloadScripts(0)]
        internal static void OnScriptReload()
        {
            if (stateManager == null)
                return;
            stateManager.OnScriptReload();
        }
    }

}
#endif