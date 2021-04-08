using Microsoft.Xna.Framework;
using ECS;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Collision;
using PhysicsWorld = tainicom.Aether.Physics2D.Dynamics.World;
using MoonSharp.Interpreter;
using InputSystem;
using MainGame.Components.UI;
namespace MainGame.Systems.UI {
	[MoonSharpUserData]
	public class ButtonClickSystem : BaseSystem {
		private readonly PhysicsWorld _physicsWorld;
		private readonly MainGame _game;

		public ButtonClickSystem(ECS.World world, MainGame game, PhysicsWorld physicsWorld) : base(world) {
			_game = game;
			_physicsWorld = physicsWorld;
		}

		private void OnClick(Vector2 position) {
			_physicsWorld.QueryAABB(Handler, new AABB(_game.ScreenToWorld(position), 1, 1));
			//Vector2 v = new Vector2(mousePos.X * _game.Resolution.X / _game.Graphics.PreferredBackBufferWidth, mousePos.Y * _game.Resolution.Y / _game.Graphics.PreferredBackBufferHeight);
		}

		private bool Handler(Fixture fixture) {
			if(((Entity)fixture.Body.Tag).TryGetComponent(out Button b)) {
				b.ClickEvent.Call(b.ClickEvent.Globals["OnClick"]);
			}
			return true;
		}
	}
}
