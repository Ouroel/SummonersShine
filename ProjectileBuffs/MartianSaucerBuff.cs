using SummonersShine.SpecialData;
using Terraria;
using Terraria.ID;

namespace SummonersShine.ProjectileBuffs
{
    public class MartianSaucerBuff : ProjectileBuff
    {
        public override ProjectileBuffIDs ID => ProjectileBuffIDs.MartianSaucerBuff;

        bool active = false;
        public MartianSaucerBuff() {
        
        }
        public override bool Update(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            active = ModUtils.IsCastingSpecialAbility(projFuncs.GetMinionProjData(), ItemID.XenoStaff);

            return true;
        }

        public override float GetAttackSpeedBuff(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            const float buff = 0.5f;
            const int MaxDur = 2 * 60;
            if (active)
                return 1 - buff * projData.castingSpecialAbilityTime / MaxDur;
            return 1;
        }
    }
}
