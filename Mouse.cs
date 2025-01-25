using Godot;
using System;

[Tool]
public partial class Mouse : Node2D
{
	private PhysicsBody2D _body;
	[Export]
    public PhysicsBody2D Body { get => _body; set {
		_body = value;

		if (_body != null) 	GetNode<PinJoint2D>("PinJoint2D").NodeA = _body.GetPath();
		else 		 		GetNode<PinJoint2D>("PinJoint2D").NodeA = null;
		
	} }
}
