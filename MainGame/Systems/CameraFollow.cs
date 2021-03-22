using Microsoft.Xna.Framework;
using System;
using MoonSharp.Interpreter;

namespace MainGame.Systems {
	using ECS;
	using Components;
	[MoonSharpUserData]
	public class CameraFollow : BaseSystem, IUpdateable {
		private MainGame _game;
		public CameraFollow(World world, MainGame game) : base(world) {
			_game = game;
		}
		public float Strength = 10f;
		public float SnappingDistance = 1.5f;
		public void Update(float deltaTime) {
			Entity e = World.GetEntity("PlayerCharacter");
			if(e != null) {
				Vector2 dif;
				Body targetBody = e.GetComponent<Body>();
				if(_game.MainCamera.Position != targetBody.Position) {
					dif = targetBody.Position - _game.MainCamera.Position;
					if(dif.Length() < SnappingDistance) {
						_game.MainCamera.Position = targetBody.Position;
					} else {
						_game.MainCamera.Position += dif * Math.Clamp(Strength * deltaTime, 0f, 1f);
					}
				}
			}
		}
	}
}
