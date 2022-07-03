using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.ModdedMinionILMods
{
    public partial class ModdedMinionILMods
    {
        /*
        public static void MassPatcher_DetectSetVector2(ILContext il, ILCursor c, string Source, bool requiresCanHit = true)
        {

            int npc_varPos = -1;
            while (c.TryGotoNext(i => i.MatchLdloc(out npc_varPos)))
            {
                c.Index++;
                if (il.Body.Variables[npc_varPos].VariableType.FullName != "Terraria.NPC")
                    break;
                if (!c.Next.MatchCallOrCallvirt<Entity>("get_Center"))
                    break;
                c.Index++;
                int vector_varPos = -1;
                if (!c.Next.MatchStloc(out vector_varPos))
                    break;
                if (il.Body.Variables[vector_varPos].VariableType.FullName != "Microsoft.XNA.Framework.Vector2")
                    break;

                c.Emit(OpCodes.Ldloc, npc_varPos);
                c.EmitDelegate(ReworkMinion_Projectile.SetMoveTarget);

                SummonersShine.logger.Info("[" + Source + "] Inserted 1 Target Tracker!");
            }
        }*/
    }
}
