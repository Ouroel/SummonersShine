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
using Terraria.GameContent;
using Terraria.ID;

namespace SummonersShine.SpecialAbilities
{
    public static partial class SpecialAbility
    {
        public static void FrogOnSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity _target, int specialType, bool fromServer)
        {
            projData.energy = 0;
            projData.castingSpecialAbilityTime = 0;
        }
        public static void FrogOnHitNPC(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (ModUtils.IsCastingSpecialAbility(projData, ItemID.VampireFrogStaff))
            {
                Player player = Main.player[projectile.owner];
                ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
                playerFuncs.lifeSteal += damage * projFuncs.GetMinionPower(projectile, 0) * 0.01f;
                int lifeSteal = (int)playerFuncs.lifeSteal;
                playerFuncs.lifeSteal -= lifeSteal;
                player.HealEffect(lifeSteal);
                player.statLife = Math.Min(player.statLife + lifeSteal, player.statLifeMax2);
                FrogOnHitNPC_Fake(projectile, projFuncs, projData, target, ref damage, ref knockback, ref crit, ref hitDirection);
                PacketHandler.WritePacket_FakeHitEffect(projectile, target, damage, knockback, crit, hitDirection);
            }
        }

        public static void FrogOnHitNPC_Fake(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[projectile.owner];

            for (int x = 0; x < 3; x++)
            {
                Vector2 pos = player.position + new Vector2(Main.rand.NextFloat(0, player.width), Main.rand.NextFloat(-player.height * 0.5f, player.height * 0.5f));
                Dust dust = Dust.NewDustDirect(pos, 1, 1, ModEffects.Lifesteal_Sanguine, 0, -0.1f, 50, Color.White, 1);
                dust = Dust.NewDustDirect(pos, 1, 1, DustID.Blood);
            }
            for (int x = 0; x < 10; x++)
            {
                Vector2 pos = target.Center + Main.rand.NextVector2Circular(8, 8);
                Dust dust = Dust.NewDustDirect(pos, 1, 1, DustID.Blood);
            }
        }
    }
}
