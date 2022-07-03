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
        public static void StardustGuardian_RegTracking(On.Terraria.Projectile.orig_AI_120_StardustGuardian_FindTarget orig, Projectile self, float lookupRange, ref int targetNPCIndex, ref float distanceToClosestTarget)
        {
            orig(self, lookupRange, ref targetNPCIndex, ref distanceToClosestTarget);
            ReworkMinion_Projectile.SetMoveTarget_FromID(self, targetNPCIndex);
        }

        public static void StardustGuardian_SteppedMovement(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchLdcR4(0.05f)))
            {
                SummonersShine.logger.Error("[StardustGuardian_SteppedMovement] Hook failed! (i.MatchLdcR4(0.05f) cannot be found)");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(StardustGuardian_GetSteppedMovement);

            if (!c.TryGotoNext(i => i.MatchLdcR4(0.2f)))
            {
                SummonersShine.logger.Error("[StardustGuardian_SteppedMovement] Hook failed! (i.MatchLdcR4(0.2f) cannot be found)");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(StardustGuardian_GetSteppedMovement);
        }

        static float StardustGuardian_GetSteppedMovement(float input, Projectile proj)
        {
            ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
            float remainder = 1 - input;
            remainder = MathF.Pow(remainder, projFuncs.GetInternalSimRate(proj));
            return 1 - remainder;
        }
    }
}
