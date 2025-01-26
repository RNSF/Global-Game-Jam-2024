using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class Glass : RigidBody2D
{
	[Export]
	public SodaFluid sodaFluid;
	public bool isPickedUp = false;

	private Area2D OuterArea {
		get => GetNode<Area2D>("OuterArea");
	}

	private Area2D InnerArea {
		get => GetNode<Area2D>("InnerArea");
	}

	private CollisionPolygon2D InnerAreaShape {
		get => GetNode<CollisionPolygon2D>("InnerArea/CollisionPolygon2D");
	}
	
	private bool isOnBar = false;
	private bool isBehindBar = false;

	ConvexPolygonShape2D innerShape;
	public float[] sodaComposition;
	public float sodaVolume;



    public override void _Ready()
    {
		Debug.Assert(sodaFluid != null);
        innerShape = new ConvexPolygonShape2D();
		innerShape.Points = InnerAreaShape.Polygon;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
	{
		isBehindBar = InnerArea.GetOverlappingBodies().Any((Node2D node) => {
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

		// detect fluid
		var results = SodaCheck();

		sodaComposition = new float[Enum.GetNames(typeof(Soda.Type)).Length];
		sodaVolume = 0.0f;

		foreach (var result in results) {
			Rid rid = result["rid"].As<Rid>();
			if (!sodaFluid.bodyToParticle.ContainsKey(rid)) continue;
			SodaFluid.Particle particle = sodaFluid.bodyToParticle[rid];

			for (int i = 0; i < sodaComposition.Count(); i++) {
				float volume = particle.radius * particle.radius;
				sodaVolume += volume;
				sodaComposition[i] += particle.sodaComposition[i] * volume;
			}
		}

		if (sodaVolume > 0) {
			for (int i = 0; i < sodaComposition.Count(); i++) {
				sodaComposition[i] /= sodaVolume;
			}

			foreach (var result in results) {
				Rid rid = result["rid"].As<Rid>();
				SodaFluid.Particle particle = sodaFluid.bodyToParticle[rid];
				Color color = Colors.Black;

				for (int i = 0; i < sodaComposition.Count(); i++) {
					particle.sodaComposition[i] = sodaComposition[i];
					color += particle.sodaComposition[i] * Soda.GetColor((Soda.Type) i);
				}

				
				RenderingServer.CanvasItemSetModulate(particle.canvasItem, color);
			}
		}

	}

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
		isOnBar = false;
		for (var i = 0; i < state.GetContactCount(); i++) {
			
			var obj = state.GetContactColliderObject(i);
			if (obj is PhysicsBody2D physicsBody2D) {
				if ((physicsBody2D.CollisionLayer & CollisionLayers.BAR) == 0) continue;
				isOnBar = isOnBar || state.GetContactLocalNormal(i).Dot(Vector2.Up) > 0.9;
			}
		}
        
    }

    public bool CanServe() {
		return !isBehindBar && isOnBar && Mathf.AngleDifference(GlobalRotation, 0) < 0.05;
	}

	public void Destroy() {
		var results = SodaCheck();

		foreach (var result in results) {
			Rid rid = result["rid"].As<Rid>();
			sodaFluid.DestroyParticle(rid);
		}

		QueueFree();
	}

	public Godot.Collections.Array<Godot.Collections.Dictionary> SodaCheck() {
		var state = GetWorld2D().DirectSpaceState;
		var query = new PhysicsShapeQueryParameters2D();
		query.CollideWithAreas = false;
		query.CollideWithBodies = true;
		query.CollisionMask = CollisionLayers.FLUID;
		query.Shape = innerShape;
		query.Transform = GlobalTransform;
		return state.IntersectShape(query, 300);
	}
	
}
