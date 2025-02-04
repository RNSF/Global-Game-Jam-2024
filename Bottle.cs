using Godot;
using System;
using System.Diagnostics;
using System.Linq;

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
	Vector2[] previousLinearVelocities = new Vector2[10];
	private Noise noise;

	

	private Node2D FluidSpawnPoint {
		get => GetNode<Node2D>("FluidSpawnPoint");
	}

	private Sprite2D BottleSprite {
		get => GetNode<Sprite2D>("Bottle");
	}


	public AudioStreamPlayer BottleCollisionSound {
		get => GetNode<AudioStreamPlayer>("BottleCollision");
	}

	public AudioStreamPlayer ShakeSound {
		get => GetNode<AudioStreamPlayer>("ShakeSound");
	}

	public AudioStreamPlayer FizzSound {
		get => GetNode<AudioStreamPlayer>("FizzSound");
	}

	int tick;

	

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Debug.Assert(sodaFluid != null, "Missing soda fluid");
		noise = new FastNoiseLite();
		this.MouseEntered += () => isHovered = true;
		this.MouseExited += () => isHovered = false;
		this.BodyEntered += (body) => {
			if (body is PhysicsBody2D physicsBody) {
				var loudness = Mathf.Clamp(Mathf.Remap(LinearVelocity.Length(), 0.0f, 30.0f, 0.0f, 1.0f), 0.0f, 1.0f);
				if ((physicsBody.CollisionLayer & (CollisionLayers.BOTTLES | CollisionLayers.GLASS)) != 0) {
					
					BottleCollisionSound.VolumeDb = Mathf.LinearToDb(loudness);
					BottleCollisionSound.Play();
				}
				
			} 
		};
	}





    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
	{
		var isPouring = false;

		if (isPickedUp && Input.IsActionPressed("pour")) {
			pourTimer -= ((float)delta);

			while (pourTimer < 0.0) {
				pourTimer += 1.0f / pourRate;
			}
			
			isPouring = true;

			sodaFluid.CreateParticle(FluidSpawnPoint.GlobalPosition, Vector2.Up.Rotated(GlobalRotation) * fizzLevel * 2000.0f, fizzLevel, sodaType);

			ApplyImpulse(-Vector2.Right.Rotated(GlobalRotation) * 5.0f);
		} else {
			pourTimer += 1.0f / pourRate;
		}
		
		var acceleration = (LinearVelocity - previousLinearVelocities[tick % previousLinearVelocities.Length]).Length() / ((float)delta);
		
		fizzLevel += acceleration / 80000 * (float) delta;
		fizzLevel -= (isPickedUp ? 0.05f  : 0.1f )* (float) delta;
		fizzLevel = Mathf.Clamp(fizzLevel, 0.0f, 1.0f);

		FizzSound.VolumeDb = Mathf.LinearToDb(fizzLevel / 2 + 0.5f * (isPouring ? 1.0f : 0.0f));
		FizzSound.PitchScale = isPouring ? 1.0f : 0.7f;

		var averagePreviousLinearVelocity = previousLinearVelocities.Aggregate(Vector2.Zero, (acc, x) => acc + x) / previousLinearVelocities.Length;

		if (previousLinearVelocities[tick % previousLinearVelocities.Length].Length() > 100 && averagePreviousLinearVelocity.Normalized().Dot(LinearVelocity.Normalized()) < 0.2 && acceleration > 500) {
			GD.Print("PLAY");
			ShakeSound.Play();
		}

		BottleSprite.Position = new Vector2(noise.GetNoise2D(Time.GetTicksMsec(), 0), noise.GetNoise2D(0, Time.GetTicksMsec()))
			* fizzLevel * 30.0f;

		if (BottleSprite.Material is ShaderMaterial shaderMaterial) {
			Color col = Soda.GetFizzColor(fizzLevel);
			shaderMaterial.SetShaderParameter("color", col);
			shaderMaterial.SetShaderParameter("width", Mathf.Lerp(4.0f, 8.0f, fizzLevel));
		}

		tick += 1;
		previousLinearVelocities[tick % previousLinearVelocities.Length] = LinearVelocity;
	}

	
}
