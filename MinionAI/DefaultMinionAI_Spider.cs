using Microsoft.Xna.Framework;
using SummonersShine.BakedConfigs;
using SummonersShine.Effects;
using SummonersShine.SpecialAbilities;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.Utils;

namespace SummonersShine.MinionAI
{
    public static partial class DefaultMinionAI
    {
        const float max_dist_sqr = 1690000;
        public static void SpiderSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            //gore
            int rand = Main.rand.Next(0, 2);
            int GoreID = ModEffects.SpiderEgg[rand];
            int index = Gore.NewGore(projectile.GetSource_FromThis(), projectile.position, new Vector2(Main.rand.NextFloatDirection(), 0.0f), GoreID, projectile.scale);
            for (int i = 0; i < 20; i++)
                Dust.NewDustDirect(projectile.Center + Main.rand.NextVector2Circular(8, 8), projectile.width, projectile.height, DustID.Web, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, Color.White, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
        }
        public static void SpiderDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            //gore
            int GoreID = ModEffects.SpiderCocoon;
            Gore.NewGore(projectile.GetSource_FromThis(), projectile.position, new Vector2(0.01f, 0.0f), GoreID, projectile.scale);
            for (int i = 0; i < 20; i++)
                Dust.NewDustDirect(projectile.Center + Main.rand.NextVector2Circular(8, 8), projectile.width, projectile.height, DustID.Web, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, Color.White, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
        }

        static void RotateSpiderTowardsVector(Projectile projectile, Vector2 vector) {
            if (projectile.frame > 3 && projectile.frame < 8) {
                projectile.rotation = vector.ToRotation() + 1.5707964f;
                return;
            }
            projectile.spriteDirection = vector.X < 0 ? 1 : -1;
        }
        public static void Groundbound_PreAI_Spider(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            projData.trackingState = projData.castingSpecialAbilityTime != -1 ? MinionTracking_State.noTracking : (projectile.ai[0] == 1) ? MinionTracking_State.retreating : projData.trackingState == MinionTracking_State.normal || (projectile.frame > 3 && projectile.frame < 8) ? MinionTracking_State.normal : MinionTracking_State.xOnly;
            Player player = Main.player[projectile.owner];
            player.GetModPlayer<ReworkMinion_Player>().SavePlayerData(player);
            player.rocketDelay2 = 0;
        }

        public static void Groundbound_PostAI_Spider(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            Player player = Main.player[projectile.owner];

            player.GetModPlayer<ReworkMinion_Player>().LoadPlayerData(player);
            if (BakedConfig.CustomSpecialPowersEnabled(ItemID.SpiderStaff))
            {
                if (projData.cancelSpecialNextFrame)
                {
                    player.MinionAttackTargetNPC = projData.actualMinionAttackTargetNPC;
                    projData.energyRegenRateMult = 1;
                    projData.castingSpecialAbilityTime = -1;
                    projData.specialCastTarget = null;
                    projData.energy = projData.maxEnergy;
                    projData.cancelSpecialNextFrame = false;
                }
                if (projData.castingSpecialAbilityTime != -1)
                {
                    player.MinionAttackTargetNPC = projData.actualMinionAttackTargetNPC;
                    if (!projData.specialCastTarget.active || projData.specialCastTarget.DistanceSQ(player.position) > max_dist_sqr)
                    {
                        projData.energyRegenRateMult = 1;
                        projData.castingSpecialAbilityTime = -1;
                        projData.specialCastTarget = null;
                    }
                    else if(projectile.IsOnRealTick(projData))
                    {
                        projData.castingSpecialAbilityTime++;

                        Vector2 direction = projData.specialCastTarget.Center - projectile.Center;
                        if (direction != Vector2.Zero)
                        {
                            Vector2 webPos;
                            if (projData.castingSpecialAbilityTime > SpecialAbility.spiderWebFormationTime)
                            {
                                webPos = projectile.Center + direction;
                                if (direction.LengthSquared() >= 1500)
                                {
                                    float remaining = projData.castingSpecialAbilityTime - SpecialAbility.spiderWebFormationTime;
                                    if (remaining > SpecialAbility.spiderDragInTime)
                                        remaining = SpecialAbility.spiderDragInTime;
                                    projectile.velocity = direction * MathHelper.Clamp(remaining / SpecialAbility.spiderDragInTime, 0, 1);
                                }
                            }
                            else
                            {
                                RotateSpiderTowardsVector(projectile, direction);
                                float webProgress = projData.castingSpecialAbilityTime / SpecialAbility.spiderWebFormationTime;
                                float stringyWebProgress = MathHelper.Clamp(webProgress, 0, 1);
                                direction *= stringyWebProgress;
                                webPos = projectile.Center + direction;
                                for (int x = 0; x < 5; x++)
                                    Dust.NewDust(webPos, 1, 1, DustID.Web);
                            }
                            if (!Collision.CanHitLine(projectile.Center, 0, 0, webPos, 0, 0) && !Collision.CanHitLine(projectile.Center + new Vector2(0, -16), 0, 0, webPos, 0, 0))
                            {
                                projData.cancelSpecialNextFrame = true;
                                for (int x = 0; x < 10; x++)
                                    Dust.NewDust(webPos, 1, 1, DustID.Web);
                            }
                        }

                        if (projData.castingSpecialAbilityTime >= 600)
                        {
                            projData.energyRegenRateMult = 1;
                        }

                        if (projData.castingSpecialAbilityTime >= projFuncs.GetMinionPower(projectile, 0) * 60)
                        {
                            projData.energyRegenRateMult = 1;
                            projData.castingSpecialAbilityTime = -1;
                            projData.specialCastTarget = null;
                        }
                    }
                }
            }

            projData.minionTrackingImperfection = (projData.moveTarget == Main.player[projectile.owner]) ? 10 : 0;
        }
    }
}
