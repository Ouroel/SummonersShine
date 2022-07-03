using Microsoft.Xna.Framework;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SummonersShine.MinionAI
{
    public static partial class DefaultMinionAI
    {
        public static void DefaultTerminateSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player, ReworkMinion_Player playerFuncs)
        {
            projData.castingSpecialAbilityTime = -1;
        }
        public static void NoEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
        }
        public static bool NoEffect_ReturnTrue(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Vector2 oldVelocity)
        {
            return true;
        }
        public static void NoEffect(Projectile projectile, Projectile source, ReworkMinion_Projectile sourceFuncs, MinionProjectileData sourceData)
        {
        }
        public static void NoEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Vector2 oldVelocity)
        {
        }
        public static void NoEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Vector4 newVel)
        {
        }
    }
}
