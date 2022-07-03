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
        public static void HelpText_Reg_CustomHelpText(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            int pos = 0;
            int highestNumber = 0;

            while (c.TryGotoNext(i => i.MatchLdsfld<Main>("helpText")))
            {
                Instruction instrs = c.Next.Next;
                int test = 0;
                if (instrs.MatchLdcI4(out test) && test > highestNumber) {
                    pos = c.Index;
                    highestNumber = test;
                }
            }

            SummonersShine.logger.Info("[HelpText_Reg_CustomHelpText] Highest helpText number found: " + highestNumber + " at instruction " + pos);

            c.Index += 2;

            ILLabel lab = il.DefineLabel();

            c.EmitDelegate(CustomHelpTextManager.GetHelpText);
            c.Emit(OpCodes.Ret);
            c.Emit(OpCodes.Ldsfld, typeof(Main).GetField(nameof(Main.helpText)));
            c.Index--;
            c.MarkLabel(lab);
            c.Index--;
            c.Emit(OpCodes.Brfalse, lab);
            c.Index+= 2;
            c.Emit(OpCodes.Ldc_I4, highestNumber + CustomHelpTextManager.helpText.Length + 1);
        }
    }
}
