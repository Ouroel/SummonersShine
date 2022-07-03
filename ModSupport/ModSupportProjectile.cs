using Microsoft.Xna.Framework;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.ModSupport
{
	public class ModSupportProjectile
	{
		public static List<ModSupportProjectile> ModSupportProjectiles = new();

		public static ModSupportProjectile Generate(int ProjID)
		{
			ModSupportProjectile rv = ModSupportProjectiles.Find(i => i.ProjectileID == ProjID);
			if (rv == null)
			{
				rv = new(ProjID);
				ModSupportProjectiles.Add(rv);
			}
			return rv;
		}

        public class ModSupportProjectile_Loader : ILoadable
        {
            public void Load(Mod mod)
            {
				ModSupportProjectiles = new();

			}

            public void Unload()
            {
				ModSupportProjectiles = null;

			}
        }

        public int ProjectileID;

		float minionTrackingAcceleration = 0;
		float minionTrackingImperfection = 0;
		float maxEnergy = 0;

		Action<Projectile, Player> onPostCreation;
		Action<Projectile> minionPreAI;
		Action<Projectile, Entity, int, bool> minionOnSpecialAbilityUsed;
		Action<Projectile, Player> minionTerminateSpecialAbility;
		Func<Projectile, bool> minionCustomAI;
		Func<Projectile, Color, bool> minionCustomPreDraw;
		Action<Projectile, Color> minionCustomPostDraw;
		Func<Projectile, Color, Color> minionModifyColor;
		Action<Projectile> minionEndOfAI;
		Action<Projectile> minionPostAI;
		Action<Projectile> minionPostAIPostRelativeVelocity;
		Action<Projectile, Player> minionSummonEffect;
		Action<Projectile, Player> minionDespawnEffect;
		Action<Projectile, Player> minionOnCreation;
		Func<Projectile, Vector2, bool> minionOnTileCollide;
		Action<Projectile, Vector4> minionOnSlopeCollide;
		Action<Projectile, Projectile> minionOnShootProjectile;
		Action<Projectile, Projectile, NPC, int, float, bool> minionOnPlayerHitNPCWithProj;

		Func<Projectile, Entity, int, float, bool, int, Tuple<int, float, bool, int>> minionOnHitNPC;
		Func<Projectile, Entity, int, float, bool, int, Tuple<int, float, bool, int>> minionOnHitNPC_Fake;

		MinionSpeedModifier minionSpeedModType = MinionSpeedModifier.normal;
		MinionTracking_State minionTrackingState = MinionTracking_State.noTracking;
		float minionFlickeringThreshold = 0.05f;
		float ArmorIgnoredPerc = 0;

		Action<Projectile> minionOnMovement;

		public ModSupportProjectile(int ProjectileID) {
			this.ProjectileID = ProjectileID;
		}
		public void AssignHook(int hookNum, object hook) {
			switch (hookNum)
			{
				case 0:
					maxEnergy = (float)hook;
					return;
				case 1:
					minionTrackingAcceleration = (float)hook;
					return;
				case 2:
					minionTrackingImperfection = (float)hook;
					return;
				case 3:
					onPostCreation = (Action<Projectile, Player>)hook;
					return;
				case 4:
					minionOnSpecialAbilityUsed = (Action<Projectile, Entity, int, bool>)hook;
					return;
				case 5:
					minionTerminateSpecialAbility = (Action<Projectile, Player>)hook;
					return;
				case 6:
					minionSummonEffect = (Action<Projectile, Player>)hook;
					return;
				case 7:
					minionDespawnEffect = (Action<Projectile, Player>)hook;
					return;
				case 8:
					minionPreAI = (Action<Projectile>)hook;
					return;
				case 9:
					minionCustomAI = (Func<Projectile, bool>)hook;
					return;
				case 10:
					minionEndOfAI = (Action<Projectile>)hook;
					return;
				case 11:
					minionPostAI = (Action<Projectile>)hook;
					return;
				case 12:
					minionPostAIPostRelativeVelocity = (Action<Projectile>)hook;
					return;
				case 13:
					minionOnCreation = (Action<Projectile, Player>)hook;
					return;
				case 14:
					minionOnMovement = (Action<Projectile>)hook;
					return;
				case 15:
					minionOnTileCollide = (Func<Projectile, Vector2, bool>)hook;
					return;
				case 16:
					minionOnSlopeCollide = (Action<Projectile, Vector4>)hook;
					return;
				case 17:
					minionOnShootProjectile = (Action<Projectile, Projectile>)hook;
					return;
				case 18:
					minionOnPlayerHitNPCWithProj = (Action<Projectile, Projectile, NPC, int, float, bool>)hook;
					return;
				case 19:
					minionOnHitNPC = (Func<Projectile, Entity, int, float, bool, int, Tuple<int, float, bool, int>>)hook;
					return;
				case 20:
					minionOnHitNPC_Fake = (Func<Projectile, Entity, int, float, bool, int, Tuple<int, float, bool, int>>)hook;
					return;
				case 21:
					minionCustomPreDraw = (Func<Projectile, Color, bool>)hook;
					return;
				case 22:
					minionCustomPostDraw = (Action<Projectile, Color>)hook;
					return;
				case 23:
					minionSpeedModType = (MinionSpeedModifier)hook;
					return;
				case 24:
					minionTrackingState = (MinionTracking_State)hook;
					return;
				case 25:
					minionFlickeringThreshold = (float)hook;
					return;
				case 26:
					ArmorIgnoredPerc = (float)hook;
					return;
				case 27:
					minionModifyColor = (Func<Projectile, Color, Color>)hook;
					return;
			}
		}

		public void onPostCreation_mod(Projectile proj, Player player)
		{
			if (onPostCreation == null)
				return;
			onPostCreation(proj, player);
		}
		public void minionPreAI_mod(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
		{
			if (minionPreAI == null)
				return;
			minionPreAI(proj);
		}
		public bool minionCustomAI_mod(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
		{
			if (minionCustomAI == null)
				return true;
			return minionCustomAI(proj);
		}
		public void minionEndOfAI_mod(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
		{
			if (minionEndOfAI == null)
				return;
			minionEndOfAI(proj);
		}
		public void minionPostAI_mod(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
		{
			if (minionPostAI == null)
				return;
			minionPostAI(proj);
		}
		public void minionPostAIPostRelativeVelocity_mod(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
		{
			if (minionPostAIPostRelativeVelocity == null)
				return;
			minionPostAIPostRelativeVelocity(proj);
		}
		public void minionSummonEffect_mod(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
		{
			if (minionSummonEffect == null)
				return;
			minionSummonEffect(proj, player);
		}
		public void minionDespawnEffect_mod(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
		{
			if (minionDespawnEffect == null)
				return;
			minionDespawnEffect(proj, player);
		}
		public void minionOnCreation_mod(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
		{
			if (minionOnCreation == null)
				return;
			minionOnCreation(proj, player);
		}

		public void minionOnHitNPC_mod(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (minionOnHitNPC != null)
			{
				Tuple<int, float, bool, int> rv = minionOnHitNPC(projectile, target, damage, knockback, crit, hitDirection);
				damage = rv.Item1;
				knockback = rv.Item2;
				crit = rv.Item3;
				hitDirection = rv.Item4;
			}
		}
		public void minionOnHitNPC_Fake_mod(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (minionOnHitNPC_Fake != null)
			{
				Tuple<int, float, bool, int> rv = minionOnHitNPC_Fake(projectile, target, damage, knockback, crit, hitDirection);
				damage = rv.Item1;
				knockback = rv.Item2;
				crit = rv.Item3;
				hitDirection = rv.Item4;
			}
		}
		public bool minionCustomPreDraw_mod(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Color color)
		{
			if (minionCustomPreDraw == null)
				return true;
			return minionCustomPreDraw(proj, color);
		}
		public void minionCustomPostDraw_mod(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Color color)
		{
			if (minionCustomPostDraw == null)
				return;
			minionCustomPostDraw(proj, color);
		}
		public bool minionOnTileCollide_mod(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Vector2 vector)
		{
			if (minionOnTileCollide == null)
				return true;
			return minionOnTileCollide(proj, vector);
		}
		public void minionOnSlopeCollide_mod(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Vector4 vector)
		{
			if (minionOnSlopeCollide == null)
				return;
			minionOnSlopeCollide(proj, vector);
		}
		public void minionOnSpecialAbilityUsed_mod(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity target, int specialType, bool fromServer)
		{
			if (minionOnSpecialAbilityUsed == null)
				return;
			minionOnSpecialAbilityUsed(proj, target, specialType, fromServer);
		}
		public void minionTerminateSpecialAbility_mod(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player, ReworkMinion_Player playerFuncs)
		{
			if (minionTerminateSpecialAbility == null)
				return;
			minionTerminateSpecialAbility(proj, player);
		}

		public void minionOnShootProjectile_mod(Projectile newproj, Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
		{
			if (minionOnShootProjectile == null)
				return;
			minionOnShootProjectile(newproj, proj);
		}
		public void minionOnMovement_mod(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
		{
			if (minionOnMovement == null)
				return;
			minionOnMovement(proj);
		}
		public void minionOnPlayerHitNPCWithProj_mod(Projectile minion, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			if (minionOnPlayerHitNPCWithProj == null)
				return;
			minionOnPlayerHitNPCWithProj(minion, proj, target, damage, knockback, crit);
		}
		public Color minionModifyColor_mod(Projectile minion, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Color color)
		{
			if(minionModifyColor == null)
				return color;
			return minionModifyColor(minion, color);
		}
		public void HookToProjectile(ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
		{

			projFuncs.minionPreAI += minionPreAI_mod;
			projFuncs.minionCustomAI = minionCustomAI_mod;
			projFuncs.minionEndOfAI += minionEndOfAI_mod;
			projFuncs.minionPostAI += minionPostAI_mod;
			projFuncs.minionPostAIPostRelativeVelocity += minionPostAIPostRelativeVelocity_mod;

			projFuncs.minionSummonEffect += minionSummonEffect_mod;
			projFuncs.minionDespawnEffect += minionDespawnEffect_mod;

			projFuncs.minionOnHitNPC += minionOnHitNPC_mod;
			projFuncs.minionOnHitNPC_Fake += minionOnHitNPC_Fake_mod;

			projFuncs.minionOnSpecialAbilityUsed = minionOnSpecialAbilityUsed_mod;
			projFuncs.minionTerminateSpecialAbility = minionTerminateSpecialAbility_mod;

			projFuncs.onPostCreation = onPostCreation_mod;
			projFuncs.minionOnCreation = minionOnCreation_mod;

			projFuncs.minionCustomPreDraw = minionCustomPreDraw_mod;
			projFuncs.minionCustomPostDraw = minionCustomPostDraw_mod;
			projFuncs.minionModifyColor = minionModifyColor_mod;

			projFuncs.minionOnTileCollide = minionOnTileCollide_mod;
			projFuncs.minionOnSlopeCollide = minionOnSlopeCollide_mod;

			projFuncs.minionOnShootProjectile = minionOnShootProjectile_mod;
			projFuncs.minionOnPlayerHitNPCWithProj = minionOnPlayerHitNPCWithProj_mod;

			projFuncs.minionOnMovement = minionOnMovement_mod;

			projData.minionTrackingAcceleration = minionTrackingAcceleration;
			projData.minionTrackingImperfection = minionTrackingImperfection;

			projData.maxEnergy = maxEnergy;

			projData.minionSpeedModType = minionSpeedModType;
			projData.trackingState = minionTrackingState;
			projData.minionFlickeringThreshold = minionFlickeringThreshold;
			projFuncs.ArmorIgnoredPerc = ArmorIgnoredPerc;
		}
	}
}
