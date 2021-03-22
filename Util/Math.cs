namespace Util {
	public static class Math {
		public static int Mod(int a, int b) => (a %= b) < 0 ? a + b : a;
		public static int IntDivRoundDown(int a, int b)
			=> a < 0 ? ((a+1) / b) - 1 : a / b;
	}
}
