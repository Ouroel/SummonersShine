using Microsoft.Xna.Framework;
using SummonersShine.BakedConfigs;
using SummonersShine.Effects;
using SummonersShine.Projectiles;
using SummonersShine.Projectiles.MiniPirateData;
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

namespace SummonersShine.MinionAI
{
    public static partial class DefaultMinionAI
    {
        public static void PirateSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            //gore
            for (int i = 0; i < 15; i++)
            {
                Color color = Color.Brown;
                switch (Main.rand.Next(0, 4))
                {
                    case 0:
                        color = Color.Gold;
                        break;
                    case 1:
                        color = Color.Silver;
                        break;
                    case 2:
                        color = Color.AntiqueWhite;
                        break;
                }
                Dust.NewDustDirect(projectile.Center, projectile.width, projectile.height, ModEffects.Coin, Main.rand.NextFloatDirection() * 0.5f, -5, 0, color, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            }
            for (int i = 0; i < 7; i++)
            {
                Color color = Color.Gold;
                switch (Main.rand.Next(0, 4))
                {
                    case 0:
                        color = Color.LightSeaGreen;
                        break;
                    case 1:
                        color = Color.OrangeRed;
                        break;
                    case 2:
                        color = Color.BlueViolet;
                        break;
                }
                Dust.NewDustDirect(projectile.Center, projectile.width, projectile.height, ModEffects.Gem, Main.rand.NextFloatDirection() * 0.5f, -5, 0, color, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            }
        }

        public static void PirateDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            PirateSummonEffect(projectile, projFuncs, projData, player);
            ReworkMinion_Player ownerData = player.GetModPlayer<ReworkMinion_Player>();
            PirateStat stats = projFuncs.GetSpecialData<PirateStat>();
            PirateStatCollection collection = ownerData.GetSpecialData<PirateStatCollection>();
            collection.Remove(stats);
        }

        public static void PiratePreAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (BakedConfig.CustomSpecialPowersEnabled(ItemID.PirateStaff))
            {
                Player player = Main.player[projectile.owner];
                PirateStat stat = projFuncs.GetSpecialData<PirateStat>();
                PirateStatCollection pirateCollection = player.GetModPlayer<ReworkMinion_Player>().GetSpecialData<PirateStatCollection>();
                ReworkMinion_Projectile ship = pirateCollection.megaMinion;

                if (pirateCollection.CanFindEnemyTarget && projData.castingSpecialAbilityTime < -1)
                    projData.castingSpecialAbilityTime = -1;

                if (projData.castingSpecialAbilityTime >= -1 && ship != null && ship.killedTicks == 0)
                {
                    Projectile boatBody = pirateCollection.megaMinionBody;

                    if (player.Center.DistanceSQ(projectile.Center) < 921600)
                    {
                        Vector2 pos = boatBody.Top + new Vector2(boatBody.width * 0.5f * boatBody.direction, -24);

                        player.GetModPlayer<ReworkMinion_Player>().SavePlayerData(player);
                        player.justJumped = false;
                        if (projData.specialCastTarget != null)
                            player.Center = projData.specialCastTarget.Center;
                        else
                            player.Center = pos;
                        player.fallStart = (int)(player.position.Y / 16);
                        player.velocity = Vector2.Zero;

                        float lowest = projFuncs.GetMinionProjData().moveTarget.Bottom.Y;
                        if (player.Bottom.Y > lowest)
                            lowest = player.Bottom.Y;

                        if (projData.castingSpecialAbilityTime == -1)
                        {
                            if (projData.specialCastPosition.Y == 0)
                            {
                                float posDiff = player.Center.X - projectile.Center.X;
                                if (lowest - projectile.position.Y > 0 && posDiff < 8 && posDiff > -8)
                                {
                                    projData.specialCastPosition.Y = 1;
                                    projectile.ai[0] = 0;
                                    player.rocketDelay2 = 0;
                                }
                                else {
                                    projectile.ai[0] = 1;
                                    player.velocity.Y = -1; //prevent setting ai[0] to 0
                                }
                            }
                            else if (projData.specialCastPosition.Y == 1)
                            {
                                player.rocketDelay2 = 0;
                                if (lowest - projectile.position.Y < -256)
                                {
                                    projectile.ai[0] = 1;
                                    projData.specialCastPosition.Y = 0;
                                    player.velocity.Y = -1;
                                }
                            }
                        }
                    }
                }
                if (projData.specialCastTarget != null)
                {
                    projectile.ai[0] = 2;
                    stat.SaveProjectileData(projectile);
                }
            }
        }
        public static void PirateEndOfAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (BakedConfig.CustomSpecialPowersEnabled(ItemID.PirateStaff))
            {
                Player player = Main.player[projectile.owner];
                PirateStat stat = projFuncs.GetSpecialData<PirateStat>();
                PirateStatCollection pirateCollection = player.GetModPlayer<ReworkMinion_Player>().GetSpecialData<PirateStatCollection>();

                stat.LoadProjectileData(projectile);
                if (projData.castingSpecialAbilityTime >= 0)
                {
                    int numUpdates = projectile.extraUpdates + 1;
                    if (projData.castingSpecialAbilityTime > 1 * numUpdates && projData.castingSpecialAbilityTime % numUpdates == 0)
                    {
                        Dust dust = Dust.NewDustDirect(projectile.Bottom, 1, 1, DustID.Smoke);
                        dust.velocity *= 0.01f;
                        dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                    }
                    else if (projData.castingSpecialAbilityTime == 0 && pirateCollection.kickPirateWrapper != null)
                    {
                        pirateCollection.kickPirateWrapper.Remove(projectile);
                    }
                    projectile.velocity = stat.GetKickVel(projectile, projFuncs, projData);
                    projectile.frame = 4 + ((projData.castingSpecialAbilityTime / 2) % 3);
                }
                else if (projData.castingSpecialAbilityTime < -1)
                {
                    projData.castingSpecialAbilityTime++;
                }
            }
        }

        public static void PiratePostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {

            if (BakedConfig.CustomSpecialPowersEnabled(ItemID.PirateStaff))
            {
                Player player = Main.player[projectile.owner];
                ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
                PirateStat stat = projFuncs.GetSpecialData<PirateStat>();
                PirateStatCollection pirateCollection = player.GetModPlayer<ReworkMinion_Player>().GetSpecialData<PirateStatCollection>();

                Projectile boat = pirateCollection.megaMinionBody;

                playerFuncs.LoadPlayerData(player);
                if (pirateCollection.megaMinionBody == null)
                    return;

                MiniPirate.MiniPirateEventWrapper_KickPirate wrapper = pirateCollection.kickPirateWrapper;

                if (wrapper != null)
                {
                    if (projData.castingSpecialAbilityTime == -1 && (boat.Center - projectile.Center).LengthSquared() < 10000)
                        pirateCollection.kickPirateWrapper.Add(projectile);
                    else
                        pirateCollection.kickPirateWrapper.Remove(projectile);
                }

                //land on boat
                if (projectile.whoAmI <= pirateCollection.megaMinionBody.whoAmI)
                {
                    PirateLandOnBoat(projectile, projFuncs, projData);
                }

                if (projData.castingSpecialAbilityTime >= 0)
                {
                    projData.trackingState = MinionTracking_State.noTracking;
                }
                else
                    projData.trackingState = (projectile.ai[0] == 1) ? MinionTracking_State.retreating : MinionTracking_State.xOnly;
            }
        }
        public static void PiratePostAIPostRelativeVelocity(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (BakedConfig.CustomSpecialPowersEnabled(ItemID.PirateStaff))
            {
                Player player = Main.player[projectile.owner];
                ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
                PirateStat stat = projFuncs.GetSpecialData<PirateStat>();
                PirateStatCollection pirateCollection = player.GetModPlayer<ReworkMinion_Player>().GetSpecialData<PirateStatCollection>();

                //land on boat
                if (pirateCollection.megaMinionBody != null && projectile.whoAmI > pirateCollection.megaMinionBody.whoAmI)
                {
                    PirateLandOnBoat(projectile, projFuncs, projData);
                }

            }
        }

        static void PirateLandOnBoat(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData) {
            if (projData.castingSpecialAbilityTime == -1)
            {
                if (!projectile.shouldFallThrough)
                {
                    Tuple<bool, Vector2> result = PlatformCollection.TestPlatformCollision(projectile);
                    if (result.Item1)
                    {
                        projectile.velocity = result.Item2;
                        projData.trackingState = MinionTracking_State.noTracking;
                    }
                }
            }
            else
            {
                PlatformCollection.UnattachAllPlatforms(projectile);
            }
        }
    }
}
