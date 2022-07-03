using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.UI;
using Terraria.ID;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {
        public static void FlameBurstTower_Ballista_FindTarget_EmitGetTarget(ILContext il)
        {

            ILCursor c = new ILCursor(il);

            ILLabel start = c.DefineLabel();
            ILLabel ret = c.DefineLabel();

            //find return
            c.Index = c.Instrs.Count - 1;
            //emit register attackspeed
            c.Emit(OpCodes.Ldarg_0);
            c.Index--;
            c.MarkLabel(start);
            c.Index++;
            c.Emit(OpCodes.Ldloc, 0);
            c.EmitDelegate(ReworkMinion_Projectile.SetMoveTarget_FromID);
            c.MarkLabel(ret);
            c.Index = 0;

            while (c.TryGotoNext(i =>
            {
                ILLabel lab;
                if (i.MatchBr(out lab))
                {
                    return lab.Target == ret.Target;
                }
                return false;
            }))
            {
                c.Next.Operand = start.Target;
                c.Index++;
                SummonersShine.logger.Info("[FlameBurstTower_Ballista_FindTarget_EmitGetTarget] ret found!");
            }
        }

        public static void Ballista_FaceProperTracking(ILContext il)
        {

            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchLdloc(2)))
            {
                SummonersShine.logger.Error("[Ballista_FaceProperTracking]Cannot find i.MatchLdloc(2)");
                return;
            }
            if (!c.TryGotoNext(i => i.MatchLdloc(2)))
            {
                SummonersShine.logger.Error("[Ballista_FaceProperTracking]Cannot find i.MatchLdloc(2)");
                return;
            }
            c.Index += 2;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, 16);
            c.Emit(OpCodes.Ldloc, 5);
            c.EmitDelegate(Ballista_GetProperFacing);
        }
            
        public static Vector2 Ballista_GetProperFacing(Vector2 vec, Projectile ballista, int npc, float speed)
        {
            return ReworkMinion_Projectile.GetTotalProjectileVelocity(vec * speed, 1, ballista, Main.npc[npc]).SafeNormalize(Vector2.UnitX * (float)ballista.direction);
        }
    }
}
