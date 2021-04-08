using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using MoonSharp.Interpreter;
namespace MainGame.Serialization.MoonSharp {
	public class ScriptConverter : JsonConverter<Script> {
		private readonly DynValue _game;
		private readonly DynValue _physicsWorld;
		private readonly DynValue _ecsWorld;
		private readonly DynValue _soundEffect;
		private readonly DynValue _mediaPlayer;
		private readonly SoundEffect _defaultSoundEffect;
		public ScriptConverter(MainGame game, tainicom.Aether.Physics2D.Dynamics.World physicsWorld, ECS.World ecsWorld) {
			_defaultSoundEffect = game.Content.Load<SoundEffect>(@"Sfx\place");
			UserData.RegisterType<MainGame>();
			UserData.RegisterType<tainicom.Aether.Physics2D.Dynamics.World>();
			UserData.RegisterType<ECS.World>();
			UserData.RegisterType<Guid>();
			UserData.RegisterType<List<ECS.Entity>>();
			UserData.RegisterType<Dictionary<string, object>>();
			UserData.RegisterType<ECS.Message>();
			
			_game = UserData.Create(game);
			_physicsWorld = UserData.Create(physicsWorld);
			_ecsWorld = UserData.Create(ecsWorld);
		}
		public override Script Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			Script s = new Script();
			s.Globals["Debug"] = (Action<string>)Debug;
			s.Globals["SetVolume"] = (Action<float>)SetVolume;
			s.Globals["GetVolume"] = (Func<float>)GetVolume;
			s.Globals["PlayTestSound"] = (Action)PlayTestSound;
			s.Globals["Game"] = _game;
			s.Globals["ECS"] = _ecsWorld;
			s.Globals["Physics"] = _physicsWorld;
			s.DoString(reader.GetString());
			return s;
		}

		public override void Write(Utf8JsonWriter writer, Script value, JsonSerializerOptions options) {
			throw new NotImplementedException();
		}

		private static void Debug(string s) {
			System.Diagnostics.Debug.WriteLine(s);
		}

		private static void SetVolume(float f) {
			if(f > 1f) f = 1f;
			else if(f < 0f) f = 0f;
			MediaPlayer.Volume = f;
			SoundEffect.MasterVolume = f;
		}
		private static float GetVolume() {
			return MediaPlayer.Volume;
		}

		private void PlayTestSound() {
			_defaultSoundEffect.Play();
		}
	}
}