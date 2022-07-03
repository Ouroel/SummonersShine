using Microsoft.Xna.Framework;
using SummonersShine.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SummonersShine.SpecialData;
using Terraria.Graphics.Shaders;
using SummonersShine.SpecialAbilities;

namespace SummonersShine.MinionAI
{
    public static partial class DefaultMinionAI
    {
        public static void SharknadoSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {

            player.GetModPlayer<ReworkMinion_Player>().AddSpecialProjDrawInFrontEffects(projectile, projFuncs, projData, SpecialAbility.SharknadoOnPlayerDraw);
        }
        public static void SharknadoDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            player.GetModPlayer<ReworkMinion_Player>().RemoveSpecialProjDrawInFrontEffects(projectile);

            if (projFuncs.killedTicks == 0) {
                Projectile newProj = CloneProjForDespawnEffect(projectile, projFuncs, projData, projectile.velocity + projData.lastRelativeVelocity);
                ReworkMinion_Projectile newProjFuncs = newProj.GetGlobalProjectile<ReworkMinion_Projectile>();
                MinionProjectileData newProjData = newProjFuncs.GetMinionProjData();
                newProj.timeLeft = (int)(51 - newProjData.alphaOverride * 0.2f);
                if (newProjData.alphaOverride == 0)
                    newProjData.alphaOverride = 1;
                newProj.netUpdate = true;
            }
        }

        public static void SharknadoPostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            Player player = Main.player[projectile.owner];
            HandleFadeInOut(projectile, projFuncs, projData, false, true);
            if (projFuncs.killedTicks > 0)
            {
                projectile.frameCounter++;
                if (projectile.frameCounter >= 12)
                {
                    projectile.frameCounter = 0;
                }
                projectile.frame = projectile.frameCounter / 2;
                if (Main.rand.Next(5) == 0)
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.FishronWings, 0f, 0f, 100, default, 2f);
                    dust.velocity *= 0.3f;
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                }
            }
        }
    }
}
