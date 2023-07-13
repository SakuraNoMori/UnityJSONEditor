using System;
using System.Collections.Generic;

using UnityEditor.Callbacks;
using UnityEditor.IMGUI.Controls;

using UnityEngine;

namespace UnityEditor.TreeViewExamples
{

	class MultiColumnWindow : EditorWindow
	{
		[NonSerialized] bool m_Initialized;
		[SerializeField] TreeViewState m_TreeViewState; // Serialized in the window layout file so it survives assembly reloading
		[SerializeField] MultiColumnHeaderState m_MultiColumnHeaderState;
		SearchField m_SearchField;
		MultiColumnTreeView m_TreeView;
		MyTreeAsset m_MyTreeAsset;

		[MenuItem("TreeView Examples/Multi Columns")]
		public static MultiColumnWindow GetWindow()
		{
			var window = GetWindow<MultiColumnWindow>();
			window.titleContent = new GUIContent("Multi Columns");
			window.Focus();
			window.Repaint();
			return window;
		}

		[OnOpenAsset]
		public static bool OnOpenAsset(int instanceID, int line)
		{
			var myTreeAsset = EditorUtility.InstanceIDToObject(instanceID) as MyTreeAsset;
			if(myTreeAsset != null)
			{
				var window = GetWindow();
				window.SetTreeAsset(myTreeAsset);
				return true;
			}

			return false; // we did not handle the open
		}

		void SetTreeAsset(MyTreeAsset myTreeAsset)
		{
			m_MyTreeAsset = myTreeAsset;
			m_Initialized = false;
		}

		Rect MultiColumnTreeViewRect
		{
			get { return new Rect(20, 30, position.width - 40, position.height - 60); }
		}

		Rect ToolbarRect
		{
			get { return new Rect(20f, 10f, position.width - 40f, 20f); }
		}

		Rect BottomToolbarRect
		{
			get { return new Rect(20f, position.height - 18f, position.width - 40f, 16f); }
		}

		public MultiColumnTreeView TreeView
		{
			get { return m_TreeView; }
		}

		void InitIfNeeded()
		{
			if(!m_Initialized)
			{
				// Check if it already exists (deserialized from window layout file or scriptable object)
				if(m_TreeViewState == null)
					m_TreeViewState = new TreeViewState();

				bool firstInit = m_MultiColumnHeaderState == null;
				var headerState = MultiColumnTreeView.CreateDefaultMultiColumnHeaderState(MultiColumnTreeViewRect.width);
				if(MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
					MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
				m_MultiColumnHeaderState = headerState;

				var multiColumnHeader = new MyMultiColumnHeader(headerState);
				if(firstInit)
					multiColumnHeader.ResizeToFit();

				var treeModel = new TreeModel<MyTreeElement>(GetData());

				m_TreeView = new MultiColumnTreeView(m_TreeViewState, multiColumnHeader, treeModel);

				m_SearchField = new SearchField();
				m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;

				m_Initialized = true;
			}
		}

		IList<MyTreeElement> GetData()
		{
			if(m_MyTreeAsset != null && m_MyTreeAsset.TreeElements != null && m_MyTreeAsset.TreeElements.Count > 0)
				return m_MyTreeAsset.TreeElements;

			// generate some test data
			return MyTreeElementGenerator.GenerateRandomTree(130);
		}

		void OnSelectionChange()
		{
			if(!m_Initialized)
				return;

			var myTreeAsset = Selection.activeObject as MyTreeAsset;
			if(myTreeAsset != null && myTreeAsset != m_MyTreeAsset)
			{
				m_MyTreeAsset = myTreeAsset;
				m_TreeView.treeModel.SetData(GetData());
				m_TreeView.Reload();
			}
		}

		void OnGUI()
		{
			InitIfNeeded();

			SearchBar(ToolbarRect);
			DoTreeView(MultiColumnTreeViewRect);
			BottomToolBar(BottomToolbarRect);
		}

		void SearchBar(Rect rect)
		{
			TreeView.searchString = m_SearchField.OnGUI(rect, TreeView.searchString);
		}

		void DoTreeView(Rect rect)
		{
			m_TreeView.OnGUI(rect);
		}

		void BottomToolBar(Rect rect)
		{
			GUILayout.BeginArea(rect);

			using(new EditorGUILayout.HorizontalScope())
			{

				var style = "miniButton";
				if(GUILayout.Button("Expand All", style))
				{
					TreeView.ExpandAll();
				}

				if(GUILayout.Button("Collapse All", style))
				{
					TreeView.CollapseAll();
				}

				GUILayout.FlexibleSpace();

				GUILayout.Label(m_MyTreeAsset != null ? AssetDatabase.GetAssetPath(m_MyTreeAsset) : string.Empty);

				GUILayout.FlexibleSpace();

				if(GUILayout.Button("Set sorting", style))
				{
					var myColumnHeader = (MyMultiColumnHeader)TreeView.multiColumnHeader;
					myColumnHeader.SetSortingColumns(new int[] { 4, 3, 2 }, new[] { true, false, true });
					myColumnHeader.Mode = MyMultiColumnHeader.HeaderMode.LargeHeader;
				}

				GUILayout.Label("Header: ", "minilabel");
				if(GUILayout.Button("Large", style))
				{
					var myColumnHeader = (MyMultiColumnHeader)TreeView.multiColumnHeader;
					myColumnHeader.Mode = MyMultiColumnHeader.HeaderMode.LargeHeader;
				}

				if(GUILayout.Button("Default", style))
				{
					var myColumnHeader = (MyMultiColumnHeader)TreeView.multiColumnHeader;
					myColumnHeader.Mode = MyMultiColumnHeader.HeaderMode.DefaultHeader;
				}

				if(GUILayout.Button("No sort", style))
				{
					var myColumnHeader = (MyMultiColumnHeader)TreeView.multiColumnHeader;
					myColumnHeader.Mode = MyMultiColumnHeader.HeaderMode.MinimumHeaderWithoutSorting;
				}

				GUILayout.Space(10);

				if(GUILayout.Button("values <-> controls", style))
				{
					TreeView.showControls = !TreeView.showControls;
				}
			}

			GUILayout.EndArea();
		}
	}
	internal class MyMultiColumnHeader : MultiColumnHeader
	{
		HeaderMode m_Mode;

		public enum HeaderMode
		{
			LargeHeader,
			DefaultHeader,
			MinimumHeaderWithoutSorting
		}

		public MyMultiColumnHeader(MultiColumnHeaderState state)
			: base(state)
		{
			Mode = HeaderMode.DefaultHeader;
		}

		public HeaderMode Mode
		{
			get
			{
				return m_Mode;
			}
			set
			{
				m_Mode = value;
				switch(m_Mode)
				{
					case HeaderMode.LargeHeader:
						canSort = true;
						height = 37f;
						break;
					case HeaderMode.DefaultHeader:
						canSort = true;
						height = DefaultGUI.defaultHeight;
						break;
					case HeaderMode.MinimumHeaderWithoutSorting:
						canSort = false;
						height = DefaultGUI.minimumHeight;
						break;
				}
			}
		}

		protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
		{
			// Default column header gui
			base.ColumnHeaderGUI(column, headerRect, columnIndex);

			// Add additional info for large header
			if(Mode == HeaderMode.LargeHeader)
			{
				// Show example overlay stuff on some of the columns
				if(columnIndex > 2)
				{
					headerRect.xMax -= 3f;
					var oldAlignment = EditorStyles.largeLabel.alignment;
					EditorStyles.largeLabel.alignment = TextAnchor.UpperRight;
					GUI.Label(headerRect, 36 + columnIndex + "%", EditorStyles.largeLabel);
					EditorStyles.largeLabel.alignment = oldAlignment;
				}
			}
		}
	}

}
