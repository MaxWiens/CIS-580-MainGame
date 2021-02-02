using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MainGame.Components {
	using JsonConverters;
	using System.Text.Json;

	//[JsonConverter(typeof(Position2DConverter)]
	public class Position2D : ECS.Component {
		//[JsonConverter(typeof(Vector2Converter))]
		[JsonInclude]
		public string t = "blob";
		[JsonInclude]
		public Vector2 Position = new Vector2();
	}

	//class Position2DConverter : JsonConverter<Position2D> {
	//	public override Position2D Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			
	//		//reader.GetVector2();
	//	}

	//	public override void Write(Utf8JsonWriter writer, Position2D value, JsonSerializerOptions options) {
			
	//	}
	//}
}
