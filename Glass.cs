using Godot;
using System;
using System.Linq;

public partial class Glass : RigidBody2D
{
	public bool isPickedUp = false;

	private Area2D OuterArea {
		get => GetNode<Area2D>("OuterArea");
	}

	private Area2D InnerArea {
		get => GetNode<Area2D>("InnerArea");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		var isBehindBar = InnerArea.GetOverlappingBodies().Any((Node2D node) => {
			if (node is PhysicsBody2D body) {
				return (body.CollisionMask & CollisionLayers.BAR) != 0;
			}
			return false;
		});

		if (isBehindBar || isPickedUp) {
			CollisionMask &= ~CollisionLayers.BAR;
		} else {
			CollisionMask |= CollisionLayers.BAR;
		}
	}
}
