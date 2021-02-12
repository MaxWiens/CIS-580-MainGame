using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.Systems {
	using ECS;
	using Components;
	public class Destruction : UpdateSystem {
		public Destruction(ZaWarudo world) : base(world) { }
		private Inventory _fallbackInventory;
		private Drops _fallbackDrops;
		public override void Update(float deltaTime) {
			var entitites = world.GetEntitiesWithComponent<Health>();
			if(entitites != null) {
				var eids = entitites.Keys;
				Health h;
				foreach(var eid in eids) {
					h = entitites[eid];
					if(h.Value <= 0) {
						ref Inventory i = ref world.TryGetComponent(eid, ref _fallbackInventory, out bool isSuccessful);
						if(isSuccessful) {

						}

						ref Drops d = ref world.TryGetComponent(eid, ref _fallbackDrops, out isSuccessful);
						if(isSuccessful) {
							foreach(string s in d.Items) {
								world.LoadEntities(s);
							}
						}
						world.DestroyEntity(eid);
					}
				}
			}
		}
	}
}
