using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

using diagonstics = System.Diagnostics;

namespace MainGame.Systems {
	using ECS;
	using Components;
	using Input;
	using Util;
	public class PlayerController : UpdateSystem {
		private Vector2 _moveValue;
		private Vector2 _lastMoveInput;
		private float _sprintValue;
		private bool _interacted;
		private bool _breakActivated;

		private Grid _grid;

		private Guid _highlighter;

		private Health _fallbackHealth;
		SoundEffect _placeSfx;
		SoundEffect _breakSfx;
		Texture2D _pixel;

		public PlayerController(ZaWarudo world) : base(world) {
			_moveValue = Vector2.Zero;
			_sprintValue = 1f;
			
		}
		private void OnContentLoad() {
			_placeSfx = world.Content.Load<SoundEffect>(@"Sfx\place");
			_breakSfx = world.Content.Load<SoundEffect>(@"Sfx\break");
			_pixel = world.Content.Load<Texture2D>(@"Textures\pixel");
			_highlighter = world.MakeEntity(
				new Sprite() { Texture = _pixel, Scale = new Vector2(16), SourceRectangle = new Rectangle(0, 0, 1, 1), Albedo = new Color(255, 255, 255, 25) },
				new Transform2D()
			);
		}

		private void OnEnable() {
			


			world.InputManager.AddListener("Move", OnMove);
			world.InputManager.AddListener("Interact", OnInteract);
			world.InputManager.AddListener("Sprint", OnSprint);
			world.InputManager.AddListener("Break", OnBreak);
		}

		private void OnDisable() {
			world.InputManager.RemoveListener("Move", OnMove);
			world.InputManager.RemoveListener("Interact", OnInteract);
			world.InputManager.RemoveListener("Sprint", OnSprint);
			world.InputManager.RemoveListener("Break", OnBreak);
		}

		public override void Update(float deltaTime) {
			var eids = world.GetEntitiesWithComponent<PlayerControl>().Keys;
			var moverMap = world.GetEntitiesWithComponent<Mover>();
			if(_grid==null) _grid = world.GetSystem<Grid>();
			foreach(var eid in eids) {
				Transform2D pos = world.GetComponent<Transform2D>(eid);
				ref RigidBody rb = ref world.GetComponent<RigidBody>(eid);
				rb.Velocity = _moveValue * 100f * _sprintValue;
				ref Transform2D highlighterTrans = ref world.GetComponent<Transform2D>(_highlighter);
				ref BlockPlacer blockPlacer = ref world.GetComponent<BlockPlacer>(eid);
				Point potentialPlace = Grid.ToGridPosition(pos.Position)+Vector2.Normalize(_lastMoveInput).ToPoint();
				highlighterTrans.Position = potentialPlace.ToVector2() * 16;
				if(_interacted && !_grid.IsCellFilled(potentialPlace)) {
					
					Guid blockeid = world.LoadEntities(blockPlacer.BlockPrefabPath)[0];
					ref Transform2D blocktransform = ref world.GetComponent<Transform2D>(blockeid);
					ref Sprite sprite = ref world.GetComponent<Sprite>(blockeid);
					blocktransform.Position = potentialPlace.ToVector2() * 16;
					_interacted = false;
					_placeSfx.Play();
				} else if(_breakActivated && _grid.GetEntityAt(potentialPlace, out Guid blockeid)) {
					ref Health h = ref world.TryGetComponent(blockeid, ref _fallbackHealth, out bool isSuccessful);
					if(isSuccessful) {
						h.Value = 0;
						_breakSfx.Play();
						_breakActivated = false;
					}
				}
			}
		}

		private void OnMove(Vector2 value) {
			if(value == Vector2.Zero)
				_lastMoveInput = _moveValue;
			else
				_lastMoveInput = new Vector2(value.X, -value.Y);
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

		private void OnBreak(Vector2 value) {
			_breakActivated = value.AsBool();
		}
	}
}
