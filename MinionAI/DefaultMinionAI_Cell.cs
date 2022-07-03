using Microsoft.Xna.Framework;
using SummonersShine.Effects;
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
        static readonly int[] CellParticleIDs = { DustID.Vortex };

        public static void CellSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            if (projectile.ai[0] == 2)
                return;
            float dustSize = 2f;
            int sign = Main.rand.Next(0, 2) == 0 ? -1 : 1;
            Vector2 totalVel = projectile.velocity + Main.player[projectile.owner].GetRealPlayerVelocity();
            ModEffects.DrawArcWithParticles(projectile.Center, totalVel.ToRotation() + MathF.PI * 0.5f, Main.rand.NextFloat(0.1f, MathF.PI * 0.3f) * sign, 2, 80, CellParticleIDs, 15, i =>
            {
                i.noGravity = true;
                i.scale = 0.7f + Main.rand.NextFloat();
                i.velocity = Vector2.Zero;
                i.fadeIn = dustSize / 2f;
                i.scale = dustSize;
                i.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                dustSize -= 0.12f;
            });
            projData.alphaOverride = 155;
        }
        public static void CellDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustDirect(projectile.Center, projectile.width, projectile.height, DustID.Vortex, 0, 0, 0, default, 1f).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            }
        }

        public static void CellPostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            projData.isTeleportFrame = projectile.ai[0] == 2f;
            HandleFadeInOut(projectile, projFuncs, projData, true, false);
        }
    }
}
