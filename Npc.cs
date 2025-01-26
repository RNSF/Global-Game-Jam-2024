using Godot;
using System;




public partial class Npc : Node2D
{
	public enum State {
		WALK_IN,
		ORDERING,
		WALK_OUT
	}

	public enum Character {
		COWBOY,
		BOWLER,
		LADY,
		OLD_MAN,
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

	private Node2D CharacterSprite {
		get => GetNode<Node2D>("Sprite");
	}

	private PackedScene TextParticleScene;

	float lifetime = 0.0f;

	[Signal]
	public delegate void OrderFullfilledEventHandler();
	
	public AudioStreamPlayer KachingSound {
		get => GetNode<AudioStreamPlayer>("Kaching");
	}

	public AudioStreamPlayer OrderSound {
		get => GetNode<AudioStreamPlayer>("Order");
	}

	public AudioStreamPlayer AngerSound {
		get => GetNode<AudioStreamPlayer>("Anger");
	}

	private Character character;

    public override void _Ready()
    {
		Array values = Enum.GetValues(typeof(Character));
		Random random = new Random();
		Character character = (Character)values.GetValue(random.Next(values.Length));

		for (int i = 0; i < CharacterSprite.GetChildCount(); i++) {

			if (CharacterSprite.GetChild(i) is Node2D node2D) {
				node2D.Visible = i == ((int)character);
			}
		}

		if (character == Character.BOWLER || character == Character.OLD_MAN) {
			OrderSound.PitchScale = 1.1f;
			AngerSound.PitchScale = 1.1f;
		}

		if (character == Character.LADY) {
			OrderSound.PitchScale = 1.5f;
			AngerSound.PitchScale = 1.5f;
		}

		
		
		TextParticleScene = GD.Load<PackedScene>("res://text_particle.tscn");
        PlacementArea.GlassServed += (glass) => {
			if (glass.sodaVolume == 0.0) return;

			var compositionDistance = 0.0f;
			var idealComposition = Soda.GetComposition(cocktail);
			
			for (var i = 0; i < idealComposition.Length; i++) {
				compositionDistance += Mathf.Abs(idealComposition[i] - glass.sodaComposition[i]);
			}

			float idealFizziness = Soda.GetFizzinessOfLevel(fizzLevel);
			
			float fizzDistance = Mathf.Clamp(Mathf.Remap(Mathf.Abs(idealFizziness - glass.fizzLevel), 0.3f, 0.6f, 0.0f, 1.0f), 0.0f, 1.0f);

			compositionDistance = Mathf.Clamp(compositionDistance, 0.0f, 1.0f);

			var volumeDistance = Mathf.Max(glass.sodaVolume - 18000, 0) / 18000;

			var percentPerfect = (1 - volumeDistance) * (1 - compositionDistance) * (1 - fizzDistance);
			GD.Print(percentPerfect, ", ", compositionDistance, ", ", volumeDistance, ", ", fizzDistance);

			glass.DestroyFluid();

			var textParticle = TextParticleScene.Instantiate<TextParticle>();
			GetTree().Root.AddChild(textParticle);
			textParticle.GlobalPosition = GlobalPosition + Vector2.Up * 150f;
			
			textParticle.TextLabel.Text = "[center]";
			
			if (percentPerfect > 0.9) {
				textParticle.TextLabel.Text += new Godot.Collections.Array<String>{
					"Perfect!",
					"Marveleous!",
					"10/10",
				}.PickRandom();;
			} else if(percentPerfect > 0.8) {
				textParticle.TextLabel.Text += new Godot.Collections.Array<String>{
					"Very Nice!",
					"Superb!",
					"Wonderful.",
				}.PickRandom();
			} else if(percentPerfect > 0.7) {
				textParticle.TextLabel.Text += new Godot.Collections.Array<String>{
					"Tasty!",
					"Thank you.",
					"Pretty Good!",
				}.PickRandom();
			} else if(percentPerfect > 0.5) {
				textParticle.TextLabel.Text += new Godot.Collections.Array<String>{
					"It's not quite there.",
					"Meh. This is alright.",
					"Could be better.",
				}.PickRandom();
			} else {
				textParticle.TextLabel.Text += new Godot.Collections.Array<String>{
					"You call that a drink?",
					"Disgusting!",
					"What is this ?!",
				}.PickRandom();
			}

			if (percentPerfect > 0.95) {
				textParticle.TextLabel.Text = "[wave amp=50.0 freq=10.0 connected=1]" + textParticle.TextLabel.Text;
			}

			EmitSignal(SignalName.OrderFullfilled);
			CurrentState = State.WALK_OUT;

			if (percentPerfect > 0.7) {
				KachingSound.Play();
			} else if (percentPerfect > 0.5) {
				OrderSound.Play();
			} else {
				AngerSound.Play();
			}

			
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
			OrderSound.Play();
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
		float fizziness = Soda.GetFizzinessOfLevel(fizzLevel);
		Color color = Soda.GetFizzColor(fizziness);
		color = Colors.Black.Lerp(color, Mathf.Clamp(Mathf.Remap(fizziness, 0.2f, 0.3f, 0.0f, 1.0f), 0.0f, 1.0f));
		
		SpeachBubble.Text = new Godot.Collections.Array<String>{
			$"One {Soda.GetName(cocktail)},\n[color={color.ToHtml()}]{Soda.GetFizzName(fizzLevel)}[/color]",
			$"A {Soda.GetName(cocktail)},\n[color={color.ToHtml()}]{Soda.GetFizzName(fizzLevel)}[/color] please.",
			$"I'll have a {Soda.GetName(cocktail)},\n[color={color.ToHtml()}]{Soda.GetFizzName(fizzLevel)}[/color]."
		}.PickRandom();
	}
}
