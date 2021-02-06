using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MainGame.Systems {
	using ECS;
	using Components;
	using Input;
	public class PlayerController : UpdateSystem {
		private Vector2 _moveValue;
		private float _sprintValue;
		private bool _interacted;

		public PlayerController(ZaWarudo world) : base(world) {
			_moveValue = Vector2.Zero;
			_sprintValue = 1f;
		}

		private void OnEnable() {
			world.InputManager.AddListener("Move", OnMove);
			world.InputManager.AddListener("Interact", OnInteract);
			world.InputManager.AddListener("Sprint", OnSprint);
		}

		private void OnDisable() {
			world.InputManager.RemoveListener("Move", OnMove);
			world.InputManager.RemoveListener("Interact", OnInteract);
			world.InputManager.RemoveListener("Sprint", OnSprint);
		}

		public override void Update(float deltaTime) {
			var eids = world.GetEntitiesWithComponent<PlayerControl>().Keys;

			foreach(var eid in eids) {
				ref Transform2D pos = ref world.GetComponent<Transform2D>(eid);
				pos.Position += _moveValue * 100f * deltaTime * _sprintValue;
				ref BlockPlacer blockPlacer = ref world.GetComponent<BlockPlacer>(eid);
				if(_interacted) {
					Guid blockeid = world.LoadEntities(blockPlacer.BallPrefabPath)[0];
					ref Transform2D blockpos = ref world.GetComponent<Transform2D>(blockeid);
					ref Sprite sprite = ref world.GetComponent<Sprite>(blockeid);
					sprite.Texture = world.Content.Load<Texture2D>(sprite.TextureName);
					blockpos = pos;
					_interacted = false;
				}
				
			}
		}

		private void OnMove(Vector2 value) {
			_moveValue = new Vector2(value.X, -value.Y);
		}

		private void OnSprint(Vector2 value) {
			if(value.AsBool())
				_sprintValue = 2f;
			else {
				_sprintValue = 1f;
			}
		}

		private void OnInteract(Vector2 value) {
			_interacted = value.AsBool();
		}
	}
}
