using MoonSharp.Interpreter;
using ECS;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class Item : Component {
		[JsonInclude] public ID Holder;
		public Item(Entity entity) : base(entity) { }
		public override IComponent Clone(Entity entity) => new Item(Entity);
	}
}
