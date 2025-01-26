using Godot;
using System;

public partial class CollisionLayers : Node
{
	public const uint BOTTLES 	= (1 << 0);
	public const uint WORKSPACE 	= (1 << 1);
	public const uint BAR 		= (1 << 2);
	public const uint GLASS 		= (1 << 3);
	public const uint SOLID 		= (1 << 4);

	public const uint FLUID 		= (1 << 5);
	public const uint GRABBABLE 	= (1 << 6);
	public const uint INNER_GLASS = (1 << 7);
	


}
