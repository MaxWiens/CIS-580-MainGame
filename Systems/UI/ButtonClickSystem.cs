using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using ECS;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Collision;
using PhysicsWorld = tainicom.Aether.Physics2D.Dynamics.World;
using MoonSharp.Interpreter;
namespace MainGame.Systems {
	using ECS;
	using ECS.S;
	using Components;
	
	public class ButtonClickSystem : System, IUpdateable {
		private readonly PhysicsWorld _physicsWorld;
		private readonly MegaDungeonGame _game;
		public ButtonClickSystem(World world, PhysicsWorld physicsWorld, MegaDungeonGame game) : base(world) {
			_physicsWorld = physicsWorld;
			_game = game;
		}
		private Collections.IRefMap<Guid, Components.UI.Button> _buttonMap;
		private MouseState _prevstate;
		public void Update(float deltaTime) {
			MouseState state = Mouse.GetState();
			_buttonMap = world.GetEntitiesWithComponent<Components.UI.Button>();
			foreach(Guid eid in _buttonMap.Keys) {
				_buttonMap[eid].IsPressed = false;
			}
			if(state.LeftButton == ButtonState.Pressed && _prevstate.LeftButton != ButtonState.Pressed) {
				Point mousePos = state.Position;
				Vector2 v = new Vector2(mousePos.X * _game.Resolution.X / _game.Graphics.PreferredBackBufferWidth, mousePos.Y * _game.Resolution.Y / _game.Graphics.PreferredBackBufferHeight);
				
				_physicsWorld.QueryAABB(Handler, new AABB(v, 1f, 1f));
			}
			_prevstate = state;
		}
		Components.UI.Button _buttonFallback;
		private bool Handler(Fixture fixture) {
			ref Components.UI.Button b = ref _buttonMap.TryGetValue((Guid)fixture.Body.Tag, ref _buttonFallback, out bool isSuccessful);
			b.IsPressed = isSuccessful;
			b.ClickEvent.Call(b.ClickEvent.Globals["OnClick"], "hello");
			return true;
		}
	}
}
