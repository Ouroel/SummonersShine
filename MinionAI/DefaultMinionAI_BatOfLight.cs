using Microsoft.Xna.Framework;
using SummonersShine.Effects;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.MinionAI
{
    public static partial class DefaultMinionAI
    {
        public static void BatOfLight_DespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            if (projFuncs.killedTicks == 0)
            {
                Vector2 velOverwrite;
                if (projectile.type == ProjectileID.BatOfLight)
                {
                    velOverwrite = Main.rand.NextVector2Circular(0.5f, 0.5f);
                }
                else
                {
                    Vector2 displacement = projectile.Center - Main.player[projectile.owner].Center;
                    float originalDistance = displacement.Length();
                    if (originalDistance > 0)
                    {
                        float distance = MathF.Min(originalDistance * 0.0111f, 0.5f);
                        displacement *= distance / originalDistance;
                    }
                    velOverwrite = displacement.RotatedBy(Main.rand.NextFloatDirection());
                }
                Projectile newProj = CloneProjForDespawnEffect(projectile, projFuncs, projData, velOverwrite);
                if (newProj != null)
                {
                    ReworkMinion_Projectile newProjFuncs = newProj.GetGlobalProjectile<ReworkMinion_Projectile>();
                    MinionProjectileData newProjData = newProjFuncs.GetMinionProjData();
                    if (projectile.type == ProjectileID.BatOfLight)
                    {
                        newProj.timeLeft = newProj.GetFadeTime(newProjData);
                        if (newProjData.alphaOverride == 0)
                            newProjData.alphaOverride = 1;
                        newProj.spriteDirection = projectile.spriteDirection;
                        newProj.netUpdate = true;
                    }
                    else
                    {
                        newProj.timeLeft = (int)(51 - newProjData.alphaOverride * 0.2f);
                        if (newProjData.alphaOverride == 0)
                            newProjData.alphaOverride = 1;
                        newProj.netUpdate = true;
                    }
                }
            }
        }
        public static void BatofLight_PostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            Player player = Main.player[projectile.owner];

            //effects
            bool shouldScaleInstead = false;
            if (projectile.type == ProjectileID.BatOfLight)
                shouldScaleInstead = true;

            HandleFadeInOut(projectile, projFuncs, projData, shouldScaleInstead, !shouldScaleInstead);

            if (projFuncs.killedTicks > 0)
            {
                if (projectile.type == ProjectileID.EmpressBlade)
                {
                    projectile.rotation += projectile.velocity.ToRotation() * 0.02f;
                    projectile.velocity *= 1.05f;
                    projectile.ai[0] = 40 + projectile.timeLeft * 0.6f;
                    int dustID = Dust.NewDust(projectile.Center + Main.rand.NextVector2Circular(48, 48), 1, 1, DustID.RainbowMk2, 0, 0, 0, Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f, byte.MaxValue));
                    Dust dust = Main.dust[dustID];
                    dust.velocity *= Main.rand.NextFloat() * 0.8f;
                    dust.noGravity = true;
                    dust.scale = 0.9f + Main.rand.NextFloat() * 1.2f;
                    dust.fadeIn = 0.4f + Main.rand.NextFloat() * 1.2f * MathHelper.Lerp(1.3f, 0.7f, projectile.Opacity);
                    dust.velocity += Vector2.UnitY * -2f;
                    dust.scale = 0.15f;
                    bool flag149 = dustID != 6000;
                    if (flag149)
                    {
                        Dust dust2 = Dust.CloneDust(dustID);
                        dust2.scale /= 2f;
                        dust2.fadeIn *= 0.85f;
                        dust2.color = new Color(255, 255, 255, 255);
                    }
                }
                else
                {
                    projectile.velocity *= 1.05f;
                    //continue animating the bat
                    int frames = projectile.frameCounter + 1;
                    projectile.frameCounter = frames;
                    if (frames >= 6)
                    {
                        projectile.frameCounter = 0;
                        frames = projectile.frame + 1;
                        projectile.frame = frames;
                        if (frames >= Main.projFrames[projectile.type] - 1)
                        {
                            projectile.frame = 0;
                        }
                    }
                }
            }



            int ai = (int)projectile.ai[0];
            int ai1 = (int)projectile.ai[1];
            projData.trackingState = ai == -1 ? MinionTracking_State.retreating : MinionTracking_State.noTracking;
            projData.minionSpeedModType = (ai1 == 0 && ai == 0) ? MinionSpeedModifier.none : MinionSpeedModifier.stepped;
        }
    }
}
