using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {
        public static void DoUpdateInWorld_DoubleUpdate(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (!c.TryGotoNext(i => i.MatchStsfld<Main>(nameof(Main.ProjectileUpdateLoopIndex))))
            {
                SummonersShine.logger.Error("[DoUpdateInWorld_DoubleUpdate] Hook failed! Can't find Main.ProjectileUpdateLoopIndex 1!");
                return;
            }
            c.Index += 3;
            if (!c.TryGotoNext(i => i.MatchStsfld<Main>(nameof(Main.ProjectileUpdateLoopIndex))))
            {
                SummonersShine.logger.Error("[DoUpdateInWorld_DoubleUpdate] Hook failed! Can't find Main.ProjectileUpdateLoopIndex 2!");
                return;
            }
            c.Index -= 1;
            c.EmitDelegate(DoubleUpdate);

        }

        static int ResetCurrentTick(int projcount) {

            Projectile proj = Main.projectile[projcount];
            if (proj == null || !proj.active)
                return projcount;
            ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
            if (projFuncs.IsMinion == ProjMinionRelation.isMinion)
            {
                MinionProjectileData projData = projFuncs.GetMinionProjData();
                //projData.currentTick = 1;
            }
            return projcount;
        }
        static void DoubleUpdate()
        {
            SingleThreadExploitation.doingDoubleUpdate = true;
            int size = SingleThreadExploitation.doubleUpdatedProjectilesSize;
            SingleThreadExploitation.doubleUpdatedProjectilesSize = 0;
            for (int i = 0; i < size; i++)
            {
                int m = SingleThreadExploitation.doubleUpdatedProjectiles[i];
                Main.ProjectileUpdateLoopIndex = m;
                Projectile proj = Main.projectile[m];
                if (proj != null && proj.active)
                {
                    proj.Update(m);
                }
            }
            if (SingleThreadExploitation.doubleUpdatedProjectilesSize > 0)
                DoubleUpdate();
            else
                SingleThreadExploitation.doingDoubleUpdate = false;
        }
    }
}
