using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class Exploding : Component {
		[JsonInclude] public float Timer;
		[JsonInclude] public bool ExplodesOnContact;
		[JsonInclude] public string ExplosionPrefabPath;

		public Exploding(Entity entity) : base(entity) { }

		public override IComponent Clone(Entity entity) {
			return new Exploding(entity) {
				Timer = Timer,
				ExplodesOnContact = ExplodesOnContact,
				ExplosionPrefabPath = ExplosionPrefabPath
			};
		}
	}
}
