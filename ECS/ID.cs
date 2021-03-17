using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ECS {
	public class ID : IEquatable<ID>, IEquatable<Guid>, ICloneable {
		public Guid Guid;
		public readonly string Tag;
		public readonly bool GivenValue;

		public ID() {
			Guid = Guid.Empty;
			GivenValue = false;
			Tag = null;
		}

		public ID(Guid guid) {
			Guid = guid;
			if(guid != Guid.Empty)
				GivenValue = true;
			else
				GivenValue = false;
			Tag = null;
		}

		public ID(Guid guid, string tag) {
			Guid = guid;
			if(guid != Guid.Empty)
				GivenValue = true;
			else
				GivenValue = false;
			Tag = tag;
		}

		public ID(string tag) {
			Guid = Guid.Empty;
			GivenValue = false;
			Tag = tag;
		}

		public bool Equals([AllowNull] ID other) => Guid == other.Guid;
		public bool Equals([AllowNull] Guid other) => Guid == other;

		public override bool Equals(object obj) => obj is ID i && Guid.Equals(i.Guid);
		public override int GetHashCode() => Guid.GetHashCode();

		public object Clone() => new ID(Guid, Tag);

		/// <summary>
		/// implicit conversion to Guids
		/// </summary>
		/// <param name="id">ID to convert</param>
		public static implicit operator Guid(ID id) => id.Guid;
		public static explicit operator ID(Guid guid) => new ID(guid);
	}
}