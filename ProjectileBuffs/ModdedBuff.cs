using Microsoft.Xna.Framework;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.ProjectileBuffs
{
    public class ModdedBuff : ProjectileBuff
    {
        public override ProjectileBuffIDs ID => ProjectileBuffIDs.ModdedBuff;

        public Func<Projectile, bool> updateHook;
        public Func<Projectile, float> getAttackSpeedBuffHook;
        public Action<Projectile, Color> preDrawHook;
        public Action<Projectile, Color> postDrawHook;
        public override bool Update(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            if (updateHook != null)
                return updateHook(projectile);
            return true;
        }

        public override float GetAttackSpeedBuff(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (getAttackSpeedBuffHook != null)
                return getAttackSpeedBuffHook(projectile);
            return 1;
        }

        public override void PreDraw(Projectile projectile, ReworkMinion_Projectile projFuncs, Color color)
        {
            if (preDrawHook != null)
                preDrawHook(projectile, color);
        }

        public override void PostDraw(Projectile projectile, ReworkMinion_Projectile projFuncs, Color color)
        {
            if (preDrawHook != null)
                preDrawHook(projectile, color);
        }

        public override bool IsEqual(ProjectileBuffIDs ID, Projectile sourceProj, Mod modID, int moddedBuffID)
        {
            return this.ID == ID && sourceProjIdentity == sourceProj.identity && this.modID == modID && this.moddedBuffID == moddedBuffID;
        }
    }
}
