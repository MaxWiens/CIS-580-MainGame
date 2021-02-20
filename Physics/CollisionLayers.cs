using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
namespace MainGame.Physics {
	[Flags]
	public enum Layer : ushort {
		None        = 0,
		General     = 0b0000_0000_0000_0001,
		Creature    = 0b0000_0000_0000_0010,
		Enviornment = 0b0000_0000_0000_0100,
		Item        = 0b0000_0000_0000_1000,
	}

	public static class CollisionLayers {
		private static readonly Dictionary<Layer, Layer> _collisionMap = new Dictionary<Layer, Layer>() {
			{Layer.None, Layer.None},
			{Layer.General,     (Layer)0b1111_1111_1111_1111},
			{Layer.Creature,    (Layer)0b1111_1111_1111_0111},
			{Layer.Enviornment, (Layer)0b0000_0000_0000_0011},
			{Layer.Item,        (Layer)0b0000_0000_0000_1111},
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool CanCollide(this Layer layer, Layer other)
			=> Layer.None != (_collisionMap[layer] & other);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool CantCollide(this Layer layer, Layer other)
			=> Layer.None == (_collisionMap[layer] & other);
	}
}
