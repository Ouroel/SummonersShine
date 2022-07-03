using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {
        public static void UpdateMaxTurrets_Reg_DontKillDeadTurrets(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (!c.TryGotoNext(i => i.MatchCallvirt(typeof(Projectile), "get_WipableTurret")))
            {
                SummonersShine.logger.Error("[UpdateMaxTurrets_Reg_DontKillDeadTurrets] Cannot find Projectile.get_WipableTurret!");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldloc_2);
            c.EmitDelegate(turretDead);
        }
        static bool turretDead(bool wipable, int projNum) {
            Projectile proj = Main.projectile[projNum];
            if (!wipable)
                return false;
            if (proj == null || !proj.active)
                return false;
            ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
            return !(projFuncs.killedTicks > 0);
        }
    }
}
