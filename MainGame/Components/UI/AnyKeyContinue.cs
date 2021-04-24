using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components.UI {
	[MoonSharpUserData]
	public class AnyKeyContinue : Component {
		public AnyKeyContinue(Entity entity) : base(entity) { }
		[JsonInclude] public string NextScene;
		[JsonInclude] public bool Reset;
		public override IComponent Clone(Entity entity) {
			return new AnyKeyContinue(entity) {
				NextScene = NextScene,
				Reset = Reset
			};
		}

		[MessageHandler]
		public bool OnEnable(Message _) {
			InputSystem.InputManager.AnyKey += OnKeyPress;
			return true;
		}

		[MessageHandler]
		public bool OnDisable(Message _) {
			InputSystem.InputManager.AnyKey -= OnKeyPress;
			return true;
		}

		private void OnKeyPress() {
			if(Reset) Entity.World.RemoveAllScenes();
			else Entity.World.PopScene();
			Entity.World.LoadEntityGroupFromFile(NextScene);
		}
	}
}
