using MoonSharp.Interpreter;
using ECS;
using MainGame.Systems;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Audio;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class Tile : Component {
		public Tile(Entity entity) : base(entity) { }
		public override IComponent Clone(Entity entity) => new Tile(entity) { 
			BreakSound = BreakSound,
			PlaceSound = PlaceSound
		};

		[JsonInclude] public SoundEffect BreakSound;
		[JsonInclude] public SoundEffect PlaceSound;

		[MessageHandler]
		public bool OnEnable(Message _) {
			var tileSystem = Entity.World.GetSystem<TileSystem>();
			tileSystem.AddTile(Entity.EID, TileSystem.ToTilePosition(Entity.GetComponent<Body>().Position));
			return true;
		}

		[MessageHandler]
		public bool OnPlace(Message _) {
			PlaceSound?.Play();
			return true;
		}

		[MessageHandler]
		public bool OnDisable(Message _) {
			var tileSystem = Entity.World.GetSystem<TileSystem>();
			tileSystem.RemoveTile(TileSystem.ToTilePosition(Entity.GetComponent<Body>().Position));
			return true;
		}

		[MessageHandler]
		public bool OnDeath(Message _) {
			BreakSound?.Play();
			return true;
		}
	}
}
