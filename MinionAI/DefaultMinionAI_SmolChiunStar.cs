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

        static int[] ChiunStarParticles = {
            DustID.PinkTorch,
            DustID.RedTorch,
            DustID.HallowedTorch,
            DustID.BlueTorch,
            DustID.BlueCrystalShard,
            DustID.PinkCrystalShard,
            DustID.PurpleCrystalShard,
        };
        public static void SmolChiunStarSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            SmolChiunStar_EstericaSigil(projectile.Center, 12, ChiunStarParticles, 10, i=> {
                Vector2 difference = i.position - projectile.position - Vector2.Normalize(projectile.velocity) * 64;
                i.velocity = difference * 0.1f;
                i.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            });
        }
        public static void SmolChiunStarDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustDirect(projectile.Center + new Vector2(0, Main.rand.NextFloat(0, 16)).RotatedBy(Main.rand.NextFloat(0, MathF.PI * 2)), projectile.width, projectile.height, DustID.PinkTorch, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, new Color(255, 255, 255), projectile.scale).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            }
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustDirect(projectile.Center + new Vector2(0, Main.rand.NextFloat(0, 16)).RotatedBy(Main.rand.NextFloat(0, MathF.PI * 2)), projectile.width, projectile.height, DustID.BlueTorch, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, new Color(255, 255, 255), projectile.scale).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            }
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustDirect(projectile.Center + new Vector2(0, Main.rand.NextFloat(0, 16)).RotatedBy(Main.rand.NextFloat(0, MathF.PI * 2)), projectile.width, projectile.height, DustID.RedTorch, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, new Color(255, 255, 255), projectile.scale).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            }
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustDirect(projectile.Center + new Vector2(0, Main.rand.NextFloat(0, 16)).RotatedBy(Main.rand.NextFloat(0, MathF.PI * 2)), projectile.width, projectile.height, DustID.HallowedTorch, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, new Color(255, 255, 255), projectile.scale).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            }
        }
        const float oneThirdPi = MathF.PI / 3;
        public static void SmolChiunStar_EstericaSigil(Vector2 center, float Radius, int[] particleIDs, int particleCount, Action<Dust> OnCreate)
        {
            //first upper half of remphan's seal, representing heaven
            Vector2 offset = new Vector2(0, Radius).RotatedBy(Main.rand.NextFloat(-0.3927f, 0.3927f));
            Vector2 yhvh = center + offset;
            Vector2 rmpn = center + offset.RotatedBy(oneThirdPi);
            Vector2 chun = center + offset.RotatedBy(oneThirdPi * 2);
            Vector2 ysua = center + offset.RotatedBy(oneThirdPi * 3);
            Vector2 hsem = center + offset.RotatedBy(oneThirdPi * 4);
            Vector2 estr = center + offset.RotatedBy(oneThirdPi * 5);

            ModEffects.DrawLineWithParticles(yhvh, chun, particleIDs, particleCount, OnCreate);
            ModEffects.DrawLineWithParticles(hsem, chun, particleIDs, particleCount, OnCreate);
            ModEffects.DrawLineWithParticles(yhvh, hsem, particleIDs, particleCount, OnCreate);
            ModEffects.DrawLineWithParticles(rmpn, ysua, particleIDs, particleCount, OnCreate);
            ModEffects.DrawLineWithParticles(rmpn, estr, particleIDs, particleCount, OnCreate);
            ModEffects.DrawLineWithParticles(estr, ysua, particleIDs, particleCount, OnCreate);
        }
    }
}
