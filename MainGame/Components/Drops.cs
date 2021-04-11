using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using MoonSharp.Interpreter;
using ECS;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class Drops : Component {
		[JsonInclude] public string[] Items;
		private bool hasDropped;
		public Drops(Entity entity) : base(entity) { }

		[MessageHandler(10)]
		public bool OnDeath(Message message) {
			if(!hasDropped) {
				hasDropped = true;
				if(Entity.TryGetComponent(out Body thisBody)) {
					foreach(string s in Items) {
						var entities = Entity.World.CloneEntityGroup(s);
						foreach(Entity e in entities) {
							if(e.TryGetComponent<Body>(out Body b)) {
								b.Position = thisBody.Position;
							}
							e.Enable();
						}
					}
				} else {
					foreach(string s in Items) {
						var entities = Entity.World.CloneEntityGroup(s);
						foreach(Entity e in entities) {
							e.Enable();
						}
					}
				}
			}
			return true;
		}

		public override IComponent Clone(Entity entity) {
			string[] items = new string[Items.Length];
			Items.CopyTo(items, 0);
			return new Drops(entity) {
				Items = items
			};
		}
	}
}
