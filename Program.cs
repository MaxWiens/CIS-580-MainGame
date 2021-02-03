using System;

namespace MainGame {
	using ECS;
	using Systems;
	public static class Program {
		[STAThread]
		static void Main() {
			using(var game = new ZaWarudo())
				game.Run();
		}
	}
}
