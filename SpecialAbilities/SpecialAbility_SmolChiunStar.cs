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
        public static void SmolChiunStarOnSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity _target, int specialType, bool fromServer)
        {
            projData.energy = 0;
            projData.castingSpecialAbilityTime = 3;
        }
        public static void SmolChiunStarOnHitNPC(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (ModUtils.IsCastingSpecialAbility(projData, ItemID.Smolstar))
            {
                if (projData.castingSpecialAbilityTime > 0 && Main.rand.NextBool(5))
                {
                    int id = ModUtils.ConvertToGlobalEntityID(target);
                    Projectile newProj = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), projectile.Center + Main.rand.NextVector2Circular(64, 64), Main.rand.NextVector2Circular(4, 1), ProjectileModIDs.ChiunShard, 100, projectile.knockBack, projectile.owner, id);
                    newProj.originalDamage = (int)projFuncs.GetMinionPower(projectile, 0);
                    newProj.netUpdate = true;

                    projData.castingSpecialAbilityTime--;
                }
            }
        }
    }
}
