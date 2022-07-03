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
        public static void GetWhipSettings_Hook(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            ILLabel start = c.DefineLabel();
            //ILLabel ret = c.DefineLabel();
            //find return
            c.Index = c.Instrs.Count - 1;
            //emit register attackspeed
            c.Emit(OpCodes.Ldarg_0);
            c.Index--;
            c.MarkLabel(start);
            c.Index++;
            c.Emit(OpCodes.Ldarg_3);
            c.EmitDelegate<modWhipLengthDelegate>(ModifyWhipLength);
            //c.MarkLabel(ret);
            //c.Index = 0;

            while (c.TryGotoPrev(i => i.MatchRet()))
            {
                c.Remove();
                c.Emit(OpCodes.Br, start.Target);
                SummonersShine.logger.Info("[GetWhipSettings] ret found!");
            }



            /*while (c.TryGotoNext(i => {
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
                SummonersShine.logger.Info("[GetWhipSettings] ret found!");
            }*/
        }

        delegate void modWhipLengthDelegate(Projectile proj, ref float length);
        static void ModifyWhipLength(Projectile proj, ref float length)
        {
            length *= Main.player[proj.owner].GetModPlayer<ReworkMinion_Player>().GetWhipRange();
        }
    }
}
