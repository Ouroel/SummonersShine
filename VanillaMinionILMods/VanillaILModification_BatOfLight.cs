using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {
        public static void BatOfLight_Reg_AttackSpeed(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (!c.TryGotoNext(i => i.MatchLdcI4(60)))
            {
                SummonersShine.logger.Error("[BatOfLight_Reg_AttackSpeed] Hook failed! 1st MatchLdcI4(60))");
                return;
            }

            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<int, Projectile, int>>(BatOfLight_ModifyAttackSpeed);

            if (!c.TryGotoNext(i => i.MatchLdcI4(60)))
            {
                SummonersShine.logger.Error("[BatOfLight_Reg_AttackSpeed] Hook failed! 2nd MatchLdcI4(60))");
                return;
            }

            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<int, Projectile, int>>(BatOfLight_ModifyAttackSpeed);

            if (!c.TryGotoNext(i => i.MatchLdcI4(66)))
            {
                SummonersShine.logger.Error("[BatOfLight_Reg_AttackSpeed] Hook failed! 1st MatchLdcI4(66))");
                return;
            }

            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<int, Projectile, int>>(BatOfLight_ModifyAttackSpeed);

            if (!c.TryGotoNext(i => i.MatchLdcI4(40)))
            {
                SummonersShine.logger.Error("[BatOfLight_Reg_AttackSpeed] Hook failed! 1st MatchLdcI4(40))");
                return;
            }

            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<int, Projectile, int>>(BatOfLight_ModifyAttackSpeed);

            if (!c.TryGotoNext(i => i.MatchLdcI4(40)))
            {
                SummonersShine.logger.Error("[BatOfLight_Reg_AttackSpeed] Hook failed! 2nd MatchLdcI4(40))");
                return;
            }

            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<int, Projectile, int>>(BatOfLight_ModifyAttackSpeed);

            if (!c.TryGotoNext(i => i.MatchCall<Projectile>("AI_156_GetIdlePosition")))
            {
                SummonersShine.logger.Error("[BatOfLight_Reg_AttackSpeed] Hook failed! 1st MatchCall<Projectile>('AI_156_GetIdlePosition')");
                return;
            }

            c.Index++;
            c.Emit(OpCodes.Ldloc, 17);
            c.Emit(OpCodes.Ldloc, 7);
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(Terraprisma_Dist_Fix);
            c.Emit(OpCodes.Stloc, 17);

            if (!c.TryGotoNext(i => i.MatchCall("Terraria.Utils", "MoveTowards")))
            {
                SummonersShine.logger.Error("[BatOfLight_Reg_AttackSpeed] Hook failed! 1st MatchCall('Terraria.Utils', 'MoveTowards')");
                return;
            }

            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<float, Projectile, float>>(BatOfLight_GetSimulationRate);
            c.Index++;
            
            if (!c.TryGotoNext(i => i.MatchCall("Terraria.Utils", "AngleLerp")))
            {
                SummonersShine.logger.Error("[BatOfLight_Reg_AttackSpeed] Hook failed! 1st MatchCall('Terraria.Utils', 'AngleLerp')");
                return;
            }

            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<float, Projectile, float>>(BatOfLight_GetSimulationRate);
            c.Index++;

            if(!c.TryGotoNext(i=>i.MatchRet()))
            {
                SummonersShine.logger.Error("[BatOfLight_Reg_AttackSpeed] Hook failed! Can't Find i.MatchRet()");
                return;
            }

            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(Terraprisma_Dist_PostFix);
            c.Index++;

            if (!c.TryGotoNext(i => i.MatchCall("Terraria.Utils", "AngleLerp")))
            {
                SummonersShine.logger.Error("[BatOfLight_Reg_AttackSpeed] Hook failed! 2nd MatchCall('Terraria.Utils', 'AngleLerp')");
                return;
            }

            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<float, Projectile, float>>(BatOfLight_GetSimulationRate);
            c.Index++;

            if (!c.TryGotoNext(i => i.MatchCall("Terraria.Utils", "AngleTowards")))
            {
                SummonersShine.logger.Error("[BatOfLight_Reg_AttackSpeed] Hook failed! 1st MatchCall('Terraria.Utils', 'AngleTowards')");
                return;
            }

            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<float, Projectile, float>>(BatOfLight_GetSimulationRate);
            c.Index++;
        }
        public static void AI_GetMyGroupIndexAndFilterBlackList_AddDyingAnimationCheck(ILContext il) {

            ILCursor c = new ILCursor(il); 

            ILLabel _out = c.DefineLabel();
            if (!c.TryGotoNext(i => i.MatchBrfalse(out _out)))
            {
                SummonersShine.logger.Error("[AI_GetMyGroupIndexAndFilterBlackList_AddDyingAnimationCheck] Hook failed! Cannot find MatchBrfalse");
                return;
            }

            c.Index++;
            c.Emit(OpCodes.Ldloc_1);
            c.EmitDelegate<Func<Projectile, bool>>(DyingAnimationCheck);
            c.Emit(OpCodes.Brtrue_S, _out);
        }

        static bool DyingAnimationCheck(Projectile proj) {
            ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
            return projFuncs.killedTicks > 15;
        }

        public static void DrawProj_EmpressBlade_ModifyAttackSpeed(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (!c.TryGotoNext(i => i.MatchLdcR4(60)))
            {
                SummonersShine.logger.Error("[DrawProj_EmpressBlade_ModifyAttackSpeed] Hook failed! 1st MatchLdcR4(60))");
                return;
            }

            c.Index++;
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate<Func<float, Projectile, float>>(BatOfLight_ModifyAttackSpeed_Float);

            if (!c.TryGotoNext(i => i.MatchLdcR4(50)))
            {
                SummonersShine.logger.Error("[DrawProj_EmpressBlade_ModifyAttackSpeed] Hook failed! 1st MatchLdcR4(50))");
                return;
            }

            c.Index++;
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate<Func<float, Projectile, float>>(BatOfLight_ModifyAttackSpeed_Float);
            if (!c.TryGotoNext(i => i.MatchLdcR4(70)))
            {
                SummonersShine.logger.Error("[DrawProj_EmpressBlade_ModifyAttackSpeed] Hook failed! 1st MatchLdcR4(70))");
                return;
            }

            c.Index++;
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate<Func<float, Projectile, float>>(BatOfLight_ModifyAttackSpeed_Float);

            if (!c.TryGotoNext(i => i.MatchLdcR4(50)))
            {
                SummonersShine.logger.Error("[DrawProj_EmpressBlade_ModifyAttackSpeed] Hook failed! 2nd MatchLdcR4(50))");
                return;
            }

            c.Index++;
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate<Func<float, Projectile, float>>(BatOfLight_ModifyAttackSpeed_Float);
            if (!c.TryGotoNext(i => i.MatchLdcR4(40)))
            {
                SummonersShine.logger.Error("[DrawProj_EmpressBlade_ModifyAttackSpeed] Hook failed! 1st MatchLdcR4(40))");
                return;
            }

            c.Index++;
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate<Func<float, Projectile, float>>(BatOfLight_ModifyAttackSpeed_Float);

            if (!c.TryGotoNext(i => i.MatchLdcR4(45)))
            {
                SummonersShine.logger.Error("[DrawProj_EmpressBlade_ModifyAttackSpeed] Hook failed! 1st MatchLdcR4(45))");
                return;
            }

            c.Index++;
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate<Func<float, Projectile, float>>(BatOfLight_ModifyAttackSpeed_Float);
        }

        static int BatOfLight_ModifyAttackSpeed(int num, Projectile projectile)
        {
            return (int)(num / projectile.GetGlobalProjectile<ReworkMinion_Projectile>().GetInternalSimRate(projectile));
        }
        static float BatOfLight_ModifyAttackSpeed_Float(float num, Projectile projectile)
        {
            return num / projectile.GetGlobalProjectile<ReworkMinion_Projectile>().GetInternalSimRate(projectile);
        }
        static float BatOfLight_GetSimulationRate(float num, Projectile projectile)
        {
            return num * projectile.GetGlobalProjectile<ReworkMinion_Projectile>().GetInternalSimRate(projectile);
        }

        //fixes Terraprisma's returning algorithm
        static Vector2 Terraprisma_Dist_Fix(Vector2 playerPos, Player player, Projectile projectile)
        {
            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            SingleThreadExploitation.Terraprisma_Dist_Fix_StoredVector = playerPos;
            return Vector2.SmoothStep(projectile.Center, playerPos, 0.45f) - player.velocity;
        }
        static void Terraprisma_Dist_PostFix(Projectile projectile)
        {
            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            projectile.Center = Vector2.SmoothStep(projectile.Center, SingleThreadExploitation.Terraprisma_Dist_Fix_StoredVector, 0.45f);
        }
    }
}
