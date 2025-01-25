using Godot;
using System;

public partial class CollisionLayers : Node
{
	public const int BOTTLES 	= (1 << 0);
	public const int WORKSPACE 	= (1 << 1);
	public const int BAR 		= (1 << 2);
	public const int GLASS 		= (1 << 3);
	public const int SOLID 		= (1 << 4);

	public const int FLUID 		= (1 << 5);
	public const int GRABBABLE 	= (1 << 6);


}
