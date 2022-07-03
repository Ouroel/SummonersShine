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
        public static void UpdatePosition_Reg_SlopeCollision(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            //hook modifyhitnpc
            if (!c.TryGotoNext(i => i.MatchCall<Collision>("SlopeCollision")))
            {
                SummonersShine.logger.Error("[Damage_Reg_AttackSpeed] Cannot find Collision.SlopeCollision!");
                return;
            }

            c.Index++;
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc_1);
            c.EmitDelegate(OnSlopeCollision);
        }

        public static void OnSlopeCollision(Projectile proj, Vector4 data)
        {
            if (data.Z == proj.velocity.X && data.W == proj.velocity.Y) //no collision
                return;
            ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
            MinionProjectileData projData = null;
            if (projFuncs.IsMinion == ProjMinionRelation.isMinion)
                projData = projFuncs.GetMinionProjData();
            projFuncs.minionOnSlopeCollide(proj, projFuncs, projData, data);
        }
    }
}
