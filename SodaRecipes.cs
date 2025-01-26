using Godot;
using System;

public partial class Soda : Node
{
	public struct Ingredient {
		public int part;
		public Type sodaType;

		public Ingredient(int _part, Type _type) {
			part = _part;
			sodaType = _type;
		}
	};

	public enum Type {
		CLUB,
		CRANBERRY,
		GINGER,
		COLA,
	};

	public enum Cocktail {
		CLASSIC_CLUB,
		CRANBERRY_FEVER,
		CRANBERRY_SURPRISE,
		COLA,
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
				new Ingredient(1, Type.CRANBERRY) };
			case Cocktail.CRANBERRY_SURPRISE: return new Ingredient[] { 
				new Ingredient(2, Type.CRANBERRY), 
				new Ingredient(1, Type.COLA), 
				new Ingredient(1, Type.CLUB) };
			case Cocktail.COLA: return new Ingredient[] { 
				new Ingredient(1, Type.COLA) };
			case Cocktail.MELLOW_GINGER: return new Ingredient[] { 
				new Ingredient(1, Type.GINGER), 
				new Ingredient(2, Type.CLUB) };
			case Cocktail.MIGHTY_GINGER: return new Ingredient[] { 
				new Ingredient(3, Type.GINGER), 
				new Ingredient(1, Type.CLUB) };
		}
	}

	static public float[] GetComposition(Cocktail cocktail) {
		var sodaComposition = new float[Enum.GetNames(typeof(Soda.Type)).Length];

		var totalParts = 0.0f;
		foreach (var ingredient in GetRecipe(cocktail)) {
			totalParts += ingredient.part;
			sodaComposition[(int) ingredient.sodaType] += ingredient.part;
		}

		foreach (var ingredient in GetRecipe(cocktail)) {
			sodaComposition[(int) ingredient.sodaType] /= totalParts;
		}
		
		return sodaComposition;
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
			case Cocktail.COLA:
				return "Cola POWA™";
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
			case Type.CLUB: 		return new Color("#d3dcdeff");
			case Type.CRANBERRY: 	return new Color("#b01e3bff");
			case Type.GINGER: 		return new Color("#abb04aff");
			case Type.COLA: 		return new Color("#471409ff");
		}
	}
}