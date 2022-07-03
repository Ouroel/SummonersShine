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

namespace SummonersShine.MinionAI
{
    public static partial class DefaultMinionAI
    {
        static int[] ImpSigilParticles = {
            DustID.SolarFlare,
            DustID.DemonTorch,
            DustID.Torch,
        };
        public static bool ImpCustomAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (ModUtils.IsCastingSpecialAbility(projData, ItemID.ImpStaff))
            {
                Player player = Main.player[projectile.owner];
                projectile.IncrementSpecialAbilityTimer(projFuncs, projData, 420);

                if (projData.specialCastTarget == null || projData.specialCastTarget.active == false || projData.specialCastTarget.DistanceSQ(player.Center) > 1000 * 1000)
                {
                    int newtarg = -1;
                    projectile.Minion_FindTargetInRange(1400, ref newtarg, false);
                    if (newtarg != -1)
                        projData.specialCastTarget = Main.npc[newtarg];
                    else
                        projData.specialCastTarget = null;
                }
                if (projData.specialCastTarget == null)
                    return true;

                if (projectile.ai[1] > 88 || projectile.ai[1] == 0)
                {
                    Vector2 projOriginalPos = projectile.Center;

                    NPC origTarget = projData.specialCastTarget;
                    projectile.Center = origTarget.Center;
                    if (!origTarget.CanBeChasedBy(projectile) || Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                    {

                        projectile.Center = projOriginalPos;

                        int targ = -1;
                        projectile.Minion_FindTargetInRange(1400, ref targ, false);
                        if (targ == -1)
                            return true;
                        origTarget = Main.npc[targ];
                        projectile.Center = origTarget.Center;
                    }
                    Vector2 targetPos = origTarget.Center;

                    int attackTarget = -1;

                    //trick to exclude special cast target
                    bool immune = origTarget.chaseable;
                    origTarget.chaseable = false;
                    projectile.Minion_FindTargetInRange(1400, ref attackTarget, false);
                    origTarget.chaseable = immune;

                    Vector2 line;
                    float dist = Main.rand.NextFloat(8, 24);
                    bool failed = false;
                    if (attackTarget != -1)
                    {
                        NPC target = Main.npc[attackTarget];
                        SingleThreadExploitation.impTarget = player.MinionAttackTargetNPC;
                        player.MinionAttackTargetNPC = attackTarget;

                        line = targetPos - target.Center;
                        line.Normalize();
                        line *= dist;
                    }
                    else
                    {
                        line = new Vector2(0, dist).RotatedBy(Main.rand.NextFloat(-MathF.PI, MathF.PI));
                        SingleThreadExploitation.impTarget = player.MinionAttackTargetNPC;
                        player.MinionAttackTargetNPC = attackTarget;
                        failed = true;
                    }

                    line *= 16;
                    /*
                    bool tooCramped = true;
                    Vector2 endPos = Vector2.Zero;
                    int give = 48;
                    for (int i = 0; i < 16; i++) {
                        endPos = ModUtils.GetLineCollision(targetPos, line, out tooCramped, give);
                        if (!tooCramped)
                        {
                            NPC targ = projData.specialCastTarget;
                            tooCramped = !Collision.CanHitLine(projectile.position, projectile.width, projectile.height, targ.position, targ.width, targ.height);
                        }
                        if (!tooCramped)
                            break;
                        else
                        {
                            line = new Vector2(0, dist).RotatedBy(Main.rand.NextFloat(-MathF.PI, MathF.PI)) * 16;
                        }
                    }
                    if (tooCramped)
                    {
                        projectile.Center = projOriginalPos;
                        return true;
                    }*/


                    Vector2 endPos = ModUtils.FurthestCanHitLine(targetPos, line);
                    bool tooFar = FindImpCenterDiffSqr(player.Center, player.direction, endPos, projectile) > 1000 * 1000;

                    if (failed)
                    {
                        for (int x = 0; x < 16; x++)
                        {
                            if (!tooFar && (endPos - targetPos).LengthSquared() > dist * dist * 64)
                            {
                                break;
                            }
                            dist = Main.rand.NextFloat(8, 24);
                            line = new Vector2(0, dist).RotatedBy(Main.rand.NextFloat(-MathF.PI, MathF.PI)) * 16;
                            endPos = ModUtils.FurthestCanHitLine(targetPos, line);
                            tooFar = FindImpCenterDiffSqr(player.Center, player.direction, endPos, projectile) > 1000 * 1000;
                        }
                    }

                    if (tooFar)
                    {
                        projectile.Center = projOriginalPos;
                        return true;
                    }


                    projectile.ai[1] = 90;
                    projectile.ai[0] = 0;

                    projectile.velocity = Vector2.Zero;
                    projData.lastRelativeVelocity = Vector2.Zero;

                    projectile.Center = projOriginalPos;
                    ImpDespawnEffect(projectile, projFuncs, projData, player);
                    projectile.Center = endPos;
                    ImpDespawnEffect(projectile, projFuncs, projData, player);


                }
                else {
                    SingleThreadExploitation.impTarget = player.MinionAttackTargetNPC;
                    player.MinionAttackTargetNPC = projData.specialCastTarget.whoAmI;
                }
            }
            return true;
        }

        static float FindImpCenterDiffSqr(Vector2 center, int playerDir, Vector2 projCenter, Projectile projectile) {
            Vector2 rv = center - projCenter;

            int projPos = 1;

            for (int m = 0; m < projectile.whoAmI; m++)
            {
                if (Main.projectile[m].active && Main.projectile[m].owner == projectile.owner && Main.projectile[m].type == projectile.type)
                {
                    projPos++;
                }
            }

            rv.X -= (float)(10 * playerDir);
            rv.X -= (float)(projPos * 40 * playerDir);
            rv.Y -= 10f;

            return rv.LengthSquared();
        }
        public static void ImpOnShootProjectile(Projectile projectile, Projectile source, ReworkMinion_Projectile sourceFuncs, MinionProjectileData sourceData)
        {
            if (ModUtils.IsCastingSpecialAbility(sourceData, ItemID.ImpStaff))
            {
                projectile.ai[1] = sourceFuncs.GetMinionPower(source, 0);
            }
        }

        public static void ImpPostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (SingleThreadExploitation.impTarget != -2)
            {
                Player player = Main.player[projectile.owner];
                player.MinionAttackTargetNPC = SingleThreadExploitation.impTarget;
                SingleThreadExploitation.impTarget = -2;
            }
        }
        public static void ImpSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            //gore
            DrawImpSigil(projectile.Center, i => {
                Vector2 difference = i.position - projectile.Center - Vector2.Normalize(projectile.velocity) * 64;
                i.velocity = difference * 0.01f;
                i.noGravity = true;
                i.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                i.scale = 1.5f;
            });
        }
        public static void ImpDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            DrawImpTeleportSigil(projectile.Center, i => {
                Vector2 difference = i.position - projectile.Center;
                i.velocity = difference * 0.01f;
                i.noGravity = true;
                i.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                i.scale = 1.5f;
            });
        }

        static void DrawImpSigil(Vector2 center, Action<Dust> OnCreate)
        {
            float angle = Main.rand.NextFloat(6.283f);
            ModEffects.DrawArcWithParticles(center, angle, Main.rand.NextFloat(4.398f, 6.6f), Main.rand.NextFloat(32, 48), Main.rand.NextFloat(32, 48), ImpSigilParticles, 48, OnCreate);
            int maxSmallCircles = Main.rand.Next(2, 6);
            for (int x = 0; x < maxSmallCircles; x++)
            {
                angle = Main.rand.NextFloat(6.283f);
                Vector2 pos = center + new Vector2(0, Main.rand.NextFloat(32, 48)).RotatedBy(Main.rand.NextFloat(6.283f));
                ModEffects.DrawArcWithParticles(pos, angle, Main.rand.NextFloat(4.398f, 6.6f), Main.rand.NextFloat(16, 32), Main.rand.NextFloat(16, 32), ImpSigilParticles, 24, OnCreate);
            }
            int maxLines = Main.rand.Next(3, 7);
            Vector2 newPos = center + new Vector2(0, Main.rand.NextFloat(0, 80)).RotatedBy(Main.rand.NextFloat(6.283f));
            for (int x = 0; x < maxLines; x++)
            {
                Vector2 pos = center + new Vector2(0, Main.rand.NextFloat(0, 80)).RotatedBy(Main.rand.NextFloat(6.283f));
                ModEffects.DrawLineWithParticles(pos, newPos, ImpSigilParticles, 15, OnCreate);
                newPos = pos;
            }
        }
        static void DrawImpTeleportSigil(Vector2 center, Action<Dust> OnCreate)
        {
            float angle = Main.rand.NextFloat(6.283f);
            ModEffects.DrawArcWithParticles(center, angle, Main.rand.NextFloat(4.398f, 6.6f), Main.rand.NextFloat(12, 24), Main.rand.NextFloat(12, 24), ImpSigilParticles, 16, OnCreate);
            int maxLines = Main.rand.Next(2, 4);
            Vector2 newPos = center + new Vector2(0, Main.rand.NextFloat(0, 30)).RotatedBy(Main.rand.NextFloat(6.283f));
            for (int x = 0; x < maxLines; x++)
            {
                Vector2 pos = center + new Vector2(0, Main.rand.NextFloat(0, 30)).RotatedBy(Main.rand.NextFloat(6.283f));
                ModEffects.DrawLineWithParticles(pos, newPos, ImpSigilParticles, 15, OnCreate);
                newPos = pos;
            }
        }
    }
}
