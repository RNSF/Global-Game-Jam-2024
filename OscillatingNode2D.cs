using Godot;
using System;

[Tool]
public partial class OscillatingNode2D : Node2D
{
	[Export]
	public float amplitude;

	[Export]
	public float frequency;	

	
	public override void _Process(double delta)
	{
		foreach (var child in GetChildren()) {
			if (child is Node2D node2D) {
				node2D.Position = new Vector2(node2D.Position.X, Mathf.Sin(frequency * Time.GetTicksMsec() / 1000) * amplitude);
			}
		}
	}
}
