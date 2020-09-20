
namespace CombatCore.StatusComponents
{
    public class Invincible : StatusComponent
    {
        public override void OnApply(Entity subject)
        {
            subject.SetInvincible (true);
        }

        public override void OnRevert(Entity subject)
        {
            subject.SetInvincible (false);
        }
    }
}
