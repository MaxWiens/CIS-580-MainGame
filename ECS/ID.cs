using System;

namespace ECS {
	public class ID : IEquatable<ID>, IEquatable<Guid>, ICloneable {
		public Guid Guid;
		private string _tag;
		public string Tag => _tag;
		private bool _givenValue;
		public bool GivenValue => _givenValue;

		public ID() {
			Guid = Guid.Empty;
			_givenValue = false;
			_tag = null;
		}

		public ID(Guid guid) {
			Guid = guid;
			if(guid != Guid.Empty)
				_givenValue = true;
			else
				_givenValue = false;
			_tag = null;
		}

		public ID(Guid guid, string tag) {
			Guid = guid;
			if(guid != Guid.Empty)
				_givenValue = true;
			else
				_givenValue = false;
			_tag = tag;
		}

		public ID(string tag) {
			Guid = Guid.Empty;
			_givenValue = false;
			_tag = tag;
		}
		
		public bool Equals(ID other) => Guid == other.Guid;
		public bool Equals(Guid other) => Guid == other;

		public override bool Equals(object obj) => obj is ID i && Guid.Equals(i.Guid);
		public override int GetHashCode() => Guid.GetHashCode();

		public object Clone() => new ID() { Guid = Guid, _tag = _tag, _givenValue = _givenValue};

		/// <summary>
		/// implicit conversion to Guids
		/// </summary>
		/// <param name="id">ID to convert</param>
		public static implicit operator Guid(ID id) => id.Guid;
	}
}