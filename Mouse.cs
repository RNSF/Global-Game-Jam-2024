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
			bool wasGrabbing = GrabLevel != Grab.NONE;
			if (value == _grabLevel) return;
			_grabLevel = value;
			bool isGrabbing = GrabLevel != Grab.NONE;

			if (wasGrabbing && !isGrabbing) {
				Body = null;
			}

			if (isGrabbing && !wasGrabbing) {
				var state = GetWorld2D().DirectSpaceState;
				var query = new PhysicsPointQueryParameters2D();
				query.CollideWithAreas = true;
				query.CollideWithBodies = true;
				query.CollisionMask = CollisionLayers.GRABBABLE;
				query.Position = PinJoint.GlobalPosition;
				var result = state.IntersectPoint(query);
				if (result.Count == 0) return;

				var collider = result[0]["collider"];
				if (collider.Obj is PhysicsBody2D) {
					Body = collider.As<PhysicsBody2D>();
				} else if (collider.Obj is Area2D) {
					Body = collider.As<Area2D>().GetParent<PhysicsBody2D>();
				} else {
					Body = null;
				}
			}

			SetBodyMode(Body, GrabLevel);
		}
	}

	private PinJoint2D PinJoint {
		get => GetNode<PinJoint2D>("PinJoint2D");
	}

	[Export]
    public PhysicsBody2D Body { 
		get => PinJoint.NodeA != "" ? GetTree().Root.GetNode<PhysicsBody2D>(PinJoint.NodeA) : null; 
		set {
			SetBodyMode(Body, Grab.NONE);

			if (value != null) 	PinJoint.NodeA = value.GetPath();
			else 		 		PinJoint.NodeA = null;

			SetBodyMode(Body, GrabLevel);

			Global.isGlassHeld = Body is Glass;
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

	public void SetBodyMode(PhysicsBody2D body, Grab gravLevel) {
		if (body is Bottle bottle) {
			bottle.isPickedUp = gravLevel != Grab.NONE;
		}

		if (body is Glass glass) {
			glass.isPickedUp = gravLevel != Grab.NONE;
		}

		if (body is RigidBody2D rigidBody) {
			rigidBody.LockRotation = gravLevel == Grab.STRONG;
		}
	}
}
