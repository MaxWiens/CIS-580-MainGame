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

		[MessageHandler(10)]
		public bool OnDamage(Message message) {
			if(message.Content.TryGetValue("DamageTiles", out object isTileDamaging)) {
				if((bool)isTileDamaging)
					return true;
			}
			return false;
		}

		[MessageHandler]
		public bool OnDeath(Message message) {
			Entity.World.GetSystem<ParticleSystem>().AddParticles<Particles.BlockBreakParticleGroup>(Entity.GetComponent<Body>().Position + new Microsoft.Xna.Framework.Vector2(8f,8f), 5);
			BreakSound?.Play();
			
			return true;
		}
	}
}
