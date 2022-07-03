using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SummonersShine.BakedConfigs;
using SummonersShine.Effects;
using SummonersShine.ProjectileBuffs;
using SummonersShine.Projectiles;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace SummonersShine.SpecialAbilities
{
    public static partial class SpecialAbility
    {
        public static void HornetOnSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity _target, int specialType, bool fromServer)
        {
            projData.energy = 0;
            projData.castingSpecialAbilityTime = 0;
            projData.energyRegenRateMult = 0;
            projData.specialCastPosition.Y = 1;
        }

        public static void HornetPreAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (ModUtils.IsCastingSpecialAbility(projData, ItemID.HornetStaff))
            {
                ModUtils.IncrementSpecialAbilityTimer(projectile, projFuncs, projData, 420);
            }
        }

        public static void HornetOnShootProjectile(Projectile projectile, Projectile source, ReworkMinion_Projectile sourceFuncs, MinionProjectileData sourceData)
        {
            if (ModUtils.IsCastingSpecialAbility(sourceData, ItemID.HornetStaff))
            {
                sourceData.specialCastPosition.Y += (float)PseudoRandom.GetPseudoRandomRNG(70);
                if (Main.rand.NextFloat() < sourceData.specialCastPosition.Y)
                {
                    projectile.originalDamage = (int)(sourceFuncs.GetMinionPower(projectile, 0) * 60);
                    projectile.netUpdate = true;
                    sourceData.specialCastPosition.Y = 0;
                }
                else
                {
                    for (int x = 0; x < 4; x++) {
                        Dust dust = Dust.NewDustDirect(projectile.Center, 0, 0, DustID.Honey);
                        dust.velocity = source.velocity + projectile.velocity / 16 + Main.rand.NextVector2Circular(0.5f, 0.5f);
                        dust = Dust.NewDustDirect(projectile.Center, 0, 0, DustID.Honey2);
                        dust.velocity = source.velocity + projectile.velocity / 16 + Main.rand.NextVector2Circular(0.5f, 0.5f);
                    }
                    for (int x = 0; x < 2; x++)
                    {
                        Dust dust = Dust.NewDustDirect(projectile.Center, 0, 0, DustID.Honey);
                        dust.velocity = source.velocity + Main.rand.NextVector2Circular(0.5f, 0.5f);
                        dust = Dust.NewDustDirect(projectile.Center, 0, 0, DustID.Honey2);
                        dust.velocity = source.velocity + Main.rand.NextVector2Circular(0.5f, 0.5f);
                    }
                    SoundEngine.PlaySound(SoundID.Item95, source.Center);
                    projectile.Kill();
                }
            }
        }
    }
}
