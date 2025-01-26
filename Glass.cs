using Godot;
using System;
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

	ConvexPolygonShape2D innerShape;


    public override void _Ready()
    {
		Debug.Assert(sodaFluid != null);
        innerShape = new ConvexPolygonShape2D();
		innerShape.Points = InnerAreaShape.Polygon;
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

		// detect fluid
		var state = GetWorld2D().DirectSpaceState;
		var query = new PhysicsShapeQueryParameters2D();
		query.CollideWithAreas = false;
		query.CollideWithBodies = true;
		query.CollisionMask = CollisionLayers.FLUID;
		query.Shape = innerShape;
		query.Transform = GlobalTransform;
		var results = state.IntersectShape(query, 300);

		float[] sodaComposition = new float[Enum.GetNames(typeof(Soda.Type)).Length];

		foreach (var result in results) {
			Rid rid = result["rid"].As<Rid>();
			if (!sodaFluid.bodyToParticle.ContainsKey(rid)) continue;
			SodaFluid.Particle particle = sodaFluid.bodyToParticle[rid];

			for (int i = 0; i < sodaComposition.Count(); i++) {
				sodaComposition[i] += particle.sodaComposition[i];
			}
			
		}

		GD.Print(sodaComposition[0]);

		for (int i = 0; i < sodaComposition.Count(); i++) {
			sodaComposition[i] /= results.Count();
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
