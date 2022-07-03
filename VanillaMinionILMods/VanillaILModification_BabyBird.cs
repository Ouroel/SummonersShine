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
        public static void BabyBird_Reg_MinionTrackingState(ILContext il)
        {

            //
            ILCursor c = new ILCursor(il);

            /*if (!c.TryGotoNext(i => i.MatchLdloc(20)))
            {
                SummonersShine.logger.Error("Hook failed! (BabyBird_Reg_MinionTrackingState, 1st MatchStloc(20))");
                return;
            }

            int originalIndex = c.Index;
            c.Index++;
            Instruction i = c.Next;
            i = ((ILLabel)i.Operand).Target;
            c.Index++;
            //bird is attacking
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldc_I4, 0);
            c.EmitDelegate<Action<Projectile, int>>(BabyBird_SetAIZero);
            //bird is retreating
            c.Goto(i);
            c.Index++;
            c.Emit(OpCodes.Ldc_I4, 1);
            c.EmitDelegate<Action<Projectile, int>>(BabyBird_SetAIZero);
            c.Emit(OpCodes.Ldarg_0);
            //bird wants to tp to the player
            if (!c.TryGotoNext(i => i.MatchCall<Entity>("set_Center")))
            {
                SummonersShine.logger.Error("Hook failed! (AI062_Reg_RelativeProjVel, 1st set_Center!)");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldc_I4, 3);
            c.EmitDelegate<Action<Projectile, int>>(BabyBird_SetAIZero);

            //bird is going to perch
            if (!c.TryGotoNext(i => i.MatchCall<Entity>("set_Center")))
            {
                SummonersShine.logger.Error("Hook failed! (AI062_Reg_RelativeProjVel, 2nd set_Center!)");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldc_I4, 3);
            c.EmitDelegate<Action<Projectile, int>>(BabyBird_SetAIZero);*/
            while (MassPatcher_TrackTarget(il, c, "BabyBird_Reg_MinionTrackingState")) { }
        }

        /*public static void BabyBird_SetAIZero(Projectile projectile, int num) {
            projectile.ai[0] = num;
        }*/
    }
}
