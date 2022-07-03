using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SummonersShine.MinionAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {
        public static void Dragon_Reg_MinionTrackingState(ILContext il)
        {
            var c = new ILCursor(il);

            /*//AI_121 Dragon

            if (!c.TryGotoNext(i => i.MatchStloc(35)))
            {
                SummonersShine.logger.Error("Hook failed! Can't find Dragon AI check!");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, 35);
            c.Emit(OpCodes.Ldloc, 18);
            c.EmitDelegate<Action<Projectile, int, int>>(DefaultMinionAI.DragonSetMinionTrackingState);*/

            while (MassPatcher_TrackTarget(il, c, "Dragon_Reg_MinionTrackingState", false)) { };

            //stepped
            c.Index = 0;
            if (!c.TryGotoNext(i => i.MatchLdcR4(0.1f)))
            {
                SummonersShine.logger.Error("[Dragon_Reg_MinionTrackingState]Hook failed! Can't find 1st MatchLdcR4(0.1f)");
                return;
            }
            c.Index++;
            if (!c.TryGotoNext(i => i.MatchLdcR4(0.1f)))
            {
                SummonersShine.logger.Error("[Dragon_Reg_MinionTrackingState]Hook failed! Can't find 2nd MatchLdcR4(0.1f)");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(GetDragonRotatePerTick);
        }

        static float GetDragonRotatePerTick(float orig, Projectile dragon)
        {
            ReworkMinion_Projectile dragonFuncs = dragon.GetGlobalProjectile<ReworkMinion_Projectile>();
            return (1 - MathF.Pow(0.9f, dragonFuncs.GetInternalSimRate(dragon)));
        }
    }
}
