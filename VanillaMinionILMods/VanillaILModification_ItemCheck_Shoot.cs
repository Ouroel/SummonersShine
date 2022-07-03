using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {
        public static void ItemCheck_Shoot_MagenChiunFloat(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (!c.TryGotoNext(i => i.MatchLdcI4(5119)))
            {
                SummonersShine.logger.Error("[ItemCheck_Shoot_MagenChiunFloat] Cannot find i.MatchLdcI4(5119)");
                return;
            }

            c.Index += 2;

            ILLabel outLab = null;
            if (!c.TryGotoNext(i => i.MatchBrtrue(out outLab)))
            {
                SummonersShine.logger.Error("[ItemCheck_Shoot_MagenChiunFloat] Cannot find i.MatchBneUn(out outLab)");
                return;
            }

            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(PlayerHasMagenChiun);
            c.Emit(OpCodes.Brtrue, outLab);
        }
    }
}
