using Mono.Cecil.Cil;
using MonoMod.Cil;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {
        static void MinionAltFeatureUse_Delete(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (!c.TryGotoNext(i => i.MatchCall("Terraria.Player", "MinionNPCTargetAim")))
            {
                SummonersShine.logger.Error("Cannot find i.MatchCall<Player>(\"MinionNPCTargetAim\")");
                return;
            }
            c.Index -= 2;
            int pos = c.Index;
            c.Index += 3;
            ILLabel newlab = c.DefineLabel();
            c.MarkLabel(newlab);

            ILLabel temp;

            if (!c.TryGotoPrev(i => i.MatchBeq(out temp)))
            {
                SummonersShine.logger.Error("Cannot find i.MatchBeq(out temp)");
                return;
            }
            c.Next.Operand = newlab.Target;

            c.Index = pos;
            c.RemoveRange(3);
        }
    }
}
