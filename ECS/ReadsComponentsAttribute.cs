using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.ECS {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	class ReadsComponentsAttribute : Attribute {
		public readonly Type[] Reads;
		public ReadsComponentsAttribute(params Type[] components) {
			Reads = components;
		}
	}
}
