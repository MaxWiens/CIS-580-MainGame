using System;
using Microsoft.Xna.Framework;

namespace InputSystem {
	using BindingReader = Func<InputState, Vector2>;

	public class CompositeBinding : Binding {
		public readonly string[] BindingNames;
		public CompositeBinding(string[] bindingNames, BindingReader reader) : base(reader) {
			BindingNames = bindingNames;
		}
	}
}
