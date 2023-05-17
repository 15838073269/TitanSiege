#if UNITY_EDITOR
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;

namespace UnityEditor.TreeViewExamples
{
    class SimpleTreeViewWindow : EditorWindow
    {
        [SerializeField] TreeViewState m_TreeViewState;

        SimpleTreeView m_TreeView;
        SearchField m_SearchField;

        void OnEnable()
        {
            if (m_TreeViewState == null)
                m_TreeViewState = new TreeViewState();

            m_TreeView = new SimpleTreeView(m_TreeViewState);
            m_SearchField = new SearchField();
            m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;
        }

        void OnGUI()
        {
            DoToolbar();
            DoTreeView();
        }

        void DoToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Space(100);
            GUILayout.FlexibleSpace();
            m_TreeView.searchString = m_SearchField.OnToolbarGUI(m_TreeView.searchString);
            GUILayout.EndHorizontal();
        }

        void DoTreeView()
        {
            Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
            m_TreeView.OnGUI(rect);
        }

        [MenuItem("GameDesigner/Framework/Framework Window")]
        static void ShowWindow()
        {
            var window = GetWindow<SimpleTreeViewWindow>();
            window.titleContent = new GUIContent("Framework Window");
            window.Show();
        }
    }

    class SimpleTreeView : TreeView
    {
        public SimpleTreeView(TreeViewState treeViewState) : base(treeViewState)
        {
            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
            var allItems = new List<TreeViewItem>
            {
                new TreeViewItem {id = 1, depth = 0, displayName = "动物"},
                new TreeViewItem {id = 2, depth = 1, displayName = "哺乳动物"},
                new TreeViewItem {id = 3, depth = 2, displayName = "老虎"},
                new TreeViewItem {id = 4, depth = 2, displayName = "大大象"},
                new TreeViewItem {id = 5, depth = 2, displayName = "鹿"},
                new TreeViewItem {id = 6, depth = 2, displayName = "aaa"},
                new TreeViewItem {id = 7, depth = 1, displayName = "bbb"},
                new TreeViewItem {id = 8, depth = 2, displayName = "ccc"},
                new TreeViewItem {id = 9, depth = 2, displayName = "ddd"},
            };

            SetupParentsAndChildrenFromDepths(root, allItems);
            return root;
        }
    }
}
#endif