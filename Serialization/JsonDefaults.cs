using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace MainGame.Serialization {
	public static class JsonDefaults {
		public static readonly JsonSerializerOptions Options = new JsonSerializerOptions() {
			IgnoreNullValues = false,
			Converters = {
				new PointConverter(),
				new Vector2Converter(),
				new Vector3Converter(),
				new ColorConverter(),
				new SoloBindingConverter(),
				new CompositeBindingConverter(),
			}
		};
	}
}
