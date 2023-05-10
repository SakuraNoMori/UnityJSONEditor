using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace UnityEditor.TreeViewExamples
{

	class TransformTreeWindow : EditorWindow
	{
		[SerializeField] TreeViewState _treeViewState;

		TreeView _treeView;

		[MenuItem("TreeView Examples/Transform Hierarchy")]
		static void ShowWindow()
		{
			var window = GetWindow<TransformTreeWindow>();
			window.titleContent = new GUIContent("My Hierarchy");
			window.Show();
		}

		void OnEnable()
		{
			if(_treeViewState == null)
				_treeViewState = new TreeViewState();

			_treeView = new TransformTreeView(_treeViewState);
		}

		void OnSelectionChange()
		{
			if(_treeView != null)
				_treeView.SetSelection(Selection.instanceIDs);
			Repaint();
		}

		void OnHierarchyChange()
		{
			if(_treeView != null)
				_treeView.Reload();
			Repaint();
		}

		void OnGUI()
		{
			DoToolbar();
			DoTreeView();

		}

		void DoTreeView()
		{
			Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
			_treeView.OnGUI(rect);
		}

		void DoToolbar()
		{
			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
	}
}
