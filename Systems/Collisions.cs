using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Linq;

namespace MainGame.Systems {
	using ECS;
	using Components;

	public class Collisions : UpdateSystem {
		public Collisions(ZaWarudo world) : base(world) {
			
		}

		public override void Update(float deltaTime) {
			Guid[] eids = world.GetEntitiesWithComponent<RectCollider>().Keys.ToArray();
			int i, j;
			Guid eid1;
			Guid eid2;
			if(eids.Length > 0) {
				// check rest
				eid1 = eids[0];
				ref RectCollider rect1 = ref world.GetComponent<RectCollider>(eid1);
				ref Transform2D pos1 = ref world.GetComponent<Transform2D>(eid1);
				rect1.collisions = new List<Guid>();
				// check self
				if(rect1.CollidesWithSelf)
					rect1.collisions.Add(eid1);

				// check rest
				for(j = 1; j < eids.Length; j++) {
					eid2 = eids[j];
					ref RectCollider rect2 = ref world.GetComponent<RectCollider>(eid2);
					ref Transform2D pos2 = ref world.GetComponent<Transform2D>(eid2);
					rect2.collisions = new List<Guid>();
					if(IsColliding(ref rect1, ref pos1, ref rect2, ref pos2)) {
						rect1.collisions.Add(eid2);
						rect2.collisions.Add(eid1);
					}
				}
			}

			for(i = 1; i<eids.Length-1;  i++) {
				eid1 = eids[i];
				ref RectCollider rect1 = ref world.GetComponent<RectCollider>(eid1);
				ref Transform2D pos1 = ref world.GetComponent<Transform2D>(eid1);
				// check self
				if(rect1.CollidesWithSelf)
					rect1.collisions.Add(eid1);

				// check rest
				for(j = i+1; j <eids.Length; j++) {
					eid2 = eids[j];
					ref RectCollider rect2 = ref world.GetComponent<RectCollider>(eid2);
					ref Transform2D pos2 = ref world.GetComponent<Transform2D>(eid2);	
					if(IsColliding(ref rect1, ref pos1, ref rect2, ref pos2)) {
						rect1.collisions.Add(eid2);
						rect2.collisions.Add(eid1);
					}
				}
			}
			// check self
			if(eids.Length >= 2) {
				Guid eid = eids[eids.Length - 1];
				ref RectCollider rect = ref world.GetComponent<RectCollider>(eid);
				if(rect.CollidesWithSelf) rect.collisions.Add(eid);
			}
		}
		
		private static bool IsColliding(ref RectCollider a, ref Transform2D apos, ref RectCollider b, ref Transform2D bpos) {
			if(a.Equals(b)) return a.CollidesWithSelf;
			Vector2 a_topLeft = apos.Position;
			Vector2 a_botRight = a_topLeft + a.Dimentions;
			Vector2 b_topLeft = bpos.Position;
			Vector2 b_botRight = b_topLeft + b.Dimentions;
			return !(a_botRight.X < b_topLeft.X || a_topLeft.X > b_botRight.X || a_topLeft.Y > b_botRight.Y || a_botRight.Y < b_topLeft.Y); ;

			//return 
			//!(a_botRight.X < b_topLeft.X || 
			//	a_topLeft.X > b_botRight.X || 

			//	a_topLeft.Y < b_botRight.Y || 
			//	a_botRight.Y < b_botRight.Y);
		}
	}
}
