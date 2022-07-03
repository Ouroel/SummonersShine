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
        const float truncateDist = 16 * 100;
        const float truncateDistSqr = truncateDist * truncateDist;

        const int FlinxAttackCD = 10;

        const int FlinxyFuryAttackCD = 15;
        public static void FlinxTerminateSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player, ReworkMinion_Player playerFuncs)
        {
            projectile.ai[1] = 0;
            projFuncs.RemoveBuff(ProjectileBuffIDs.FlinxyFuryBuff, projectile);

            projFuncs.originalNPCHitCooldown = FlinxAttackCD;
            projectile.localNPCHitCooldown = FlinxAttackCD;
        }
        const int FlinxyFuryExtraUpdates = 15;
        const int FlinxyFuryNumUpdates = FlinxyFuryExtraUpdates + 1;

        public static bool FlinxCustomAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (ModUtils.IsCastingSpecialAbility(projData, ItemID.FlinxStaff))
            {
                Player player = Main.player[projectile.owner];
                if (!projectile.tileCollide && !Collision.SolidTiles(projectile.position, projectile.width, projectile.height))
                    projectile.tileCollide = true;
                projFuncs.AddBuff(projectile, ProjectileBuffIDs.FlinxyFuryBuff, projectile);
                if (player.HasBuff(BuffID.FlinxMinion))
                {
                    projectile.timeLeft = 2;
                }
                else
                    player.flinxMinion = false;

                projectile.ai[0] = 0;
                
                if ((projectile.Center - player.Center).LengthSquared() > truncateDistSqr)
                {
                    projData.castingSpecialAbilityTime = -1;
                    FlinxTerminateSpecialAbility(projectile, projFuncs, projData, player, null);
                }

                projectile.rotation += 10 * projectile.spriteDirection;
                NPC target = projData.specialCastTarget;
                if (target == null || !target.active)
                {
                    int index = projectile.RandomMinionTarget();
                    if (index != -1)
                    {
                        target = Main.npc[index];
                        projData.specialCastTarget = target;
                    }
                    else
                        target = null;
                }

                if (target != null)
                {
                    if (projectile.ai[1] <= 0)
                    {
                        if (projectile.Hitbox.Intersects(target.Hitbox))
                        {
                            FlinxChangeTarget(projectile, projFuncs, projData);
                        }
                        Vector2 direction = target.position + new Vector2(Main.rand.NextFloat(0, target.width), Main.rand.NextFloat(0, target.height)) - projectile.Center;
                        direction.Normalize();
                        direction *= 4;
                        projectile.velocity = direction;
                    }
                }
                /*else if (projectile.velocity == Vector2.Zero) {
                    projectile.velocity = Main.rand.NextVector2Circular(4, 4);
                }
                else if (projectile.velocity.LengthSquared() != 16)
                {
                    projectile.velocity.Normalize();
                    projectile.velocity *= 4;
                }*/

                projectile.ai[1]--;
                if (projectile.IncrementSpecialAbilityTimer(projFuncs, projData, 120))
                {
                    FlinxTerminateSpecialAbility(projectile, projFuncs, projData, player, null);
                }
                projectile.extraUpdates = FlinxyFuryExtraUpdates;
                return false;
            }
            projectile.extraUpdates = 0;
            return true;
        }
        public static void FlinxOnSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity target, int specialType, bool fromServer)
        {
            projData.energy = 0;
            projData.specialCastTarget = (NPC)target;
            projData.castingSpecialAbilityTime = 0;
            projFuncs.AddBuff(projectile, ProjectileBuffIDs.FlinxyFuryBuff, projectile);

            projFuncs.originalNPCHitCooldown = FlinxyFuryAttackCD;
            projectile.localNPCHitCooldown = FlinxyFuryAttackCD * FlinxyFuryNumUpdates;
        }

        public static bool FlinxOnTileCollide(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Vector2 oldVelocity)
        {
            if (ModUtils.IsCastingSpecialAbility(projData, ItemID.FlinxStaff))
            {
                SingleThreadExploitation.flinx_justTileCollided = true;
                if (SingleThreadExploitation.flinxOldVel == Vector2.Zero)
                    SingleThreadExploitation.flinxOldVel = oldVelocity;
                return false;
            }
            return true;
        }

        public static void Flinx_OnSlopeCollide(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Vector4 newVel)
        {
            if (ModUtils.IsCastingSpecialAbility(projData, ItemID.FlinxStaff))
            {
                SingleThreadExploitation.flinx_justTileCollided = true;
                if (SingleThreadExploitation.flinxOldVel == Vector2.Zero)
                    SingleThreadExploitation.flinxOldVel = projectile.velocity;
            }
        }

        public static void FlinxOnMovement(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (!SingleThreadExploitation.flinx_justTileCollided)
                return;

            int index = FlinxChangeTarget(projectile, projFuncs, projData);
            if (index == -1) {
                Vector2 normal = projectile.velocity - SingleThreadExploitation.flinxOldVel;
                normal.Normalize();
                normal *= Vector2.Dot(normal, SingleThreadExploitation.flinxOldVel);
                projectile.velocity = (SingleThreadExploitation.flinxOldVel - normal - normal).RotatedBy(Main.rand.NextFloat(-MathF.PI / 6, MathF.PI / 6));
            }

            SingleThreadExploitation.flinxOldVel = Vector2.Zero;
            SingleThreadExploitation.flinx_justTileCollided = false;
        }

        static int FlinxChangeTarget(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            Gore.NewGoreDirect(projectile.GetSource_FromThis(), projectile.Center - new Vector2(25, 25), Vector2.Zero, ModEffects.SnowPuff);
            NPC oldNPC = projData.specialCastTarget;
            int oldIndex;
            if (oldNPC != null)
                oldIndex = oldNPC.whoAmI;
            else
                oldIndex = -1;
            int newIndex = projectile.RandomMinionTarget(oldIndex);
            if (newIndex != -1)
            {
                projData.specialCastTarget = Main.npc[newIndex];
                if (oldIndex != -1)
                {
                    NPC newNPC = projData.specialCastTarget;
                    if (oldNPC != null && newNPC != null && oldNPC.active && newNPC.active && (oldNPC.Center - newNPC.Center).LengthSquared() < 6400)
                    {
                        projectile.ai[1] = 10;
                        projectile.velocity = new Vector2(8, 0).RotatedBy(Main.rand.NextFloat(0, MathF.PI * 2)); 
                    }
                }
            }
            else
                projData.specialCastTarget = null;
            return newIndex;
        }
        public static void FlinxOnHitNPC(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (ModUtils.IsCastingSpecialAbility(projData, ItemID.FlinxStaff))
            {
                if (projData.specialCastTarget == null || projData.specialCastTarget == target)
                    FlinxChangeTarget(projectile, projFuncs, projData);
                knockback *= projFuncs.GetMinionPower(projectile, 0);
                hitDirection = MathF.Sign(target.Center.X - Main.player[projectile.owner].Center.X);
            }
        }
    }
}
