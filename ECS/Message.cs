using System.Collections.Generic;
using MoonSharp.Interpreter;
namespace ECS {
	public delegate bool MessageHandler(Message message);
	[MoonSharpUserData]
	public class Message {
		public readonly string HandlerName;
		public Dictionary<string,object> Content = new Dictionary<string, object>();
		public Message(string handlerName) {
			HandlerName = handlerName;
			Content = new Dictionary<string, object>();
		}

		public static Message EnabledMessage = new Message("OnEnable");
		public static Message DisabledMessage = new Message("OnDisable");
		public static Message CreatedMessage = new Message("OnCreation");
		public static Message DestroyMessage = new Message("OnDestroy");
	}
}
