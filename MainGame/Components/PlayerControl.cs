using MoonSharp.Interpreter;
using Microsoft.Xna.Framework;
using ECS;
using InputSystem;
using System;
using tainicom.Aether.Physics2D.Dynamics;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class PlayerControl : Component {

		public PlayerControl(Entity entity) : base(entity) { }

		public override IComponent Clone(Entity entity) => new PlayerControl(entity);


		[MessageHandler]
		public bool OnDeath(Message _) {
			Entity.World.RemoveAllScenes();
			Entity.World.LoadEntityGroupFromFile("Assets/Scenes/MainMenuScene.json");
			return false;
		}

		//[MessageHandler]
		//public bool OnCollision(Message message) {
		//	if(((Entity)((Fixture)message.Content["Other"]).Body.Tag).TryGetComponent(out Components.AI.BasicEnemyAI enemy)) {
		//		enemy.Entity.SendMessage(new Message("OnDamage") { Content = { { "Total", 1 } } });
		//	}
		//	return true;
		//}

		/*
		[MessageHandler]
		public bool OnEnable(Message message) {
			InputManager.AddListener("Pause", OnPause);
			InputManager.AddListener("Move", OnMove);
			InputManager.AddListener("Interact", OnInteract);
			InputManager.AddListener("Sprint", OnSprint);
			InputManager.AddListener("Break", OnBreak);
			return true;
		}

		[MessageHandler]
		public bool OnDisable(Message message) {
			InputManager.RemoveListener("Pause", OnPause);
			InputManager.RemoveListener("Move", OnMove);
			InputManager.RemoveListener("Interact", OnInteract);
			InputManager.RemoveListener("Sprint", OnSprint);
			InputManager.RemoveListener("Break", OnBreak);
			return true;
		}


		//public void Update(float deltaTime) {
		//	var eids = .Keys;
		//	if(_grid==null) _grid = World.GetSystem<TileSystem>();
		//	foreach(var eid in eids) {
		//		Body pos = World.GetComponent<Body>(eid);
		//		//global::System.Diagnostics.Debug.WriteLine(pos.Position.ToString());
		//		ref Body rb = ref World.GetComponent<Body>(eid);
		//		rb.LinearVelocity = _moveValue * 100f * _sprintValue;
		//		//ref Body highlighterTrans = ref World.GetComponent<Body>(_highlighter);
		//		ref BlockPlacer blockPlacer = ref World.GetComponent<BlockPlacer>(eid);
		//		Point potentialPlace = TileSystem.ToTilePosition(pos.Position)+Vector2.Normalize(_lastMoveInput).ToPoint();
		//		//highlighterTrans.Position = potentialPlace.ToVector2() * 16;
		//		if(_interacted && !_grid.IsCellFilled(potentialPlace)) {
		//			Guid blockeid = World.CloneEntityGroup(blockPlacer.BlockPrefabPath)[0];
		//			ref Body blockBody = ref World.GetComponent<Body>(blockeid);
		//			ref Sprite sprite = ref World.GetComponent<Sprite>(blockeid);
		//			blockBody.Position = potentialPlace.ToVector2() * 16;
		//			_interacted = false;
		//			_placeSfx.Play();
		//		} else if(_breakActivated && _grid.GetEntityAt(potentialPlace, out Guid blockeid)) {
		//			ref Health h = ref world.TryGetComponent(blockeid, ref _fallbackHealth, out bool isSuccessful);
		//			if(isSuccessful) {
		//				h.Value = 0;
		//				_breakSfx.Play();
		//				_breakActivated = false;
		//			}
		//		}
		//	}
		//}

		private void OnPause(Vector2 obj) {
			if(obj.AsBool()) {
				Scene top = Entity.World.TopScene;
				if(top.Name == "Gameplay") {
					Entity.World.LoadEntityGroupFromFile("Assets/Scenes/PauseMenuScene.json");
					Entity.World.DeltaTimeScale = 0f;
				} else if(top.Name == "Pause") {
					Entity.World.PopScene();
					Entity.World.DeltaTimeScale = 1f;
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
		*/
	}
}
