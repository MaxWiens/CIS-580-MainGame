using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MainGame.Components {
	public struct Animation {
		[JsonInclude] public float Timer;
		[JsonInclude] public float Rate;
		[JsonInclude] public string SheetName;
	}
}
