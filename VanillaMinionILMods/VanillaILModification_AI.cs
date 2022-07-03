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
using Terraria.ModLoader;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {
        public static void AI_EncapsulateAllModsWithinPrePostAI(On.Terraria.Projectile.orig_AI orig, Projectile projectile)
        {
            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            if (projFuncs.MinionPreAI(projectile))
            {
                try
                {
                    ProjectileLoader.ProjectileAI(projectile);
                }
                catch
                {
                }
            }
            projFuncs.MinionPostAI(projectile);

            //Retains the player vel
            SingleThreadExploitation.UncomparativelySlowPlayer();
        }

        public static bool GetAIStyle(ILContext il, ILCursor c, int aiStyle, string function = "")
        {
            if (!c.TryGotoNext(i => i.MatchLdfld<Projectile>("aiStyle") && i.Next.MatchLdcI4(aiStyle)))
            {
                SummonersShine.logger.Error("[" + function + "] Hook failed! (aiStyle " + aiStyle + " cannot be found)");
                return false;
            }
            return true;
        }
        public static bool GetType(ILContext il, ILCursor c, int type, string function = "")
        {
            if (!c.TryGotoNext(i => i.MatchLdfld<Projectile>("type") && i.Next.MatchLdcI4(type)))
            {
                SummonersShine.logger.Error("[" + function + "] Hook failed! (type " + type + " cannot be found)");
                return false;
            }
            return true;
        }
        public static bool MassPatcher_TrackTarget(ILContext il, ILCursor c, string function = "", bool requiresCanHit = true) {

            if (!c.TryGotoNext(i => i.MatchCallvirt<NPC>("CanBeChasedBy")))
                return false;

            SummonersShine.logger.Info("["+ function + "] CanBeChasedBy Found!");

            if (requiresCanHit)
                if (!c.TryGotoNext(i => i.MatchCall<Collision>("CanHit") || i.MatchCall<Collision>("CanHitLine")))
                {
                    SummonersShine.logger.Error("Hook failed! (" + function + ", CanHit cannot be found)");
                    return false;
                }

            int npc_varPos = -1;
            if (!c.TryGotoPrev(i => i.MatchLdloc(out npc_varPos)))
            {
                SummonersShine.logger.Error("Hook failed! (" + function + ", Last LdLoc cannot be found!)");
                return false;
            }

            bool isNumber = il.Body.Variables[npc_varPos].VariableType.IsValueType;

            ILLabel val;
            if (!c.TryGotoNext(i => i.MatchBrfalse(out val)))
            {
                SummonersShine.logger.Error("Hook failed! (" + function + ", Cannot find brfalse!)");
                return false;
            }
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, npc_varPos);
            if (isNumber)
            {
                c.EmitDelegate(ReworkMinion_Projectile.SetMoveTarget_FromID);
            }
            else
            {
                c.EmitDelegate(ReworkMinion_Projectile.SetMoveTarget);
            }
            return true;
        }
    }
}
