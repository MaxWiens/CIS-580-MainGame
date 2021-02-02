using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
namespace MainGame.Components {
	class Sprite : ECS.Component {
		public Texture2D Texture;
		[JsonInclude] public string TextureName = string.Empty;
	}
}
