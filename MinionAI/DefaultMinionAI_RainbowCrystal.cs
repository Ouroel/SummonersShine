using Microsoft.Xna.Framework;
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
        public static void FadeOutTurretDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
		{
            if (projFuncs.killedTicks == 0)
            {
                projData.alphaOverride = projectile.alpha;
                Projectile newProj = CloneProjForDespawnEffect(projectile, projFuncs, projData, projectile.velocity);
                ReworkMinion_Projectile newProjFuncs = newProj.GetGlobalProjectile<ReworkMinion_Projectile>();
                MinionProjectileData newProjData = newProjFuncs.GetMinionProjData();
                newProj.timeLeft = newProj.GetFadeTime(newProjData);
                if (newProjData.alphaOverride == 0)
                    newProjData.alphaOverride = 1;
                newProj.netUpdate = true;
            }
        }
        public static void RainbowCrystalPostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (projFuncs.killedTicks > 0)
            {
                Player player = Main.player[projectile.owner];
                HandleFadeInOut(projectile, projFuncs, projData, false, true);
                //animate
                int num3 = projectile.frameCounter + 1;
                projectile.frameCounter = num3;
                if (num3 >= 3)
                {
                    projectile.frameCounter = 0;
                    num3 = projectile.frame + 1;
                    projectile.frame = num3;
                    if (num3 >= Main.projFrames[projectile.type])
                    {
                        projectile.frame = 0;
                    }
                }
            }
        }
    }
}
