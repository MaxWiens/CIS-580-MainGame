using Microsoft.Xna.Framework;
using ECS;
using MoonSharp.Interpreter;
using InputSystem;
using MainGame.Components.UI;
namespace MainGame.Systems.UI {
	[MoonSharpUserData]
	public class ElementRepositionSystem : BaseSystem {
		private readonly MainGame _game;

		public ElementRepositionSystem(World world, MainGame game) : base(world) {
			_game = game;
			_game.WindowResized += OnWindowResize;
		}

		private void OnWindowResize(Point newDimentions) {
			var v = World.GetEntitiesWith<Element>();
			foreach(Element e in v.Values) {
				RepositionElement(e);
			}
		}

		public void RepositionElement(Element e) {
			Components.Body b = e.Entity.GetComponent<Components.Body>();
			b.Position = new Vector2((e.Anchor.X * _game.MainCamera.Resolution.X) + e.Offset.X, (e.Anchor.Y * _game.MainCamera.Resolution.Y) + e.Offset.Y);
		}
	}
}
