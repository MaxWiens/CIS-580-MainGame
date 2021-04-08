using System;

namespace ECS {
	[AttributeUsage(validOn: AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class MessageHandlerAttribute : Attribute {
		public readonly uint Priority;
		public MessageHandlerAttribute(uint priority = uint.MaxValue) {
			Priority = priority;
		}
	}
}
