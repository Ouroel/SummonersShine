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
        public static void TwinsSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            //gore
            float Rotation = Main.rand.NextFloat(0, MathF.PI * 2);
            int GoreID = ModEffects.Retinassembler;
            Gore gore = Main.gore[Gore.NewGore(projectile.GetSource_FromThis(), projectile.position, new Vector2(0.01f, 0.0f), GoreID, projectile.scale)];
            gore.rotation = Rotation;
            GoreID = ModEffects.Spazmatiassembler;
            gore = Main.gore[Gore.NewGore(projectile.GetSource_FromThis(), projectile.position, new Vector2(0.01f, 0.0f), GoreID, projectile.scale)];
            gore.rotation = Rotation;// + Vector2.UnitY.RotatedBy(Rotation) * Main.rand.NextFloatDirection()
            Vector2 rotatedDustPos = Vector2.UnitY.RotatedBy(Rotation);
            for (int i = 0; i < 30; i++)
                Dust.NewDustDirect(projectile.Center + rotatedDustPos * Main.rand.NextFloatDirection(), projectile.width, projectile.height, DustID.MartianSaucerSpark, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, Color.Orange, 1);
            for (int i = 0; i < 10; i++)
                Dust.NewDustDirect(projectile.Center + rotatedDustPos * Main.rand.NextFloatDirection(), projectile.width, projectile.height, DustID.FireworkFountain_Yellow, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, Color.White, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
        }
        public static void RetDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            for (int i = 0; i < 5; i++)
                Dust.NewDustDirect(projectile.Center, projectile.width, projectile.height, DustID.MartianSaucerSpark, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, Color.Orange, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            for (int i = 0; i < 5; i++)
                Dust.NewDustDirect(projectile.Center, projectile.width, projectile.height, DustID.FireworkFountain_Yellow, Main.rand.NextFloatDirection() * 0.1f, -0.5f, 0, Color.DarkSlateGray, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            for (int i = 0; i < 2; i++)
                Dust.NewDustDirect(projectile.Center, projectile.width, projectile.height, ModEffects.SightSoul, Main.rand.NextFloatDirection() * 0.5f, -1.5f, 100, Color.White, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
        }
        public static void SpazDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            for (int i = 0; i < 5; i++)
                Dust.NewDustDirect(projectile.Center, projectile.width, projectile.height, DustID.MartianSaucerSpark, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, Color.Orange, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            for (int i = 0; i < 5; i++)
                Dust.NewDustDirect(projectile.Center, projectile.width, projectile.height, DustID.FireworkFountain_Yellow, Main.rand.NextFloatDirection() * 0.1f, -0.5f, 0, Color.DarkSlateGray, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            for (int i = 0; i < 2; i++)
                Dust.NewDustDirect(projectile.Center, projectile.width, projectile.height, ModEffects.SightSoul, Main.rand.NextFloatDirection() * 0.5f, -1.5f, 100, Color.White, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
        }
    }
}
