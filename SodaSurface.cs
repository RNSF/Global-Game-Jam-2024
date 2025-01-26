using Godot;
using System;

[Tool]
public partial class SodaSurface : SubViewportContainer
{

	private SubViewport SubViewportNode {
		get => GetNode<SubViewport>("SubViewport");
	}

	private GpuParticles2D BubbleParticles {
		get => GetNode<GpuParticles2D>("BubbleParticles");
	}

	public CanvasGroup CanvasGroupNode {
		get => GetNode<CanvasGroup>("SubViewport/CanvasGroup");
	}

	public Camera2D SubCamera {
		get => GetNode<Camera2D>("SubViewport/Camera2D");
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ItemRectChanged += () => {
			UpdateSize(Position, Size);
		};

		UpdateSize(Position, Size);

		
	}

    public override void _Process(double delta)
    {
		//SubCamera.GlobalTransform = GetGlobalTransform().Inverse() * GetViewport().GetCamera2D().Transform.Inverse();
		//SubCamera.GlobalPosition =  GetViewport().GetCamera2D().GlobalPosition - GetParent<Node2D>().GlobalPosition;
		
        SubCamera.GlobalPosition = SubCamera.GlobalPosition + new Vector2( Input.GetAxis("ui_left", "ui_right"),  Input.GetAxis("ui_up", "ui_down")) * 100.0f * ((float)delta);
    }

    public void UpdateSize(Vector2 position, Vector2 size) {
		SubViewportNode.Size = new Vector2I(((int)size.X), ((int)size.Y));
		if (BubbleParticles.ProcessMaterial is ParticleProcessMaterial mat) {
			mat.EmissionBoxExtents = new Vector3(size.X / 2, Size.Y / 2, 0);
			mat.EmissionShapeOffset = new Vector3(size.X / 2, Size.Y / 2, 0);
			BubbleParticles.Amount = (int) (size.X * size.Y * 0.01f);
		};

		SubCamera.GlobalPosition = Size / 2;
		SubCamera.Transform *= GetTransform().Inverse();

		
		
	}
}
