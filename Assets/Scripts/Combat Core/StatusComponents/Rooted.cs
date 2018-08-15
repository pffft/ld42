
namespace CombatCore.StatusComponents
{
	public class Rooted : StatusComponent
	{
		public override void OnApply(Entity subject)
		{
			subject.SetRooted (true);
		}

		public override void OnRevert(Entity subject)
		{
			subject.SetRooted (false);
		}
	}
}
