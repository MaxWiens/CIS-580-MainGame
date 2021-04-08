using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using diagonstics = System.Diagnostics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Collision;
using MoonSharp.Interpreter;
using InputSystem;
namespace MainGame.Systems {
	using ECS;
	using Components;
	using Util;
	[MoonSharpUserData]
	public class PlayerController : BaseSystem, IUpdateable {
		private Vector2 _moveValue;
		private Vector2 _lastMoveInput;
		private float _sprintValue;
		private bool _interacted;
		private bool _breakActivated;

		private TileSystem _grid;

		//private Guid _highlighter;

		SoundEffect _placeSfx;
		SoundEffect _breakSfx;
		Texture2D _pixel;

		private readonly MainGame _game;
		private readonly PhysicsSystem _physicsSystem;

		public PlayerController(World world, ContentManager content, MainGame game, PhysicsSystem physicsSystem) : base(world) {
			_game = game;
			_physicsSystem = physicsSystem;
			_placeSfx = content.Load<SoundEffect>(@"Sfx\place");
			_breakSfx = content.Load<SoundEffect>(@"Sfx\break");
			_pixel = content.Load<Texture2D>(@"Textures\pixel");
			//_highlighter = world.MakeEntity(
			//	new Sprite() { Texture = _pixel, Scale = new Vector2(16), SourceRectangle = new Rectangle(0, 0, 1, 1), Albedo = new Color(255, 255, 255, 25) },
			//	new Body()
			//);
			_moveValue = Vector2.Zero;
			_sprintValue = 1f;

			InputManager.AddListener("Pause", OnPause);
			InputManager.AddListener("Move", OnMove);
			InputManager.AddListener("Interact", OnInteract);
			InputManager.AddListener("Sprint", OnSprint);
			InputManager.AddListener("Break", OnBreak);
			InputManager.MousePressed += OnClick;
		}

		public void Update(float deltaTime) {
			if(_grid == null)
				_grid = World.GetSystem<TileSystem>();

			if(deltaTime <= 0) return;

			foreach(var entity in World.GetEntitiesWith<PlayerControl>().Keys) {
				Body body = entity.GetComponent<Body>();
				body.LinearVelocity = _moveValue * 100f * _sprintValue;
				BlockPlacer blockPlacer = entity.GetComponent<BlockPlacer>();
				Point potentialPlace = TileSystem.ToTilePosition(body.Position) + Vector2.Normalize(_lastMoveInput).ToPoint();
				//highlighterTrans.Position = potentialPlace.ToVector2() * 16;
				if(_interacted) {
					Entity dam = World.CloneEntityGroup("Assets/Prefabs/DamageRectangle.json")[0];
					Body blockBody = dam.GetComponent<Body>();
					blockBody.Position = (body.Position + _lastMoveInput*16);
					World.GetSystem<ParticleSystem>().AddParticles<Particles.AttackParticleGroup>(blockBody.Position, 12);
					_breakSfx.Play();
				} else
				//if(_interacted && !_grid.IsCellFilled(potentialPlace)) {
				//	Entity placedBlock = World.CloneEntityGroup(blockPlacer.BlockPrefabPath)[0];
				//	Body blockBody = placedBlock.GetComponent<Body>();
				//	blockBody.Position = potentialPlace.ToVector2() * TileSystem.TILE_SIZE;
				//	placedBlock.Enable();
				//	placedBlock.SendMessage(new Message("OnPlace"));
				//} else 
				
				if(_breakActivated && _grid.GetEntityAt(potentialPlace, out Guid blockeid)) {
					World.GetEntity(blockeid).SendMessage(new Message("OnDamage") { Content = { { "Total", 100 } } });
				}
				_interacted = false;
				_breakActivated = false;
			}
		}

		private void OnPause(Vector2 obj) {
			if(obj.AsBool()) {
				Scene top = World.TopScene;
				if(top.Name == "Gameplay") {
					World.LoadEntityGroupFromFile("Assets/Scenes/PauseMenuScene.json");
					World.DeltaTimeScale = 0f;
				}else if(top.Name == "PauseMenu") {
					World.PopScene();
					World.DeltaTimeScale = 1f;
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
			else
				_sprintValue = 1f;
		}

		private void OnInteract(Vector2 value) {
			_interacted = value.AsBool();
		}

		private void OnBreak(Vector2 value) {
			_breakActivated = value.AsBool();
		}

		private void OnClick(Vector2 position) {
			_physicsSystem.PhysicsWorld.QueryAABB(Handler, new AABB(_game.ScreenToWorld(position), 1, 1));
			//Vector2 v = new Vector2(mousePos.X * _game.Resolution.X / _game.Graphics.PreferredBackBufferWidth, mousePos.Y * _game.Resolution.Y / _game.Graphics.PreferredBackBufferHeight);
		}

		private bool Handler(Fixture fixture) {
			((Entity)fixture.Body.Tag).SendMessage(new Message("OnClick"));
			return true;
		}
	}
}
