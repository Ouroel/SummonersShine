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
using SummonersShine.BakedConfigs;

namespace SummonersShine.SpecialAbilities
{
    public static partial class SpecialAbility
    {
        public static void BatOfLightOnSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity _target, int specialType, bool fromServer)
        {
            projData.energy = 0;
            projData.castingSpecialAbilityTime = 0;
            projData.specialCastPosition = Vector2.Zero;
        }
        public static void BatOfLightOnHitNPC(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (ModUtils.IsCastingSpecialAbility(projData, ItemID.SanguineStaff))
            {
                int cd = (int)(60 * projFuncs.GetSpeed(projectile));
                int counter = 0;

                bool firstAttack = projData.specialCastPosition.Y == 0;
                int lifeSteal = 0;
                if (firstAttack)
                {
                    lifeSteal = 1;
                    projData.specialCastPosition.X = 60 * projFuncs.GetSpeed(projectile);
                }
                projData.specialCastPosition.Y++;
                if (projData.specialCastPosition.Y <= 2)
                    ModUtils.FindValidNPCsOrderedByDist(projectile, projectile.Center).ForEach((NPC npc) =>
                    {
                        if (counter < 10 && npc != target && Main.rand.Next(0, 100) < projFuncs.GetMinionPower(projectile, 0))
                        {
                            Projectile batling = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ProjectileModIDs.BatlingOfLight, projectile.originalDamage, projectile.knockBack, projectile.owner, ModUtils.ConvertToGlobalEntityID(npc), lifeSteal);
                            batling.originalDamage = ModUtils.ConvertToGlobalEntityID(projectile);
                            batling.netUpdate = true;
                            counter++;
                        }
                    });
            }
        }
        public static void BatOfLighPostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (ModUtils.IsCastingSpecialAbility(projData, ItemID.SanguineStaff) && projectile.IsOnRealTick(projData))
            {
                projData.castingSpecialAbilityTime++;
                if (projData.castingSpecialAbilityTime > 300)
                    projData.castingSpecialAbilityTime = -1;
                if (projData.specialCastPosition.X > 0)
                    projData.specialCastPosition.X--;
                else
                    projData.specialCastPosition.Y = 0;
            }
        }
    }
}
