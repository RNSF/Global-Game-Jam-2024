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
	private Soda.Cocktail cocktail;
	private Soda.Fizz fizzLevel;

	public State CurrentState {
		get => _currentState;
		set {
			var oldState = _currentState;
			_currentState = value;
			switch (CurrentState) {
				case State.WALK_IN: 	GetParent()?.MoveChild(this, 0); break;
				case State.ORDERING: 	GetParent()?.MoveChild(this, -1); break;
				case State.WALK_OUT: 	GetParent()?.MoveChild(this, 0); break;
			}

			bool isOrdering = CurrentState == State.ORDERING;
			GetParent()?.MoveChild(this, isOrdering ? -1 : 0);
			SpeachBubble.Visible = isOrdering;
			if (oldState != _currentState) {
				SpeachBubble.PercentVisible = 0.0f;
			}

			if (CharacterSprite != null)
				CharacterSprite.Modulate = Colors.White.Lerp(Colors.Black, CurrentState == State.ORDERING ? 0.0f : 0.3f);
		}
	}

	private PlacementZone PlacementArea {
		get => GetNode<PlacementZone>("PlacementZone");
	}

	private OrderBubble SpeachBubble {
		get => GetNode<OrderBubble>("OrderBubble");
	}

	private Sprite2D CharacterSprite {
		get => GetNode<Sprite2D>("Sprite");
	}

	private PackedScene TextParticleScene;

	float lifetime = 0.0f;

    public override void _Ready()
    {
		TextParticleScene = GD.Load<PackedScene>("res://text_particle.tscn");
        PlacementArea.GlassServed += (glass) => {
			if (glass.sodaVolume == 0.0) return;

			var compositionDistance = 0.0f;
			var idealComposition = Soda.GetComposition(cocktail);
			
			for (var i = 0; i < idealComposition.Length; i++) {
				compositionDistance += Mathf.Abs(idealComposition[i] - glass.sodaComposition[i]);
			}

			float idealFizziness = 0.0f;
			switch (fizzLevel) {
				case Soda.Fizz.FLAT: 		idealFizziness = 0.0f; break;
				case Soda.Fizz.LIGHT:		idealFizziness = 0.3f; break;
				case Soda.Fizz.STANDARD:	idealFizziness = 0.6f; break;
				case Soda.Fizz.EXTRA:		idealFizziness = 0.9f; break;
			}

			float fizzDistance = Mathf.Clamp(Mathf.Remap(Mathf.Abs(idealFizziness - glass.fizzLevel), 0.3f, 0.6f, 0.0f, 1.0f), 0.0f, 1.0f);

			compositionDistance = Mathf.Clamp(compositionDistance, 0.0f, 1.0f);

			var volumeDistance = Mathf.Max(glass.sodaVolume - 18000, 0) / 18000;

			var percentPerfect = (1 - volumeDistance) * (1 - compositionDistance) * (1 - fizzDistance);
			GD.Print(percentPerfect, ", ", compositionDistance, ", ", volumeDistance, ", ", fizzDistance);

			glass.DestroyFluid();

			var textParticle = TextParticleScene.Instantiate<TextParticle>();
			GetTree().Root.AddChild(textParticle);
			textParticle.GlobalPosition = GlobalPosition + Vector2.Up * 150f;
			
			if (percentPerfect > 0.9) {
				textParticle.TextLabel.Text = "Perfect!";
			} else if(percentPerfect > 0.8) {
				textParticle.TextLabel.Text = "Very Nice.";
			} else if(percentPerfect > 0.7) {
				textParticle.TextLabel.Text = "Tasty!";
			} else if(percentPerfect > 0.5) {
				textParticle.TextLabel.Text = "It's Missing Something...";
			} else {
				textParticle.TextLabel.Text = "You call that a drink?";
			}

			CurrentState = State.WALK_OUT;
		};
    }

    public override void _PhysicsProcess(double delta)
	{
		var targetPosition = GetTargetPosition();
		var displacement = targetPosition - GlobalPosition;
		var dx = displacement.Normalized() * 200.0f * (float) delta;

		lifetime += ((float)delta) * (CurrentState == State.ORDERING ? 2.0f : 4.0f);
		float amplitude = CurrentState == State.ORDERING ? 3.0f : 8.0f;

		float bob = Mathf.Abs(Mathf.Sin(lifetime) * amplitude) - amplitude / 2;
		if (CurrentState == State.ORDERING) bob = Mathf.Sin(lifetime) * amplitude;

		CharacterSprite.Position = Vector2.Up * bob;
		

		dx = dx.Normalized() * (float) Math.Clamp(dx.Length(), 0.0, displacement.Length());
		GlobalPosition = GlobalPosition + dx;
		
		bool atTarget = GlobalPosition.IsEqualApprox(targetPosition);
		if (atTarget && CurrentState == State.WALK_IN) {
			CurrentState++;
		}

		if (atTarget && CurrentState == State.WALK_OUT) {
			GD.Print("Freed Customer");
			QueueFree();
		}

		if (CurrentState == State.ORDERING) {
			SpeachBubble.PercentVisible += 1.0f * ((float)delta);
		}

		PlacementArea.IsEnabled = CurrentState == State.ORDERING;

		
	}

	private Vector2 GetTargetPosition() {
		switch (CurrentState) {
			case State.WALK_IN: 	return spawnPoint.Position;
			case State.ORDERING: 	return spawnPoint.Position;
			case State.WALK_OUT: 	return endPoint.Position;
			default:				return endPoint.Position;
		}
	}

	public void SetOrder(Soda.Cocktail newCocktail, Soda.Fizz newFizzLevel) {
		cocktail = newCocktail;
		fizzLevel = newFizzLevel;
		SpeachBubble.Text = $"One {Soda.GetName(cocktail)},\n{Soda.GetFizzName(fizzLevel)}";
	}
}
