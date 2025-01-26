using Godot;
using System;

public partial class Soda : Node
{
	public struct Ingredient {
		int part;
		Type sodaType;

		public Ingredient(int _part, Type _type) {
			part = _part;
			sodaType = _type;
		}
	};

	public enum Type {
		CLUB,
		CRANBERRY,
		ORANGE,
		GINGER,
		MAGIC,
	};

	public enum Cocktail {
		CLASSIC_CLUB,
		CRANBERRY_FEVER,
		CRANBERRY_SURPRISE,
		ORANGE,
		MELLOW_GINGER,
		MIGHTY_GINGER,

	};

	public enum Fizz {
		FLAT,
		LIGHT,
		STANDARD,
		EXTRA,
	}

	static public Ingredient[] GetRecipe(Cocktail cocktail) {

		switch(cocktail) {
			default:
			case Cocktail.CLASSIC_CLUB: return new Ingredient[] { 
				new Ingredient(1, Type.CLUB) };
			case Cocktail.CRANBERRY_FEVER: return new Ingredient[] { 
				new Ingredient(1, Type.CLUB), 
				new Ingredient(1, Type.CLUB) };
			case Cocktail.CRANBERRY_SURPRISE: return new Ingredient[] { 
				new Ingredient(1, Type.ORANGE), 
				new Ingredient(2, Type.CRANBERRY), 
				new Ingredient(1, Type.CLUB) };
			case Cocktail.ORANGE: return new Ingredient[] { 
				new Ingredient(1, Type.ORANGE) };
			case Cocktail.MELLOW_GINGER: return new Ingredient[] { 
				new Ingredient(1, Type.GINGER), 
				new Ingredient(3, Type.CLUB) };
			case Cocktail.MIGHTY_GINGER: return new Ingredient[] { 
				new Ingredient(3, Type.GINGER), 
				new Ingredient(1, Type.CLUB) };
		}
	}

	static public String GetName(Cocktail cocktail) {

		switch(cocktail) {
			default:
			case Cocktail.CLASSIC_CLUB:
				return "Classic Club";
			case Cocktail.CRANBERRY_FEVER:
				return "Cranberry Fever";
			case Cocktail.CRANBERRY_SURPRISE:
				return "Cranberry Surprise";
			case Cocktail.ORANGE:
				return "Orange POWA™";
			case Cocktail.MELLOW_GINGER:
				return "Mellow Ginger";
			case Cocktail.MIGHTY_GINGER:
				return "Mighty Ginger";
		}
	}

	static public String GetFizzName(Fizz fizz) {

		switch(fizz) {
			default:
			case Fizz.FLAT: return "No Fizz";
			case Fizz.LIGHT: return "Lightly Fizzed";
			case Fizz.STANDARD: return "Standard Fizz";
			case Fizz.EXTRA: return "Extra Fizz";
		}
	}

	static public Color GetColor(Type type) {
		switch(type) {
			default:
			case Type.CLUB: return Colors.LightGray;
			case Type.CRANBERRY: return Colors.DarkRed;
			case Type.ORANGE: return Colors.Orange;
			case Type.GINGER: return Colors.Beige;
			case Type.MAGIC: return Colors.Black;
		}
	}
}