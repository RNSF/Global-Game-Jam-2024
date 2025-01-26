using Godot;
using System;
using System.Diagnostics;

public partial class Bottle : RigidBody2D
{

	[Export]
	public SodaFluid sodaFluid;

	[Export]
	public Soda.Type sodaType;
	public bool isPickedUp;
	
	public bool isHovered = false;
	private float pourTimer = 1.0f;
	private float pourRate = 5.0f;

	private float fizzLevel = 0.0f;
	private Vector2 previousLinearVelocity = Vector2.Zero;

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

			sodaFluid.CreateParticle(FluidSpawnPoint.GlobalPosition, Vector2.Up.Rotated(GlobalRotation) * fizzLevel * 2000.0f, fizzLevel, sodaType);

			ApplyImpulse(-Vector2.Right.Rotated(GlobalRotation) * 5.0f);
		} else {
			pourTimer += 1.0f / pourRate;
		}
		
		var acceleration = (LinearVelocity - previousLinearVelocity).Length() / ((float)delta);
		
		fizzLevel += acceleration / 80000 * (float) delta;
		fizzLevel -= (isPickedUp ? 0.05f  : 0.1f )* (float) delta;
		fizzLevel = Mathf.Clamp(fizzLevel, 0.0f, 1.0f);


		previousLinearVelocity = LinearVelocity;
	}

	
}
