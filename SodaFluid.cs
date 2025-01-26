using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;




public partial class SodaFluid : Node2D
{
	
	public struct Particle {
		public Rid body;
		public Rid canvasItem;
		public float[] sodaComposition;
		public float radius;
	}

	const float MIN_PARTICLE_RADIUS = 6.0f;
	const float MAX_PARTICLE_RADIUS = 8.0f;
	public Dictionary<Rid, Particle> bodyToParticle = new Dictionary<Rid, Particle>();
	
	CanvasGroup SodaCanvasGroup {
		get => GetNode<CanvasGroup>("SodaSurface/SubViewport/CanvasGroup");
	}

    public override void _Ready() {
		
        PhysicsServer2D.SpaceSetParam(GetWorld2D().Space, PhysicsServer2D.SpaceParameter.SolverIterations, 20.0f);
    }

    public override void _PhysicsProcess(double delta) {

		if (Input.IsActionPressed("debug_1")) {
			CreateParticle(GetGlobalMousePosition(), Vector2.Zero, Soda.Type.CRANBERRY);
		}

		foreach(var particle in bodyToParticle.Values) {
			var particleTransform = PhysicsServer2D.BodyGetState(particle.body, PhysicsServer2D.BodyState.Transform).As<Transform2D>();
			// particleTransform.Origin = particleTransform.Origin - GlobalPosition;
			RenderingServer.CanvasItemSetTransform(particle.canvasItem, particleTransform);
			RenderingServer.CanvasItemSetParent(particle.canvasItem, SodaCanvasGroup.GetCanvasItem());
		}
	}

	public void CreateParticle(Vector2 particlePosition, Vector2 particleVelocity, Soda.Type sodaType) {
		// based on https://github.com/Chevifier/Fluid-Simulation-in-Godot/blob/main/FluidSim2D/WaterGenPhysicsServer.gd

		if (!IsNodeReady()) return;

		Rid particleShape = PhysicsServer2D.CircleShapeCreate();
		Random rng = new Random();
		float particleRadius = Mathf.Lerp(MIN_PARTICLE_RADIUS, MAX_PARTICLE_RADIUS, ((float)rng.NextDouble()));
		PhysicsServer2D.ShapeSetData(particleShape, particleRadius);

		Transform2D particleTransform = Transform2D.Identity.Translated(particlePosition);

		var body = PhysicsServer2D.BodyCreate();
		PhysicsServer2D.BodyAddShape(body, particleShape);
		PhysicsServer2D.BodySetMode(body, PhysicsServer2D.BodyMode.Rigid);
		PhysicsServer2D.BodySetSpace(body, GetWorld2D().Space);
		PhysicsServer2D.BodySetCollisionLayer(body, CollisionLayers.FLUID | CollisionLayers.SOLID);
		PhysicsServer2D.BodySetCollisionMask(body, CollisionLayers.FLUID | CollisionLayers.SOLID);
		PhysicsServer2D.BodySetParam(body, PhysicsServer2D.BodyParameter.Friction, 0.0);
		PhysicsServer2D.BodySetParam(body, PhysicsServer2D.BodyParameter.Mass, 0.05);
		PhysicsServer2D.BodySetParam(body, PhysicsServer2D.BodyParameter.GravityScale, 0.5);
		PhysicsServer2D.BodySetParam(body, PhysicsServer2D.BodyParameter.Bounce, 0.0);
		PhysicsServer2D.BodySetParam(body, PhysicsServer2D.BodyParameter.LinearDamp, 0.1);
		PhysicsServer2D.BodySetState(body, PhysicsServer2D.BodyState.Transform, particleTransform);
		PhysicsServer2D.BodySetState(body, PhysicsServer2D.BodyState.LinearVelocity, particleVelocity);
		PhysicsServer2D.BodySetContinuousCollisionDetectionMode(body, PhysicsServer2D.CcdMode.CastRay);

		var canvasItem = RenderingServer.CanvasItemCreate();
		RenderingServer.CanvasItemSetParent(canvasItem, SodaCanvasGroup.GetCanvasItem());
		RenderingServer.CanvasItemSetTransform(canvasItem, particleTransform);
		RenderingServer.CanvasItemAddCircle(canvasItem, Vector2.Zero, particleRadius * 2.0f, Colors.White, false);
		RenderingServer.CanvasItemSetModulate(canvasItem, Soda.GetColor(sodaType));

		float[] sodaComposition = new float[Enum.GetNames(typeof(Soda.Type)).Length];
		sodaComposition[(int) sodaType] = 1.0f;
		var particle = new Particle {body = body, canvasItem = canvasItem, sodaComposition = sodaComposition, radius = particleRadius};
		bodyToParticle.Add(body, particle);
	}

	public void DestroyParticle(Rid bodyRid) {
		Particle particle = bodyToParticle[bodyRid];
		bodyToParticle.Remove(bodyRid);
		RenderingServer.FreeRid(particle.canvasItem);
		PhysicsServer2D.FreeRid(particle.body);
	}
}
