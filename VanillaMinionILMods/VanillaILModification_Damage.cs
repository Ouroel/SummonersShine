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
        public static void Damage_Reg_PostAttackSpeed(On.Terraria.Projectile.orig_Damage orig, Projectile proj) {

            PreDamage(proj);
            orig(proj);
            PostDamage(proj);
        }
        public static void Damage_Reg_AttackSpeed(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            ILLabel start = c.DefineLabel();

            //hook modifyhitnpc
            if (!c.TryGotoNext(i => i.MatchCall(typeof(ProjectileLoader), "ModifyHitNPC")))
            {
                SummonersShine.logger.Error("[Damage_Reg_AttackSpeed] Cannot find ProjectileLoader.ModifyHitNPC!");
                return;
            }

            c.EmitDelegate(ModifyHitNPC);
            c.Remove();

            //hook modify damage immunity timer

            if (!c.TryGotoNext(i => i.MatchLdfld<Projectile>("usesIDStaticNPCImmunity")))
            {
                SummonersShine.logger.Error("[Damage_Reg_AttackSpeed] Cannot find Projectile.usesIDStaticNPCImmunity!");
                return;
            }

            c.EmitDelegate(DynamicallyUpdateCooldown);
            c.Emit(OpCodes.Ldarg_0);

            //hook projectile damage attempt
            if (!c.TryGotoNext(i => i.MatchStloc(2)))
            {
                SummonersShine.logger.Error("[Damage_Reg_AttackSpeed] i.MatchStloc(2) not found!");
                return;
            }

            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc_1);
            c.EmitDelegate<Action<Projectile, Rectangle>>(ProjectileOnHit.Iterate);
        }
        public static void ModifyHitNPC(Projectile proj, NPC npc, ref int dmg, ref float kb, ref bool crit, ref int hitDir)
        {
            ProjectileLoader.ModifyHitNPC(proj, npc, ref dmg, ref kb, ref crit, ref hitDir);
            ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
            projFuncs.ModifyHitNPC_Minion(proj, npc, ref dmg, ref kb, ref crit, ref hitDir);
        }

        public static void DynamicallyUpdateCooldown(Projectile projectile)
        {

            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();

            //Dynamically update attack CD
            if (projFuncs.minionCDType != MinionCDType.noCooldown)
            {
                ReworkMinion_Player playerFuncs = Main.player[projectile.owner].GetModPlayer<ReworkMinion_Player>();
                float speed = projFuncs.GetSpeed(projectile);
                float newAttackCD = projFuncs.originalNPCHitCooldown * speed + playerFuncs.LeftoverProjectileImmunity[projectile.type];
                int newAttackCD_baked = (int)newAttackCD;
                if (projFuncs.minionCDType == MinionCDType.idStaticNPCHitCooldown)
                {
                    projectile.idStaticNPCHitCooldown = newAttackCD_baked;
                    newAttackCD -= newAttackCD_baked;
                    playerFuncs.LeftoverProjectileImmunity[projectile.type] = newAttackCD;
                }
                else if (speed > 1 || projFuncs.IsMinion != ProjMinionRelation.isMinion)
                {
                    projectile.localNPCHitCooldown = newAttackCD_baked;
                    newAttackCD -= newAttackCD_baked;
                    playerFuncs.LeftoverProjectileImmunity[projectile.type] = newAttackCD;
                }
            }
        }

        public static void PreDamage(Projectile projectile)
        {
            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();

            if (projFuncs.IsMinion == ProjMinionRelation.isMinion)
            {
                MinionProjectileData minionProjData = projFuncs.GetMinionProjData();
                if (minionProjData.updatedSim)
                {
                    float simRate = minionProjData.lastSimRateInv;
                    projectile.velocity /= simRate;
                }
            }
        }
        public static void PostDamage(Projectile projectile)
        {
            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            if (projFuncs.IsMinion == ProjMinionRelation.isMinion)
            {
                MinionProjectileData minionProjData = projFuncs.GetMinionProjData();

                if (minionProjData.updatedSim)
                {
                    float simRate = minionProjData.lastSimRateInv;
                    projectile.velocity *= simRate;
                }
            }
        }
    }
}
