#if TOOLS
using Godot;
using System;

[Tool]
public partial class HistogramAddon : EditorPlugin
{
	public override void _EnterTree()
	{
		// Initialization of the plugin goes here.
		AddCustomType("Histogram", "Node3D", GD.Load<Script>("res://addons/histogram/Histogram.cs"),null);
	}

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
		RemoveCustomType("Histogram");
	}
}
#endif
