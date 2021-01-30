using System;
using System.Collections.Generic;
using System.Text;

namespace MainGame.ECS {
	
	public abstract class System {}

	public interface IEnableable {
		bool Enabled { set; }
		void OnEnable();
		void OnDisable();
	}

	public interface IUpdatable : IEnableable {
		void Update(float deltaTime);
	}
	
	public interface IStart : IEnableable {
		void OnStart();
	}

	public interface IEnd : IEnableable {
		void OnEnd();
	}
}
