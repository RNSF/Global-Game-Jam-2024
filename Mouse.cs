using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class Mouse : Node2D
{

	public enum Grab {
		NONE,
		WEAK,
		STRONG
	}

	private bool _isLeftPressed = false;
	private bool IsLeftPressed {
		get => _isLeftPressed;
		set {
			_isLeftPressed = value;
			GrabLevel = CalculateGrabLevel();
		}
	}

	private bool _isRightPressed = false;
	private bool IsRightPressed {
		get => _isRightPressed;
		set {
			_isRightPressed = value;
			GrabLevel = CalculateGrabLevel();
		}
	}

	private Grab _grabLevel;
	public Grab GrabLevel {
		get => _grabLevel;
		set {
			if (value == _grabLevel) return;
			_grabLevel = value;
			
			switch (_grabLevel) {
				case Grab.NONE: {
					Body = null;
				} break;

				case Grab.WEAK: {
					var state = GetWorld2D().DirectSpaceState;
					var query = new PhysicsPointQueryParameters2D();
					query.CollisionMask = CollisionLayers.BOTTLES;
					query.Position = PinJoint.GlobalPosition;
					var result = state.IntersectPoint(query);
					if (result.Count == 0) return;
					Body = result[0]["collider"].As<PhysicsBody2D>();
				} break;

				case Grab.STRONG: {

				} break;
			}
		}
	}

	private PinJoint2D PinJoint {
		get => GetNode<PinJoint2D>("PinJoint2D");
	}

	[Export]
    public PhysicsBody2D Body { 
		get => PinJoint.NodeA != "" ? GetTree().Root.GetNode<PhysicsBody2D>(PinJoint.NodeA) : null; 
		set {
			if (value != null) 	PinJoint.NodeA = value.GetPath();
			else 		 		PinJoint.NodeA = null;
		} 	
	}

	public override void _UnhandledInput(InputEvent @event) {
		if (@event is InputEventMouseMotion mouseMotion) {
			Vector2 oldPosition = GlobalPosition;
			GlobalPosition = GetGlobalMousePosition();
		}

		else if (@event is InputEventMouseButton mouseButton) {
			if (mouseButton.ButtonIndex == MouseButton.Left)  IsLeftPressed = mouseButton.Pressed;
			if (mouseButton.ButtonIndex == MouseButton.Right) IsRightPressed = mouseButton.Pressed;
		}
	}

	


	public Grab CalculateGrabLevel() {
		return (Grab) ((IsLeftPressed ? 1 : 0) + (IsRightPressed ? 1 : 0));
	}
}
