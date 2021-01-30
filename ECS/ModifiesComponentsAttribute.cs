using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.ECS {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	class ModifiesComponentsAttribute : Attribute {
		public readonly Type[] Modifies;
		public ModifiesComponentsAttribute(params Type[] components) {
			foreach(Type t in components) {
				if(!typeof(Component).IsAssignableFrom(t))
					throw new ArgumentException("Type {t} does not inharet Component!");
			}
			Modifies = components;
		}
	}
}
