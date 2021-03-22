using System.Collections.Generic;
namespace ECS {
	public interface IPriority {
		uint Priority { get; }
	}

	internal class IPriorityComparer : IComparer<IPriority> {
		internal static IPriorityComparer Comparer = new IPriorityComparer();
		public int Compare(IPriority x, IPriority y)
			=> x.Priority.CompareTo(y.Priority);
	}
}
