using Godot;
using System;

public partial class Table : RigidBody2D
{
	[Export]
	public SodaFluid sodaFluid;
    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {

		for (int i = 0; i < state.GetContactCount(); i++) {
			Rid body = state.GetContactCollider(i);
			if (sodaFluid.bodyToParticle.ContainsKey(body)) {
				sodaFluid.DestroyParticle(body);
			}

			
		}
    }

	public AudioStreamPlayer TableCollisionSound {
		get => GetNode<AudioStreamPlayer>("TableCollision");
	}

    public override void _Ready()
    {
        BodyEntered += (body) => {
			if (body is RigidBody2D physicsBody) {
				var loudness = Mathf.Clamp(Mathf.Remap(physicsBody.LinearVelocity.Length(), 0.0f, 30.0f, 0.0f, 1.0f), 0.0f, 1.0f);
				GD.Print(loudness);
				if ((physicsBody.CollisionLayer & (CollisionLayers.BOTTLES | CollisionLayers.GLASS)) != 0) {
					TableCollisionSound.VolumeDb = Mathf.LinearToDb(loudness);
					TableCollisionSound.Play();
				}
				
			} 
		};
    }



}
