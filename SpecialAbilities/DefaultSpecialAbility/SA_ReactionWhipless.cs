using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ID;
using SummonersShine.BakedConfigs;

namespace SummonersShine.SpecialAbilities.DefaultSpecialAbility
{
    class SA_ReactionWhipless : DefaultSpecialAbility
    {
        public override string ToString()
        {
            return "ReactionWhipless";
        }
        public override void GetRandomMinionPower(minionPower[] minionPowers, Random rand)
        {
            GetMinionPower(minionPowers, rand.Next(4, 7) * 5, 0);
        }
        public override void GetMinionPower(minionPower[] minionPowers, float val1, float val2)
        {
            minionPowers[0] = minionPower.NewMP(val1, mpScalingType.add);
        }

        public override bool ValidForItem(Item item, Projectile projectile)
        {
            return item.shoot == ProjectileID.None || IsNotWhip(projectile);
        }
        public override void OnProjectileCreated(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            projFuncs.minionPreAI += MinionPreAI;
            Player player = Main.player[projectile.owner];
            if (player != null)
            {
                ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
                playerFuncs.GetSpecialData<RightClickReactionCollection>().Add(projectile, projFuncs, projFuncs.GetMinionProjData());
            }
        }
        public override void UnhookMinionPower(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            projFuncs.minionPreAI -= MinionPreAI;
            projFuncs.RemoveBuff(ProjectileBuffs.ProjectileBuffIDs.ReactionEnrageBuff, projectile);
            projFuncs.GetMinionProjData().castingSpecialAbilityTime = -1;
            Player player = Main.player[projectile.owner];
            if (player != null)
            {
                ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
                playerFuncs.GetSpecialData<RightClickReactionCollection>().Remove(projectile);
            }
        }

        const int enrageStart = 960 * 100;
        const int enrageEnd = 1200 * 100;
        const int enrageReset = 1200 * 100;
        const int enrageEndGap = (enrageReset - enrageEnd);
        const int totalCooldownTime = (enrageReset - enrageEnd) + (enrageStart + 1);

        public override bool GetArbitraryBuffGaugeData(List<Projectile> projList, out float topGauge, out float bottomGauge)
        {
            float highest = -1;
            float lowest = -1;
            projList.ForEach(i => {
                if (BakedConfig.BlacklistedProjectiles[i.type] || BakedConfig.ProjectilesNotCountedAsMinion[i.type])
                    return;
                ReworkMinion_Projectile projFuncs = i.GetGlobalProjectile<ReworkMinion_Projectile>();
                MinionProjectileData projData = projFuncs.GetMinionProjData();

                int castSpecialTime = projData.castingSpecialAbilityTime;
                if (castSpecialTime == enrageStart) {

                    highest = 1;
                    if (lowest == -1)
                        lowest = 1;
                    return;
                }

                else if (castSpecialTime > enrageStart && castSpecialTime < enrageEnd)
                {
                    lowest = 0;
                    if (highest == -1)
                        highest = 0;
                    return;
                }

                if (castSpecialTime < enrageStart)
                    castSpecialTime += (enrageEndGap);
                else
                    castSpecialTime -= enrageEnd;

                float percentage = (float)(castSpecialTime) / (totalCooldownTime);

                if (highest == -1 || highest < percentage)
                    highest = percentage;
                if (lowest == -1 || lowest > percentage)
                    lowest = percentage;
            });
            if (highest == -1 || lowest == -1)
            {
                topGauge = 0;
                bottomGauge = 0;
                return false;
            }
            topGauge = highest;
            bottomGauge = lowest;
            return true;
        }

        const float normalMultRatio = 0.8f / 0.2f;
        public void MinionPreAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (projData.currentTick != 1 || projectile.numUpdates > -1)
                return;
            Player player = Main.player[projectile.owner];
            ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();

            int castSpecialTime = projData.castingSpecialAbilityTime;
            if (castSpecialTime == enrageStart)
            {
                bool flag = playerFuncs.attackedThisFrame == 1 || (player.whoAmI == Main.myPlayer && player.selectedItem != 58 && player.controlUseTile && !player.tileInteractionHappened && Main.mouseRight && !player.mouseInterface && !CaptureManager.Instance.Active && !Main.HoveringOverAnNPC && !Main.SmartInteractShowingGenuine); ;

                if (flag)
                {
                    projFuncs.AddBuff(projectile, ProjectileBuffs.ProjectileBuffIDs.ReactionEnrageBuff, projectile);
                    projData.castingSpecialAbilityTime += 100;
                }
            }
            else if (castSpecialTime > enrageStart && castSpecialTime < enrageEnd)
            {
                projData.castingSpecialAbilityTime += 100;
                if(projData.castingSpecialAbilityTime > enrageEnd)
                    projData.castingSpecialAbilityTime = enrageEnd;
            }
            else if (castSpecialTime == enrageEnd)
            {
                projFuncs.RemoveBuff(ProjectileBuffs.ProjectileBuffIDs.ReactionEnrageBuff, projectile);
                AdvanceCooldown(projectile, projFuncs, projData);
            }
            else
            {
                AdvanceCooldown(projectile, projFuncs, projData);
            }
        }

        void AdvanceCooldown(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            int CDTick;
            float denominator = projFuncs.GetMinionPower(projectile, 0);
            if (100 - denominator <= 0)
                CDTick = 100000000;
            else if (denominator < 0)
                CDTick = 0;
            else
            {
                CDTick = (int)(100 * denominator / (100 - denominator) * normalMultRatio);
            }
            bool initiallyAfterEnd = projData.castingSpecialAbilityTime >= enrageEnd;

            projData.castingSpecialAbilityTime += CDTick;

            //do wrappy and clampy
            if (initiallyAfterEnd && projData.castingSpecialAbilityTime >= enrageReset)
            {
                projData.castingSpecialAbilityTime -= enrageReset + 1;
                initiallyAfterEnd = false;
            }
            if(!initiallyAfterEnd && projData.castingSpecialAbilityTime > enrageStart)
                projData.castingSpecialAbilityTime = enrageStart;
        }
    }
}
