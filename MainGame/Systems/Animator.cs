using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
namespace MainGame.Systems {
	using ECS;
	using Components;
	[MoonSharpUserData]
	public class Animator : BaseSystem, IUpdateable {
		public Animator(World world) : base(world) { }

		public void Update(float deltaTime) {
			var aniMap = World.GetEntitiesWith<TileAnimation>();
			var spriteMap = World.GetEntitiesWith<Sprite>();
			foreach(TileAnimation animation in aniMap.Values) {
				if((animation.Timer += deltaTime) >= animation.FrameDelay) {
					animation.Timer -= animation.FrameDelay;
					++animation.FrameIdx;
					Sprite sprite = (Sprite)spriteMap[animation.Entity];
					var asset = animation.Asset as Assets.TileSheet;
					sprite.Texture = asset.Texture;
					int x = asset.NumTiles.X;
					if(animation.FrameIdx >= x*asset.NumTiles.Y) {
						 animation.FrameIdx = 0;
					}
					sprite.SourceRectangle = new Rectangle(new Point(animation.FrameIdx % x, animation.FrameIdx / x)*asset.TileDimentions, asset.TileDimentions);
				}
			}
		}
	}
}
