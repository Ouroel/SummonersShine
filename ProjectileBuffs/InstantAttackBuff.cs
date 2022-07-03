using SummonersShine.SpecialData;
using Terraria;

namespace SummonersShine.ProjectileBuffs
{
    public abstract partial class ProjectileBuff
    {
        class InstantAttackBuff : ProjectileBuff
        {
            public override ProjectileBuffIDs ID => ProjectileBuffIDs.InstantAttackBuff;
            public override bool Update(Projectile projectile, ReworkMinion_Projectile projFuncs)
            {
                return true;
            }

            public override float GetAttackSpeedBuff(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
            {
                return 0.01f;
            }
        }
    }
}
