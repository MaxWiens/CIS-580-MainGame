using System;
namespace MainGame {
	public static class Program {
		[STAThread]
		static void Main() {
			using MegaDungeonGame game = new MegaDungeonGame();
			game.Run();
		}
	}
}
