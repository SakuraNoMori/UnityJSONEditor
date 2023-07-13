using System.Collections.Generic;

using UnityEngine;

namespace UnityEditor.TreeViewExamples
{

	[CreateAssetMenu(fileName = "TreeDataAsset", menuName = "Tree Asset", order = 1)]
	public class MyTreeAsset : ScriptableObject
	{
		[SerializeField] List<MyTreeElement> _treeElements = new ();

		internal List<MyTreeElement> TreeElements
		{
			get { return _treeElements; }
			set { _treeElements = value; }
		}

		void Awake()
		{
			if(_treeElements.Count == 0)
				_treeElements = MyTreeElementGenerator.GenerateRandomTree(160);
		}
	}
}
