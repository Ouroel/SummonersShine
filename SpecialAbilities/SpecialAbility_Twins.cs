using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SummonersShine.BakedConfigs;
using SummonersShine.Effects;
using SummonersShine.ProjectileBuffs;
using SummonersShine.Projectiles;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace SummonersShine.SpecialAbilities
{
    public static partial class SpecialAbility
    {

        const float Spaz_Fireball_Speed = 5f;
        const int Spaz_Shooty_Dist = 192;
        const int Spaz_Avoid_Radius = 32 * 32;
        public static void Ret_Spaz_OnSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity _target, int specialType, bool fromServer)
        {
            projData.energy = 0;
            projData.castingSpecialAbilityTime = 0;
            projData.energyRegenRateMult = 0;
            projData.minionTrackingAcceleration = 10000;
            projData.minionTrackingImperfection = 2f;
        }

        public static void Ret_OnHitNPCWithProj(Projectile minion, Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (proj.type != ProjectileModIDs.SpazmaminiFireball)
                return;
            ReworkMinion_Projectile projFuncs = minion.GetGlobalProjectile<ReworkMinion_Projectile>();
            MinionProjectileData projData = projFuncs.GetMinionProjData();
            if (target == projData.moveTarget)
                return;
            if (Main.rand.Next(0, 101) < projFuncs.GetMinionPower(minion, 0))
            {
                Vector2 retHead = minion.Center + new Vector2(-24, 0).RotatedBy(minion.rotation);
                Vector2 diff = target.Center - retHead;
                diff.Normalize();
                diff *= 8;
                Entity originalTarget = projData.moveTarget;
                projData.moveTarget = target;
                Projectile.NewProjectileDirect(minion.GetSource_FromThis(), retHead, diff, ProjectileID.MiniRetinaLaser, minion.damage, minion.knockBack, minion.owner, 0, -1);
                projData.moveTarget = originalTarget;
            }
        }
        public static void RetaniminiPreAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (ModUtils.IsCastingSpecialAbility(projData, ItemID.OpticStaff))
            {
                if (projectile.IncrementSpecialAbilityTimer(projFuncs, projData, 420))
                {
                    Ret_Spaz_TerminateSpecialAbility(projectile, projFuncs, projData, null, null);
                }
            }
        }

        public static Vector2 Spaz_AimFireball(Projectile projectile, Vector2 targetCenter) {

            Vector2 diff = targetCenter - projectile.Center;
            float ticksReq = diff.Length() / Spaz_Fireball_Speed;
            diff /= ticksReq;
            diff.Y -= 0.5f * SpazmaminiFireball.grav * ticksReq;
            return diff;
        }
        public static void Spaz_ShootCursedFireball(Projectile projectile, ReworkMinion_Projectile projFuncs, Vector2 diff) {
            Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), projectile.Center, diff, ProjectileModIDs.SpazmaminiFireball, projectile.damage, projectile.knockBack, projectile.owner, 0, -1);
        }

        public static void Ret_Spaz_TerminateSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player, ReworkMinion_Player playerFuncs)
        {
            projData.minionTrackingAcceleration = 10;
            projData.minionTrackingImperfection = 5;
        }

        public static bool SpazmaminiAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (ModUtils.IsCastingSpecialAbility(projData, ItemID.OpticStaff))
            {
                Player player = Main.player[projectile.owner];

                bool dead = player.dead;
                if (dead)
                {
                    player.twinsMinion = false;
                }
                bool twinsMinion = player.twinsMinion;
                if (twinsMinion)
                {
                    projectile.timeLeft = 2;
                }

                ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
                if (projectile.IncrementSpecialAbilityTimer(projFuncs, projData, 420))
                {
                    Ret_Spaz_TerminateSpecialAbility(projectile, projFuncs, projData, null, null);
                    return true;
                }
                projectile.extraUpdates = 0;

                projectile.ai[1]++;
                int targetID = -1;
                projectile.Minion_FindTargetInRange(1400, ref targetID, true);
                if (targetID != -1)
                {
                    projectile.ai[0] = 0;

                    NPC target = Main.npc[targetID];
                    Vector2 center = target.Center;

                    Vector2 diff = Spaz_AimFireball(projectile, center);

                    if (projectile.ai[1] > 40f)
                    {
                        projectile.ai[1] = 0f;
                        Spaz_ShootCursedFireball(projectile, projFuncs, diff);
                    }
                    projectile.rotation = ReworkMinion_Projectile.GetTotalProjectileVelocity(diff, 1, projectile, target).ToRotation() + MathF.PI;

                    //move to spazzy shooty point
                    bool invertShovy = diff.X > 0;
                    float angle = 0;

                    MinionEnergyCounter counters = playerFuncs.GetMinionCollection(ItemID.OpticStaff);
                    Vector2 spazShootyVec = new Vector2(invertShovy ? -Spaz_Shooty_Dist : Spaz_Shooty_Dist, 0);

                    bool tooCramped;
                    Vector2 shootyPoint = ModUtils.GetLineCollision(center, spazShootyVec, out tooCramped);

                    bool ignore = false;
                    bool invertThis = false;
                    int dir = invertShovy ? 1 : -1;
                    counters.minions.ForEach(i => {
                        if (i == projectile)
                            ignore = true;
                        if (ignore)
                            return;
                        Vector2 projDiff = shootyPoint - i.Center;
                        float dist = projDiff.LengthSquared();
                        if (dist < Spaz_Avoid_Radius)
                        {
                            for (int x = 0; x < 10; x++)
                            {
                                angle = -angle;
                                if (!invertThis)
                                    angle += dir * MathF.PI / 9;

                                invertThis = !invertThis;

                                bool tooCramped;
                                shootyPoint = ModUtils.GetLineCollision(center, spazShootyVec.RotatedBy(angle), out tooCramped);
                                if (!Collision.SolidTiles(shootyPoint - projectile.Size * 0.5f, projectile.width, projectile.height))
                                    break;
                            }
                        }
                    });

                    diff = shootyPoint - projectile.Center;
                    diff.Normalize();
                    diff *= 14;
                    projectile.velocity = (projectile.velocity * 40 + diff) / 41;

                    return false;
                }
                else
                    projectile.ai[0] = 1;
            }
            return true;
        }
    }
}
