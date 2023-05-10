using UnityEditor.IMGUI.Controls;

public abstract class JSONTreeElement : TreeViewItem
{
	public JSONTreeElement(int depth, int id, string name) : base(id, depth, name) { }

	public string _label;
	public string Label { get => _label; set => _label = value; }
}
