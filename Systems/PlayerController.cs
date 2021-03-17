using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using diagonstics = System.Diagnostics;

namespace MainGame.Systems {
	using ECS;
	using ECS.S;
	using Components;
	using Input;
	using Util;
	public class PlayerController : System, IUpdateable, IEnableHandler, IDisableHandler {
		private Vector2 _moveValue;
		private Vector2 _lastMoveInput;
		private float _sprintValue;
		private bool _interacted;
		private bool _breakActivated;

		private TileSystem _grid;

		//private Guid _highlighter;

		private Health _fallbackHealth;
		SoundEffect _placeSfx;
		SoundEffect _breakSfx;
		Texture2D _pixel;

		public PlayerController(GameWorld world, ContentManager content) : base(world) {
			_placeSfx = content.Load<SoundEffect>(@"Sfx\place");
			_breakSfx = content.Load<SoundEffect>(@"Sfx\break");
			_pixel = content.Load<Texture2D>(@"Textures\pixel");
			//_highlighter = world.MakeEntity(
			//	new Sprite() { Texture = _pixel, Scale = new Vector2(16), SourceRectangle = new Rectangle(0, 0, 1, 1), Albedo = new Color(255, 255, 255, 25) },
			//	new Body()
			//);
			_moveValue = Vector2.Zero;
			_sprintValue = 1f;
		}

		public void OnEnable() {
			InputManager.AddListener("Pause", OnPause);
			InputManager.AddListener("Move", OnMove);
			InputManager.AddListener("Interact", OnInteract);
			InputManager.AddListener("Sprint", OnSprint);
			InputManager.AddListener("Break", OnBreak);
		}

		public void OnDisable() {
			InputManager.RemoveListener("Pause", OnPause);
			InputManager.RemoveListener("Move", OnMove);
			InputManager.RemoveListener("Interact", OnInteract);
			InputManager.RemoveListener("Sprint", OnSprint);
			InputManager.RemoveListener("Break", OnBreak);
		}

		public void Update(float deltaTime) {
			var eids = world.GetEntitiesWithComponent<PlayerControl>().Keys;
			if(_grid==null) _grid = world.GetSystem<TileSystem>();
			foreach(var eid in eids) {
				Body pos = world.GetComponent<Body>(eid);
				ref Body rb = ref world.GetComponent<Body>(eid);
				rb.LinearVelocity = _moveValue * 100f * _sprintValue;
				//ref Body highlighterTrans = ref world.GetComponent<Body>(_highlighter);
				ref BlockPlacer blockPlacer = ref world.GetComponent<BlockPlacer>(eid);
				Point potentialPlace = TileSystem.ToTilePosition(pos.Position)+Vector2.Normalize(_lastMoveInput).ToPoint();
				//highlighterTrans.Position = potentialPlace.ToVector2() * 16;
				if(_interacted && !_grid.IsCellFilled(potentialPlace)) {
					Guid blockeid = world.LoadEntityGroupFromFile(blockPlacer.BlockPrefabPath, Guid.Empty)[0];
					ref Body blocktransform = ref world.GetComponent<Body>(blockeid);
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

		private void OnPause(Vector2 obj) {
			if(obj.AsBool()) {
				Guid top = world.TopScene;
				if(top == Guid.Parse("FA60209B-C86C-40E1-BB37-C8121C728F45")) {
					world.LoadEntityGroupFromFile("Assets/Scenes/PauseMenuScene.json");
					world.DeltaTimeScale = 0f;
				}else if(top == Guid.Parse("2689D0B7-BD3C-4DBA-9660-5349E0701878")) {
					world.PopScene();
					world.DeltaTimeScale = 1f;
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
