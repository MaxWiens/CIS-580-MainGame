using System;
namespace ECS {
	public interface ISystem {
		World World { get; }
	}
}
