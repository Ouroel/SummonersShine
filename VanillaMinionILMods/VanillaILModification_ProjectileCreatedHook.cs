/* using Mono.Cecil.Cil;
using MonoMod.Cil;
using SummonersShine.Projectiles;
using SummonersShine.SpecialAbilities.DefaultSpecialAbility;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.UI;
using Terraria.ID;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {


        /*****************
		 * Summoner Crit *
		 *****************

        /* Temporary hook until tmodipshits add a hook for projectile creation which includes owner *

        /*
        static void EmitHookNewProjectile(ILContext il)
        {
            var c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchLdsfld<Main>("netMode")))
            {
                SummonersShine.logger.Error("[EmitHookNewProjectile] Hook failed! Cannot find i.MatchLdsfld<Main>(\"NetMode\")");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldloc_1);
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(SetMinionProjectileStats);
        }
        //public static event NewProjectile_Hook OnNewProjectile;

        //finally something good

        public static void InitDynamicMinionCD(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            if (projectile.idStaticNPCHitCooldown > 0)
            {
                projFuncs.originalNPCHitCooldown = projectile.idStaticNPCHitCooldown;
                projFuncs.minionCDType = MinionCDType.idStaticNPCHitCooldown;
            }
            else if (projectile.localNPCHitCooldown > 0)
            {

                projFuncs.originalNPCHitCooldown = projectile.localNPCHitCooldown;
                projFuncs.minionCDType = MinionCDType.localNPCHitCooldown;
            }
            else
            {
                projFuncs.minionCDType = MinionCDType.noCooldown;
            }
        }
        public static void SetMinionProjectileStats(Projectile projectile, IEntitySource projectileSource)
        {

            //rest of the stuff

            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();

            //update cache
            if (!Config.current.DisableDynamicProjectileCacheUpdate)
            {
                Player player = Main.player[projectile.owner];
                player.ownedProjectileCounts[projectile.type]++;
            }
            
            if (projFuncs.IsMinion == ProjMinionRelation.isMinion)
            {
                EntitySource_ItemUse projSource = projectileSource as EntitySource_ItemUse;
                MinionProjectileData projData = projFuncs.GetMinionProjData();
                if (projSource != null)
                {
                    Item item = projSource.Item;
                    ReworkMinion_Item globalItem = item.GetGlobalItem<ReworkMinion_Item>();
                    projFuncs.ProjectileCrit = item.crit;
                    projFuncs.MinionASMod = globalItem.GetUseTimeModifier(item);

                    projData.prefixMinionPower = globalItem.prefixMinionPower;

                    //PacketHandler.WritePacket_UpdateMinionStats(projectile, projFuncs, globalProj);
                    MinionDataHandler.HookSourceItem(projectile, projFuncs, item.netID);

                }

                projFuncs.OnCreation(projectile);
            }
            EntitySource_Parent nextParentProjSource = projectileSource as EntitySource_Parent;
            if (nextParentProjSource != null)
            {
                Projectile nextParentProj = nextParentProjSource.Entity as Projectile;
                if (nextParentProj != null)
                {
                    ReworkMinion_Projectile parentProjFuncs = nextParentProj.GetGlobalProjectile<ReworkMinion_Projectile>();
                    if (parentProjFuncs.IsMinion == ProjMinionRelation.notMinion)
                        return;

                    projFuncs.ProjectileCrit = parentProjFuncs.ProjectileCrit;
                    projFuncs.MinionASMod = parentProjFuncs.MinionASMod;
                    MinionProjectileData parentProjData = parentProjFuncs.GetMinionProjData();

                    if (projFuncs.IsMinion == ProjMinionRelation.isMinion)
                    {
                        MinionProjectileData projData = projFuncs.GetMinionProjData();
                        projData.prefixMinionPower = parentProjData.prefixMinionPower;
                        MinionDataHandler.HookSourceItem(projectile, projFuncs, parentProjFuncs.SourceItem);
                    }
                    else
                    {
                        InitDynamicMinionCD(projectile, projFuncs);
                        projFuncs.IsMinion = ProjMinionRelation.fromMinion;
                    }
                    if (parentProjFuncs.IsMinion != ProjMinionRelation.isMinion)
                        return;
                    parentProjFuncs.minionOnShootProjectile(projectile, nextParentProj, parentProjFuncs, parentProjData);

                    bool configEnabled = true;
                    NPC moveTarget = null;

                    Config.current.ProjectilesForceAimbot.ForEach(i =>
                    {
                        if (i.proj.Type == nextParentProj.type)
                        {
                            int attackTarget = -1;
                            nextParentProj.Minion_FindTargetInRange(i.startAttackRange, ref attackTarget, i.skipIfCannotHit);
                            if(attackTarget != -1)
                                moveTarget = Main.npc[attackTarget];
                        }
                    });

                    if (MinionDataHandler.NoTrackingProjectiles[projectile.type])
                        return;

                    if (moveTarget == null) {

                        if (Config.current.DisableModdedMinionTracking && (nextParentProj.ModProjectile != null && nextParentProj.ModProjectile.Mod != SummonersShine.modInstance))
                            configEnabled = false;
                        moveTarget = parentProjData.moveTarget as NPC;
                    }
                    Config.current.ProjectilesIgnoreTracking.ForEach(i =>
                    {
                        if (i.Type == nextParentProj.type) configEnabled = false;
                    });

                    if (configEnabled && !ModUtils.IsProjectileStationary(projectile.type))
                        projectile.velocity = ReworkMinion_Projectile.GetTotalProjectileVelocity(projectile, nextParentProj, moveTarget);
                }
            }
        }
    }
}
*/