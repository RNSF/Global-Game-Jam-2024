using Godot;
using System;

[Tool]
public partial class Menu : Control
{
	public PackedScene MenuRecipeScene;

	public Control RecipesContainer {
		get => GetNode<Control>("VBoxContainer/RecipesContainer");
	}

	public Control ActivePosition {
		get => GetNode<Control>("ActivePosition");
	}

	public Control UnactivePosition {
		get => GetNode<Control>("UnactivePosition");
	}

	public bool isActive;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MenuRecipeScene = GD.Load<PackedScene>("res://menu_recipe.tscn");

		foreach (var child in RecipesContainer.GetChildren()) {
			RecipesContainer.RemoveChild(child);
		}

		foreach (var cocktail in Enum.GetValues(typeof(Soda.Cocktail))) {
			var menuRecipe = MenuRecipeScene.Instantiate<MenuRecipe>();
			RecipesContainer.AddChild(menuRecipe);
			menuRecipe.Cocktail = (Soda.Cocktail) cocktail;
		}

		if (Engine.IsEditorHint()) return;
		MouseEntered 	+= () => { isActive = true; };
		MouseExited 	+= () => { isActive = false; };

		
	}

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint()) return;

		
		GD.Print(isActive);
		if (ActivePosition != null && UnactivePosition != null) {
			GlobalPosition = GlobalPosition.Lerp((isActive ? (ActivePosition) : (UnactivePosition)).GlobalPosition, ((float)delta) * 10.0f);
		}

		
    }
}
