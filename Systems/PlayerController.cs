using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MainGame.Systems {
	using ECS;
	using Components;
	using Input;
	public class PlayerController : UpdateSystem {
		private Vector2 _moveValue;
		private Vector2 _jumpValue;
		private float _sprintValue;

		public PlayerController(ZaWarudo world) : base(world) {
			_moveValue = Vector2.Zero;
			_jumpValue = Vector2.Zero;
			_sprintValue = 1f;
		}

		private void OnEnable() {
			world.InputManager.AddListener("Move", OnMove);
			world.InputManager.AddListener("Interact", OnInteract);
			world.InputManager.AddListener("Jump", OnJump);
		}

		private void OnDisable() {
			world.InputManager.RemoveListener("Move", OnMove);
			world.InputManager.RemoveListener("Interact", OnInteract);
			world.InputManager.RemoveListener("Jump", OnJump);
		}

		public override void Update(float deltaTime) {
			var eids = world.GetEntitiesWithComponent<PlayerControl>().Keys;

			foreach(var eid in eids) {
				ref Position2D pos = ref world.GetComponent<Position2D>(eid);
				;
				pos.Position += _moveValue * 100f * deltaTime * _sprintValue;
				if(_jumpValue != Vector2.Zero) {
					pos.Position += _jumpValue;
					_jumpValue = Vector2.Zero;
				}
			}
		}

		private void OnMove(Vector2 value) {
			_moveValue = new Vector2(value.X, -value.Y);
		}

		private void OnJump(Vector2 value) {
			_jumpValue = new Vector2(0f, -value.Y) * 10f;
		}

		private void OnInteract(Vector2 value) {
			if(value != Vector2.Zero)
				_sprintValue = 2f;
			else
				_sprintValue = 1f;
		}
	}
}
