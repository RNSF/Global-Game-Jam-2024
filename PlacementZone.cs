using Godot;
using System;

public partial class PlacementZone : Area2D
{
	
	[Signal]
	public delegate void GlassServedEventHandler(Glass glass);

	private bool _isEnabled = false;
	public bool IsEnabled {
		get => _isEnabled;
		set {
			_isEnabled = value;
			Monitoring = value;
			Visible = value;
		}
	}

	public Node2D Sprites {
		get => GetNode<Node2D>("Sprites");
	}

	public override void _PhysicsProcess(double delta)
	{
		Sprites.Visible = Global.isGlassHeld;
		foreach(var node in GetOverlappingBodies()) {
			if (node is Glass glass && glass.CanServe()) {
				EmitSignal(SignalName.GlassServed, glass);
			}
		}
	}
}
