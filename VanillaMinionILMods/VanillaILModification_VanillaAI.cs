using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {
        public static void VanillaAI_RegSetMoveTarget(ILContext il)
        {
            var c = new ILCursor(il);
            string func = "VanillaAI_RegSetMoveTarget";

            //AI 053 frost hydra

            if (!GetAIStyle(il, c, 53, func))
                return;

            for (int x = 0; x < 2; x++)
            {
                if (!MassPatcher_TrackTarget(il, c, func))// PROBLEM FUNCTION
                return;
            }

            //AI_054 Ravens

            if (!GetAIStyle(il, c, 54, func))
                return;

            if (!c.TryGotoNext(i => i.MatchLdcR4(900)))
            {
                SummonersShine.logger.Error("[VanillaAI_RegSetMoveTarget] Hook failed! Can't find Raven AI 054 range!");
                return;
            }
            c.Remove();
            c.Emit(OpCodes.Ldc_R4, 1100f);

            if (!c.TryGotoNext(i => i.MatchLdcI4(500)))
            {
                SummonersShine.logger.Error("[VanillaAI_RegSetMoveTarget] Hook failed! Can't find Raven AI 054 range!");
                return;
            }
            c.Remove();
            c.Emit(OpCodes.Ldc_I4, 1100);

            for (int x = 0; x < 2; x++)
            {
                if (!MassPatcher_TrackTarget(il, c, func))
                    return;
            }

            //AI_066 Ret, Spaz and Spheres

            if (!GetAIStyle(il, c, 66, func))
                return;

            for (int x = 0; x < 2; x++)
            {
                if (!MassPatcher_TrackTarget(il, c, func))
                    return;
            }

            //make ret face in the shooty direction
            for (int x = 0; x < 5; x++)
                GetType(il, c, 387, func);
            if (!c.TryGotoNext(i => i.MatchCall("Terraria.Utils", "ToRotation")))
            {
                SummonersShine.logger.Error("[VanillaAI_RegSetMoveTarget] Hook failed! Can't find Ret Turny Hook!");
                return;
            }

            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(Retanimini_FaceActualTarget);

            //fix Deadly Sphere range

            if (!c.TryGotoNext(i => i.MatchStloc(829)))
            {
                SummonersShine.logger.Error("[VanillaAI_RegSetMoveTarget] Hook failed! Can't find Deadly Sphere AI 066 range!");
                return;
            }
            for (int x = 0; x < 3; x++)
            {
                if (!c.TryGotoNext(i => i.MatchStloc(829)))
                {
                    SummonersShine.logger.Error("[VanillaAI_RegSetMoveTarget] Hook failed! Can't find Deadly Sphere AI 066 range!");
                    return;
                }
                c.EmitDelegate(GetDeadlySphereRange);
                c.Index += 3;
            }

            //fix moon lord turret laser
            if (!c.TryGotoNext(i => i.MatchLdcI4(642)))
            {
                SummonersShine.logger.Error("[VanillaAI_RegSetMoveTarget] Hook failed! Can't find Moon Lord Laser AI");
                return;
            }
            if (!c.TryGotoNext(i => i.MatchLdcI4(642)))
            {
                SummonersShine.logger.Error("[VanillaAI_RegSetMoveTarget] Hook failed! Can't find Moon Lord Laser AI");
                return;
            }
            if (!c.TryGotoNext(i => i.MatchLdcR4(1)))
            {
                SummonersShine.logger.Error("[VanillaAI_RegSetMoveTarget] Hook failed! Can't find Moon Lord Laser AI Life");
                return;
            }
            if (!c.TryGotoNext(i => i.MatchLdarg(0)))
            {
                SummonersShine.logger.Error("[VanillaAI_RegSetMoveTarget] Hook failed! Can't find Moon Lord Laser AI Life");
                return;
            }
                
            c.Index++;
            c.EmitDelegate(MoonlordTurretLaser_UpdateLocalAI);
            c.RemoveRange(12);

            if (!c.TryGotoNext(i => i.MatchLdloc(1015)))
            {
                SummonersShine.logger.Error("[VanillaAI_RegSetMoveTarget] Hook failed! Can't find Moon Lord Laser AI Length");
                return;
            }
            c.Index++;  
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(MoonlordTurretLaser_UpdateLocalAI_Length);
            return;
        }   
        public static void MoonlordTurretLaser_UpdateLocalAI(Projectile projectile)
        {
            Projectile parentProj = Main.projectile[(int)projectile.ai[1]];
            projectile.localAI[0] = MathHelper.Clamp((int)((parentProj.ai[0] + 60) * 50 / 40), 1, 50);
        }
        public static float MoonlordTurretLaser_UpdateLocalAI_Length(float amount, Projectile projectile)
        {
            if (projectile.type != ProjectileID.MoonlordTurretLaser)
                return amount;
            Projectile parentProj = Main.projectile[(int)projectile.ai[1]];
            float simRate = parentProj.GetGlobalProjectile<ReworkMinion_Projectile>().GetSimulationRate(parentProj);
            return 1 - MathF.Pow(1 - amount, simRate);
        }

        public static Vector2 Retanimini_FaceActualTarget(Vector2 facingVel, Projectile projectile)
        {
            return ReworkMinion_Projectile.GetTotalProjectileVelocity(facingVel.SafeNormalize(Vector2.Zero) * 8, 3, projectile, null);
        }

        static int GetDeadlySphereRange(int range) {
            return 2000;
        }
    }
}   
