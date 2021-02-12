using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame {
	public static class CollisionLayers {
		public static Layers[] CollidesWith = LayerConverter(new Dictionary<string, Layers> {
			{"General",     (Layers)0b1111_1111_1111_1111},
			{"Creature",    (Layers)0b1111_1111_1111_1111},
			{"Envionrment", (Layers)0b0000_0000_0000_0011},
			{"Item",        (Layers)0b0000_0000_0000_0111},
		});
		[Flags]
		public enum Layers : ushort {
			General     = 0b0000_0000_0000_0001,
			Creature    = 0b0000_0000_0000_0010,
			Enviornment = 0b0000_0000_0000_0100,
			Item        = 0b0000_0000_0000_1000,
		}

		private static Layers[] LayerConverter(Dictionary<string, Layers> dict) {
			Layers[] layers = new Layers[16];
			List<string> enumValueNames = new List<string>(Enum.GetNames(typeof(Layers)));
			if(enumValueNames.Count > 16) throw new Exception("Too many layers");
			foreach(var kvp in dict)
				layers[enumValueNames.IndexOf(kvp.Key)] = (Layers)kvp.Value;
			return layers;
		}
	}
}
