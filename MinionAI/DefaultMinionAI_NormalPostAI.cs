using Microsoft.Xna.Framework;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using SummonersShine.BakedConfigs;

namespace SummonersShine.MinionAI
{
    public static partial class DefaultMinionAI
    {
        public static void Default_PostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (BakedConfig.IgnoreTracking(projectile))
            {
                projData.trackingState = MinionTracking_State.noTracking;
            }
            else
            {
                projData.trackingState = (projData.moveTarget as NPC == null) ? MinionTracking_State.retreating : MinionTracking_State.normal;
            }
        }
        public static void Default_PreAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (BakedConfig.IgnoreTracking(projectile))
            {
            }
            else
            {
                ReworkMinion_Projectile.SetMoveTarget(projectile, Main.player[projectile.owner]);
            }
        }
        public static void Default_PostAI_NoTracking(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            projData.trackingState = MinionTracking_State.noTracking;
        }

        public static void Normal_PostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            int ai = (int)projectile.ai[0];

            //projData.trackingState = (ai == 1) ? MinionTracking_State.retreating : MinionTracking_State.normal;
            projData.trackingState = (ai == 1) ? MinionTracking_State.retreating : MinionTracking_State.normal;
        }

        public static void UFO_PostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            int ai = (int)projectile.ai[0];

            //projData.trackingState = (ai == 1) ? MinionTracking_State.retreating : MinionTracking_State.normal;
            if (ai == 1)
            {
                projData.trackingState = MinionTracking_State.retreating;
                projData.minionTrackingAcceleration = 0.1f;
                projData.minionTrackingImperfection = 5f;
            }
            else
            {
                projData.trackingState = MinionTracking_State.normal;
                projData.minionTrackingAcceleration = 10000f;
                projData.minionTrackingImperfection = 2f;
            }
        }
        public static void Groundbound_PostAI_Slime(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            switch (projectile.ai[0])
            {
                case 1:
                    projData.trackingState = MinionTracking_State.retreating;
                    projData.minionTrackingAcceleration = 0.1f;
                    projData.minionTrackingImperfection = 5f;
                    break;
                default:
                    projData.trackingState = MinionTracking_State.xOnly;
                    projData.minionTrackingAcceleration = 10f;
                    projData.minionTrackingImperfection = 5f;
                    break;
            }
        }
        public static void Groundbound_PostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            switch (projectile.ai[0])
            {
                case 1:
                    projData.trackingState = MinionTracking_State.retreating;
                    projData.minionTrackingAcceleration = 0.1f;
                    projData.minionTrackingImperfection = 10f;
                    break;
                default:
                    projData.trackingState = MinionTracking_State.xOnly;
                    projData.minionTrackingAcceleration = 10f;
                    projData.minionTrackingImperfection = 5f;
                    break;
            }
        }
        public static void Groundbound_PostAI_Tiger(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            switch (projectile.ai[0])
            {
                case 1:
                    projData.trackingState = MinionTracking_State.retreating;
                    projData.minionTrackingAcceleration = 0.1f;
                    projData.minionTrackingImperfection = 10f;
                    break;
                case 4:
                    projData.trackingState = MinionTracking_State.normal;
                    projData.minionTrackingAcceleration = 10f;
                    projData.minionTrackingImperfection = 5f;
                    break;
                default:
                    projData.trackingState = MinionTracking_State.xOnly;
                    projData.minionTrackingAcceleration = 10f;
                    projData.minionTrackingImperfection = 5f;
                    break;
            }
        }

        public static void StardustGuardian_PostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            projData.trackingState = (projData.moveTarget as NPC == null) ? MinionTracking_State.noTracking : MinionTracking_State.normal;
        }

        public static void Smolchiunstars_PostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (projectile.ai[0] == 60)
            {
                projData.trackingState = MinionTracking_State.normal;
                projData.minionTrackingImperfection = 6f;
                return;
            }
            projData.trackingState = MinionTracking_State.retreating;
            projData.minionTrackingImperfection = 20f;
            ReworkMinion_Projectile.SetMoveTarget_FromID(projectile, -1);
        }
        public static void StormTiger_PostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            int ai = (int)projectile.ai[0];
            projData.trackingState = ai == 1 ? MinionTracking_State.retreating : ai == 4 ? MinionTracking_State.noTracking : MinionTracking_State.normal;
        }

        public static void HandleFadeInOut(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, bool scale, bool alpha, int magnitude = 5)
        {
            if (!projectile.IsOnRealTick(projData))
                return;
            int direction = 1;// shouldScaleInstead ? -1 : 1;
            if (projFuncs.killedTicks > 0)
            {
                direction *= -1;
            }
            if (projData.alphaOverride == -1)
            {
                projData.alphaOverride = 128 + 126 * direction;
            }
            else if (projData.alphaOverride > 0 && projData.alphaOverride < 255)
            {
                projData.alphaOverride -= magnitude * direction;
                projData.alphaOverride = Math.Clamp(projData.alphaOverride, 0, 255);
            }
            if (scale)
                projectile.scale = 1 - projData.alphaOverride / 255f;
            if (alpha)
                projectile.alpha = projData.alphaOverride;
        }

        public static int GetFadeTime(this Projectile projectile, MinionProjectileData projData) {
            return (int)(51 - projData.alphaOverride * 0.2f) * (projectile.extraUpdates + 1);
        }

        public static Projectile CloneProjForDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Vector2 velocityOverwrite) {
            Projectile newProj = Projectile.NewProjectileDirect(null, projectile.Center, velocityOverwrite, projectile.type, 0, 0, projectile.owner, projectile.ai[0], projectile.ai[1]);
            if (newProj != null)
            {
                ReworkMinion_Projectile newProjFuncs = newProj.GetGlobalProjectile<ReworkMinion_Projectile>();
                MinionProjectileData newProjData = newProjFuncs.GetMinionProjData();
                newProj.Center = projectile.Center;
                newProj.rotation = projectile.rotation;
                newProj.frameCounter = projectile.frameCounter;
                newProj.frame = projectile.frame;
                newProj.originalDamage = 0;
                newProj.minionSlots = 0;
                newProjData.alphaOverride = projData.alphaOverride;
                newProjFuncs.killedTicks = 1;
                newProjFuncs.killed = true;
                projectile.localAI.CopyTo(newProj.localAI, 0);
                newProj.netUpdate = true;
            }
            return newProj;
        }
    }
}
