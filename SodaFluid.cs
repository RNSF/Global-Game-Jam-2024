using Godot;
using System;
using System.Threading.Tasks;


struct FluidParticle {
	public Rid body;
	public Rid canvasItem;
}

public partial class SodaFluid : Node2D
{
	
	const float PARTICLE_RADIUS = 6.0f;
	Rid particleShape;
	System.Collections.Generic.List<FluidParticle> particles = new System.Collections.Generic.List<FluidParticle>();
	

    public override void _Ready() {
        particleShape = PhysicsServer2D.CircleShapeCreate();
		PhysicsServer2D.ShapeSetData(particleShape, PARTICLE_RADIUS);
    }

    public override void _PhysicsProcess(double delta) {

		if (Input.IsActionPressed("debug_1")) {
			CreateParticle(GetGlobalMousePosition());
		}

		foreach(var particle in particles) {
			var particleTransform = PhysicsServer2D.BodyGetState(particle.body, PhysicsServer2D.BodyState.Transform).As<Transform2D>();
			particleTransform.Origin = particleTransform.Origin - GlobalPosition;
			RenderingServer.CanvasItemSetTransform(particle.canvasItem, particleTransform);
		}
	}

	public void CreateParticle(Vector2 particlePosition) {
		// based on https://github.com/Chevifier/Fluid-Simulation-in-Godot/blob/main/FluidSim2D/WaterGenPhysicsServer.gd

		if (!IsNodeReady()) return;

		Transform2D particleTransform = Transform2D.Identity.Translated(particlePosition);

		var body = PhysicsServer2D.BodyCreate();
		PhysicsServer2D.BodyAddShape(body, particleShape);
		PhysicsServer2D.BodySetMode(body, PhysicsServer2D.BodyMode.Rigid);
		PhysicsServer2D.BodySetSpace(body, GetWorld2D().Space);
		PhysicsServer2D.BodySetCollisionLayer(body, CollisionLayers.FLUID | CollisionLayers.SOLID);
		PhysicsServer2D.BodySetCollisionMask(body, CollisionLayers.FLUID | CollisionLayers.SOLID);
		PhysicsServer2D.BodySetParam(body, PhysicsServer2D.BodyParameter.Friction, 0.0);
		PhysicsServer2D.BodySetParam(body, PhysicsServer2D.BodyParameter.Mass, 0.05);
		PhysicsServer2D.BodySetParam(body, PhysicsServer2D.BodyParameter.GravityScale, 1.0);
		PhysicsServer2D.BodySetState(body, PhysicsServer2D.BodyState.Transform, particleTransform);

		var canvasItem = RenderingServer.CanvasItemCreate();
		RenderingServer.CanvasItemSetParent(canvasItem, GetCanvasItem());
		RenderingServer.CanvasItemSetTransform(canvasItem, particleTransform);
		RenderingServer.CanvasItemAddCircle(canvasItem, Vector2.Zero, PARTICLE_RADIUS + 3, Colors.White, true);

		var particle = new FluidParticle {body = body, canvasItem = canvasItem};
		particles.Add(particle);

		
	}
}
