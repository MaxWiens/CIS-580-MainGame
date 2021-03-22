using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class Drops : Component {
		[JsonInclude] public string[] Items;

		public Drops(Entity entity) : base(entity) { }

		[MessageHandler]
		public bool OnDeath(Message message) {
			
			return true;
		}

		public override IComponent Clone(Entity entity) {
			string[] items = new string[Items.Length];
			Items.CopyTo(items, 0);
			return new Drops(entity) {
				Items = items
			};
		}
	}
}
