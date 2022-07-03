using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SummonersShine.BakedConfigs;
using SummonersShine.Projectiles;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {
        //Hornet, Imp, Tempest, Xeno, Stardust Cell
        public static void AI062_Reg_RelativeProjVel(ILContext il)
        {
            ILCursor c = new(il);

            if (!c.TryGotoNext(i => i.MatchStloc(11)))
            {
                SummonersShine.logger.Error("Hook failed! (AI062_Reg_RelativeProjVel, 1st i.MatchStloc(6))");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, 10);
            c.EmitDelegate<Action<Projectile, int>>(ReworkMinion_Projectile.SetMoveTarget_FromID);


            if (!c.TryGotoNext(i => i.MatchLdcR4(3600)))
            {
                SummonersShine.logger.Error("Hook failed! (AI062_Reg_RelativeProjVel, 1st MatchLdcR4(3600))");
                return;
            }
            c.Index -= 3;
            c.RemoveRange(5);

            if (!c.TryGotoNext(i => i.MatchLdcI4(374)))
            {
                SummonersShine.logger.Error("Hook failed! (AI062_Reg_RelativeProjVel, 1st MatchLdcI4(374))");
                return;
            }
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<Projectile, int>>(GetHornetProjectile);
            c.Remove();

            if (!c.TryGotoNext(i => i.MatchLdcI4(376)))
            {
                SummonersShine.logger.Error("Hook failed! (AI062_Reg_RelativeProjVel, 1st MatchLdcI4(376))");
                return;
            }
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<Projectile, int>>(GetImpProjectile);
            c.Remove();
        }

        public static int GetImpProjectile(Projectile proj)
        {
            ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
            if (BakedConfig.CustomSpecialPowersEnabled(projFuncs.SourceItem))
            {
                MinionProjectileData projData = projFuncs.GetMinionProjData();
                if (projData.castingSpecialAbilityTime != -1)
                    return ProjectileModIDs.ImpSuperFireball;
            }
            return ProjectileID.ImpFireball;
        }
        public static int GetHornetProjectile(Projectile proj)
        {
            ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
            if (BakedConfig.CustomSpecialPowersEnabled(projFuncs.SourceItem))
            {
                MinionProjectileData projData = projFuncs.GetMinionProjData();
                if (projData.castingSpecialAbilityTime != -1)
                    return ProjectileModIDs.HornetCyst;
            }
            return ProjectileID.HornetStinger;
        }
    }
}
