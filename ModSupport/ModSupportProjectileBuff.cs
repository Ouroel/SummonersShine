using Microsoft.Xna.Framework;
using SummonersShine.ProjectileBuffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.ModSupport
{
    public static class ModSupportProjectileBuff
    {
        public static void AddModdedBuff(Projectile sourceProj, Projectile proj, Mod modID, int moddedBuffID,
            Func<Projectile, bool> updateHook,
            Func<Projectile, float> getAttackSpeedBuffHook,
            Action<Projectile, Color> preDrawHook,
            Action<Projectile, Color> postDrawHook)
        {
            ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
            ModdedBuff moddedBuff = (ModdedBuff)projFuncs.AddBuff(proj, ProjectileBuffIDs.ModdedBuff, sourceProj, modID, moddedBuffID);
            moddedBuff.updateHook = updateHook;
            moddedBuff.getAttackSpeedBuffHook = getAttackSpeedBuffHook;
            moddedBuff.preDrawHook = preDrawHook;
            moddedBuff.postDrawHook = postDrawHook;
        }

        public static void RemoveModdedBuff(Projectile sourceProj, Projectile proj, Mod modID, int moddedBuffID)
        {
            ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
            projFuncs.RemoveBuff(ProjectileBuffIDs.ModdedBuff, sourceProj, modID, moddedBuffID);
        }
        public static void AddProjectileBuff(Projectile sourceProj, Projectile proj, int projectileBuffID)
        {
            ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
            projFuncs.AddBuff(proj, (ProjectileBuffIDs)projectileBuffID, sourceProj);
        }
        public static void RemoveProjectileBuff(Projectile sourceProj, Projectile proj, int projectileBuffID)
        {
            ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
            projFuncs.RemoveBuff((ProjectileBuffIDs)projectileBuffID, sourceProj);
        }
    }
}
