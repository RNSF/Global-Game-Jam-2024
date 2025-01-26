using Godot;
using System;

public partial class Table : RigidBody2D
{
	[Export]
	public SodaFluid sodaFluid;
    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {

		GD.Print(state.GetContactCount());

		for (int i = 0; i < state.GetContactCount(); i++) {
			Rid body = state.GetContactCollider(i);
			if (sodaFluid.bodyToParticle.ContainsKey(body)) {
				sodaFluid.DestroyParticle(body);
			}
		}
    }
}
