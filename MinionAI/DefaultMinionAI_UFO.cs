using Microsoft.Xna.Framework;
using SummonersShine.BakedConfigs;
using SummonersShine.Effects;
using SummonersShine.Projectiles;
using SummonersShine.SpecialData;
using SummonersShine.Textures;
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
        static int[] UFOParticleIDs = { DustID.Water_Hallowed };

        public static void UFOSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            float dustSize = 0.5f;
            int sign = Main.rand.Next(0, 2) == 0 ? -1 : 1;
            Vector2 totalVel = projectile.velocity + Main.player[projectile.owner].GetRealPlayerVelocity();
            ModEffects.DrawArcWithParticles(projectile.Center, totalVel.ToRotation() + MathF.PI * 0.5f, Main.rand.NextFloat(0.1f, MathF.PI * 0.5f) * sign, 2, 256, CellParticleIDs, 120, i =>
            {
                i.noGravity = true;
                i.noLight = true;
                i.scale = (float)Main.rand.NextDouble() * 0.3f + dustSize;
                i.velocity *= 0.4f * 0.5f / dustSize;
                i.scale = 0.5f + (float)Main.rand.NextDouble() * 0.3f;
                i.frame.Y = 80;
                i.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                dustSize -= 0.004166f;
            });
        }
        public static void UFODespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            float dustSize = 0.5f;
            int sign = Main.rand.Next(0, 2) == 0 ? -1 : 1;
            Vector2 totalVel = projectile.velocity + Main.player[projectile.owner].velocity;
            ModEffects.DrawArcWithParticles(projectile.Center, totalVel.ToRotation() + MathF.PI * 0.5f, Main.rand.NextFloat(3.14f, MathF.PI * 10f) * sign, 2, 64, CellParticleIDs, 120, i =>
            {
                i.noGravity = true;
                i.noLight = true;
                i.scale = (float)Main.rand.NextDouble() * 0.3f + dustSize;
                i.velocity *= 0.4f * 0.5f / dustSize;
                i.scale = 0.5f + (float)Main.rand.NextDouble() * 0.3f;
                i.frame.Y = 80;
                i.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                dustSize -= 0.004166f;
            });

            if (projData.specialCastTarget != null)
            {
                NPC target = projData.specialCastTarget;
                if (!target.active || !target.CountsAsACritter || Main.npc[target.whoAmI] != target)
                {
                    return;
                }

                ReworkMinion_NPC targetFuncs = target.GetGlobalNPC<ReworkMinion_NPC>();
                target.dontTakeDamage = false;
                projData.specialCastTarget = null;
                targetFuncs.abducted = false;
            }
        }
        const float UFOlightTimeMax = 60 * 2;
        public static Color UFOModifyColor(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Color lightColor)
        {
            if (ModUtils.IsCastingSpecialAbility(projData, ItemID.XenoStaff))
            {
                float diffR = 255 - lightColor.R;
                float diffG = 255 - lightColor.G;
                float diffB = 255 - lightColor.B;
                float multVal = projData.castingSpecialAbilityTime / UFOlightTimeMax;
                diffR *= multVal;
                diffG *= multVal;
                diffB *= multVal;
                return new(lightColor.R + diffR, lightColor.G + diffG, lightColor.B + diffB);
            }
            return lightColor;
        }

        public static void UFOPostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            const int latchLength = 20;
            const int latchLenSqr = latchLength * latchLength * 16 * 16;
            const float distPercDrag = 0.1f;
            const int maxTargetVel = 8;
            const int maxTargetVelSqr = maxTargetVel * maxTargetVel;
            const int relVelKicksIn = 16;
            const int relVelKicksInUnits = relVelKicksIn * 16;

            projData.isTeleportFrame = projectile.ai[0] == 2f;

            //lightning speed boost expire
            if (ModUtils.IsCastingSpecialAbility(projData, ItemID.XenoStaff))
            {
                projData.castingSpecialAbilityTime--;
                if(Main.rand.NextBool(10))
                {
                    Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Electric);
                }
            }

            //tractor beam critters
            UFOSpecialDrawData specialDrawData = projFuncs.GetSpecialData<UFOSpecialDrawData>();
            if (specialDrawData.progress < UFOSpecialDrawData.maxProgress)
                specialDrawData.progress++;

            Vector2 targetPos;
            if (projData.specialCastTarget == null)
                targetPos = projectile.Center;
            else
                targetPos = projData.specialCastTarget.Center;
            specialDrawData.LerpTarget(targetPos - projectile.Center);

            if (projData.specialCastTarget != null)
            {
                NPC target = projData.specialCastTarget;
                if (!target.active || !target.CountsAsACritter || Main.npc[target.whoAmI] != target)
                {
                    projData.specialCastTarget = null;
                    specialDrawData.progress = 0;
                    return;
                }
                ReworkMinion_NPC targetFuncs = target.GetGlobalNPC<ReworkMinion_NPC>();
                if (target.Top.Y < projectile.Bottom.Y)
                {
                    target.dontTakeDamage = false;
                    projData.specialCastTarget = null;
                    targetFuncs.abducted = false;
                    specialDrawData.progress = 0;
                    return;
                }
                if (!targetFuncs.spawnedFromPlayer)
                    target.dontTakeDamage = true;
                else
                    target.dontTakeDamage = false;
                Vector2 diff = projectile.Center - target.Center;
                diff.Y += 16 * 5;
                float len = diff.Length();
                Vector2 vel;
                if (len == 0)
                {
                    vel = diff;
                }
                else if (len * distPercDrag < 4)
                    vel = diff / len * 4f;
                else
                    vel = diff * distPercDrag;

                if (len < relVelKicksInUnits)
                {
                    vel += (projectile.velocity + projData.lastRelativeVelocity) * (relVelKicksInUnits - len) / relVelKicksInUnits;
                }
                ModUtils.SyncedApplyPositionToNPC(target, target.position + vel);
                if (target.velocity.LengthSquared() > maxTargetVelSqr)
                {
                    ModUtils.SyncedApplyVelocityToNPC(target, -target.velocity * 0.9f);
                }
                return;
            }
            for (int x = 0; x < Main.maxNPCs; x++)
            {
                NPC test = Main.npc[x];
                if (test.active && !test.dontTakeDamage && test.CountsAsACritter && test.Center.DistanceSQ(projectile.Center) < latchLenSqr && projectile.Bottom.Y + 16 < test.Top.Y)
                {
                    ReworkMinion_NPC testFuncs = test.GetGlobalNPC<ReworkMinion_NPC>();
                    if (testFuncs.abducted)
                        continue;
                    testFuncs.abducted = true;
                    if (!testFuncs.spawnedFromPlayer)
                        test.dontTakeDamage = true;
                    projData.specialCastTarget = test;
                    specialDrawData.progress = 0;
                }
            }
        }

        public static bool UFOCustomDraw(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Color lightColor)
        {
            UFOSpecialDrawData specialDrawData = projFuncs.GetSpecialData<UFOSpecialDrawData>();
            if (projData.specialCastTarget == null && specialDrawData.progress == UFOSpecialDrawData.maxProgress)
                return true;
            const int maxLen = 30;
            const int maxLenSqr = maxLen * maxLen;
            Vector2 diff = specialDrawData.location;
            if (diff == Vector2.Zero)
                return true;
            Vector2 diffNormalized = new(0, 1);
            if (diff.LengthSquared() < maxLenSqr)
            {
                diffNormalized *= diff.Length() / maxLen;
            }
            Rectangle beamRect = new(0, 0, ModTextures.TractorBeam.Width, 60);
            Rectangle beamRectEnd = new(0, 60, ModTextures.TractorBeam.Width, 20);
            float rot = diff.ToRotation() - MathF.PI / 2;
            DrawUFOTractorBeam(projectile.Center, diff, beamRect, beamRectEnd, 0.5f, rot, 180, Color.LightGoldenrodYellow, 0.6f);
            float clamp1 = Math.Clamp(rot, -0.6f, 0.6f);
            float clamp2 = Math.Clamp(rot, -0.5f, 0.5f);
            DrawUFOTractorBeam(projectile.Center, diffNormalized.RotatedBy(clamp1) * 25, beamRect, beamRectEnd, 2f, clamp1, 150, Color.DeepSkyBlue, 0.8f);
            DrawUFOTractorBeam(projectile.Center, diffNormalized.RotatedBy(clamp2) * 10, beamRect, beamRectEnd, 4f, clamp2, 100, Color.LightGreen, 0.8f);
            return true;
        }

        static void DrawUFOTractorBeam(Vector2 beamRectStartPos, Vector2 beamRectDiff, Rectangle beamRect, Rectangle beamRectEnd, float scaleMult, float rot, float alpha, Color color, float fixedWidth = 0)
        {
            float scale = beamRectDiff.Length() / 60;
            Vector2 beamRectPos = beamRectDiff / 2 + beamRectStartPos;
            Vector2 beamRectEndPos = beamRectStartPos + beamRectDiff;
            beamRectDiff.Normalize();
            beamRectEndPos += beamRectDiff * 10;
            float minScale;
            minScale = Math.Min(scale, 1f) * scaleMult;
            ModTextures.JustDraw_Projectile(ModTextures.TractorBeam, beamRectPos, beamRect, new Vector2(minScale, scale), false, color, alpha, rot: rot);
            ModTextures.JustDraw_Projectile(ModTextures.TractorBeam, beamRectEndPos, beamRectEnd, new Vector2(minScale, 1), false, color, alpha, rot: rot);

        }
        public static void UFOSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity _target, int specialType, bool fromServer)
        {
            projData.energy = 0;
            if (Main.myPlayer == projectile.owner)
                Projectile.NewProjectile(projectile.GetSource_FromThis(), _target.position, Vector2.Zero, ProjectileModIDs.MartianShieldGenerator, projectile.damage, projectile.knockBack, projectile.owner);
        }
    }
}
