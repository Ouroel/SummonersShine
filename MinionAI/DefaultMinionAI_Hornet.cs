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
        public static void HornetSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            //gore
            int last = -1;
            for (int i = -1; i < 2; i += 2)
            {
                int rand = Main.rand.Next(0, 3);
                if (rand == last)
                    rand -= 1;
                if (rand == -1)
                    rand = 2;
                last = rand;
                int GoreID = ModEffects.HornetHive[rand];
                int index = Gore.NewGore(projectile.GetSource_FromThis(), projectile.position, new Vector2(1f * i, 0.0f), GoreID, projectile.scale);
                Gore gore = Main.gore[index];
                gore.rotation = MathF.PI * i * 0.5f;
            }
            for (int i = 0; i < 20; i++)
                Dust.NewDustDirect(projectile.Center + Main.rand.NextVector2Circular(8, 8), projectile.width, projectile.height, DustID.Honey + Main.rand.Next(0, 2), Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, Color.White, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player); ;
        }
        public static void HornetDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            //gore
            int GoreID = ModEffects.HornetCocoon;
            Gore.NewGore(projectile.GetSource_FromThis(), projectile.position, new Vector2(0.01f, 0.0f), GoreID, projectile.scale);
            for (int i = 0; i < 20; i++)
                Dust.NewDustDirect(projectile.Center, projectile.width, projectile.height, DustID.Honey + Main.rand.Next(0, 2), Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, Color.White, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player); ;
        }
    }
}
