using ECS;
using tainicom.Aether.Physics2D.Dynamics;
using MoonSharp.Interpreter;
namespace MainGame.Components {
	[MoonSharpUserData]
	public class Body : tainicom.Aether.Physics2D.Dynamics.Body, IComponent {
		public object Clone() {
			Body b = new Body() {
				AngularDamping = AngularDamping,
				AngularVelocity = AngularVelocity,
				Awake = Awake,
				BodyType = BodyType,
				ControllerFilter = ControllerFilter,
				Enabled = Enabled,
				IgnoreCCD = IgnoreCCD,
				IgnoreGravity = IgnoreGravity,
				Inertia = Inertia,
				IsBullet = IsBullet,
				IslandIndex = IslandIndex,
				FixedRotation = FixedRotation,
				LinearDamping = LinearDamping,
				LinearVelocity = LinearVelocity,
				LocalCenter = LocalCenter,
				SleepingAllowed = SleepingAllowed,
				Position = Position,
				Rotation = Rotation,
				Mass = Mass
			};
			foreach(Fixture f in FixtureList) {
				f.CloneOnto(b);
			}
			World?.Add(b);
			return b;
		}
	}
}
