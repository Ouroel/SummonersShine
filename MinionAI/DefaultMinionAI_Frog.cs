using Microsoft.Xna.Framework;
using SummonersShine.BakedConfigs;
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
        public static void FrogSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            //gore
            for (int i = 0; i < 20; i++)
                Dust.NewDustDirect(projectile.Center + Main.rand.NextVector2Circular(16, 16), projectile.width, projectile.height, DustID.Blood, Main.rand.NextFloatDirection() * 2, Main.rand.NextFloatDirection() * 2).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            for (int i = 0; i < 20; i++)
                Dust.NewDustDirect(projectile.Center + Main.rand.NextVector2Circular(16, 16), projectile.width, projectile.height, DustID.BloodWater, Main.rand.NextFloatDirection() * 2, Main.rand.NextFloatDirection() * 2).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            projData.alphaOverride = 155;
        }
        public static void FrogPostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (ModUtils.IsCastingSpecialAbility(projData, ItemID.VampireFrogStaff) && projectile.IsOnRealTick(projData))
            {
                projData.castingSpecialAbilityTime++;
                if (projData.castingSpecialAbilityTime > 300)
                    projData.castingSpecialAbilityTime = -1;
            }
            HandleFadeInOut(projectile, projFuncs, projData, true, false);
        }
    }
}
