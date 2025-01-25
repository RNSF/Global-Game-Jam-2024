using Godot;
using System;
using System.Diagnostics;

public partial class Bottle : RigidBody2D
{

	[Export]
	public SodaFluid sodaFluid;
	public bool isPickedUp;
	
	public bool isHovered = false;
	private float pourTimer = 1.0f;
	private float pourRate = 5.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Debug.Assert(sodaFluid != null, "Missing soda fluid");
		this.MouseEntered += () => isHovered = true;
		this.MouseExited += () => isHovered = false;
	}

	private Node2D FluidSpawnPoint {
		get => GetNode<Node2D>("FluidSpawnPoint");
	}

	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (isPickedUp && Input.IsActionPressed("pour")) {
			pourTimer -= ((float)delta);

			while (pourTimer < 0.0) {
				pourTimer += 1.0f / pourRate;
			}

			sodaFluid.CreateParticle(FluidSpawnPoint.GlobalPosition);

			ApplyImpulse(-Vector2.Right.Rotated(GlobalRotation) * 5.0f);
		} else {
			pourTimer += 1.0f / pourRate;
		}
	}
}
