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
        public static void PygmySummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            //gore
            for (int i = 0; i < 5; i++)
                Gore.NewGore(projectile.GetSource_FromThis(), projectile.Center, new Vector2(Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection()), GoreID.TreeLeaf_Jungle);
            for (int i = 0; i < 5; i++)
                Dust.NewDustDirect(projectile.Center, projectile.width, projectile.height, ModEffects.Gem, Main.rand.NextFloatDirection() * 0.5f, -5, 0, Color.Lime, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);

        }
        public static void PygmyDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            player.GetModPlayer<ReworkMinion_Player>().GetSpecialData<PygmyPlatformCollection>().DeletePygmy(projectile);
        }
    }
}
