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
        public static void BallistraTowerSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            DD2TurretSummonEffect(projectile, 0, new Rectangle((int)projectile.position.X - 32, (int)projectile.position.Y, projectile.width + 32, projectile.height), player);
        }
        public static void BallistraTowerDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            DD2TurretDespawnEffect(new Rectangle((int)projectile.position.X - 32, (int)projectile.position.Y, projectile.width + 32, projectile.height), player, projectile);
        }
        public static void LightningAuraSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            DD2TurretSummonEffect(projectile, 3, new Rectangle((int)projectile.Top.X - 16, (int)projectile.Top.Y - 16, 32, 32), player);
        }
        public static void LightningAuraDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            DD2TurretDespawnEffect(new Rectangle((int) projectile.Top.X -16, (int)projectile.Top.Y - 16, 32, 32), player, projectile);
        }
        public static void ExplosiveTrapSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            DD2TurretSummonEffect(projectile, 6, new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height), player);
        }
        public static void ExplosiveTrapDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            DD2TurretDespawnEffect(new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height), player, projectile);
        }
        public static void FlameBurstTowerSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            DD2TurretSummonEffect(projectile, 9, new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height), player);
        }
        public static void FlameBurstTowerDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            DD2TurretDespawnEffect(new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height), player, projectile);
        }

        static void DD2TurretSummonEffect(Projectile proj, int hammerPosType, Rectangle sparksPos, Player player)
        {
            Vector2 hammerPos = DD2Hammer.GetRelPos(hammerPosType) + proj.Top;
            Dust hammer = Dust.NewDustDirect(hammerPos, 0, 0, ModEffects.DD2Hammer, newColor: Color.LightGoldenrodYellow);
            hammer.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            hammer.customData = proj.whoAmI + (Main.maxProjectiles + 1) * hammerPosType;
            for (int x = 0; x < 12; x++)
            {
                Vector2 center = Main.rand.NextVector2FromRectangle(sparksPos);
                Dust.NewDustDirect(center, 0, 0, DustID.MartianSaucerSpark).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                Dust.NewDustDirect(center, 0, 0, DustID.FireworkFountain_Yellow, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, Color.White, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            }
        }

        public static void DD2TurretDespawnEffect(Rectangle sparksPos, Player player, Projectile proj)
        {
            for (int x = 0; x < 12; x++)
            {
                Vector2 center = Main.rand.NextVector2FromRectangle(sparksPos);
                Dust.NewDustDirect(center, 0, 0, DustID.MartianSaucerSpark).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                Dust.NewDustDirect(center, 0, 0, DustID.FireworkFountain_Yellow, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, Color.White, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            }
            for (int x = 0; x < 5; x++)
            {
                Vector2 center = Main.rand.NextVector2FromRectangle(sparksPos);
                Gore.NewGoreDirect(proj.GetSource_FromThis(), center, Vector2.Zero, GoreID.Smoke1 + Main.rand.Next(0, 3));
            }
        }
    }
}
