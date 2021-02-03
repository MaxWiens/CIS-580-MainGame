using System;
using Microsoft.Xna.Framework;

namespace MainGame.Input {
	class SoloBinding : Binding {
		public readonly string BindingName;
		public SoloBinding(string bindingName, Func<InputState, Vector2> reader) : base(reader) {
			BindingName = bindingName;
		}
	}
}
