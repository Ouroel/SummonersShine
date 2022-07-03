using Microsoft.Xna.Framework;
using SummonersShine.BakedConfigs;
using SummonersShine.Items.MagenChiun;
using SummonersShine.MinionAI;
using SummonersShine.ProjectileBuffs;
using SummonersShine.Projectiles;
using SummonersShine.SpecialAbilities;
using SummonersShine.SpecialAbilities.DefaultSpecialAbility;
using SummonersShine.SpecialData;
using SummonersShine.VanillaMinionILMods;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine
{

    public enum MinionTracking_State
	{
		normal,
		retreating,
		noTracking,
		xOnly,
		normalProjected,
		retreatingProjected,
	}

	public enum MinionCDType
	{
		idStaticNPCHitCooldown,
		localNPCHitCooldown,
		noCooldown
	}
	public enum MinionSpeedModifier
	{
		normal,
		stepped,
		none,
		letothersupdate,
	}

	public enum ProjMinionRelation { 
		notMinion,
		isMinion,
		fromMinion,
		isWhip,
		fromWhip,
	}

	public enum DrawBehindType {
		normal,
		none,
		npc_tiles,
		npc,
		projectiles,
		players,
		wires
	}
	public class ProjectileOnHit {
		static List<ProjectileOnHit> hooked = new();

		public Rectangle projRect;
		public Projectile projBody;
		ReworkMinion_Projectile projFuncs;
		MinionProjectileData projData;
		Action<Projectile, ReworkMinion_Projectile, MinionProjectileData, Projectile> OnCollide;

		public class ProjectileOnHit_Loader : ILoadable
		{
			public void Load(Mod mod)
			{
				hooked = new();
			}

			public void Unload()
			{
				hooked = null;
			}
		}
		public void CollideTest(Projectile source, Rectangle sourceRect)
        {
			Rectangle testRect = projRect;
			testRect.X += (int)projBody.Center.X;
			testRect.Y += (int)projBody.Center.Y;
			if(source.Colliding(sourceRect, testRect))
				OnCollide(projBody, projFuncs, projData, source);
        }

		public ProjectileOnHit(Projectile projBody, Rectangle projRect, Action<Projectile, ReworkMinion_Projectile, MinionProjectileData, Projectile> OnCollide)
		{
			projFuncs = projBody.GetGlobalProjectile<ReworkMinion_Projectile>();
			projData = projFuncs.GetMinionProjData();
			this.projBody = projBody;
			this.OnCollide = OnCollide;
			this.projRect = projRect;
			Rehook();
		}

		public void Unhook() {
			hooked.Remove(this);
		}

		public void Rehook() {
			if (!hooked.Contains(this))
				hooked.Add(this);
		}

		public static void Iterate(Projectile source, Rectangle sourceRect)
		{
			hooked.ForEach(i => i.CollideTest(source, sourceRect));
		}

		public static void UnhookProjectile(Projectile proj) {
			hooked.RemoveAll(i => i.projBody == proj);
		}
	}
	public class ReworkMinion_Projectile : GlobalProjectile
	{
		public const float return_SafeDistanceFromPlayer = (80 * 16); //30 blocks * 16 units per block
		public const float return_SafeDistanceFromPlayer_sqr = return_SafeDistanceFromPlayer * return_SafeDistanceFromPlayer; //30 blocks * 16 units per block
		public const float return_QuicklyAccelerateDistanceFromPlayer = (5 * 16); //12 blocks * 16 units per block
		public const float return_QuicklyAccelerateDistanceFromPlayer_sqr = return_QuicklyAccelerateDistanceFromPlayer * return_QuicklyAccelerateDistanceFromPlayer; //12 blocks * 16 units per block
		public const float onetick = 0.01667f;

		public override bool InstancePerEntity => true;

        List<SpecialDataBase> specialData = new();
		MinionProjectileData minionProjData;

		public Action<Projectile, Player> onPostCreation = Minion_PostCreation.OnPostCreation;
		public event Action<Projectile, ReworkMinion_Projectile, MinionProjectileData> minionPreAI;
		public Action<Projectile, ReworkMinion_Projectile, MinionProjectileData, Entity, int, bool> minionOnSpecialAbilityUsed = SpecialAbility.NoOnSpecialAbilityUsed;
		public Action<Projectile, ReworkMinion_Projectile, MinionProjectileData, Player, ReworkMinion_Player> minionTerminateSpecialAbility = DefaultMinionAI.DefaultTerminateSpecialAbility;
		public Func<Projectile, ReworkMinion_Projectile, MinionProjectileData, bool> minionCustomAI = SpecialAbility.NoCustomAI;
		public Func<Projectile, ReworkMinion_Projectile, MinionProjectileData, Color, bool> minionCustomPreDraw = SpecialAbility.NoCustomPreDraw;
		public Action<Projectile, ReworkMinion_Projectile, MinionProjectileData, Color> minionCustomPostDraw = SpecialAbility.NoCustomPostDraw;
		public Func<Projectile, ReworkMinion_Projectile, MinionProjectileData, Color, Color> minionModifyColor = SpecialAbility.NoCustomModifyColor;
		public event Action<Projectile, ReworkMinion_Projectile, MinionProjectileData> minionEndOfAI;
		public event Action<Projectile, ReworkMinion_Projectile, MinionProjectileData> minionPostAI;
		public event Action<Projectile, ReworkMinion_Projectile, MinionProjectileData> minionPostAIPostRelativeVelocity;
		public event Action<Projectile, ReworkMinion_Projectile, MinionProjectileData, Player> minionSummonEffect;
		public event Action<Projectile, ReworkMinion_Projectile, MinionProjectileData, Player> minionDespawnEffect;
		public Action<Projectile, ReworkMinion_Projectile, MinionProjectileData, Player> minionOnCreation = DefaultMinionAI.NoEffect;
		public Func<Projectile, ReworkMinion_Projectile, MinionProjectileData, Vector2, bool> minionOnTileCollide = DefaultMinionAI.NoEffect_ReturnTrue;
		public Action<Projectile, ReworkMinion_Projectile, MinionProjectileData, Vector4> minionOnSlopeCollide = DefaultMinionAI.NoEffect;
		public Action<Projectile, Projectile, ReworkMinion_Projectile, MinionProjectileData> minionOnShootProjectile = DefaultMinionAI.NoEffect;
		public Action<Projectile, Projectile, NPC, int, float, bool> minionOnPlayerHitNPCWithProj;

		public event onHitNPCHook minionOnHitNPC;
		public event onHitNPCHook minionOnHitNPC_Fake;

		public delegate void onHitNPCHook(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection);

		public Action<Projectile, ReworkMinion_Projectile, MinionProjectileData> minionOnMovement = DefaultMinionAI.NoEffect;

		private bool created = false;
		private bool initialized = false;
		public bool killed = false;
		public byte killedTicks = 0;

		public ProjMinionRelation IsMinion = ProjMinionRelation.notMinion;

		public int SourceItem
		{
			get
			{
				if (SourceItemOverride != -2)
					return SourceItemOverride;
				if (SourceItemConfigOverride != -2)
					return SourceItemConfigOverride;
				return OriginalSourceItem;
			}
		}
		public int OriginalSourceItem = -1;
		public int SourceItemConfigOverride = -2;
		public int SourceItemOverride = -2;

		public float PrefixMinionPower = 1;
		public int ProjectileCrit = 0;
		public float MinionASMod = 1;
		public float ArmorIgnoredPerc = 0;

		public bool LimitedLife = false;
		public int originalNPCHitCooldown;
		public MinionCDType minionCDType = MinionCDType.noCooldown;

		public DrawBehindType drawBehindType = DrawBehindType.normal;

		public Platform hookedPlatform = null;
		int hookedPlatformID_delayed = 0;

		public bool ComparativelySlowPlayer = true;

		//projectile effects
		List<ProjectileBuff> buffs = new();

		public void OverrideSourceItem(Projectile projectile, int newSource)
		{
			if (newSource == SourceItemOverride)
				return;
			ReworkMinion_Player playerFuncs = Main.player[projectile.owner].GetModPlayer<ReworkMinion_Player>();
			DefaultSpecialAbility.UnhookSpecialPower(projectile);
			if (SourceItem != -1)
			{
				MinionEnergyCounter counter = playerFuncs.GetMinionCollection(SourceItem);
				counter.minions.Remove(projectile);
			}
			SourceItemOverride = newSource;
			if (SourceItem != -1)
			{
				MinionEnergyCounter counter = playerFuncs.GetMinionCollection(SourceItem);
				counter.minions.Add(projectile);
			}
			DefaultSpecialAbility.HookSpecialPower(projectile);
		}
		public override GlobalProjectile Clone(Projectile projectile, Projectile projectileClone)
		{
			ReworkMinion_Projectile rv = (ReworkMinion_Projectile)base.Clone(projectile, projectileClone);
			rv.specialData = new();
			rv.buffs = new();
			return rv;
		}
        public override GlobalProjectile NewInstance(Projectile target)
		{
			ReworkMinion_Projectile rv = (ReworkMinion_Projectile)base.NewInstance(target);
			rv.specialData = new();
			rv.buffs = new();
			return rv;
		}
		public MinionProjectileData GetMinionProjData() {
			MinionProjectileData rv = minionProjData;
			if (rv == null) {
				rv = new MinionProjectileData();
				minionProjData = rv;
			}
			return rv;
		}
		public T GetSpecialData<T>() where T : SpecialDataBase, new()
		{
			T rv = specialData.Find(i => i.GetType() == typeof(T)) as T;
			if (rv == null)
			{
				rv = new T();
				specialData.Add(rv);
			}
			return rv;
		}

		public void InitDynamicMinionCD(Projectile projectile)
		{
			if (projectile.idStaticNPCHitCooldown > 0)
			{
				originalNPCHitCooldown = projectile.idStaticNPCHitCooldown;
				minionCDType = MinionCDType.idStaticNPCHitCooldown;
			}
			else if (projectile.localNPCHitCooldown > 0)
			{

				originalNPCHitCooldown = projectile.localNPCHitCooldown;
				minionCDType = MinionCDType.localNPCHitCooldown;
			}
			else
			{
				minionCDType = MinionCDType.noCooldown;
			}
		}

		void OnSpawn_Inner(Projectile projectile, IEntitySource source)
		{
			//Magen Chiun Check for Lightning Aura Sentry
			if (projectile.aiStyle == 137)
			{
				Player player = Main.player[projectile.owner];
				ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
				if (playerFuncs.HasMagenChiunEquipped)
				{
					projectile.LightningAura_MagenChiun();
				}
			}

			//update cache
			if (!Config.current.DisableDynamicProjectileCacheUpdate)
			{
				Player player = Main.player[projectile.owner];
				player.ownedProjectileCounts[projectile.type]++;
			}

			EntitySource_Parent nextParentProjSource = source as EntitySource_Parent;
			//item was spawned by player
			EntitySource_ItemUse projSource = source as EntitySource_ItemUse;
			Item item = null;
			if (projSource != null)
			{
				item = projSource.Item;
			}
			else if (nextParentProjSource != null)
			{
				item = nextParentProjSource.Entity as Item;
			}
			if (item != null)
			{
				if (IsMinion != ProjMinionRelation.notMinion)
				{
					MinionDataHandler.HookSourceItem(projectile, this, item.netID);
					ReworkMinion_Item globalItem = item.GetGlobalItem<ReworkMinion_Item>();

					//both minion and whip stuff
					ProjectileCrit = item.crit;
					MinionASMod = globalItem.GetUseTimeModifier(item);

					//minion specific
					if (IsMinion == ProjMinionRelation.isMinion)
					{
						MinionProjectileData projData = GetMinionProjData();

						PrefixMinionPower = globalItem.prefixMinionPower;

						//PacketHandler.WritePacket_UpdateMinionStats(projectile, projFuncs, globalProj);
						OnCreation(projectile);
					}
				}
				return;
			}

			//item was spawned by projectile
			else if (nextParentProjSource != null)
			{
				Projectile nextParentProj = nextParentProjSource.Entity as Projectile;
				if (nextParentProj != null)
				{
					ReworkMinion_Projectile parentProjFuncs = nextParentProj.GetGlobalProjectile<ReworkMinion_Projectile>();

					//if it's not related to minions, skip everything
					if (parentProjFuncs.IsMinion == ProjMinionRelation.notMinion)
						return;
					MinionDataHandler.HookSourceItem(projectile, this, parentProjFuncs.SourceItem);

					//basic inheritance
					ProjectileCrit = parentProjFuncs.ProjectileCrit;
					MinionASMod = parentProjFuncs.MinionASMod;

					//inherit
					PrefixMinionPower = parentProjFuncs.PrefixMinionPower;

					if (IsMinion == ProjMinionRelation.isMinion)
					{
						OnCreation(projectile);
					}
					else
					{
						InitDynamicMinionCD(projectile);
						if (IsOrFromMinion(parentProjFuncs.IsMinion))
							IsMinion = ProjMinionRelation.fromMinion;
						else
							IsMinion = ProjMinionRelation.fromWhip;
					}

					//minion aimbot stuff below

					if (parentProjFuncs.IsMinion != ProjMinionRelation.isMinion)
						return;
					
					MinionProjectileData parentProjData = null;
					//don't call GetMinionProjData/inherit unless parent is really minion
					if (parentProjFuncs.IsMinion == ProjMinionRelation.isMinion)
					{
						parentProjData = parentProjFuncs.GetMinionProjData();
					}
					parentProjFuncs.minionOnShootProjectile(projectile, nextParentProj, parentProjFuncs, parentProjData);

					bool configEnabled = true;
					NPC moveTarget = null;

					Config.current.ProjectilesForceAimbot.ForEach(i =>
					{
						if (i.proj.Type == nextParentProj.type)
						{
							int attackTarget = -1;
							nextParentProj.Minion_FindTargetInRange(i.startAttackRange, ref attackTarget, i.skipIfCannotHit);
							if (attackTarget != -1)
								moveTarget = Main.npc[attackTarget];
						}
					});

					if (ModUtils.IsProjectileStationary(projectile.type, nextParentProj.type))
						return;

					if (moveTarget == null)
					{
						if (BakedConfig.IgnoreTracking(projectile))
							configEnabled = false;
						moveTarget = parentProjData.moveTarget as NPC;
					}
					/*
					Config.current.ProjectilesIgnoreTracking.ForEach(i =>
					{
						if (i.Type == nextParentProj.type) configEnabled = false;
					});*/

					if (configEnabled)
						projectile.velocity = GetTotalProjectileVelocity(projectile, nextParentProj, moveTarget);
				}
			}
			else if (IsMinion != ProjMinionRelation.notMinion)
				MinionDataHandler.HookSourceItem(projectile, this, -1);
		}
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
			OnSpawn_Inner(projectile, source);
			if (IsMinion == ProjMinionRelation.isMinion)
			{
				OnCreation(projectile);
			}
		}

		public void OnCreation(Projectile projectile) {
			if (created)
				return;
			Player player = Main.player[projectile.owner];
			if (!player.active)
				return;
			created = true;

			//placed it here because otherwise modded minions with vanilla AI will cause this to bug out
			if (IsMinion != ProjMinionRelation.notMinion)
			{
				if (projectile.ModProjectile as Projectiles.CustomSpecialAbilitySourceMinion == null) //not custom minion
				{
					MinionDataHandler.ProjectileSetDefaults(projectile, this);
				}
			}

			InitDynamicMinionCD(projectile);
			SetMoveTarget(projectile, player);
			ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
			if (SourceItem != -1)
			{
				List<Projectile> projList = playerFuncs.GetMinionCollection(SourceItem).minions;

				if (projList.Count == 0)
				{
					int buffID = BakedConfig.GetMinionItemBuff(SourceItem);
					if (buffID != -1)
						player.AddBuff(buffID, 2); //for minion despawn effect
				}
				if (!projList.Contains(projectile))
					projList.Add(projectile);
			}
			if (minionOnPlayerHitNPCWithProj != null)
				playerFuncs.Minion_OnHitNPCWithProj.Add(projectile);
			minionOnCreation(projectile, this, minionProjData, Main.player[projectile.owner]);
		}

		public void ReadMinionProjectileData(BinaryReader reader)
		{
			BitsByte notDefault = reader.ReadByte();
			IsMinion = notDefault[0] ? ProjMinionRelation.isMinion : notDefault[1] ? ProjMinionRelation.fromMinion : ProjMinionRelation.notMinion;
			if (IsMinion == ProjMinionRelation.isMinion)
			{
				BitsByte notDefault_Special = reader.ReadByte();
				if (notDefault[2])
					ProjectileCrit = reader.Read7BitEncodedSignedInt();
				else
					ProjectileCrit = 0;

				if (notDefault[3])
					MinionASMod = reader.ReadSingle();
				else
					MinionASMod = 1;

				if (notDefault[4])
					PrefixMinionPower = reader.ReadSingle();
				else
					PrefixMinionPower = 1;

				if (notDefault[5])
					minionProjData.castingSpecialAbilityTime = reader.Read7BitEncodedSignedInt();
				else
					minionProjData.castingSpecialAbilityTime = -1;

				if (notDefault[6])
					minionProjData.energy = reader.ReadSingle();
				else
					minionProjData.energy = minionProjData.maxEnergy;

				if (notDefault[7])
					minionProjData.energyRegenRateMult = reader.ReadSingle();
				else
					minionProjData.energyRegenRateMult = 1;


				if (notDefault_Special[0])
					minionProjData.specialCastTarget = Main.npc[reader.Read7BitEncodedInt()];
				else
					minionProjData.specialCastTarget = null;

				if (notDefault_Special[1])
					minionProjData.specialCastPosition = reader.ReadVector2();
				else
					minionProjData.specialCastPosition = Vector2.Zero;

				if (notDefault_Special[2])
				{
					hookedPlatformID_delayed = reader.Read7BitEncodedInt();
					hookedPlatform = PlatformCollection.FindPlatform(hookedPlatformID_delayed);
				}
				else
					hookedPlatform = null;

				int new_SourceItemOverride;
				if (notDefault_Special[3])
					new_SourceItemOverride = reader.Read7BitEncodedInt() - 1;
				else
					new_SourceItemOverride = -1;

				if (notDefault_Special[4])
					killedTicks = reader.ReadByte();
				else
					killedTicks = 0;

				SourceItemOverride = new_SourceItemOverride;

			}
			else if (IsMinion == ProjMinionRelation.fromMinion)
			{
				if (notDefault[2])
					ProjectileCrit = reader.Read7BitEncodedSignedInt();
				else
					ProjectileCrit = 0;

				if (notDefault[3])
					MinionASMod = reader.ReadSingle();
				else
					MinionASMod = 1;
			}
			else
			{
				if (notDefault[2])
				{
					hookedPlatformID_delayed = reader.Read7BitEncodedInt();
					hookedPlatform = PlatformCollection.FindPlatform(hookedPlatformID_delayed);
					if (hookedPlatform == null) initialized = true;
				}
				else
					hookedPlatform = null;
			}
			int count = reader.Read7BitEncodedInt();
			List<ProjectileBuff> buffsToKeep = new();
			if (count > 0)
				for (int x = 0; x < count; x++)
				{
					ProjectileBuffIDs id = (ProjectileBuffIDs)reader.Read7BitEncodedInt();
					int sourceProjId = reader.Read7BitEncodedInt() - 1;

					int modID = 0;
					int moddedBuffID = 0;
					if (id == ProjectileBuffIDs.ModdedBuff) {
						modID = reader.Read7BitEncodedInt();
						moddedBuffID = reader.Read7BitEncodedInt();
					}

					ProjectileBuff buff = AddBuff_Net(id, sourceProjId, modID, moddedBuffID);
					buffsToKeep.Add(buff);
					buff.LoadNetData(reader);
				}
			buffs = buffsToKeep;
		}
		public void WriteMinionProjectileData(BinaryWriter writer)
		{
			BitsByte notDefault = new();
			if (IsMinion == ProjMinionRelation.isMinion)
			{
				notDefault[0] = true;

				if (ProjectileCrit != 0)
					notDefault[2] = true;

				if (MinionASMod != 1)
					notDefault[3] = true;

				float prefixMinionPower = PrefixMinionPower;
				if (prefixMinionPower != 1)
					notDefault[4] = true;

				int castingSpecialAbilityTime = minionProjData.castingSpecialAbilityTime;
				if (castingSpecialAbilityTime != -1)
					notDefault[5] = true;

				float energy = minionProjData.energy;
				if (energy != minionProjData.maxEnergy)
					notDefault[6] = true;

				float energyRegenRateMult = minionProjData.energyRegenRateMult;
				if (energyRegenRateMult != 1)
					notDefault[7] = true;

				BitsByte notDefault_Special = new();

				NPC specialCastTarget = minionProjData.specialCastTarget;
				if (specialCastTarget != null)
					notDefault_Special[0] = true;

				Vector2 specialCastPosition = minionProjData.specialCastPosition;
				if (specialCastPosition != Vector2.Zero)
					notDefault_Special[1] = true;

				if (hookedPlatform != null)
					notDefault_Special[2] = true;

				if (SourceItemOverride != -1)
					notDefault_Special[3] = true;

				if (killedTicks != 0)
					notDefault_Special[4] = true;

				writer.Write(notDefault);
				writer.Write(notDefault_Special);

				if (notDefault[2])
					writer.Write7BitEncodedSignedInt(ProjectileCrit);

				if (notDefault[3])
					writer.Write(MinionASMod);

				if (notDefault[4])
					writer.Write(prefixMinionPower);

				if (notDefault[5])
					writer.Write7BitEncodedSignedInt(castingSpecialAbilityTime);

				if (notDefault[6])
					writer.Write(energy);

				if (notDefault[7])
					writer.Write(energyRegenRateMult);

				if (notDefault_Special[0])
					writer.Write7BitEncodedInt(specialCastTarget.whoAmI);

				if (notDefault_Special[1])
					writer.WriteVector2(specialCastPosition);

				if (notDefault_Special[2])
					writer.Write7BitEncodedInt(hookedPlatform.Net_GetKey());

				if (notDefault_Special[3])
					writer.Write7BitEncodedInt(SourceItemOverride + 1);

				if (notDefault_Special[4])
					writer.Write(killedTicks);
			}
			else if (IsMinion == ProjMinionRelation.fromMinion)
			{
				notDefault[1] = true;

				if (ProjectileCrit != 0)
					notDefault[2] = true;

				if (MinionASMod != 1)
					notDefault[3] = true;

				writer.Write(notDefault);

				if (notDefault[2])
					writer.Write7BitEncodedSignedInt(ProjectileCrit);

				if (notDefault[3])
					writer.Write(MinionASMod);
			}
			else
			{
				if (hookedPlatform != null)
					notDefault[2] = true;

				writer.Write(notDefault);

				if (notDefault[2])
					writer.Write7BitEncodedInt(hookedPlatform.Net_GetKey());
			}

			writer.Write7BitEncodedInt(buffs.Count);
			buffs.ForEach(i => {
				writer.Write7BitEncodedInt((int)i.ID);
				writer.Write7BitEncodedInt(i.sourceProjIdentity + 1);
				if (i.ID == ProjectileBuffIDs.ModdedBuff)
				{
					writer.Write7BitEncodedInt(i.modID.NetID);
					writer.Write7BitEncodedInt(i.moddedBuffID);
				}
				i.SaveNetData(writer);
			});
		}

		public void RemoveBuff(ProjectileBuffIDs ID, Projectile sourceProj, Mod mod = null, int moddedBuffID = 0)
		{
			buffs.RemoveAll(i => i.IsEqual(ID, sourceProj, mod, moddedBuffID));
		}

		public ProjectileBuff AddBuff(Projectile proj, ProjectileBuffIDs ID, Projectile sourceProj, Mod mod = null, int moddedBuffID = 0)
		{
			ProjectileBuff rv = buffs.Find(i => i.IsEqual(ID, sourceProj, mod, moddedBuffID));
			if (rv != null)
				return rv;
			rv = ProjectileBuff.NewBuff(ID, sourceProj, mod, moddedBuffID);
			buffs.Add(rv);
			proj.netUpdate = true;
			return rv;
		}
		ProjectileBuff AddBuff_Net(ProjectileBuffIDs ID, int sourceProjIdentity, int modID, int moddedBuffID)
		{
			ProjectileBuff rv = buffs.Find(i => ID == i.ID && sourceProjIdentity == i.sourceProjIdentity);
			if (rv != null)
				return rv;
			rv = ProjectileBuff.NewBuff(ID, null, ModNet.GetMod(modID), moddedBuffID);
			rv.sourceProjIdentity = sourceProjIdentity;
			buffs.Add(rv);
			return rv;
		}

		public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
		{
			ReworkMinion_Player playerFuncs = Main.player[projectile.owner].GetModPlayer<ReworkMinion_Player>();
			if (playerFuncs.attackedThisFrame == 0 && (BakedConfig.ItemCountAsWhip[SourceItem] || ProjectileID.Sets.IsAWhip[projectile.type]))
			{
				playerFuncs.attackedThisFrame = 2;
				playerFuncs.lastAttackedTarget = target;
			}
		}
		public void ModifyHitNPC_Fake(Projectile projectile, Entity target, int damage, float knockback, bool crit, int hitDirection)
		{
			if (IsMinion == ProjMinionRelation.isMinion)
			{
				minionOnHitNPC_Fake(projectile, this, GetMinionProjData(), target, ref damage, ref knockback, ref crit, ref hitDirection);
			}
		}
		bool IsSummon(Projectile proj)
		{
			return proj.DamageType == DamageClass.Summon || proj.DamageType == DamageClass.SummonMeleeSpeed || IsMinion != ProjMinionRelation.notMinion;
		}
		public void ModifyHitNPC_Minion(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (IsSummon(projectile))
			{
				Player player = Main.player[projectile.owner];
				if (!Config.current.DisableMinionCrits && ProjectileCrit != -1)
				{
					if (!crit)
						crit = Main.rand.Next(1, 101) <= ProjectileCrit + player.GetCritChance(DamageClass.Summon) + player.GetCritChance(DamageClass.Generic);
				}
				if (ProjectileID.Sets.IsAWhip[projectile.type] || (SourceItem >= 0 && ReworkMinion_Item.defaultSpecialAbilityUsed[SourceItem] as DefaultSpecialAbility_Whip != null))
				{
					player.GetModPlayer<ReworkMinion_Player>().OnWhippedEnemy(target);
				}
				bool isOrFromMinion = IsOrFromMinion(IsMinion);

				float outgoingDamageMod = BakedConfig.MinionOutgoingDamageMod[projectile.type];
				float ignoredArmor;
				if (isOrFromMinion)
				{
					ignoredArmor = ArmorIgnoredPerc + (1 - ArmorIgnoredPerc) * player.GetModPlayer<ReworkMinion_Player>().GetMinionArmorNegationPerc(target);
				}
				else
				{
					ignoredArmor = 0;
				}
				if (outgoingDamageMod < 1)
				{
					float innateArmorPiercing = 1 - outgoingDamageMod;
					damage = (int)(damage * outgoingDamageMod);
					ignoredArmor *= outgoingDamageMod;
					ignoredArmor += innateArmorPiercing;
				}
				int def = (target.defense - projectile.ArmorPenetration);
				if (def < 0) def = 0;
				damage += target.checkArmorPenetration((int)(def * (ignoredArmor)));
				if (isOrFromMinion)
				{
					General_HitSomething(projectile, target, ref damage, ref crit);

					if (IsMinion == ProjMinionRelation.isMinion)
						minionOnHitNPC(projectile, this, GetMinionProjData(), target, ref damage, ref knockback, ref crit, ref hitDirection);
				}
			}
		}

		public override void ModifyHitPlayer(Projectile projectile, Player target, ref int damage, ref bool crit)
		{
			if (IsOrFromMinion(IsMinion))
			{
				General_HitSomething(projectile, target, ref damage, ref crit);
			}
		}

		public void General_HitSomething(Projectile projectile, Entity ent, ref int damage, ref bool crit)
		{
			Player player = Main.player[projectile.owner];
			ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
			Items.BloodMatzo.BloodMatzo_Base bloodMatzo = playerFuncs.currentBloodMatzo;
			if (bloodMatzo != null)
			{
				bloodMatzo.OnHitNPCWithProj(projectile, ent, ref damage, ref crit, playerFuncs.CursorPos);
			}
		}
		public bool MinionPreAI(Projectile projectile)
		{
			if (!created && IsMinion == ProjMinionRelation.isMinion && projectile.active)
			{
				OnCreation(projectile); //Assorted Crazy Things Anti-Crash/Emergency load
			}

			if (killedTicks > 0)
				return false;
			if (projectile.aiStyle == 7 && initialized)
			{
				hookedPlatform = PlatformCollection.FindPlatform(hookedPlatformID_delayed);
				initialized = false;
			}
			if (IsMinion == ProjMinionRelation.isMinion && projectile.active)
			{
				Player player = Main.player[projectile.owner];
				if (!initialized && killedTicks == 0)
				{
					onPostCreation(projectile, player);
					if (!BakedConfig.ItemHasBuffLinked[SourceItem])
						SingleThreadExploitation.projectileBuffCountIntDetector = player.GetBuffCount();
				}

				//if last tick's velocity was overwritten by skipping function, loads stored velocity
				//Deletes simulation rate velocity modifier before calculation
				if (minionProjData.updatedSim)
				{
					projectile.velocity /= minionProjData.lastSimRateInv;
					minionProjData.updatedSim = false;
				}
				if (minionProjData.currentTick == 1 && projectile.numUpdates < 0)
				{
					float realTicksPerTick = GetRealTicksPerTick(projectile);
					UpdateProjectileRelativeVelocity(projectile, realTicksPerTick);
				}

				//modifies the player vel so the minion things the player is going slower
				if (ComparativelySlowPlayer)
				{
					float lastSimRate = GetSimulationRate(projectile);
					SingleThreadExploitation.ComparativelySlowPlayer(player, lastSimRate);
				}

				//if this tick would be skipped
				if (minionProjData.minionSpeedModType == MinionSpeedModifier.normal && minionProjData.nextTicks < 0)
				{
					return false;
				}

				else if (minionProjData.minionSpeedModType == MinionSpeedModifier.letothersupdate)
				{
					return false;
				}

				float projVelX = projectile.velocity.X;

				//stop minion flickering
				if (projVelX != 0 && projVelX < minionProjData.minionFlickeringThreshold && projVelX > -minionProjData.minionFlickeringThreshold)
				{
                    switch (projectile.type)
					{
						case ProjectileID.StormTigerTier1:
						case ProjectileID.StormTigerTier2:
						case ProjectileID.StormTigerTier3:
							projectile.velocity.X = 0;
							break;
						default:
							SingleThreadExploitation.minionFlickerFixStored = true;
							SingleThreadExploitation.minionFlickeringFix = projectile.spriteDirection;
							break;
					}
				}
				minionPreAI(projectile, this, minionProjData);
				return minionCustomAI(projectile, this, minionProjData);
			}
			return true;
		}
		public void MinionPostAI(Projectile projectile)
		{
			//minion
			Player player = Main.player[projectile.owner];
			buffs.ForEach(i => i.Update(projectile, this));
			if (projectile.active)
			{
				if (IsMinion == ProjMinionRelation.isMinion)
				{
					if (!initialized && killedTicks == 0)
					{
						int buffCount = SingleThreadExploitation.projectileBuffCountIntDetector;
						if (buffCount != player.GetBuffCount() && BakedConfig.initialized && SourceItem != -1 && !BakedConfig.ItemHasBuffLinked[SourceItem])
						{
							//last resort buff-hook
							BakedConfig.ItemHasBuffLinked[SourceItem] = true;
							BakedConfig.AddBuffSourceItem(player.buffType[buffCount], SourceItem);
						}
						minionSummonEffect(projectile, this, minionProjData, player);
						initialized = true;
					}
					if (SingleThreadExploitation.minionFlickerFixStored)
					{
						SingleThreadExploitation.minionFlickerFixStored = false;
						projectile.spriteDirection = SingleThreadExploitation.minionFlickeringFix;
						SingleThreadExploitation.minionFlickeringFix = 0;
					}

					ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
					if (projectile.sentry && playerFuncs.HasMagenChiunEquipped)
					{
						projectile.velocity.Y = 0;
					}

					if (minionProjData.minionSpeedModType == MinionSpeedModifier.normal && minionProjData.nextTicks < 0)
					{
						if (projectile.numUpdates < 0)
							minionProjData.nextTicks += 1;
						if (projectile.minion || !LimitedLife)
							projectile.timeLeft += 1;
					}
					else
						minionEndOfAI(projectile, this, minionProjData);
					minionPostAI(projectile, this, minionProjData);

					if (killedTicks > 0)
					{
						if (killedTicks < byte.MaxValue)
							killedTicks++;
						return;
					}

					//Retains the player vel

					SingleThreadExploitation.UncomparativelySlowPlayer();

					/*float realTicksPerTick = GetRealTicksPerTick(projectile);

					bool realTick = minionProjData.currentTick <= realTicksPerTick;
					minionProjData.isRealTick = realTick;*/
					bool realTick = IsRealTick(projectile);
					minionProjData.isRealTick = realTick;
					if (realTick && !minionProjData.isTeleportFrame)
					{
						if (minionProjData.minionSpeedModType == MinionSpeedModifier.normal)
						{
							//Adds speed based on speed. Yes, the math checks out.
							float lastSimRate = GetInternalSimRate(projectile);
							minionProjData.lastSimRateInv = lastSimRate;
							projectile.velocity *= lastSimRate;
							minionProjData.updatedSim = true;
						}


						minionPostAIPostRelativeVelocity(projectile, this, minionProjData);
						ClampProjectileToWorldBounds(projectile);
						return;
					}

					minionPostAIPostRelativeVelocity(projectile, this, minionProjData);
					ClampProjectileToWorldBounds(projectile);
				}
			}

			//grapple

			if (projectile.aiStyle == 7 && hookedPlatform != null)
			{
				if (!hookedPlatform.grapples.Contains(projectile))
					hookedPlatform.grapples.Add(projectile);
				projectile.Center = new Vector2(Math.Clamp(projectile.Center.X, hookedPlatform.pos.X, hookedPlatform.pos.X + hookedPlatform.width), hookedPlatform.pos.Y);
			}
		}

		void ClampProjectileToWorldBounds(Projectile projectile)
		{
			projectile.position.X = Math.Clamp(projectile.position.X, 1, (Main.maxTilesX - 1) * 16);
			projectile.position.Y = Math.Clamp(projectile.position.Y, 1, (Main.maxTilesY - 1) * 16);
			if (ModUtils.NextVelOutOfBounds(projectile))
			{
				projectile.velocity = Vector2.Zero;
				minionProjData.lastRelativeVelocity = Vector2.Zero;
			}
		}

		bool IsRealTick(Projectile proj)
		{
			float realTicksPerTick = GetRealTicksPerTick(proj);
			bool realTick = minionProjData.currentTick <= realTicksPerTick;
			return realTick;
		}

		public override bool ShouldUpdatePosition(Projectile projectile)
		{
			if (IsMinion != ProjMinionRelation.isMinion)
				return true;
			GetMinionProjData();
			if (minionProjData.isTeleportFrame)
			{
				if (minionProjData.minionSpeedModType == MinionSpeedModifier.normal && minionProjData.nextTicks < 0)
					return false;
				return true;
			}
			return minionProjData.isRealTick;
		}

		public override void Kill(Projectile projectile, int timeLeft)
		{
			buffs.Clear();
			killed = true;
			Player player = Main.player[projectile.owner];
			DefaultSpecialAbility.UnhookSpecialPower(projectile);

			//update cache
			if (!Config.current.DisableDynamicProjectileCacheUpdate)
			{
				player.ownedProjectileCounts[projectile.type]--;
			}
			//grapple
			if (projectile.aiStyle == 7 && hookedPlatform != null)
			{
				hookedPlatform.grapples.Remove(projectile);
			}
			if (IsMinion == ProjMinionRelation.isMinion)
			{
				ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
				minionTerminateSpecialAbility(projectile, this, GetMinionProjData(), player, playerFuncs);
				//gore
				minionDespawnEffect(projectile, this, minionProjData, player);
				if (ClientConfig.current.PrintMinionDeaths)
				{
					SummonersShine.logger.Info("MINION KILLED - TIMELEFT: " + projectile.timeLeft + " MINIONSPEEDMOD: " + MinionASMod + " CURRENTTICK: " + GetMinionProjData().currentTick + " NEXTTICKS: " + GetMinionProjData().nextTicks + " SLOTSMINIONS: " + player.slotsMinions + " MAXMINIONS: " + player.maxMinions);
					SummonersShine.logger.Info("STACK TRACE: " + Environment.StackTrace);
				}
				if (SourceItem != -1)
				{
					MinionEnergyCounter counter = playerFuncs.GetMinionCollection(SourceItem);
					counter.minions.Remove(projectile);
					if (counter.minions.Count == 0)
                    {
						int buffID = BakedConfig.GetMinionItemBuff(SourceItem);
						if(buffID != -1 && player.HasBuff(buffID))
							player.ClearBuff(buffID); //for minion despawn effect
					}
				}
				if (minionOnPlayerHitNPCWithProj != null)
					playerFuncs.Minion_OnHitNPCWithProj.Remove(projectile);
			}
		}

		public override bool PreDraw(Projectile projectile, ref Color lightColor)
		{
			Color color = lightColor;
			buffs.ForEach(i => i.PreDraw(projectile, this, color));
			lightColor = minionModifyColor(projectile, this, minionProjData, lightColor);
			return minionCustomPreDraw(projectile, this, minionProjData, lightColor);
		}
		public override void PostDraw(Projectile projectile, Color lightColor)
		{
			buffs.ForEach(i => i.PostDraw(projectile, this, lightColor));
			minionCustomPostDraw(projectile, this, minionProjData, lightColor);


			if (projectile.sentry)
			{
				Player player = Main.player[projectile.owner];
				ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();

				if (playerFuncs.HasMagenChiunEquipped)
				{
					Vector2 origin;
					if (projectile.aiStyle == 137)
						origin = projectile.Center + new Vector2(0, 8);
					else
						origin = projectile.Bottom;
					MagenChiun_Broken.MagenChiun_Draw(origin, projectile, this, GetMinionProjData(), lightColor);
				}
			}
		}

		public override void DrawBehind(Projectile projectile, int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			if (drawBehindType != DrawBehindType.normal) {
				behindNPCsAndTiles.Remove(index);
				behindNPCs.Remove(index);
				behindProjectiles.Remove(index);
				overPlayers.Remove(index);
				overWiresUI.Remove(index);
			}
			switch (drawBehindType)
			{
				case DrawBehindType.npc_tiles:
					behindNPCsAndTiles.Add(index);
					break;
				case DrawBehindType.npc:
					behindNPCs.Add(index);
					break;
				case DrawBehindType.projectiles:
					behindProjectiles.Add(index);
					break;
				case DrawBehindType.players:
					overPlayers.Add(index);
					break;
				case DrawBehindType.wires:
					overWiresUI.Add(index);
					break;
			}
		}

		public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
		{
			return minionOnTileCollide(projectile, this, minionProjData, oldVelocity);
		}

		//Gets the number of real ticks per tick, in the case of a very fast minion
		public float GetRealTicksPerTick(Projectile projectile) {
			float rv = MathF.Floor(GetSimulationRate(projectile));
			if (rv < 1)
				return 1;
			return rv;
		}

		//Gets the speed per tick, if there are multiple important ticks
		public float GetInternalSimRate(Projectile projectile) {
			float simRate = GetSimulationRate(projectile);
			float roundedSimRate = MathF.Floor(simRate);
			if (roundedSimRate < 1)
				return simRate;
			return simRate / roundedSimRate;
		}

		//I/O

		//Gets the number of ticks added per tick
		public float GetSpeed(Projectile projectile) {
			float num = MinionASMod;
			buffs.ForEach(i => num *= i.GetAttackSpeedBuff(projectile, this, GetMinionProjData()));
			num *= Main.player[projectile.owner].GetModPlayer<ReworkMinion_Player>().GetMinionAttackSpeed(SourceItem, GetMinionProjData().moveTarget);
			return num;
		}
		public float GetSimulationRate(Projectile projectile) {

			/*if (projectile.type == ProjectileID.StardustDragon1)
			{
				float rv = 0;
				List<Projectile> list = ModUtils.GetWholeStardustDragon(projectile.owner);
				list.ForEach(i => rv += 1 / i.GetGlobalProjectile<ReworkMinion_Projectile>().GetSpeed(i));
				return rv / list.Count;
			}*/
			return 1 / GetSpeed(projectile);
		}
		public override void SetDefaults(Projectile projectile)
		{
			switch (projectile.type)
			{
				case ProjectileID.DeadlySphere:
				case ProjectileID.FlinxMinion:
				case ProjectileID.ImpFireball:
				case ProjectileID.DangerousSpider:
				case ProjectileID.JumperSpider:
				case ProjectileID.VenomSpider:
					/*projectile.usesLocalNPCImmunity = true;
					projectile.usesIDStaticNPCImmunity = false;
					projectile.localNPCHitCooldown = projectile.idStaticNPCHitCooldown;
					projectile.idStaticNPCHitCooldown = 0;*/
					break;
				case ProjectileID.Raven:
					projectile.localNPCHitCooldown = 40;
					break;
			}

			bool isWhip = projectile.DamageType == DamageClass.SummonMeleeSpeed;
			if (projectile.CountsAsClass(DamageClass.Summon) || BakedConfig.Get_ProjectileCountedAsMinion(projectile.type))
			{
				if (ProjectileID.Sets.MinionShot[projectile.type] || BakedConfig.Get_ProjectileNotCountedAsMinion(projectile.type))
				{
					IsMinion = ProjMinionRelation.fromMinion;
				}
				else
				{
					if (isWhip)
						IsMinion = ProjMinionRelation.isWhip;
					else
					{
						IsMinion = ProjMinionRelation.isMinion;
						GetMinionProjData();
						if (projectile.ModProjectile != null && projectile.ModProjectile.Mod != SummonersShine.modInstance && !projectile.usesIDStaticNPCImmunity && !projectile.usesLocalNPCImmunity)
						{
							projectile.usesIDStaticNPCImmunity = true;
							projectile.idStaticNPCHitCooldown = 10;
						}
					}
				}
			}
			else
			{
				minionOnHitNPC += DefaultMinionAI.Empty;
			}
		}
		//SetMoveTarget - Used when the minion wants to approach a location relative to an entity (player or otherwise)
		public static void SetMoveTarget(Projectile projectile, Entity moveTarget)
		{
			ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
			if (projFuncs.IsMinion != ProjMinionRelation.isMinion)
				return;
			MinionProjectileData projData = projFuncs.GetMinionProjData();
			/*if (moveTarget != projData.moveTarget)
				projData.moveTargetLastPosition = moveTarget.position;*/
			projData.moveTarget = moveTarget;
		}

		public static void SetMoveTarget_FromID(Projectile projectile, int ID) {
			if (ID != -1)
				SetMoveTarget(projectile, Main.npc[ID]);
			else
				SetMoveTarget(projectile, Main.player[projectile.owner]);
		}

		bool IsProjectileRetreating(MinionTracking_State trackingState)
		{
			switch (trackingState)
			{
				case MinionTracking_State.retreating:
				case MinionTracking_State.retreatingProjected:
					return true;
			}
			return false;
		}
		bool ShouldProjectVelSpeed(MinionTracking_State trackingState)
		{
			switch (trackingState)
			{
				case MinionTracking_State.normalProjected:
				case MinionTracking_State.retreatingProjected:
					return true;
			}
			return false;
		}

		//Updates the projectile's relative motion
		public void UpdateProjectileRelativeVelocity(Projectile projectile, float realTicksPerTick)
		{
			if (minionProjData.trackingState == MinionTracking_State.noTracking) {
				minionProjData.lastRelativeVelocity = Vector2.Zero;
				return;
			}

			Player player = Main.player[projectile.owner];
			if (minionProjData.moveTarget == null || minionProjData.moveTarget.active == false)
				SetMoveTarget(projectile, player);

			Entity moveTarget = player;
			if (!IsProjectileRetreating(minionProjData.trackingState)) {
				moveTarget = minionProjData.moveTarget;
			}

			Vector2 entityVelocity = moveTarget.velocity;
			if (moveTarget == player)
				entityVelocity = player.GetRealPlayerVelocity();
			else
			{
				NPC npc = moveTarget as NPC;
				if (npc != null)
				{
					entityVelocity = npc.GetRealNPCVelocity();
				}
			}

			Vector2 projDistance = moveTarget.Center - projectile.Center;
			float projDistanceLenSqr = projDistance.LengthSquared();
			bool useRetreatingLogic = IsProjectileRetreating(minionProjData.trackingState) && projDistanceLenSqr > return_SafeDistanceFromPlayer_sqr;
			if (!useRetreatingLogic)
			{
				float Length = entityVelocity.Length();
				float NewLength = Length - minionProjData.minionTrackingImperfection * GetSimulationRate(projectile);
				if (NewLength <= 0)
				{
					entityVelocity = Vector2.Zero;
				}
				else
				{
					entityVelocity *= NewLength / Length;
				}
			}
			if (ShouldProjectVelSpeed(minionProjData.trackingState))
			{
				Vector2 projVel = projectile.velocity;
				float dot = Vector2.Dot(projVel, entityVelocity);
				if (dot <= 0 || entityVelocity == Vector2.Zero || projVel == Vector2.Zero)
				{
					entityVelocity = Vector2.Zero;
				}
				else
				{
					float maxLen = entityVelocity.Length() * 1.2f;
					Vector2 testVel = projVel * entityVelocity.LengthSquared() / dot;
					if (testVel.LengthSquared() > maxLen * maxLen)
					{
						testVel = projVel * maxLen / projVel.Length();
					}
					entityVelocity = testVel;
				}
			}
			else if (minionProjData.trackingState == MinionTracking_State.xOnly)
				entityVelocity.Y = 0;

			// prevent being pushed further away from intended target
			bool doAccelerateTowardsPlayer = false;
			float ratio = 0;
			if (moveTarget == player && projDistanceLenSqr > return_QuicklyAccelerateDistanceFromPlayer_sqr)
			{
				ratio = (projDistanceLenSqr * GetSpeed(projectile) - return_QuicklyAccelerateDistanceFromPlayer_sqr) / return_SafeDistanceFromPlayer_sqr;
				doAccelerateTowardsPlayer = true;
				if (ratio > 1)
					ratio = 1;
			}
			float combinedLength = entityVelocity.Length() * projectile.velocity.Length();
			if (combinedLength > 0)
			{
				float velEntDot = Vector2.Dot(entityVelocity, projectile.velocity);
				velEntDot /= combinedLength;
				if (velEntDot <= 0)
					entityVelocity *= ratio;
				else
					entityVelocity *= 1 - (1 - velEntDot) * (1 - ratio);
			}


			Vector2 targetCurrentDiff = entityVelocity - minionProjData.lastRelativeVelocity;

			if (useRetreatingLogic ||
				(targetCurrentDiff.LengthSquared() <= (minionProjData.minionTrackingAcceleration * minionProjData.minionTrackingAcceleration)))
				minionProjData.lastRelativeVelocity = entityVelocity;
			else if (player == moveTarget && minionProjData.lastRelativeVelocity.LengthSquared() > entityVelocity.LengthSquared())
			{
				Vector2 difference = minionProjData.lastRelativeVelocity - entityVelocity;
				minionProjData.lastRelativeVelocity = entityVelocity;
			}
			else
			{
				float accelPerTick = minionProjData.minionTrackingAcceleration;
				float targetCurrentTickLen = targetCurrentDiff.Length();
				float entVelLen = entityVelocity.LengthSquared();
				if (entVelLen != 0 && doAccelerateTowardsPlayer)
				{
					accelPerTick = MathHelper.Lerp(accelPerTick, targetCurrentTickLen, ratio);

					float dot = Vector2.Dot(entityVelocity, minionProjData.lastRelativeVelocity);
					Vector2 projected = entityVelocity * dot / entVelLen;
					projected = minionProjData.lastRelativeVelocity - projected;
					minionProjData.lastRelativeVelocity -= projected;
					if (ShouldProjectVelSpeed(minionProjData.trackingState))
						projected *= 0.5f;
					else
						projected *= 0.95f;

					if (dot < 0)
						minionProjData.lastRelativeVelocity *= 0.95f;
					minionProjData.lastRelativeVelocity += projected;
				}
				if (targetCurrentTickLen > 0)
					targetCurrentDiff /= targetCurrentTickLen;
				minionProjData.lastRelativeVelocity += targetCurrentDiff * accelPerTick;
			}

			//do collision
			Vector2 oldPos = projectile.position;
			Vector2 finalPos = projectile.position;
			for (int x = 0; x < realTicksPerTick; x++) {
				Vector2 change = minionProjData.lastRelativeVelocity / realTicksPerTick;
				if (projectile.tileCollide && change != Vector2.Zero)
				{
					change = Collision.TileCollision(finalPos, change, projectile.width, projectile.height, projectile.shouldFallThrough, projectile.shouldFallThrough);
					Vector4 slopeCollisionData = Collision.SlopeCollision(finalPos, change, projectile.width, projectile.height);
					change.X = slopeCollisionData.Z;
					change.Y = slopeCollisionData.W;
					finalPos.X = slopeCollisionData.X;
					finalPos.Y = slopeCollisionData.Y;
				}

				finalPos += change;
				finalPos.X = Math.Clamp(finalPos.X, 320f, 16f * Main.maxTilesX - 320f);
				finalPos.Y = Math.Clamp(finalPos.Y, 320f, 16f * Main.maxTilesY - 320f);

			}
			Vector2 diff = finalPos - oldPos;
			projectile.position += diff;
			//fix blur effects
			for (int i = 0; i < projectile.oldPos.Length; i++)
			{
				projectile.oldPos[i] += diff;
			}
		}
		public float GetMinionPower(Projectile projectile, int index)
		{
			if (SourceItem == -1)
				return 0;
			return Main.player[projectile.owner].GetModPlayer<ReworkMinion_Player>().GetMinionPower(index, ReworkMinion_Item.minionPowers[SourceItem], PrefixMinionPower);
		}

		//Adds literal aimbot to minion projectiles
		public static Vector2 GetTotalProjectileVelocity(Projectile newProj, Projectile projectile, NPC npc)
		{
			return GetTotalProjectileVelocity(newProj.velocity, newProj.extraUpdates + 1, projectile, npc);
		}
		public static Vector2 GetTotalProjectileVelocity(Vector2 vel, float numUpdates, Projectile projectile, NPC npc)
		{
			MinionProjectileData projData = projectile.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData();
			if (projData.moveTarget == null || projData.moveTarget == Main.player[projectile.owner] || vel == Vector2.Zero)
				return vel;
			Vector2 lastVel;

			//Get projectile velocity
			if (npc != null)
			{
				lastVel = npc.GetRealNPCVelocity();
			}
			else
			{
				lastVel = projData.moveTarget.velocity;
			}

			Vector2 projVelBoost = projectile.velocity + projData.lastRelativeVelocity;
			projVelBoost /= numUpdates;

			//prevent backwards shots
			float velLenSqr = vel.LengthSquared();
			if (Vector2.Dot(vel, projVelBoost) < 0)
				projVelBoost -= vel * Vector2.Dot(projVelBoost, vel) / velLenSqr;
			//the no cheating version
			Vector2 tVel = lastVel - projVelBoost;
			if (tVel.LengthSquared() != 0)
			{
				Vector2 target = projData.moveTarget.Center;
				Vector2 start = projectile.Center;

				vel = ProjTracking_NoCheat(start, target, tVel, lastVel, vel.Length() * numUpdates) / numUpdates;
			}
			vel += projVelBoost;

			return vel;
		}

		public static Vector2 ProjTracking_NoCheat(Vector2 start, Vector2 target, Vector2 targetVel, Vector2 actualTargetVel, float speed)
		{
			Vector2 disp = target - start;
			float tVelLen = targetVel.Length();
			float dispLen = disp.Length();
			float dot = Vector2.Dot(-disp, targetVel);
			float oneOverDispLen = 1 / dispLen;
			float cosT = dot * oneOverDispLen / tVelLen;
			float aCosT = tVelLen * cosT;

			float disc = speed * speed - tVelLen * tVelLen + aCosT * aCosT;
			if (disc < 0)
				return disp * speed * oneOverDispLen;
			disc = MathF.Sqrt(disc);
			float b = aCosT + disc;
			float mult = b * oneOverDispLen;
			
			if (mult > 0)
			{
				Vector2 fullTargetVel = actualTargetVel * 1 / mult;
				Vector2 newFullTargetVel = ModUtils.GetTileCollideModifier(target, fullTargetVel);
				if (fullTargetVel != newFullTargetVel)
				{
					newFullTargetVel = (disp + newFullTargetVel);
					newFullTargetVel.Normalize();
					return newFullTargetVel * speed;
				}
			}
			disp *= mult;
			return disp + targetVel;
		}

		static bool IsOrFromMinion(ProjMinionRelation projMinionRelation) {
			return projMinionRelation == ProjMinionRelation.isMinion || projMinionRelation == ProjMinionRelation.fromMinion;
		}

        public override void SetStaticDefaults()
        {
			MinionDataHandler.ProjectileSetStaticDefaults();
        }
    }

	public static class ProjectileMethods {
		public static int GetDamageTaken(this Projectile projectile, int damage)
		{
			damage = Main.DamageVar(damage, 0f - Main.player[projectile.owner].luck);

			float mult = Main.GameModeInfo.EnemyDamageMultiplier;
			bool isJourneyMode = Main.GameModeInfo.IsJourneyMode;
			if (isJourneyMode)
			{
                CreativePowers.DifficultySliderPower power = CreativePowerManager.Instance.GetPower<CreativePowers.DifficultySliderPower>();
				if (power.GetIsUnlocked())
				{
					mult = power.StrengthMultiplierToGiveNPCs;
				}
			}
			damage = (int)(damage * mult * 2);
			return damage;
		}

		public static bool IsOnRealTick(this Projectile projectile, MinionProjectileData projData)
		{
			return projData.currentTick == 1 && projectile.numUpdates < 0;
		}
		public static bool IsOnRealVanillaTick(this Projectile projectile, MinionProjectileData projData)
		{
			return projData.currentTick == 1;
		}
	}
}