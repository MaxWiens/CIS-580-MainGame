namespace ECS {
	public interface IFixedUpdateable : IPriority {
		[MoonSharp.Interpreter.MoonSharpHidden]
		void FixedUpdate(float fixedDeltaTime);
	}
}
