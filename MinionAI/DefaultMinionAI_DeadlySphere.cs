using Microsoft.Xna.Framework;
using SummonersShine.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SummonersShine.SpecialData;
using SummonersShine.SpecialAbilities;
using SummonersShine.BakedConfigs;

namespace SummonersShine.MinionAI
{
    public static partial class DefaultMinionAI
    {
        public static void DeadlySphere_SummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            Gore gore = Gore.NewGoreDirect(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ModEffects.ChiunStar);
            gore.velocity = Vector2.Zero;
            gore.position = projectile.Center - new Vector2(gore.Width, gore.Height) * 0.5f;
            gore.scale = 0.5f;
            projData.alphaOverride = 200;
        }
        public static void DeadlySphere_DespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            Gore gore = Main.gore[Gore.NewGore(projectile.GetSource_FromThis(), projectile.Center, projectile.velocity, GoreID.DeadlySphere1)];
            gore.timeLeft = 1;
            gore = Main.gore[Gore.NewGore(projectile.GetSource_FromThis(), projectile.Center, projectile.velocity, GoreID.DeadlySphere2)];
            gore.timeLeft = 1;
            for (int i = 0; i < 10; i++) {
                Dust.NewDust(projectile.Center + Main.rand.NextVector2Circular(8, 8), projectile.width, projectile.height, DustID.MartianSaucerSpark);
            }

            ProjectileOnHit.UnhookProjectile(projectile);
        }

        public static void DeadlySphere_PostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            HandleFadeInOut(projectile, projFuncs, projData, true, true);
            int ai = (int)projectile.ai[0];
            projData.trackingState = (ai > 8) ? MinionTracking_State.retreating : MinionTracking_State.normal;


            if (BakedConfig.CustomSpecialPowersEnabled(ItemID.DeadlySphereStaff) && projectile.IsOnRealTick(projData))
            {
                SpecialAbility.CountDownDeadlySphereTimer(projData);
            }
        }
    }
}
