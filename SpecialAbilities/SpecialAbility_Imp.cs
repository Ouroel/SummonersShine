using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public static void ImpOnSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity _target, int specialType, bool fromServer)
        {
            projData.energy = 0;
            projData.castingSpecialAbilityTime = 0;
            projData.specialCastTarget = (NPC)_target;
            projData.energyRegenRateMult = 0;

            projectile.ai[1] = 90 - specialType;
            projectile.ai[0] = 0;
        }
        public static List<Projectile> PreAbility_Imp(Item summonItem, ReworkMinion_Player player)
        {
            List<Projectile> rv = PreAbility_FindAllMinions(summonItem, player);
            if (rv.Count > 1) {
                int count = 0;
                int max = rv.Count - 1;
                rv.ForEach(i =>
                {
                    ReworkMinion_Projectile projFuncs = i.GetGlobalProjectile<ReworkMinion_Projectile>();
                    MinionProjectileData projData = projFuncs.GetMinionProjData();
                    projData.castingSpecialAbilityType = count * 30 / max;
                    count++;
                });
            }
            return rv;
        }
    }
}
