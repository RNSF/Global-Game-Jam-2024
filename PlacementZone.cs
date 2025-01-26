using Godot;
using System;

public partial class PlacementZone : Area2D
{
	
	[Signal]
	public delegate void GlassServedEventHandler(Glass glass);

	public bool IsEnabled {
		set {
			Visible = value;
			Monitoring = value;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		foreach(var node in GetOverlappingBodies()) {
			if (node is Glass glass && glass.CanServe()) {
				EmitSignal(SignalName.GlassServed, glass);
			}
		}
	}
}
