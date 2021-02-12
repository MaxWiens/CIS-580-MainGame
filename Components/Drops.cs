using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
namespace MainGame.Components {
	public struct Drops {
		[JsonInclude] public string[] Items;
	}
}
