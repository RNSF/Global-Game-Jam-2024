using Godot;
using System;




public partial class Npc : Node2D
{
	public enum State {
		WALK_IN,
		ORDERING,
		WALK_OUT
	}
	
	public Node2D spawnPoint;
	public Node2D endPoint;
	private State _currentState = State.WALK_IN;
	public State CurrentState {
		get => _currentState;
		set {
			_currentState = value;
			switch (CurrentState) {
				case State.WALK_IN: 	GetParent()?.MoveChild(this, 0); break;
				case State.ORDERING: 	GetParent()?.MoveChild(this, -1); break;
				case State.WALK_OUT: 	GetParent()?.MoveChild(this, 0); break;
			}
		}
	}


	public override void _PhysicsProcess(double delta)
	{
		var targetPosition = GetTargetPosition();
		var displacement = targetPosition - GlobalPosition;
		var dx = displacement.Normalized() * 200.0f * (float) delta;
		

		dx = dx.Normalized() * (float) Math.Clamp(dx.Length(), 0.0, displacement.Length());
		GlobalPosition = GlobalPosition + dx;
		
		bool atTarget = GlobalPosition.IsEqualApprox(targetPosition);
		if (atTarget && CurrentState == State.WALK_IN) {
			CurrentState++;
		}

		if (atTarget && CurrentState == State.WALK_OUT) {
			QueueFree();
		}

		
	}

	private Vector2 GetTargetPosition() {
		switch (CurrentState) {
			case State.WALK_IN: 	return spawnPoint.Position;
			case State.ORDERING: 	return spawnPoint.Position;
			case State.WALK_OUT: 	return endPoint.Position;
			default:				return endPoint.Position;
		}
	}
}
