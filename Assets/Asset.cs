using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Assets {
	public abstract class Asset {
		[JsonInclude] public Guid ID;
	}
}
