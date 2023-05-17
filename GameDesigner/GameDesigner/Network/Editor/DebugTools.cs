#if UNITY_EDITOR
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DebugTools : EditorWindow
{
    [MenuItem("GameDesigner/Network/DebugTools")]
    static void ShowWindow()
    {
        var window = GetWindow<DebugTools>("Debug����");
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox(@"�˹��߿��԰����ڿ���̨��ӡʱ, ֱ����ʾRpc���õ��ķ���, ����˫������Rpc����λ��, ʹ�ô˹��߻��޸�
unity�༭����UnityEditor.CoreModule.dll,�޸�ILָ���ÿ���̨�жϴ�ӡ����Ҳ��ʾ����, !!!ע��:�뱸�����UnityEditor.CoreModule.dll,
����ע��ILָ������,unity���򼯴�����unity�򲻿���������!!!", MessageType.Info);
        if (GUILayout.Button("ע��༭��UnityEditor.CoreModule.dll", GUILayout.Height(30)))
        {
            var modCtx = ModuleDef.CreateModuleContext();
            var path = typeof(EditorWindow).Assembly.Location;
            var stream = new MemoryStream(File.ReadAllBytes(path));
            var module = ModuleDefMD.Load(stream, modCtx);
            foreach (var type in module.Types)
            {
                if (type.FullName == "UnityEditor.ConsoleWindow")
                {
                    foreach (var method in type.Methods)
                    {
                        if (method.Name == "StacktraceWithHyperlinks")
                        {
                            for (int i = 0; i < method.Body.Instructions.Count; i++)
                            {
                                var instruction = method.Body.Instructions[i];
                                if (instruction.OpCode.Code == Code.Ldarg_1)
                                {
                                    method.Body.Instructions[i] = Instruction.CreateLdcI4(0);
                                }
                            }
                        }
                    }
                }
            }
            module.Write(path);
            Debug.Log("ע�����!");
        }
    }
}
#endif