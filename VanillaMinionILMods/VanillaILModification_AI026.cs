using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {
        //Pygmy, Slime, Spider
        public static void AI026_Reg_MinionTrackingTarget(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            while (MassPatcher_TrackTarget(il, c, "AI026_Reg_MinionTrackingTarget")) {
            }

            /*if (!c.TryGotoNext(i => i.MatchStloc(576)))
            {
                SummonersShine.logger.Error("Hook failed! (AI026_Reg_MinionTrackingTarget, 1st MatchStloc(576))");
                return;
            }
            if (!c.TryGotoNext(i => i.MatchStloc(576)))
            {
                SummonersShine.logger.Error("Hook failed! (AI026_Reg_MinionTrackingTarget, 2nd MatchStloc(576))");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, 576);
            c.EmitDelegate<Action<Projectile, int>>(ReworkMinion_Projectile.SetMoveTarget_FromID);

            if (!c.TryGotoNext(i => i.MatchStloc(788)))
            {
                SummonersShine.logger.Error("Hook failed! (AI026_Reg_MinionTrackingTarget, 1st MatchStloc(788))");
                return;
            }
            if (!c.TryGotoNext(i => i.MatchStloc(788)))
            {
                SummonersShine.logger.Error("Hook failed! (AI026_Reg_MinionTrackingTarget, 2nd MatchStloc(788))");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, 788);
            c.EmitDelegate<Action<Projectile, int>>(ReworkMinion_Projectile.SetMoveTarget_FromID);
            if (!c.TryGotoNext(i => i.MatchStloc(788)))
            {
                SummonersShine.logger.Error("Hook failed! (AI026_Reg_MinionTrackingTarget, 2nd MatchStloc(788))");
                return;
            }

            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, 788);
            c.EmitDelegate<Action<Projectile, int>>(ReworkMinion_Projectile.SetMoveTarget_FromID);*/

            //Pygmy compensation

            /*if (!c.TryGotoNext(i => i.MatchStloc(823)))
            {
                SummonersShine.logger.Error("Hook failed! (AI026_Reg_MinionTrackingTarget, 1st MatchStloc(823))");
                return;
            }
            if (!c.TryGotoNext(i => i.MatchStloc(823)))
            {
                SummonersShine.logger.Error("Hook failed! (AI026_Reg_MinionTrackingTarget, 2nd MatchStloc(823))");
                return;
            }
            c.Emit(OpCodes.Ldloc, 821); //load x for later
            c.Emit(OpCodes.Ldloc, 821); //load x
            c.Emit(OpCodes.Ldloc, 823); //load y
            c.Emit(OpCodes.Ldc_I4, 0);
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<float, float, bool, Projectile, float>>(ReworkMinion_Projectile.GetTotalProjectileVelocity_NoCheating);
            c.Emit(OpCodes.Stloc, 821); //save x
            c.Emit(OpCodes.Ldloc, 823); //load y, x alr loaded
            c.Emit(OpCodes.Ldc_I4, 1);
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<float, float, bool, Projectile, float>>(ReworkMinion_Projectile.GetTotalProjectileVelocity_NoCheating);
            c.Emit(OpCodes.Stloc, 823); //save y */
            //821, 823
            //More tracks

            /*if (!c.TryGotoNext(i => i.MatchStloc(839)))
            {
                SummonersShine.logger.Error("Hook failed! (AI026_Reg_MinionTrackingTarget, 1st MatchStloc(839))");
                return;
            }
            if (!c.TryGotoNext(i => i.MatchStloc(839)))
            {
                SummonersShine.logger.Error("Hook failed! (AI026_Reg_MinionTrackingTarget, 2nd MatchStloc(839))");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, 839);
            c.EmitDelegate<Action<Projectile, int>>(ReworkMinion_Projectile.SetMoveTarget_FromID);
            if (!c.TryGotoNext(i => i.MatchStloc(839)))
            {
                SummonersShine.logger.Error("Hook failed! (AI026_Reg_MinionTrackingTarget, 2nd MatchStloc(839))");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, 839);
            c.EmitDelegate<Action<Projectile, int>>(ReworkMinion_Projectile.SetMoveTarget_FromID);*/


        }
    }
}
