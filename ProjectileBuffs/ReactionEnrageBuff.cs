using SummonersShine.SpecialData;
using Terraria;

namespace SummonersShine.ProjectileBuffs
{
    public abstract partial class ProjectileBuff
    {
        class ReactionEnrageBuff : ProjectileBuff
        {
            public override ProjectileBuffIDs ID => ProjectileBuffIDs.ReactionEnrageBuff;
            public override bool Update(Projectile projectile, ReworkMinion_Projectile projFuncs)
            {
                return true;
            }

            public override float GetAttackSpeedBuff(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
            {
                return 0.5f;// 100 / (100 + projFuncs.GetMinionPower(projectile, 0));
            }
        }
    }
}
