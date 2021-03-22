using ECS;
using MoonSharp.Interpreter;
namespace MainGame.Components.UI {
	[MoonSharpUserData]
	public class Volume : Component {
		public Volume(Entity entity) : base(entity) { }

		public override IComponent Clone(Entity entity) => new Volume(entity);
	}
}
