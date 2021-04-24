using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components.AI {
	[MoonSharpUserData]
	public class Boss : Component {
		[JsonInclude] public SoundEffect Growl;
		public float GrowlTimer;
		public Boss(Entity entity) : base(entity) { }
		
		[MessageHandler]
		public bool OnEnable(Message _) {
			Growl.Play();
			GrowlTimer += Util.Rand.Float()*3f+3f;
			return true;
		}

		[MessageHandler(10)]
		public bool OnDeath(Message message) {
			Entity.World.GetSystem<Systems.UI.HealthBarSystem>().AddKill(true);
			Entity.World.RemoveAllScenes();
			Entity.World.LoadEntityGroupFromFile("Assets/Scenes/WinScene.json");
			return false;
		}

		public override IComponent Clone(Entity entity) => new Boss(entity) {
			Growl = Growl,
			GrowlTimer = GrowlTimer
		};
	}
}
