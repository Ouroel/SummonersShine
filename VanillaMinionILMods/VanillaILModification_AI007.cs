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
        public static void AI_007_LatchOntoPlatforms(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            ILLabel lab = il.DefineLabel();
            if (!c.TryGotoNext(i => i.MatchCallOrCallvirt<Tile>("nactive")))
            {
                SummonersShine.logger.Error("[AI_007_LatchOntoPlatforms] i.MatchCallVirt<Player>(\"nactive\") not found!");
                return;
            }
            if (!c.TryGotoNext(i => i.MatchCallOrCallvirt<Tile>("nactive")))
            {
                SummonersShine.logger.Error("[AI_007_LatchOntoPlatforms] i.MatchCallVirt<Player>(\"nactive\") 2 not found!");
                return;
            }
            int ind = c.Index;
            if (!c.TryGotoNext(i => i.MatchCallvirt<Player>("IsBlacklistedForGrappling")))
            {
                SummonersShine.logger.Error("[AI_007_LatchOntoPlatforms] i.MatchCallVirt<Player>(\"IsBlacklistedForGrappling\") not found!");
                return;
            }
            c.Index += 2;
            c.MarkLabel(lab);
            c.Index = ind - 1;

            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<Projectile, bool>>(LatchOntoPlatforms);
            c.Emit(OpCodes.Brtrue, lab);

            if (!c.TryGotoNext(i => i.MatchCallOrCallvirt<Tile>("nactive")))
            {
                SummonersShine.logger.Error("[AI_007_LatchOntoPlatforms] i.MatchCallVirtt<Player>(\"nactive\") 3 not found!");
                return;
            }
            if (!c.TryGotoNext(i => i.MatchLdloc(44)))
            {
                SummonersShine.logger.Error("[AI_007_LatchOntoPlatforms] i.MatchLdloc(43) not found!");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<bool, Projectile, bool>>(IsLatchingOntoPlatforms);
        }

        static bool LatchOntoPlatforms(Projectile proj)
        {
            proj.GetGlobalProjectile<ReworkMinion_Projectile>().hookedPlatform = PlatformCollection.AttemptGrapple(proj);
            return proj.GetGlobalProjectile<ReworkMinion_Projectile>().hookedPlatform != null;
        }

        static bool IsLatchingOntoPlatforms(bool original, Projectile proj)
        {
            if (!original)
                return false;
            return proj.GetGlobalProjectile<ReworkMinion_Projectile>().hookedPlatform == null;
        }
    }
}
