using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SummonersShine.ModSupport
{
	public static class ModSupportUtil
	{
		public static object ExtractVariable_itemFuncs(Item item, int index)
		{
			ReworkMinion_Item itemFuncs = item.GetGlobalItem<ReworkMinion_Item>();
			switch (index)
			{
				case 0:
					return itemFuncs.prefixMinionPower;
				case 1:
					return itemFuncs.UsingSpecialAbility;
				default:
					throw new ArgumentOutOfRangeException("[SetVariable_itemFuncs] index (range is 0-0!)");
			}
		}
		public static void SetVariable_itemFuncs(Item item, int index, object value)
		{
			ReworkMinion_Item itemFuncs = item.GetGlobalItem<ReworkMinion_Item>();
			switch (index)
			{
				case 0:
					itemFuncs.prefixMinionPower = (float)value;
					return;
				case 1:
					itemFuncs.UsingSpecialAbility = (bool)value;
					return;
				default:
					throw new ArgumentOutOfRangeException("[SetVariable_itemFuncs] index (range is 0-0!)");
			}
		}
		public static object ExtractVariable_playerFuncs(Player player, int index)
		{
			ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
			switch (index)
			{
				case 0:
					return playerFuncs.minionPower;
				case 1:
					return playerFuncs.energyRestoreRate;
				case 2:
					return playerFuncs.minionAS;
				case 3:
					return playerFuncs.minionASRetreating;
				case 4:
					return playerFuncs.minionASIgnoreMainWeapon;
				default:
					throw new ArgumentOutOfRangeException("[ExtractVariable_playerFuncs] index (range is 0-4!)");
			}
		}
		public static void SetVariable_playerFuncs(Player player, int index, object value)
		{
			ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
			switch (index)
			{
				case 0:
					playerFuncs.minionPower = (float)value;
					return;
				case 1:
					playerFuncs.energyRestoreRate = (float)value;
					return;
				case 2:
					playerFuncs.minionAS = (float)value;
					return;
				case 3:
					playerFuncs.minionASRetreating = (float)value;
					return;
				case 4:
					playerFuncs.minionASIgnoreMainWeapon = (float)value;
					return;
				default:
					throw new ArgumentOutOfRangeException("[SetVariable_playerFuncs] index (range is 0-4!)");
			}
		}
		public static object ExtractVariable_projFuncs(Projectile projectile, int index)
		{
			ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
			switch (index)
			{
				case 0:
					return projFuncs.ProjectileCrit;
				case 1:
					return projFuncs.MinionASMod;
				case 2:
					return (int)projFuncs.IsMinion;
				case 3:
					return projFuncs.killed;
				case 4:
					return projFuncs.killedTicks;
				case 5:
					return projFuncs.LimitedLife;
				case 6:
					return projFuncs.originalNPCHitCooldown;
				case 7:
					return (int)projFuncs.minionCDType;
				case 8:
					return (int)projFuncs.drawBehindType;
				case 9:
					return projFuncs.ArmorIgnoredPerc;
				case 10:
					return projFuncs.PrefixMinionPower;
				default:
					throw new ArgumentOutOfRangeException("[ExtractVariable_projFuncs] numToExtract (range is 0-10!)");
			}
		}
		public static object ExtractVariable_projData(Projectile projectile, int numToExtract)
		{
			ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
			MinionProjectileData projData = projFuncs.GetMinionProjData();
			switch (numToExtract)
			{

				case 0:
					return projData.castingSpecialAbilityType;
				case 1:
					return projData.energyRegenRateMult;
				case 2:
					return projData.energy;
				case 3:
					return projData.maxEnergy;
				case 4:
					return projData.energyRegenRate;
				case 5:
					return projData.castingSpecialAbilityTime;
				case 6:
					return projData.specialCastTarget;
				case 7:
					return projData.specialCastPosition;
				case 8:
					return projData.cancelSpecialNextFrame;
				case 9:
					return projData.minionFlickeringThreshold;
				case 10:
					throw new ArgumentOutOfRangeException("[ExtractVariable_projData] prefixMinionPower is moved from projData to projFuncs!");
					//return projData.prefixMinionPower;
				case 11:
					return projData.minionTrackingAcceleration;
				case 12:
					return projData.minionTrackingImperfection;
				case 13:
					return (int)projData.trackingState;
				case 14:
					return (int)projData.minionSpeedModType;
				case 15:
					return projData.actualMinionAttackTargetNPC;
				case 16:
					return projData.moveTarget;
				case 17:
					return projData.currentTick;
				case 18:
					return projData.nextTicks;
				case 19:
					return projData.lastSimRateInv;
				case 20:
					return projData.updatedSim;
				case 21:
					return projData.isRealTick;
				case 22:
					return projData.lastRelativeVelocity;
				case 23:
					return projData.alphaOverride;
				case 24:
					return projData.isTeleportFrame;
				default:
					throw new ArgumentOutOfRangeException("[ExtractVariable_projData] index (range is 0-25!)");
			}
		}
		public static void SetVariable_projFuncs(Projectile projectile, int index, object value)
		{
			ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
			switch (index)
			{
				case 0:
					projFuncs.ProjectileCrit = (int)value;
					return;
				case 1:
					projFuncs.MinionASMod = (float)value;
					return;
				case 2:
					projFuncs.IsMinion = (ProjMinionRelation)value;
					return;
				case 3:
					projFuncs.killed = (bool)value;
					return;
				case 4:
					projFuncs.killedTicks = (byte)value;
					return;
				case 5:
					projFuncs.LimitedLife = (bool)value;
					return;
				case 6:
					projFuncs.originalNPCHitCooldown = (int)value;
					return;
				case 7:
					projFuncs.minionCDType = (MinionCDType)value;
					return;
				case 8:
					projFuncs.drawBehindType = (DrawBehindType)value;
					return;
				case 9:
					projFuncs.ArmorIgnoredPerc = (float)value;
					return;
				case 10:
					projFuncs.PrefixMinionPower = (float)value;
					return;
				default:
					throw new ArgumentOutOfRangeException("[SetVariable_projFuncs] index (range is 0-9!)");
			}
		}
		public static void SetVariable_projData(Projectile projectile, int index, object value)
		{
			ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
			MinionProjectileData projData = projFuncs.GetMinionProjData();
			switch (index)
			{
				case 0:
					projData.castingSpecialAbilityType = (int)value;
					return;
				case 1:
					projData.energyRegenRateMult = (float)value;
					return;
				case 2:
					projData.energy = (float)value;
					return;
				case 3:
					projData.maxEnergy = (float)value;
					return;
				case 4:
					projData.energyRegenRate = (float)value;
					return;
				case 5:
					projData.castingSpecialAbilityTime = (int)value;
					return;
				case 6:
					projData.specialCastTarget = (NPC)value;
					return;
				case 7:
					projData.specialCastPosition = (Microsoft.Xna.Framework.Vector2)value;
					return;
				case 8:
					projData.cancelSpecialNextFrame = (bool)value;
					return;
				case 9:
					projData.minionFlickeringThreshold = (float)value;
					return;
				case 10:
					throw new ArgumentOutOfRangeException("[SetVariable_projData] prefixMinionPower is moved from projData to projFuncs!");
					//projData.prefixMinionPower = (float)value;
					return;
				case 11:
					projData.minionTrackingAcceleration = (float)value;
					return;
				case 12:
					projData.minionTrackingImperfection = (float)value;
					return;
				case 13:
					projData.trackingState = (MinionTracking_State)value;
					return;
				case 14:
					projData.minionSpeedModType = (MinionSpeedModifier)value;
					return;
				case 15:
					projData.actualMinionAttackTargetNPC = (int)value;
					return;
				case 16:
					projData.moveTarget = (Entity)value;
					return;
				case 17:
					projData.currentTick = (float)value;
					return;
				case 18:
					projData.nextTicks = (float)value;
					return;
				case 19:
					projData.lastSimRateInv = (float)value;
					return;
				case 20:
					projData.updatedSim = (bool)value;
					return;
				case 21:
					projData.isRealTick = (bool)value;
					return;
				case 22:
					projData.lastRelativeVelocity = (Microsoft.Xna.Framework.Vector2)value;
					return;
				case 23:
					projData.alphaOverride = (int)value;
					return;
				case 24:
					projData.isTeleportFrame = (bool)value;
					return;
				default:
					throw new ArgumentOutOfRangeException("[SetVariable_projData] index (range is 0-25!)");
			}
		}
	}
}
