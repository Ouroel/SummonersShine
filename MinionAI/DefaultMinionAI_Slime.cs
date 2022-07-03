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
        public static void SlimeSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            //gore
            for (int i = 0; i < 20; i++)
                Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Chlorophyte, Main.rand.NextFloatDirection() * 2, Main.rand.NextFloatDirection() * 2, 100, new Color(132, 199, 154), projectile.scale).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            for (int i = 0; i < 5; i++)
                Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Snow, Main.rand.NextFloatDirection() * 2, Main.rand.NextFloatDirection() * 2, 100, new Color(132, 199, 154), projectile.scale).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
        }
    }
}
