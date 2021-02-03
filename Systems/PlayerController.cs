using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MainGame.Systems {
	using ECS;
	using Components;
	using Input;
	public class PlayerController : UpdateSystem {
		private Vector2 _moveValue;
		private float _sprintValue;

		public PlayerController(ZaWarudo world) : base(world) {
			_moveValue = Vector2.Zero;
			_sprintValue = 1f;
		}

		private void OnEnable() {
			world.InputManager.AddListener("Move", OnMove);
			world.InputManager.AddListener("Interact", OnInteract);
		}

		private void OnDisable() {
			world.InputManager.RemoveListener("Move", OnMove);
			world.InputManager.AddListener("Interact", OnInteract);
		}

		public override void Update(float deltaTime) {
			var entityComponents = world.GetEntitiesWithComponent<PlayerControl>();
			Position2D pos;
			foreach(KeyValuePair<ulong, Component> kvp in entityComponents) {
				pos = world.GetComponent<Position2D>(kvp.Key);
				;
				pos.Position += _moveValue * 100f * deltaTime * _sprintValue;
			}
		}

		private void OnMove(Vector2 value) {
			_moveValue = new Vector2(value.X, -value.Y);
		}

		private void OnInteract(Vector2 value) {
			if(value != Vector2.Zero)
				_sprintValue = 2f;
			else
				_sprintValue = 1f;
		}
	}
}
