using System;
using System.Collections.Generic;
using System.Text;

namespace ECS {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	class ModifiesComponentsAttribute : Attribute {
		public readonly Type[] Modifies;
		public ModifiesComponentsAttribute(params Type[] components) {
			Modifies = components;
		}
	}
}
