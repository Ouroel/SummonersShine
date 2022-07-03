using Microsoft.Xna.Framework;
using SummonersShine.Effects;
using SummonersShine.SpecialAbilities;
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
        public static void BabyBirdSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            for (int i = 0; i < 20; i++)
                Dust.NewDustDirect(projectile.Center, projectile.width, projectile.height, ModEffects.FinchFeather).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
        }
        public static void BabyBirdDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            for (int i = 0; i < 5; i++)
                Dust.NewDustDirect(projectile.Center, projectile.width, projectile.height, ModEffects.FinchFeather).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            for (int i = 0; i < 2; i++)
                Gore.NewGoreDirect(projectile.GetSource_FromThis(), projectile.Center, new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-3, -1)), Main.rand.Next(11, 14));
            ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
            playerFuncs.RemoveMovementMod(projectile, SpecialAbility.BabyBirdOnMovement);
        }
    }
}
