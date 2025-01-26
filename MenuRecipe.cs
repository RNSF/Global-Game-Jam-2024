using Godot;
using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;





[Tool]
public partial class MenuRecipe : HBoxContainer
{

	public PackedScene MenuIngredientScene;

	public Label NameLabel {
		get => GetNode<Label>("Label");
	}

	public Control IngredientsContainer {
		get => GetNode<Control>("Ingredients");
	}

	

	
	private Soda.Cocktail _cocktail;

	[Export]
	public Soda.Cocktail Cocktail {
		get => _cocktail;
		set {
			_cocktail = value;
			NameLabel.Text = Soda.GetName(value);
			var recipe = Soda.GetRecipe(value);

			foreach (var child in IngredientsContainer.GetChildren()) {
				IngredientsContainer.RemoveChild(child);
			}

			var totalParts = recipe.Aggregate(0, (acc, x) => acc + x.part);

			if (MenuIngredientScene == null) 
				MenuIngredientScene = GD.Load<PackedScene>("res://menu_ingredient.tscn");

			foreach (var ingredient in recipe) {
				var menuIngredient = MenuIngredientScene.Instantiate<MenuIngredient>();
				IngredientsContainer.AddChild(menuIngredient);
				int factor = gcf(ingredient.part, totalParts);
				menuIngredient.Part = ingredient.part / factor;
				menuIngredient.Total = totalParts / factor;
				menuIngredient.SodaType = ingredient.sodaType;
			}
		}
	}

	// from https://stackoverflow.com/questions/13569810/least-common-multiple
	static int gcf(int a, int b)
	{
		while (b != 0)
		{
			int temp = b;
			b = a % b;
			a = temp;
		}
		return a;
	}
}
