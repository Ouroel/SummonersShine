using SummonersShine.SpecialData;
using System;
using Terraria;
using Terraria.ID;

namespace SummonersShine.SpecialAbilities.DefaultSpecialAbility
{
    class SA_Reaction : DefaultSpecialAbility
    {
        public override string ToString()
        {
            return "Reaction";
        }
        public override void GetRandomMinionPower(minionPower[] minionPowers, Random rand)
        {
            GetMinionPower(minionPowers, 100, 0);
        }
        public override void GetMinionPower(minionPower[] minionPowers, float val1, float val2)
        {
            minionPowers[0] = minionPower.NewMP(val1);
        }

        public override bool ValidForItem(Item item, Projectile projectile)
        {
            return item.shoot != ProjectileID.None && IsNotWhip(projectile);
        }
        public override void OnProjectileCreated(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            projFuncs.minionPreAI += MinionPreAI;
        }
        public override void UnhookMinionPower(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            projFuncs.minionPreAI -= MinionPreAI;
            projFuncs.RemoveBuff(ProjectileBuffs.ProjectileBuffIDs.ReactionEnrageBuff, projectile);
            projFuncs.GetMinionProjData().castingSpecialAbilityTime = -1;
        }

        const int enrageStart = 0;
        const int enrageEnd = 120;
        const int enrageReset = 600;

        public void MinionPreAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (projData.currentTick != 1 || projectile.numUpdates > -1)
                return;
            Player player = Main.player[projectile.owner];
            ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();

            if (projData.castingSpecialAbilityTime == enrageStart)
            {
                if (playerFuncs.attackedThisFrame == 1)
                {
                    projFuncs.AddBuff(projectile, ProjectileBuffs.ProjectileBuffIDs.ReactionEnrageBuff, projectile);
                    projData.castingSpecialAbilityTime++;
                }
            }
            else if (projData.castingSpecialAbilityTime == enrageEnd)
            {
                projFuncs.RemoveBuff(ProjectileBuffs.ProjectileBuffIDs.ReactionEnrageBuff, projectile);
                projData.castingSpecialAbilityTime++;
            }
            else if (projData.castingSpecialAbilityTime >= enrageReset)
            {
                projFuncs.RemoveBuff(ProjectileBuffs.ProjectileBuffIDs.ReactionEnrageBuff, projectile);
                projData.castingSpecialAbilityTime = -1;
            }
            else
                projData.castingSpecialAbilityTime++;
        }
    }
}
