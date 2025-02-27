using Godot;
using Godot.Collections;
using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

public partial class Npcs : Node2D
{

	PackedScene NpcScene;
	Array<NpcSpawnPoint> spawnPoints = new Array<NpcSpawnPoint>();


	private Node2D StartPointLeft {
		get => GetNode<Node2D>("StartPointLeft");
	}

	private Node2D StartPointRight {
		get => GetNode<Node2D>("StartPointRight");
	}


	float spawnTimer = 0.0f;
	const float MIN_SPAWN_TIME = 15.0f;
	const float MAX_SPAWN_TIME = 30.0f;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		NpcScene = GD.Load<PackedScene>("res://npc.tscn");
		foreach(Node node in GetTree().GetNodesInGroup("NpcSpawnPoints")) {
			
			if (node is NpcSpawnPoint spawnPoint) {
				spawnPoints.Add(spawnPoint);
			}
		}
	}

    public override void _Process(double delta)
    {
        if (false && Input.IsActionJustPressed("debug_2")) {
			SpawnNpc();
		}

		var filledSpawnPoints = spawnPoints.Where<NpcSpawnPoint>((a) => a.isFilled);
		if (filledSpawnPoints.Count() == 0) spawnTimer = 0.0f;
		spawnTimer -= ((float)delta);
		if (spawnTimer < 0.0f) {
			Random rng = new Random();
			spawnTimer += Mathf.Lerp(MIN_SPAWN_TIME, MAX_SPAWN_TIME, rng.NextSingle());
			SpawnNpc();
		}
    }

    public void SpawnNpc() {
		var possibleSpawnPoints = spawnPoints.Where<NpcSpawnPoint>((a) => !a.isFilled);
		if (possibleSpawnPoints.Count() == 0) return;
		var rng = new Random();
		var spawnPoint = possibleSpawnPoints.ElementAt(rng.Next(possibleSpawnPoints.Count()));
		spawnPoint.isFilled = true;
		var npc = NpcScene.Instantiate<Npc>();
		
		var startingPoint 	= rng.Next(2) == 1 ? StartPointLeft : StartPointRight;
		var endingPoint 	= rng.Next(2) == 1 ? StartPointLeft : StartPointRight;

		npc.GlobalPosition = startingPoint.GlobalPosition;
		npc.spawnPoint = spawnPoint;
		npc.endPoint = endingPoint;
		AddChild(npc);
		npc.CurrentState = Npc.State.WALK_IN;

		var possibleFizzValues = Enum.GetValues(typeof(Soda.Fizz));
		Soda.Fizz fizz = (Soda.Fizz)possibleFizzValues.GetValue(rng.Next(possibleFizzValues.Length));

		var possibleCocktails = Enum.GetValues(typeof(Soda.Cocktail));
		Soda.Cocktail cocktail = (Soda.Cocktail)possibleCocktails.GetValue(rng.Next(possibleCocktails.Length));

		npc.SetOrder(cocktail, fizz);

		GD.Print("Spawning NPC!");

		npc.OrderFullfilled += () => {
			if (npc.spawnPoint is NpcSpawnPoint npcSpawnPoint) {
				npcSpawnPoint.isFilled = false;
			}
		};

		

		
	}

	
	
}
