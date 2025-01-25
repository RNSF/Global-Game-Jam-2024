using Godot;
using System;

public partial class Bottle : RigidBody2D
{

	public bool isHovered = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.MouseEntered += () => isHovered = true;
		this.MouseExited += () => isHovered = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
