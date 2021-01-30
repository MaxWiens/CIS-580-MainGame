using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.ECS {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	class ReadsComponentsAttribute : Attribute {
		public readonly Type[] Reads;
		public ReadsComponentsAttribute(params Type[] components) {
			foreach(Type t in components) {
				if(!typeof(Component).IsAssignableFrom(t))
					throw new ArgumentException("Type {t} does not inharet Component!");
			}
			Reads = components;
		}
	}
}
