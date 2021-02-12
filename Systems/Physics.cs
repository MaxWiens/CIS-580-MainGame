using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Linq;

namespace MainGame.Systems {
	using ECS;
	using Components;

	public class Physics : UpdateSystem {
		public Physics(ZaWarudo world) : base(world) {}

		public override void Update(float deltaTime) {
			var rbMap = world.GetEntitiesWithComponent<RigidBody>();
			if(rbMap != null) {
				var rbEIDs = rbMap.Keys.ToArray();
				var sbMap = world.GetEntitiesWithComponent<StaticBody>();
				var sbEIDs = sbMap.Keys;
				var transformMap = world.GetEntitiesWithComponent<Transform2D>();
			}
			/*
			#region Move Rigid Bodies;
			{
				foreach(Guid eid in rbEIDs) {
					ref RigidBody rb = ref rbMap[eid];
					ref Transform2D transform = ref transformMap[eid];
					transform.Position += rb.Velocity * deltaTime;
				}
			}
			#endregion

			#region Check Collisions
			{
				var rectBoundsMap = world.GetEntitiesWithComponent<RectBounds>();
				// Guid[] rectBoundsEIDs = rectBoundsMap.Keys.ToArray();
				int i, j;
				Guid eid1;
				Guid eid2;
				
				if(rbEIDs.Length > 0) {
					eid1 = rectBoundsEIDs[0];
					ref RectBounds rect1 = ref rectBoundsMap[eid1];
					ref Transform2D pos1 = ref transformMap[eid1];
					rect1.collisions = new List<Guid>();

					// check rigid bodies
					for(j = 1; j < rectBoundsEIDs.Length; j++) {
						eid2 = rectBoundsEIDs[j];
						ref RectBounds rect2 = ref rectBoundsMap[eid2];
						ref Transform2D pos2 = ref transformMap[eid2];
						rect2.collisions = new List<Guid>();
						if(IsColliding(ref rect1, ref pos1, ref rect2, ref pos2)) {
							rect1.collisions.Add(eid2);
							rect2.collisions.Add(eid1);
						}
					}

					// check static  bodies
					foreach(var eid in sbEIDs) {
						StaticBody s = sbMap[eid];
						if( & s.Layers == 0) continue;

						
					}

					for(j = 0; j < rectBoundsEIDs.Length; j++) {
						eid2 = rectBoundsEIDs[j];
						ref RectBounds rect2 = ref rectBoundsMap[eid2];
						ref Transform2D pos2 = ref transformMap[eid2];
						rect2.collisions = new List<Guid>();
						if(IsColliding(ref rect1, ref pos1, ref rect2, ref pos2)) {
							rect1.collisions.Add(eid2);
							rect2.collisions.Add(eid1);
						}
					}
				}

				for(i = 1; i < rectBoundsEIDs.Length - 1; i++) {
					eid1 = rectBoundsEIDs[i];
					ref RectBounds rect1 = ref world.GetComponent<RectBounds>(eid1);
					ref Transform2D pos1 = ref world.GetComponent<Transform2D>(eid1);

					// check rest
					for(j = i + 1; j < rectBoundsEIDs.Length; j++) {
						eid2 = rectBoundsEIDs[j];
						ref RectBounds rect2 = ref world.GetComponent<RectBounds>(eid2);
						ref Transform2D pos2 = ref world.GetComponent<Transform2D>(eid2);
						if(IsColliding(ref rect1, ref pos1, ref rect2, ref pos2)) {
							rect1.collisions.Add(eid2);
							rect2.collisions.Add(eid1);
						}
					}
				}
			}
			#endregion
			*/
		}
		
		private static bool IsColliding(ref RectBounds a, ref Transform2D apos, ref RectBounds b, ref Transform2D bpos) {
			Vector2 a_topLeft = apos.Position - a.Offset;
			Vector2 a_botRight = a_topLeft + a.Dimentions;
			Vector2 b_topLeft = bpos.Position - b.Offset;
			Vector2 b_botRight = b_topLeft + b.Dimentions;
			return !(a_botRight.X <= b_topLeft.X || a_topLeft.X >= b_botRight.X || a_topLeft.Y >= b_botRight.Y || a_botRight.Y <= b_topLeft.Y); ;
		}

		private static bool IsColliding(ref CircleBounds a, ref Transform2D apos, ref CircleBounds b, ref Transform2D bpos) {
			float abRadius = a.radius + b.radius;
			return (apos.Position - a.Offset - (bpos.Position - b.Offset)).LengthSquared() < abRadius * abRadius;
		}

		private static bool IsColliding(ref RectBounds a, ref Transform2D apos, ref CircleBounds b, ref Transform2D bpos) {
			Vector2 a_adjPos = apos.Position - a.Offset;
			Vector2 a_botRight = a_adjPos + a.Dimentions;
			Vector2 b_adjPos = bpos.Position - b.Offset;
			float b_radius = b.radius;
			return (b_adjPos - Vector2.Clamp(b_adjPos, a_adjPos, a_botRight)).LengthSquared() < b_radius * b_radius;
			//return !(a_botRight.X <= b_topLeft.X || a_adjPos.X >= b_botRight.X || a_adjPos.Y >= b_botRight.Y || a_botRight.Y <= b_topLeft.Y); ;
		}
	}
}
