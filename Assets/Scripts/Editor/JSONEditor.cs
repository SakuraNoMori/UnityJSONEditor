using UnityEditor;
using UnityEditor.IMGUI.Controls;

using UnityEngine;

namespace JSONEditor
{
	public class JSONEditor : EditorWindow
	{
		[MenuItem("SakuraNoMori/JSONEditor/Open")]
		public static void OpenWindow()
		{
			_window = GetWindow(typeof(JSONEditor));
			_window.titleContent = new GUIContent("JSONEditor");
			_window.minSize = new Vector2(250f, 500f);
		}

		[MenuItem("SakuraNoMori/JSONEditor/Close")]
		public static void CloseWindow()
		{
			_window.Close();
		}

		private void OnEnable()
		{
			if(_treeState == null)
			{
				_treeState = new();

			}

			_treeView = new(_treeState);
		}

		private void OnDisable()
		{

		}

		private void OnGUI()
		{
			_treeView.OnGUI(new Rect(0, 0, position.width, position.height));
		}

		private void DrawTopBar()
		{

		}

		private void DrawTreeView()
		{

		}

		private void DrawBottomBar()
		{

		}

		private static EditorWindow _window;
		[SerializeField] private TreeViewState _treeState = null;
		private JSONTreeView _treeView = null;
	}
}
