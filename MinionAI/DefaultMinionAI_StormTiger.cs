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
        public static void StormTigerGemSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            //gore
            for (int i = 0; i < 5; i++)
                Dust.NewDustDirect(projectile.Center + Main.rand.NextVector2Circular(16, 16), projectile.width, projectile.height, DustID.SpelunkerGlowstickSparkle, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, Color.White, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
        }

        public static void StormTigerGemDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            ReworkMinion_Player ownerData = Main.player[projectile.owner].GetModPlayer<ReworkMinion_Player>();
            ReworkMinion_Projectile tigerGemData = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            DesertTigerStat stats = tigerGemData.GetSpecialData<DesertTigerStat>();
            DesertTigerStatCollection collection = ownerData.GetSpecialData<DesertTigerStatCollection>();
            collection.Remove(stats);
            for (int i = 0; i < 5; i++)
                Dust.NewDustDirect(projectile.Center + Main.rand.NextVector2Circular(16, 16), projectile.width, projectile.height, DustID.SpelunkerGlowstickSparkle, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, Color.White, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);

        }
        public static void StormTigerSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            for (int i = 0; i < 20; i++)
                Dust.NewDustDirect(projectile.Center + Main.rand.NextVector2Circular(16, 16), projectile.width, projectile.height, DustID.GoldFlame, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, Color.White, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
        }
        public static void StormTigerDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            //gore
            ReworkMinion_Player ownerData = Main.player[projectile.owner].GetModPlayer<ReworkMinion_Player>();
            DesertTigerStatCollection collection = ownerData.GetSpecialData<DesertTigerStatCollection>();
            collection.megaMinion = null;
            collection.megaMinionBody = null;
            collection.TransferEnergyInCaseOfDeath(projData.energy, projData.energyRegenRate, projData.maxEnergy);
            for (int i = 0; i < 20; i++)
                Dust.NewDustDirect(projectile.Center + Main.rand.NextVector2Circular(16, 16), projectile.width, projectile.height, DustID.GoldFlame, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, Color.White, 1).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
        }

        const int fullTurban = 6;
        const int fullHalfTurban = 2;
        public static Vector2 StormTigerGem_GetHomeLocation(Player player, int stackedIndex, int totalIndexes)
        {
            int num = player.bodyFrame.Height;
            bool flag = num == 0;
            if (flag)
            {
                num = 1;
            }
            Vector2 vector = player.MountedCenter + Main.OffsetsPlayerHeadgear[player.bodyFrame.Y / num];
            vector += new Vector2(0f, player.gravDir * -24f);

            int numInGem = 2 - (totalIndexes % 2);
            float turbanHeight = MathF.Max(fullTurban, totalIndexes - numInGem) * 2.667f;
            //sultan gem
            float gemHeight = turbanHeight + (turbanHeight + 16) * (1 - MathF.Cos(Math.Min((float)totalIndexes / fullTurban, 1) * MathF.PI * 0.5f));
            if (stackedIndex < numInGem)
            {
                vector.Y -= gemHeight;
                float time = player.miscCounterNormalized * 2f;
                Vector2 rotationVector = Vector2.UnitX * 12;
                return vector + rotationVector.RotatedBy((time * 6.2831855f) + stackedIndex * Math.PI);
            }
            //sultan turban
            stackedIndex -= numInGem;
            totalIndexes -= numInGem;
            int even = (stackedIndex % 2) == 0 ? 1 : -1;
            totalIndexes = (totalIndexes / 2) - 1;
            stackedIndex = (stackedIndex / 2);
            vector.Y -= turbanHeight;

            int turbanSize = Math.Max(fullHalfTurban, totalIndexes);
            float perc = (float)stackedIndex / turbanSize;
            float turbanStart = MathF.Atan(4 / turbanHeight);
            Vector2 rotation = new Vector2(
                MathF.Sin((MathF.PI - MathF.Atan(6 / turbanHeight) - turbanStart) * perc + turbanStart) * (float)(turbanHeight * 0.5) + 24,
                -perc * turbanHeight * 2 + turbanHeight
                );
            rotation.X *= even;

            return vector + rotation;
        }
    }
}
