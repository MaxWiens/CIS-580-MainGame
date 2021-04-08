using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileSheetContentPipeline {
	public class TileSheet : Texture2D {

		public readonly int TileWidth;
		public readonly int TileHeight;
		public readonly int TileCount;

		

		public TileSheet(GraphicsDevice graphicsDevice, int width, int height) : base(graphicsDevice, width, height) { }
		public TileSheet(GraphicsDevice graphicsDevice, int width, int height, bool mipmap, SurfaceFormat format) : base(graphicsDevice, width, height, mipmap, format) { }
		public TileSheet(GraphicsDevice graphicsDevice, int width, int height, bool mipmap, SurfaceFormat format, int arraySize) : base(graphicsDevice, width, height, mipmap, format, arraySize) { }
		protected TileSheet(GraphicsDevice graphicsDevice, int width, int height, bool mipmap, SurfaceFormat format, SurfaceType type, bool shared, int arraySize) : base(graphicsDevice, width, height, mipmap, format, type, shared, arraySize) { }
	}
}
