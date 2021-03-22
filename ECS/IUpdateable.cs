namespace ECS {
	public interface IUpdateable : IPriority {
		[MoonSharp.Interpreter.MoonSharpHidden]
		void Update(float deltaTime);
	}
}
