using System;

namespace ECS {
	[AttributeUsage(validOn: AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class MessageHandlerAttribute : Attribute {}
}
