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
        public static void FlinxSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            Gore.NewGore(projectile.GetSource_FromThis(), projectile.Center - new Vector2(25, 25), new Vector2(0.0f, 0.0f), ModEffects.SnowPuff, 1);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustDirect(projectile.Center, projectile.width, projectile.height, DustID.Snow, Main.rand.NextFloatDirection() * 2, Main.rand.NextFloatDirection() * 2, 0, new Color(255, 255, 255), projectile.scale).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            }
        }
    }
}
