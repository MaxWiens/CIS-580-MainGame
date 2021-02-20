using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Linq;
using MainGame.Physics;

namespace MainGame.Systems {
	using ECS;
	using Components;
	public class Physics : UpdateSystem {
		public Physics(ZaWarudo world) : base(world) {}

		private RectBounds _fallbackRectBounds;
		public override void Update(float deltaTime) {
			var rbMap = world.GetEntitiesWithComponent<RigidBody>();
			var rbEIDs = rbMap?.Keys.ToArray() ?? new Guid[0];
			var sbMap = world.GetEntitiesWithComponent<StaticBody>();
			var sbEIDs = sbMap?.Keys ?? new Guid[0];

			var rectBoundsMap = world.GetEntitiesWithComponent<RectBounds>();
			var circleBoundsMap = world.GetEntitiesWithComponent<CircleBounds>();

			var transformMap = world.GetEntitiesWithComponent<Transform2D>();
			
			#region Move Rigid Bodies;
			foreach(Guid eid in rbEIDs) {
				ref RigidBody rb = ref rbMap[eid];
				ref Transform2D transform = ref transformMap[eid];
				rb.Velocity += rb.Acceleration * deltaTime;
				transform.Position += rb.Velocity * deltaTime;
			}
			#endregion
			
			#region Check Collisions
			// Guid[] rectBoundsEIDs = rectBoundsMap.Keys.ToArray();
			int i, j;
			Guid eid1;
			Guid eid2;
			Vector2 normal;
			bool isRectBounds = false;

			if(rbEIDs.Length > 0) {
				eid1 = rbEIDs[0];
				ref RigidBody rb1 = ref rbMap[eid1];
				rb1.Collisions = new List<(Guid, Vector2)>();
				ref Transform2D trans1 = ref transformMap[eid1];
				RectBounds rect1 = rectBoundsMap.TryGetValue(eid1, ref _fallbackRectBounds, out isRectBounds); ;
				if(isRectBounds) {
					// go through rigid bodies
					for(j = 1; j < rbEIDs.Length; j++) {
						eid2 = rbEIDs[j];
						ref RigidBody rb2 = ref rbMap[eid2];
						rb2.Collisions = new List<(Guid, Vector2)>();
						if(rb1.Layer.CantCollide(rb2.Layer)) continue;
						ref Transform2D trans2 = ref transformMap[eid2];
						ref RectBounds rect2 = ref rectBoundsMap.TryGetValue(eid2, ref _fallbackRectBounds, out isRectBounds);
						if(isRectBounds) {
							if(IsColliding(ref rect1, ref trans1, ref rect2, ref trans2, out normal)) {
								if(!rb2.IsTrigger) trans1.Position += normal;
								if(!rb1.IsTrigger) trans2.Position -= normal;
								rb1.Collisions.Add((eid2, normal));
								rb2.Collisions.Add((eid1, -normal));
							}
						} else {
							ref CircleBounds circ2 = ref circleBoundsMap[eid2];
							if(IsColliding(ref rect1, ref trans1, ref circ2, ref trans2, out normal)) {
								if(!rb2.IsTrigger) trans1.Position += normal;
								if(!rb1.IsTrigger) trans2.Position -= normal;
								rb1.Collisions.Add((eid2, normal));
								rb2.Collisions.Add((eid1, -normal));
							}
						}
					}
					// go through static bodies
					foreach(Guid sbeid in sbEIDs) {
						ref StaticBody sb2 = ref sbMap[sbeid];
						sb2.Collisions = new List<(Guid, Vector2)>();
						if(rb1.Layer.CantCollide(sb2.Layer)) continue;
						ref Transform2D trans2 = ref transformMap[sbeid];
						ref RectBounds rect2 = ref rectBoundsMap.TryGetValue(sbeid, ref _fallbackRectBounds, out isRectBounds);
						if(isRectBounds) {
							if(IsColliding(ref rect1, ref trans1, ref rect2, ref trans2, out normal)) {
								if(!sb2.IsTrigger) trans1.Position += normal;
								rb1.Collisions.Add((sbeid, normal));
								sb2.Collisions.Add((eid1, -normal));
							}
						} else {
							ref CircleBounds circ2 = ref circleBoundsMap[sbeid];
							if(IsColliding(ref rect1, ref trans1, ref circ2, ref trans2, out normal)) {
								if(!sb2.IsTrigger) trans1.Position += normal;
								rb1.Collisions.Add((sbeid, normal));
								sb2.Collisions.Add((eid1, -normal));
							}
						}
					}

				} else {
					ref CircleBounds circ1 = ref circleBoundsMap[eid1];
					// go through rigid bodies
					for(j = 1; j < rbEIDs.Length; j++) {
						eid2 = rbEIDs[j];
						ref RigidBody rb2 = ref rbMap[eid2];
						rb2.Collisions = new List<(Guid, Vector2)>();
						if(rb1.Layer.CantCollide(rb2.Layer)) continue;
						ref Transform2D trans2 = ref transformMap[eid2];
						RectBounds rect2 = rectBoundsMap.TryGetValue(eid2, ref _fallbackRectBounds, out isRectBounds);
						if(isRectBounds) {
							if(IsColliding(ref rect2, ref trans2, ref circ1, ref trans1, out normal)) {
								if(!rb2.IsTrigger) trans1.Position -= normal;
								rb1.Collisions.Add((eid2, -normal));
								if(!rb1.IsTrigger) trans2.Position += normal;
								rb2.Collisions.Add((eid1, normal));
							}
						} else {
							ref CircleBounds circ2 = ref circleBoundsMap[eid2];
							if(IsColliding(ref circ1, ref trans1, ref circ2, ref trans2, out normal)) {
								if(!rb2.IsTrigger) trans1.Position += normal;
								rb1.Collisions.Add((eid2, normal));
								if(!rb1.IsTrigger) trans2.Position -= normal;
								rb2.Collisions.Add((eid1, -normal));
							}
						}
					}
					// go through static bodies
					foreach(Guid sbeid in sbEIDs) {
						ref StaticBody sb2 = ref sbMap[sbeid];
						sb2.Collisions = new List<(Guid, Vector2)>();
						if(rb1.Layer.CantCollide(sb2.Layer)) continue;
						ref Transform2D trans2 = ref transformMap[sbeid];
						ref RectBounds rect2 = ref rectBoundsMap.TryGetValue(sbeid, ref _fallbackRectBounds, out isRectBounds);
						if(isRectBounds) {
							if(IsColliding(ref rect2, ref trans2, ref circ1, ref trans1, out normal)) {
								if(!sb2.IsTrigger) trans1.Position -= normal;
								rb1.Collisions.Add((sbeid, -normal));
								sb2.Collisions.Add((eid1, normal));
							}
						} else {
							ref CircleBounds circ2 = ref circleBoundsMap[sbeid];
							if(IsColliding(ref circ1, ref trans1, ref circ2, ref trans2, out normal)) {
								if(!sb2.IsTrigger) trans1.Position += normal;
								rb1.Collisions.Add((sbeid, normal));
								sb2.Collisions.Add((eid1, -normal));
							}
						}
					}
				}

				for(i=1; i<rbEIDs.Length; i++) {
					eid1 = rbEIDs[i];
					rb1 = ref rbMap[eid1];
					trans1 = ref transformMap[eid1];
					rect1 = rectBoundsMap.TryGetValue(eid1, ref _fallbackRectBounds, out isRectBounds);
					if(isRectBounds) {
						// go through rigid bodies
						for(j = 1; j < rbEIDs.Length; j++) {
							eid2 = rbEIDs[j];
							ref RigidBody rb2 = ref rbMap[eid2];
							if(rb1.Layer.CantCollide(rb2.Layer)) continue;
							ref Transform2D trans2 = ref transformMap[eid2];
							ref RectBounds rect2 = ref rectBoundsMap.TryGetValue(eid2, ref _fallbackRectBounds, out isRectBounds);
							if(isRectBounds) {
								if(IsColliding(ref rect1, ref trans1, ref rect2, ref trans2, out normal)) {
									if(!rb2.IsTrigger) trans1.Position += normal;
									if(!rb1.IsTrigger) trans2.Position -= normal;
									rb1.Collisions.Add((eid2, normal));
									rb2.Collisions.Add((eid1, -normal));
								}
							} else {
								ref CircleBounds circ2 = ref circleBoundsMap[eid2];
								if(IsColliding(ref rect1, ref trans1, ref circ2, ref trans2, out normal)) {
									if(!rb2.IsTrigger) trans1.Position += normal;
									if(!rb1.IsTrigger) trans2.Position -= normal;
									rb1.Collisions.Add((eid2, normal));
									rb2.Collisions.Add((eid1, -normal));
								}
							}
						}
						// go through static bodies
						foreach(Guid sbeid in sbEIDs) {
							ref StaticBody sb2 = ref sbMap[sbeid];
							if(rb1.Layer.CantCollide(sb2.Layer)) continue;
							ref Transform2D trans2 = ref transformMap[sbeid];
							ref RectBounds rect2 = ref rectBoundsMap.TryGetValue(sbeid, ref _fallbackRectBounds, out isRectBounds);
							if(isRectBounds) {
								if(IsColliding(ref rect1, ref trans1, ref rect2, ref trans2, out normal)) {
									if(!sb2.IsTrigger) trans1.Position += normal;
									rb1.Collisions.Add((sbeid, normal));
									sb2.Collisions.Add((eid1, -normal));
								}
							} else {
								ref CircleBounds circ2 = ref circleBoundsMap[sbeid];
								if(IsColliding(ref rect1, ref trans1, ref circ2, ref trans2, out normal)) {
									if(!sb2.IsTrigger) trans1.Position += normal;
									rb1.Collisions.Add((sbeid, normal));
									sb2.Collisions.Add((eid1, -normal));
								}
							}
						}

					} else {
						ref CircleBounds circ1 = ref circleBoundsMap[eid1];
						// go through rigid bodies
						for(j = i + 1; j < rbEIDs.Length; j++) {
							eid2 = rbEIDs[j];
							ref RigidBody rb2 = ref rbMap[eid2];
							if(rb1.Layer.CantCollide(rb2.Layer)) continue;
							ref Transform2D trans2 = ref transformMap[eid2];
							ref RectBounds rect2 = ref rectBoundsMap.TryGetValue(eid2, ref _fallbackRectBounds, out isRectBounds);
							if(isRectBounds) {
								if(IsColliding(ref rect2, ref trans2, ref circ1, ref trans1, out normal)) {
									if(!rb2.IsTrigger) trans1.Position -= normal;
									if(!rb1.IsTrigger) trans2.Position += normal;
									rb1.Collisions.Add((eid2, -normal));
									rb2.Collisions.Add((eid1, normal));
								}
							} else {
								ref CircleBounds circ2 = ref circleBoundsMap[eid2];
								if(IsColliding(ref circ1, ref trans1, ref circ2, ref trans2, out normal)) {
									if(!rb2.IsTrigger) trans1.Position += normal;
									if(!rb1.IsTrigger) trans2.Position -= normal;
									rb1.Collisions.Add((eid2, normal));
									rb2.Collisions.Add((eid1, -normal));
								}
							}
						}
						// go through static bodies
						foreach(Guid sbeid in sbEIDs) {
							ref StaticBody sb2 = ref sbMap[sbeid];
							if(rb1.Layer.CantCollide(sb2.Layer)) continue;
							ref Transform2D trans2 = ref transformMap[sbeid];
							ref RectBounds rect2 = ref rectBoundsMap.TryGetValue(sbeid, ref _fallbackRectBounds, out isRectBounds);
							if(isRectBounds) {
								if(IsColliding(ref rect2, ref trans2, ref circ1, ref trans1, out normal)) {
									if(!sb2.IsTrigger) trans1.Position -= normal;
									rb1.Collisions.Add((sbeid, -normal));
									sb2.Collisions.Add((eid1, normal));
								}
							} else {
								ref CircleBounds circ2 = ref circleBoundsMap[sbeid];
								if(IsColliding(ref circ1, ref trans1, ref circ2, ref trans2, out normal)) {
									if(!sb2.IsTrigger) trans1.Position += normal;
									rb1.Collisions.Add((sbeid, normal));
									sb2.Collisions.Add((eid1, -normal));
								}
							}
						}
					}
				}
			}
			#endregion
		}

		private static bool IsIntersecting(ref RectBounds a, ref Transform2D apos, ref RectBounds b, ref Transform2D bpos) {
			/*
			Vector2 a_topLeft = apos.Position - a.Offset;
			Vector2 a_botRight = a_topLeft + a.Dimentions;
			Vector2 b_topLeft = bpos.Position - b.Offset;
			Vector2 b_botRight = b_topLeft + b.Dimentions;
			
			return !(a_botRight.X < b_topLeft.X || a_topLeft.X > b_botRight.X || a_topLeft.Y > b_botRight.Y || a_botRight.Y <= b_topLeft.Y);
			*/
			return false;
		}

		private static bool IsColliding(ref RectBounds a, ref Transform2D atrans, ref RectBounds b, ref Transform2D btrans, out Vector2 normal) {
			Vector2 a_topLeft = atrans.Position - a.Offset;
			Vector2 a_botRight = a_topLeft + a.Dimentions;
			Vector2 b_topLeft = btrans.Position - b.Offset;
			Vector2 b_botRight = b_topLeft + b.Dimentions;

			if(!(a_botRight.X < b_topLeft.X || a_topLeft.X > b_botRight.X || a_topLeft.Y > b_botRight.Y || a_botRight.Y < b_topLeft.Y)) {
				float difx1 = b_topLeft.X - a_botRight.X; //move towards left
				float difx = b_botRight.X - a_topLeft.X;

				float dify1 = b_topLeft.Y - a_botRight.Y; //move towards top
				float dify = b_botRight.Y - a_topLeft.Y;

				if(-difx1 < difx) {
					if(-dify1 < dify) {
						if(difx1 > dify1) {
							normal = new Vector2(difx1, 0f);
						} else {
							normal = new Vector2(0f, dify1);
						}
					} else {
						if(-difx1 < dify) {
							normal = new Vector2(difx1, 0f);
						} else {
							normal = new Vector2(0f, dify);
						}
					}
				} else {
					if(-dify1 < dify) {
						if(difx < -dify1) {
							normal = new Vector2(difx, 0f);
						} else {
							normal = new Vector2(0f, dify1);
						}
					} else {
						if(difx < dify) {
							normal = new Vector2(difx, 0f);
						} else {
							normal = new Vector2(0f, dify);
						}
					}
				}
				return true;
			}
			normal = Vector2.Zero;
			return false;
		}

		private static bool IsColliding(ref CircleBounds a, ref Transform2D atrans, ref CircleBounds b, ref Transform2D btrans, out Vector2 normal) {
			float abRadius = a.Radius + b.Radius;
			Vector2 dif = (atrans.Position+new Vector2(a.Radius) -a.Offset) - (btrans.Position+new Vector2(b.Radius)-b.Offset);

			if(dif.LengthSquared() <= abRadius * abRadius) {
				normal = Vector2.Normalize(dif) * (abRadius - dif.Length());
				return true;
			}
			normal = Vector2.Zero;

			return false;
		}

		private static bool IsColliding(ref RectBounds a, ref Transform2D atrans, ref CircleBounds b, ref Transform2D btrans, out Vector2 normal) {
			Vector2 center = btrans.Position + new Vector2(b.Radius) - b.Offset;
			Vector2 a_topLeft = atrans.Position - a.Offset;
			Vector2 a_botRight = a_topLeft + a.Dimentions;
			// get difference between nearest point on rectbounds and the center of circle
			Vector2 cv = Vector2.Clamp(center, a_topLeft, a_botRight);
			Vector2 dif = cv - center;
			if(dif == Vector2.Zero) {
				Vector2 dif1 = center - a_topLeft;
				Vector2 dif2 = a_botRight - center;
				
				if(dif1.X < dif2.X) {
					if(dif1.Y < dif2.Y) {
						if(dif1.X < dif1.Y)
							normal = new Vector2(-dif1.X*b.Radius, 0f);
						else
							normal = new Vector2(0f, -dif1.Y * b.Radius);
					} else {
						if(dif1.X < dif2.Y)
							normal = new Vector2(-dif1.X * b.Radius, 0f);
						else
							normal = new Vector2(0f, dif2.Y * b.Radius);
					}
				} else {
					if(dif1.Y < dif2.Y) {
						if(dif2.X < dif1.Y)
							normal = new Vector2(dif2.X * b.Radius, 0f);
						else
							normal = new Vector2(0f, -dif1.Y * b.Radius);
					} else {
						if(dif2.X < dif2.Y)
							normal = new Vector2(dif2.X * b.Radius, 0f);
						else
							normal = new Vector2(0f, dif2.Y * b.Radius);
					}
				}
				return true;
			}else if(b.Radius * b.Radius >= dif.LengthSquared()) {
				normal = (Vector2.Normalize(dif) * b.Radius) - dif;
				return true;
			}
			normal = Vector2.Zero;
			return false;
		}
	}
}
