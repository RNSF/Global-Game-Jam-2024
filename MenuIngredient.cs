using Godot;
using System;

[Tool]
public partial class MenuIngredient : HBoxContainer
{
	
	

	public Label NumeratorLabel {
		get => GetNode<Label>("VBoxContainer/Numerator");
	}

	public Label DenominatorLabel {
		get => GetNode<Label>("VBoxContainer/Denominator");
	}

	public TextureRect BottleIcon {
		get => GetNode<TextureRect>("BottleIcon");
	}

	[Export]
	public int Part {
		get => 0;
		set {
			NumeratorLabel.Text = $"{value}";
		}
	}

	[Export]
	public int Total {
		get => 0;
		set {
			DenominatorLabel.Text = $"{value}";
		}
	}

	[Export]
	public Soda.Type SodaType {
		get => 0;
		set {
			switch(value) {
				case Soda.Type.CLUB:		BottleIcon.Texture = GD.Load<Texture2D>("res://temp_assets/glass.svg"); break;
				case Soda.Type.CRANBERRY:	BottleIcon.Texture = GD.Load<Texture2D>("res://temp_assets/glass.svg"); break;
				case Soda.Type.GINGER:		BottleIcon.Texture = GD.Load<Texture2D>("res://temp_assets/glass.svg"); break;
				case Soda.Type.COLA:		BottleIcon.Texture = GD.Load<Texture2D>("res://temp_assets/glass.svg"); break;
			}
		}
	}

}
