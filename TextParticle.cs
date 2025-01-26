using Godot;
using System;
using System.Reflection;

public partial class TextParticle : Control
{

	float animationTime = 5.0f;

	public Label TextLabel {
		get => GetNode<Label>("Label");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = Position + Vector2.Up * ((float)delta) * 40.0f;
		Modulate = Colors.White - new Color(0, 0, 0, Mathf.Clamp(Mathf.Remap(animationTime, 2.0f, 0.0f, 0.0f, 1.0f), 0.0f, 1.0f));
		animationTime -= ((float)delta);

		if (animationTime < 0.0f) {
			QueueFree();
		}
	}
}
