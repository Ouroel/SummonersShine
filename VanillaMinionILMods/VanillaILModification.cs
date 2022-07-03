using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SummonersShine.BakedConfigs;
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

        delegate void BasicILModification(ILContext il);

        public static void RegisterAll()
        {
            On.Terraria.Player.LookForTileInteractions += LookForTileInteractions_PrioritizeRightClickSpecial;
            IL.Terraria.Player.ItemCheck_ManageRightClickFeatures += ManageRightClickFeatures_DontOpenChesterWhenCastingSpecial;
            On.Terraria.Player.ItemCheck_Inner += ItemCheck_Inner_SetAltUse;
            IL.Terraria.Main.TryInteractingWithMoneyTrough += Main_TryInteractingWithX_DontOpenChesterWhenCastingSpecial;
            IL.Terraria.Main.TryInteractingWithMoneyTrough2 += Main_TryInteractingWithX_DontOpenChesterWhenCastingSpecial;
            IL.Terraria.Main.TryInteractingWithVoidLens += Main_TryInteractingWithX_DontOpenChesterWhenCastingSpecial;

            On.Terraria.Projectile.AI += AI_EncapsulateAllModsWithinPrePostAI;
            IL.Terraria.Projectile.Damage += Damage_Reg_AttackSpeed;
            On.Terraria.Projectile.Damage += Damage_Reg_PostAttackSpeed;
            IL.Terraria.Main.DoUpdateInWorld += DoUpdateInWorld_DoubleUpdate;
            IL.Terraria.Projectile.Update += Update_EmitMinionDoubleUpdate;
            IL.Terraria.Projectile.UpdatePosition += UpdatePosition_Reg_SlopeCollision;
            On.Terraria.Projectile.Minion_FindTargetInRange += Minion_FindTargetInRange_EmitTracking;
            IL.Terraria.Projectile.AI_026 += AI026_Reg_MinionTrackingTarget;
            IL.Terraria.Projectile.AI_062 += AI062_Reg_RelativeProjVel;
            IL.Terraria.Projectile.AI_158_BabyBird += BabyBird_Reg_MinionTrackingState;
            On.Terraria.Projectile.AI_120_StardustGuardian_FindTarget += StardustGuardian_RegTracking;
            //IL.Terraria.Projectile.AI_120_StardustGuardian += StardustGuardian_SteppedMovement;
            IL.Terraria.Projectile.AI_121_StardustDragon += Dragon_Reg_MinionTrackingState;
            IL.Terraria.Projectile.AI_164_StormTigerGem += AI164_ReplaceHomeLocation;
            IL.Terraria.Projectile.AI_156_Think += BatOfLight_Reg_AttackSpeed;
            IL.Terraria.Projectile.AI_GetMyGroupIndexAndFillBlackList += AI_GetMyGroupIndexAndFilterBlackList_AddDyingAnimationCheck;
            IL.Terraria.Main.DrawProj_EmpressBlade += DrawProj_EmpressBlade_ModifyAttackSpeed;

            
            IL.Terraria.Projectile.VanillaAI += VanillaAI_RegSetMoveTarget; //PROBLEM - CAUSES CRASH UPON UNLOAD
            IL.Terraria.Projectile.AI_007_GrapplingHooks += AI_007_LatchOntoPlatforms;
            IL.Terraria.Projectile.GetWhipSettings += GetWhipSettings_Hook;
            IL.Terraria.Projectile.AI_130_FlameBurstTower_FindTarget += FlameBurstTower_Ballista_FindTarget_EmitGetTarget;
            IL.Terraria.Projectile.AI_134_Ballista_FindTarget += FlameBurstTower_Ballista_FindTarget_EmitGetTarget;
            IL.Terraria.Projectile.AI_134_Ballista += Ballista_FaceProperTracking;
            IL.Terraria.Main.DrawInterface_1_1_DrawEmoteBubblesInWorld += DrawInterface_1_1_DrawEmoteBubblesInWorld_EmitDrawSummonerBubbles;
            On.Terraria.Main.DrawInterface_Resources_Buffs += DrawInterface_Resources_Buffs_DrawSummonerPins;
            IL.Terraria.Player.FreeUpPetsAndMinions += FreeUpPetsAndMinions_EmitIgnore0Slots; //PROBLEM - CAUSES CRASH UPON UNLOAD
            IL.Terraria.Player.ItemCheck_MinionAltFeatureUse += MinionAltFeatureUse_Delete;
            On.Terraria.Player.FindSentryRestingSpot += FindSentryRestingSpot_EmitMagenChiunCheck;
            On.Terraria.Player.ItemCheck_Shoot += ItemCheck_Shoot_CatchSpawnedBuffs; //PROBLEM - CAUSES CRASH UPON UNLOAD
            IL.Terraria.Player.ItemCheck_Shoot += ItemCheck_Shoot_MagenChiunFloat; //PROBLEM - CAUSES CRASH UPON UNLOAD
            IL.Terraria.Player.UpdateMaxTurrets += UpdateMaxTurrets_Reg_DontKillDeadTurrets;

            IL.Terraria.Main.HelpText += HelpText_Reg_CustomHelpText; //PROBLEM - CAUSES CRASH UPON UNLOAD
            IL.Terraria.Main.DoDraw += DoDraw_CustomPostDrawProjectiles;
            IL.Terraria.Main.DrawProjectiles += DrawProjectiles_CustomPostDrawProjectiles;
            On.Terraria.Main.DoDraw_DrawNPCsOverTiles += DoDraw_DrawNPCsOverTiles_CustomPostDrawProjectiles;
            IL.Terraria.NetMessage.SendData += SendData_EmitMinionDataValues; //PROBLEM - CAUSES CRASH UPON UNLOAD
            IL.Terraria.MessageBuffer.GetData += GetData_EmitMinionDataValues;
            On.Terraria.Item.Prefix += Prefix_ConvertCalaPrefixes;
            IL.Terraria.Main.DrawInventory += DrawInventory_PostReforgeEncapsulate;

            //On.Terraria.NetMessage.SendData += SendData_MinionDataValues;

            On.Terraria.Main.UpdateTimeRate += UpdateTimeRate_ModifyTimeRate;
            On.Terraria.Graphics.Effects.SkyManager.Update += UpdateSkyManager_DontAccelerateSurroundings;
            On.Terraria.Cloud.UpdateClouds += UpdateClouds_DontAccelerateSurroundings;

            On.Terraria.Player.UpdateAbigailStatus += DeleteAbigailStatus;
        }

        private static void ItemCheck_Shoot_CatchSpawnedBuffs(On.Terraria.Player.orig_ItemCheck_Shoot orig, Player self, int i, Item sItem, int weaponDamage)
        {
            bool countsAsClass = sItem.CountsAsClass(DamageClass.Summon) && !BakedConfig.ItemHasBuffLinked[sItem.type];
            int prevBuffCount = 0;
            int buffID = 0;
            if (countsAsClass)
            {
                prevBuffCount = self.GetBuffCount();
                if (prevBuffCount > 0)
                    buffID = self.buffType[prevBuffCount - 1];
            }
            orig(self, i, sItem, weaponDamage);

            if (countsAsClass)
            {
                int newBuffCount = self.GetBuffCount();
                if (newBuffCount > 0)
                {
                    int newBuffID = self.buffType[newBuffCount - 1];
                    if (newBuffCount > prevBuffCount)
                    {
                        BakedConfig.ItemHasBuffLinked[sItem.type] = true;
                        BakedConfig.AddBuffSourceItem(self.buffType[prevBuffCount], sItem.type);
                    }
                    else if (buffID != newBuffID)
                    {
                        BakedConfig.ItemHasBuffLinked[sItem.type] = true;
                        BakedConfig.AddBuffSourceItem(self.buffType[newBuffCount - 1], sItem.type);
                    }
                }
            }
        }

        public static void UnregisterAll()
        {
        }

        public static void Update_EmitMinionDoubleUpdate(ILContext il)
        {
            ILCursor c = new ILCursor(il);            

            if (!c.TryGotoNext(i => i.MatchStfld<Projectile>("numUpdates")))
            {
                SummonersShine.logger.Error("[Update_EmitMinionDoubleUpdate] numUpdates not found!");
                return;
            }
            c.Index -= 2;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate<Action<Projectile, int>>(Update_MinionDoubleUpdate);

            /*//update damage
            if (!c.TryGotoNext(
                i => i.MatchLdfld<Projectile>("numUpdates"),
                i => i.MatchLdcI4(-1)
                ))
            {
                SummonersShine.logger.Error("[Update_EmitMinionDoubleUpdate] numUpdates 1 not found!");
                return;
            }

            ILLabel lab = c.DefineLabel();
            c.Index += 3;
            lab.Target = ((ILLabel)c.Prev.Operand).Target;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(IsUpdateTick);
            c.Emit(OpCodes.Brfalse, lab);*/

            //can update minion count
            if (!c.TryGotoNext(
                i => i.MatchLdfld<Projectile>("numUpdates"),
                i => i.MatchLdcI4(-1)
                ))
            {
                SummonersShine.logger.Error("[Update_EmitMinionDoubleUpdate] numUpdates 2 not found!");
                return;
            }

            ILLabel lab = c.DefineLabel();
            c.Index += 3;
            lab.Target = ((ILLabel)c.Prev.Operand).Target;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(CanMinionUpdateMinionCount);
            c.Emit(OpCodes.Brfalse, lab);

            // add post handle movement hook

            if (!c.TryGotoNext(i => i.MatchCall<Projectile>("HandleMovement")))
            {
                SummonersShine.logger.Error("[Update_EmitMinionDoubleUpdate] HandleMovement not found!");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Action<Projectile>>(OnMovement);

            //stop tickdown for duped ticks
            if (!c.TryGotoNext(i => i.MatchStfld<Projectile>("timeLeft")))
            {
                SummonersShine.logger.Error("[Update_EmitMinionDoubleUpdate] timeLeft not found!");
                return;
            }

            c.Index--;
            //c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(DontDecreIfDoubleUpdate);
            c.Index += 2;
            //stop tickdown for duped ticks
            if (!c.TryGotoNext(i => i.MatchStfld<Projectile>("timeLeft")))
            {
                SummonersShine.logger.Error("[Update_EmitMinionDoubleUpdate] timeLeft not found!");
                return;
            }
            c.Index--;
            //c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(DontDecreIfDoubleUpdate);
        }

        static int DontDecreIfDoubleUpdate(int change) {
            if (SingleThreadExploitation.doingDoubleUpdate) {
                return 0;
            }
            return change;
        }

        //This stacks with the game's extraupdates and can be called multiple times! literally simulating the minion 2x to make it faster.
        public static void Update_MinionDoubleUpdate(Projectile projectile, int i)
        {
            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            if (projFuncs.IsMinion == ProjMinionRelation.isMinion)
            {
                MinionProjectileData projData = projFuncs.GetMinionProjData();
                Player player = Main.player[projectile.owner];
                if (projFuncs.killedTicks > 0)
                {
                    projData.currentTick = 1;
                    return;
                }

                //do not take or add any ticks - pause the doubleupdate thingy
                if (projData.minionSpeedModType == MinionSpeedModifier.none || projData.minionSpeedModType == MinionSpeedModifier.letothersupdate)
                    return;

                float maxRealTicks = projFuncs.GetRealTicksPerTick(projectile);
                projData.nextTicks -= 1;

                projData.currentTick = MathF.Floor(projData.nextTicks) + 1;
                if (projData.currentTick < 1) {
                    projData.currentTick = 1;
                }

                //Not whole tick. Take one tick away and don't simulate this. fake frame. difference is added in VanillaILModification_BatOfLight.
                if (projData.minionSpeedModType == MinionSpeedModifier.stepped && projData.currentTick > maxRealTicks)
                {
                    projData.nextTicks -= 1;
                    projData.currentTick -= 1;
                }

                if (projData.nextTicks >= 1)
                {
                    // such that duplicated update loops do not count a minion 2x/tick

                    SingleThreadExploitation.doubleUpdatedProjectiles[SingleThreadExploitation.doubleUpdatedProjectilesSize] = i;
                    SingleThreadExploitation.doubleUpdatedProjectilesSize++;
                    /*
                    projData.currentTick++;
                    projectile.timeLeft += projectile.extraUpdates + 1;
                    projectile.Update(i);
                    projData.currentTick--;
                    */
                }
                else
                {
                    projData.nextTicks += projFuncs.GetSimulationRate(projectile);
                }
            }
        }
        public static bool IsUpdateTick(Projectile projectile)
        {
            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            if (projFuncs.IsMinion == ProjMinionRelation.isMinion)
            {
                MinionProjectileData projData = projFuncs.GetMinionProjData();
                return projData.currentTick == 1;
            }
            return true;
        }
        public static bool CanMinionUpdateMinionCount(Projectile projectile)
        {
            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            if (SingleThreadExploitation.doingDoubleUpdate || projectile.minionSlots <= 0)
                return false;

            if (projFuncs.IsMinion == ProjMinionRelation.isMinion)
            {
                if (projFuncs.killedTicks > 0)
                    return false;
            }
            return true;
        }
        public static void OnMovement(Projectile projectile)
        {
            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            if (projFuncs.IsMinion == ProjMinionRelation.isMinion)
                projFuncs.minionOnMovement(projectile, projFuncs, projFuncs.GetMinionProjData());
        }
        private static void Minion_FindTargetInRange_EmitTracking(On.Terraria.Projectile.orig_Minion_FindTargetInRange orig, Projectile self, int startAttackRange, ref int attackTarget, bool skipIfCannotHitWithOwnBody, Func<Entity, int, bool> customEliminationCheck)
        {
            orig(self, startAttackRange, ref attackTarget, skipIfCannotHitWithOwnBody, customEliminationCheck);
            ReworkMinion_Projectile.SetMoveTarget_FromID(self, attackTarget);
        }
        public static void DrawInterface_1_1_DrawEmoteBubblesInWorld_EmitDrawSummonerBubbles(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchCall<EmoteBubble>("DrawAll")))
            {
                SummonersShine.logger.Error("[DrawInterface_1_1_DrawEmoteBubblesInWorld_EmitDrawSummonerBubbles] DrawAll not found!");
                return;
            }
            c.Index++;
            c.EmitDelegate<Action>(EnergyDisplay.DrawAll);
        }
        public static void  FreeUpPetsAndMinions_EmitIgnore0Slots(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchLdfld<Projectile>("minion")))
            {
                SummonersShine.logger.Error("[FreeUpPetsAndMinions_EmitIgnore0Slots] Cannot find minion check!");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldloc_S, (byte)6);
            c.EmitDelegate(Ignore0Slots);
        }
        public static bool Ignore0Slots(int original, int index)
        {
            if (original != 1) return false;
            Projectile proj = Main.projectile[index];
            if (proj != null && proj.active)
            {
                ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
                if (projFuncs.killedTicks > 0) return false;
            }

            return Main.projectile[index].minionSlots > 0;
        }

        public static void DeleteAbigailStatus(On.Terraria.Player.orig_UpdateAbigailStatus orig, Player player) {

            int num = 963;
            if (player.ownedProjectileCounts[num] < 1)
            {
                Projectile.NewProjectile(player.GetSource_Misc("AbigailTierSwap"), player.Center, Vector2.Zero, num, 0, 0f, player.whoAmI, 0f, 0f);
            }
        }
    }
}
