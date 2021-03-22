using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InputSystem.Serialization {
	class SoloBindingConverter : JsonConverter<SoloBinding> {
		public override SoloBinding Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			JsonDocument document = JsonDocument.ParseValue(ref reader);
			JsonElement root = document.RootElement;
			if(root.ValueKind == JsonValueKind.String) {
				string bindingName = root.GetString();
				document.Dispose();
				return new SoloBinding(bindingName, Binding.GetInputClosure(bindingName));
			}
			throw new JsonException();
		}

		public override void Write(Utf8JsonWriter writer, SoloBinding value, JsonSerializerOptions options)
			=> writer.WriteStringValue(value.BindingName);
	}
}