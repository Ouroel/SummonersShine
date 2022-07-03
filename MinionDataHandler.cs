using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SummonersShine.BakedConfigs;
using SummonersShine.MinionAI;
using SummonersShine.Projectiles;
using SummonersShine.SpecialAbilities;
using SummonersShine.SpecialAbilities.DefaultSpecialAbility;
using SummonersShine.SpecialData;
using SummonersShine.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using static SummonersShine.Config;

using SummonersShine.ModSupport;

namespace SummonersShine
{

	public class MinionDataHandler_Loader : ModSystem
	{
		public override void PostSetupContent()
		{
			/*MinionDataHandler.NoTrackingProjectiles = new bool[ItemLoader.ItemCount];
			MinionDataHandler.NoTrackingProjectiles[ProjectileID.WhiteTigerPounce] = true;
			MinionDataHandler.NoTrackingProjectiles[ProjectileID.UFOLaser] = true;
			MinionDataHandler.NoTrackingProjectiles[ProjectileID.MoonlordTurretLaser] = true;
			MinionDataHandler.NoTrackingProjectiles[ProjectileModIDs.GoldenFruit] = true;*/
		}
	}
	public static class MinionDataHandler
	{
		static bool ProjectilesAlreadyLoaded = false;
		static bool ItemsAlreadyLoaded = false;
		static bool BuffsAlreadyLoaded = false;

		public static List<Func<int, int, Tuple<Texture2D, Rectangle>>> ModdedGetDisplayData = new();

		public static bool[] NoTrackingProjectiles;

		public static ModSupportProjectile[] ModSupportProjectiles;
		public static ModSupportBuff[] ModSupportBuffs;

		public static MassBitsByte ItemsLoadedStaticDefaults;

		public class MinionDataHandler_Loader : ILoadable
		{
			public void Load(Mod mod)
			{
				ModdedGetDisplayData = new();
				ProjectilesAlreadyLoaded = false;
				ItemsAlreadyLoaded = false;
			}

			public void Unload()
			{
				ModSupportProjectiles = null;
				ModSupportBuffs = null;
				ModdedGetDisplayData = null;
				ProjectilesAlreadyLoaded = false;
				ItemsAlreadyLoaded = false;
				//NoTrackingProjectiles = null;
				ItemsLoadedStaticDefaults = null;
			}
		}

		public static Tuple<Texture2D, Rectangle> GetEnergyThoughtTexture(int itemID, int frame)
		{
			int offsetNum;
			switch (itemID)
			{
				case ItemID.BabyBirdStaff:
					offsetNum = 1;
					break;
				case ItemID.SlimeStaff:
					offsetNum = 2;
					break;
				case ItemID.FlinxStaff:
					offsetNum = 3;
					break;
				case ItemID.VampireFrogStaff:
					offsetNum = 4;
					break;
				case ItemID.HornetStaff:
					offsetNum = 5;
					break;
				case ItemID.ImpStaff:
					offsetNum = 6;
					break;
				case ItemID.SpiderStaff:
					offsetNum = 7;
					break;
				case ItemID.PirateStaff:
					offsetNum = 8;
					break;
				case ItemID.Smolstar:
					offsetNum = 9;
					break;
				case ItemID.SanguineStaff:
					offsetNum = 10;
					break;
				case ItemID.TempestStaff:
					offsetNum = 11;
					break;
				case ItemID.OpticStaff:
					offsetNum = 12;
					break;
				case ItemID.PygmyStaff:
					offsetNum = 13;
					break;
				case ItemID.DeadlySphereStaff:
					offsetNum = 14;
					break;
				case ItemID.RavenStaff:
					offsetNum = 15;
					break;
				case ItemID.XenoStaff:
					offsetNum = 16;
					break;
				case ItemID.StardustCellStaff:
					offsetNum = 17;
					break;
				case ItemID.StardustDragonStaff:
					offsetNum = 18;
					break;
				case ItemID.StormTigerStaff:
					offsetNum = 19;
					break;
				case ItemID.EmpressBlade:
					offsetNum = 20;
					break;
				case ItemID.AbigailsFlower:
					offsetNum = 21;
					break;
				case ItemID.HoundiusShootius:
					offsetNum = 22;
					break;
				case ItemID.QueenSpiderStaff:
					offsetNum = 23;
					break;
				default:
					return null;
			}

			Texture2D value = ModTextures.ThoughtBubble;
			Rectangle rectangle = value.Frame(6, 46, frame, offsetNum, 0, 0);
			return new Tuple<Texture2D, Rectangle>(value, rectangle);
		}
		public static void ProjectileSetDefaults(Projectile projectile, ReworkMinion_Projectile projFuncs)
		{
			MinionProjectileData projData = projFuncs.GetMinionProjData();

			if (ModSupportProjectiles != null && ModSupportProjectiles[projectile.type] != null)
			{
				ModSupportProjectiles[projectile.type].HookToProjectile(projFuncs, projData);
				return;
			}
			//modded defaults
			if (projectile.ModProjectile != null)
			{
				projFuncs.minionPreAI += DefaultMinionAI.NoEffect;
				projFuncs.minionPostAI += DefaultMinionAI.NoEffect;
				projFuncs.minionEndOfAI += DefaultMinionAI.Empty;
				projFuncs.minionPostAIPostRelativeVelocity += DefaultMinionAI.Empty;
				projFuncs.minionSummonEffect += DefaultMinionAI.NoEffect;
				projFuncs.minionDespawnEffect += DefaultMinionAI.NoEffect;
				projFuncs.minionOnHitNPC += DefaultMinionAI.Empty;
				projFuncs.minionOnHitNPC_Fake += DefaultMinionAI.Empty;
				//ConfigRehookSourceItem(projectile, projFuncs);
				return;
			}
			projData.minionFlickeringThreshold = 1f;

			// post AI post relative vel
			switch (projectile.aiStyle)
			{
				case 121:
					if (projectile.type == ProjectileID.StardustDragon1)
					{
						projFuncs.minionPostAIPostRelativeVelocity += DefaultMinionAI.Dragon_PostAIPostRelativeVelocity;
					}
					else
						projFuncs.minionPostAIPostRelativeVelocity += DefaultMinionAI.Empty;
					break;
				default:
					projFuncs.minionPostAIPostRelativeVelocity += DefaultMinionAI.Empty;
					break;
			}

			// post AI
			switch (projectile.aiStyle)
			{
				case 26:
				case 67:
					projFuncs.minionPreAI += DefaultMinionAI.AI_067_ResetAggro;
					projFuncs.minionPostAI += DefaultMinionAI.AI_067_ResetRocketDelay;
					switch (projectile.type)
					{
						case ProjectileID.BabySlime:
							projFuncs.minionPostAI += DefaultMinionAI.Groundbound_PostAI_Slime;
							break;
						case ProjectileID.Pygmy:
						case ProjectileID.Pygmy2:
						case ProjectileID.Pygmy3:
						case ProjectileID.Pygmy4:
							break;
						case ProjectileID.VenomSpider:
						case ProjectileID.JumperSpider:
						case ProjectileID.DangerousSpider:
							projFuncs.minionPostAI += DefaultMinionAI.Groundbound_PostAI_Spider;
							break;
						case ProjectileID.StormTigerTier1:
						case ProjectileID.StormTigerTier2:
						case ProjectileID.StormTigerTier3:
							projFuncs.minionPostAI += DefaultMinionAI.Groundbound_PostAI_Tiger;
							break;
						default:
							projFuncs.minionPostAI += DefaultMinionAI.Groundbound_PostAI;
							break;
					}
					break;
				case 169:
					projFuncs.minionPostAI += DefaultMinionAI.Smolchiunstars_PostAI;
					break;
				case 66:
					switch (projectile.type)
					{
						case 387:
						case 388:
							projFuncs.minionPostAI += DefaultMinionAI.Normal_PostAI;
							break;
						case 533:
							projFuncs.minionPostAI += DefaultMinionAI.DeadlySphere_PostAI;
							break;
					}
					break;
				case 156:
					projFuncs.minionPostAI += DefaultMinionAI.BatofLight_PostAI;
					projData.minionSpeedModType = MinionSpeedModifier.stepped;
					break;
				case 54:
				case 62:
				case 158:
					if (projectile.type == ProjectileID.UFOMinion)
						projFuncs.minionPostAI += DefaultMinionAI.UFO_PostAI;
					projFuncs.minionPostAI += DefaultMinionAI.Normal_PostAI;
					break;
				case 120:
					projFuncs.minionPostAI += DefaultMinionAI.StardustGuardian_PostAI;
					projData.minionSpeedModType = MinionSpeedModifier.stepped;
					break;
				case 121:
					if (projectile.type == ProjectileID.StardustDragon1)
					{
						projFuncs.minionPostAI += DefaultMinionAI.DragonPostAI;
					}
					else
					{
						projFuncs.minionPostAI += DefaultMinionAI.NoEffect;
						projData.minionSpeedModType = MinionSpeedModifier.stepped;
						//projFuncs.minionPostAI += DefaultMinionAI.DragonBodyPostAI;
						//projData.minionSpeedModType = MinionSpeedModifier.letothersupdate;
					}
					break;
				default:
					if (projectile.minion)
					{
						projFuncs.minionPreAI += DefaultMinionAI.NoEffect;
						projFuncs.minionPostAI += DefaultMinionAI.NoEffect;
					}
					else
						projFuncs.minionPostAI += DefaultMinionAI.NoEffect;
					break;
			}
			// pre AI
			switch (projectile.type)
			{
				case ProjectileID.SoulscourgePirate:
				case ProjectileID.PirateCaptain:
				case ProjectileID.OneEyedPirate:
					projFuncs.minionPreAI += DefaultMinionAI.PiratePreAI;
					projFuncs.minionEndOfAI += DefaultMinionAI.PirateEndOfAI;
					projFuncs.minionPostAI += DefaultMinionAI.PiratePostAI;
					projFuncs.minionPostAIPostRelativeVelocity += DefaultMinionAI.PiratePostAIPostRelativeVelocity;
					break;
				case ProjectileID.VenomSpider:
				case ProjectileID.JumperSpider:
				case ProjectileID.DangerousSpider:
					projFuncs.minionPreAI += DefaultMinionAI.Groundbound_PreAI_Spider;
					projFuncs.minionEndOfAI += DefaultMinionAI.Empty;
					break;
				default:
					projFuncs.minionPreAI += DefaultMinionAI.Empty;
					projFuncs.minionEndOfAI += DefaultMinionAI.Empty;
					break;

			}

			// post AI
			switch (projectile.type)
			{
				case ProjectileID.FlyingImp:
					projFuncs.minionPostAI += DefaultMinionAI.ImpPostAI;
					break;
			}

			// on hit NPC

			switch (projectile.type)
			{
				case ProjectileID.FlinxMinion:
					projFuncs.minionOnHitNPC += SpecialAbility.FlinxOnHitNPC;
					projFuncs.minionOnHitNPC_Fake += DefaultMinionAI.Empty;
					projFuncs.minionOnTileCollide += SpecialAbility.FlinxOnTileCollide;
					projFuncs.minionOnSlopeCollide += SpecialAbility.Flinx_OnSlopeCollide;
					projFuncs.minionOnMovement += SpecialAbility.FlinxOnMovement;
					break;
				case ProjectileID.VampireFrog:
					projFuncs.minionOnHitNPC += SpecialAbility.FrogOnHitNPC;
					projFuncs.minionOnHitNPC_Fake += SpecialAbility.FrogOnHitNPC_Fake;
					break;
				case ProjectileID.Smolstar:
					projFuncs.minionOnHitNPC += SpecialAbility.SmolChiunStarOnHitNPC;
					projFuncs.minionOnHitNPC_Fake += DefaultMinionAI.Empty;
					break;
				case ProjectileID.DeadlySphere:
					projFuncs.minionOnHitNPC += SpecialAbility.DeadlySphereOnHitNPC;
					projFuncs.minionOnHitNPC_Fake += DefaultMinionAI.Empty;
					break;
				default:
					projFuncs.minionOnHitNPC += DefaultMinionAI.Empty;
					projFuncs.minionOnHitNPC_Fake += DefaultMinionAI.Empty;
					break;

			}

			// remove tracking from specific minions
			switch (projectile.type)
			{
				case ProjectileID.StardustDragon2:
				case ProjectileID.StardustDragon3:
				case ProjectileID.StardustDragon4:
				case ProjectileID.BatOfLight:
				case ProjectileID.EmpressBlade:
					projFuncs.GetMinionProjData().trackingState = MinionTracking_State.noTracking;
					break;
			}
			// set minion tracking
			switch (projectile.type)
			{
				case ProjectileID.BabyBird:
					projData.minionTrackingAcceleration = 0.1f;
					projData.minionTrackingImperfection = 5f;
					break;
				case ProjectileID.AbigailCounter:
					break;
				case ProjectileID.AbigailMinion:
					projData.minionTrackingAcceleration = 0.1f;
					projData.minionTrackingImperfection = 5f;
					break;
				case ProjectileID.BabySlime:
					projData.minionTrackingAcceleration = 0.1f;
					projData.minionTrackingImperfection = 5f;
					break;
				case ProjectileID.FlinxMinion:
					projData.minionTrackingAcceleration = 0.1f;
					projData.minionTrackingImperfection = 5f;
					projData.minionFlickeringThreshold = 0.1f;
					break;
				case ProjectileID.VampireFrog:
					projData.minionTrackingAcceleration = 0.1f;
					projData.minionTrackingImperfection = 5f;
					break;
				case ProjectileID.Hornet:
					projData.minionTrackingAcceleration = 0.1f;
					projData.minionTrackingImperfection = 5f;
					break;
				case ProjectileID.FlyingImp:
					projData.minionTrackingAcceleration = 0.1f;
					projData.minionTrackingImperfection = 5f;
					break;
				case ProjectileID.VenomSpider:
				case ProjectileID.JumperSpider:
				case ProjectileID.DangerousSpider:
					projData.minionTrackingAcceleration = 100000f;
					projData.minionTrackingImperfection = 0f;
					break;
				case ProjectileID.SoulscourgePirate:
				case ProjectileID.PirateCaptain:
				case ProjectileID.OneEyedPirate:
					projData.minionTrackingAcceleration = 0.1f;
					projData.minionTrackingImperfection = 5f;
					break;
				case ProjectileID.BatOfLight:
					projData.minionTrackingAcceleration = 0f;
					projData.minionTrackingImperfection = 0f;
					break;
				case ProjectileID.Smolstar:
					projData.minionTrackingAcceleration = 100f;
					projData.minionTrackingImperfection = 6f;
					break;
				case ProjectileID.Retanimini:
				case ProjectileID.Spazmamini:
					projData.minionTrackingAcceleration = 10f;
					projData.minionTrackingImperfection = 5f;
					break;
				case ProjectileID.Pygmy:
				case ProjectileID.Pygmy2:
				case ProjectileID.Pygmy3:
				case ProjectileID.Pygmy4:
					projData.minionTrackingAcceleration = 0.1f;
					projData.minionTrackingImperfection = 10f;
					break;
				case ProjectileID.DeadlySphere:
					projData.minionTrackingAcceleration = 10f;
					projData.minionTrackingImperfection = 5f;
					break;
				case ProjectileID.Raven:
					projData.minionTrackingAcceleration = 0.1f;
					projData.minionTrackingImperfection = 7f;
					//projFuncs.AddBuff(projectile, ProjectileBuffs.ProjectileBuffIDs.RavenSpeedBuff, projectile);
					break;
				case ProjectileID.Tempest:
					projData.minionTrackingAcceleration = 0.1f;
					projData.minionTrackingImperfection = 5f;
					break;
				case ProjectileID.UFOMinion:
					projData.minionTrackingAcceleration = 10000f;
					projData.minionTrackingImperfection = 2f;
					break;
				case ProjectileID.StardustCellMinion:
					projData.minionTrackingAcceleration = 0.01f;
					projData.minionTrackingImperfection = 5f;
					break;
				case ProjectileID.StardustGuardian:
					projData.minionTrackingAcceleration = 100f;
					projData.minionTrackingImperfection = 5f;
					break;
				case ProjectileID.StardustDragon1:
					projData.minionTrackingAcceleration = 0.1f;
					projData.minionTrackingImperfection = 5f;
					break;
				case ProjectileID.EmpressBlade:
					projData.minionTrackingAcceleration = 0f;
					projData.minionTrackingImperfection = 0f;
					break;
				case ProjectileID.StormTigerGem:
					projectile.hide = false;
					projData.minionSpeedModType = MinionSpeedModifier.none;
					break;
				case ProjectileID.StormTigerTier1:
				case ProjectileID.StormTigerTier2:
				case ProjectileID.StormTigerTier3:
					projData.minionTrackingAcceleration = 10f;
					projData.minionTrackingImperfection = 5f;
					projData.minionFlickeringThreshold = 0.1f;
					break;
			}
			// summon/despawn effects
			switch (projectile.type)
			{
				case ProjectileID.BabySlime:
					projFuncs.minionSummonEffect += DefaultMinionAI.SlimeSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.SlimeSummonEffect;
					break;
				case ProjectileID.BabyBird:
					projFuncs.minionSummonEffect += DefaultMinionAI.BabyBirdSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.BabyBirdDespawnEffect;
					break;
				case ProjectileID.AbigailCounter:
					projFuncs.minionSummonEffect += DefaultMinionAI.AbigailCounterSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.AbigailCounterDespawnEffect;
					projFuncs.minionCustomAI = DefaultMinionAI.AbigailCounterCustomAI;
					projFuncs.minionCustomPostDraw = DefaultMinionAI.AbigailCounterPostDraw;
					break;
				case ProjectileID.AbigailMinion:
					projFuncs.minionSummonEffect += DefaultMinionAI.AbigailSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.AbigailDespawnEffect;
					projFuncs.minionPostAI += DefaultMinionAI.AbigailPostAI;
					break;
				case ProjectileID.VampireFrog:
					projFuncs.minionSummonEffect += DefaultMinionAI.FrogSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.FrogSummonEffect;
					projFuncs.minionPostAI += DefaultMinionAI.FrogPostAI;
					break;
				case ProjectileID.Hornet:
					projFuncs.minionSummonEffect += DefaultMinionAI.HornetSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.HornetDespawnEffect;
					break;
				case ProjectileID.FlinxMinion:
					projFuncs.minionSummonEffect += DefaultMinionAI.FlinxSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.FlinxSummonEffect;
					break;
				case ProjectileID.FlyingImp:
					projFuncs.minionSummonEffect += DefaultMinionAI.ImpSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.ImpDespawnEffect;
					break;
				case ProjectileID.VenomSpider:
				case ProjectileID.JumperSpider:
				case ProjectileID.DangerousSpider:
					projFuncs.minionSummonEffect += DefaultMinionAI.SpiderSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.SpiderDespawnEffect;
					break;
				case ProjectileID.SoulscourgePirate:
				case ProjectileID.PirateCaptain:
				case ProjectileID.OneEyedPirate:
					projFuncs.minionSummonEffect += DefaultMinionAI.PirateSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.PirateDespawnEffect;
					break;
				case ProjectileID.Smolstar:
					projFuncs.minionSummonEffect += DefaultMinionAI.SmolChiunStarSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.SmolChiunStarDespawnEffect;
					break;
				case ProjectileID.Tempest:
					projFuncs.minionSummonEffect += DefaultMinionAI.SharknadoSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.SharknadoDespawnEffect;
					projFuncs.minionPostAI += DefaultMinionAI.SharknadoPostAI;
					break;
				case ProjectileID.Retanimini:
					projFuncs.minionSummonEffect += DefaultMinionAI.TwinsSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.RetDespawnEffect;
					break;
				case ProjectileID.Raven:
					projFuncs.minionSummonEffect += DefaultMinionAI.RavenSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.RavenDespawnEffect;
					projFuncs.minionPostAI += DefaultMinionAI.Raven_PostAI;
					break;
				case ProjectileID.Spazmamini:
					projFuncs.minionSummonEffect += DefaultMinionAI.NoEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.SpazDespawnEffect;
					break;
				case ProjectileID.Pygmy:
				case ProjectileID.Pygmy2:
				case ProjectileID.Pygmy3:
				case ProjectileID.Pygmy4:
					projFuncs.minionSummonEffect += DefaultMinionAI.PygmySummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.PygmyDespawnEffect;
					break;
				case ProjectileID.DeadlySphere:
					projFuncs.minionSummonEffect += DefaultMinionAI.DeadlySphere_SummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.DeadlySphere_DespawnEffect;
					break;
				case ProjectileID.UFOMinion:
					projFuncs.minionSummonEffect += DefaultMinionAI.UFOSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.UFODespawnEffect;
					projFuncs.minionPostAI += DefaultMinionAI.UFOPostAI;
					break;
				case ProjectileID.StardustCellMinion:
					projFuncs.minionSummonEffect += DefaultMinionAI.CellSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.CellDespawnEffect;
					projFuncs.minionPostAI += DefaultMinionAI.CellPostAI;
					break;
				case ProjectileID.StardustDragon1:
				case ProjectileID.StardustDragon2:
				case ProjectileID.StardustDragon3:
				case ProjectileID.StardustDragon4:
					projFuncs.minionSummonEffect += DefaultMinionAI.DragonSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.DragonDespawnEffect;
					break;
				case ProjectileID.BatOfLight:
				case ProjectileID.EmpressBlade:
					projFuncs.minionSummonEffect += DefaultMinionAI.NoEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.BatOfLight_DespawnEffect;
					break;
				case ProjectileID.StormTigerGem:
					projFuncs.minionSummonEffect += DefaultMinionAI.StormTigerGemSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.StormTigerGemDespawnEffect;
					break;
				case ProjectileID.StormTigerTier1:
				case ProjectileID.StormTigerTier2:
				case ProjectileID.StormTigerTier3:
					projFuncs.minionSummonEffect += DefaultMinionAI.StormTigerSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.StormTigerDespawnEffect;
					projData.minionFlickeringThreshold = 0.1f;
					break;

				case ProjectileID.HoundiusShootius:
					projFuncs.minionSummonEffect += DefaultMinionAI.NoEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.HoundiusShootiusDespawnEffect;
					projFuncs.minionOnMovement += DefaultMinionAI.HoundiusShootiusOnMovement;
					break;
				case ProjectileID.SpiderHiver:
					projFuncs.minionSummonEffect += DefaultMinionAI.NoEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.SpiderHiverDespawnEffect;
					projFuncs.minionPostAI += DefaultMinionAI.SpiderHiverPostAI;
					break;
				case ProjectileID.RainbowCrystal:
					projFuncs.minionSummonEffect += DefaultMinionAI.NoEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.FadeOutTurretDespawnEffect;
					projFuncs.minionPostAI += DefaultMinionAI.RainbowCrystalPostAI;
					break;
				case ProjectileID.MoonlordTurret:
					projFuncs.minionSummonEffect += DefaultMinionAI.NoEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.FadeOutTurretDespawnEffect;
					projFuncs.minionPostAI += DefaultMinionAI.MoonlordTurretPostAI;
					break;
				case ProjectileID.DD2BallistraTowerT1:
				case ProjectileID.DD2BallistraTowerT2:
				case ProjectileID.DD2BallistraTowerT3:
					projFuncs.minionSummonEffect += DefaultMinionAI.BallistraTowerSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.BallistraTowerDespawnEffect;
					break;
				case ProjectileID.DD2LightningAuraT1:
				case ProjectileID.DD2LightningAuraT2:
				case ProjectileID.DD2LightningAuraT3:
					projFuncs.minionSummonEffect += DefaultMinionAI.LightningAuraSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.LightningAuraDespawnEffect;
					break;
				case ProjectileID.DD2ExplosiveTrapT1:
				case ProjectileID.DD2ExplosiveTrapT2:
				case ProjectileID.DD2ExplosiveTrapT3:
					projFuncs.minionSummonEffect += DefaultMinionAI.ExplosiveTrapSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.ExplosiveTrapDespawnEffect;
					break;
				case ProjectileID.DD2FlameBurstTowerT1:
				case ProjectileID.DD2FlameBurstTowerT2:
				case ProjectileID.DD2FlameBurstTowerT3:
					projFuncs.minionSummonEffect += DefaultMinionAI.FlameBurstTowerSummonEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.FlameBurstTowerDespawnEffect;
					break;
				default:
					projFuncs.minionSummonEffect += DefaultMinionAI.NoEffect;
					projFuncs.minionDespawnEffect += DefaultMinionAI.NoEffect;
					break;
			}

			//ConfigRehookSourceItem(projectile, projFuncs);

			switch (projectile.type)
			{
				case ProjectileID.AbigailCounter:
					projFuncs.minionOnSpecialAbilityUsed += DefaultMinionAI.AbigailCounterOnSpecialAbility;
					projData.maxEnergy += GetMinionPowerRechargeTime(ItemID.AbigailsFlower);
					break;
				case ProjectileID.BabySlime:
					projFuncs.minionOnSpecialAbilityUsed += SpecialAbility.SlimeSpecialAbility;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.SlimeStaff);
					break;
				case ProjectileID.BabyBird:
					projFuncs.minionOnSpecialAbilityUsed += SpecialAbility.BabyBirdSpecialAbility;
					projFuncs.minionTerminateSpecialAbility = SpecialAbility.BabyBirdTerminateSpecialAbility;
					projFuncs.minionCustomAI = SpecialAbility.BabyBirdAI;
					projFuncs.minionPostAI += SpecialAbility.BabyBirdPostAI;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.BabyBirdStaff);
					break;
				case ProjectileID.AbigailMinion:
					break;
				case ProjectileID.FlinxMinion:
					projFuncs.minionOnSpecialAbilityUsed += SpecialAbility.FlinxOnSpecialAbility;
					projFuncs.minionTerminateSpecialAbility = SpecialAbility.FlinxTerminateSpecialAbility;
					projFuncs.minionCustomAI = SpecialAbility.FlinxCustomAI;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.FlinxStaff);
					break;
				case ProjectileID.VampireFrog:
					projFuncs.minionOnSpecialAbilityUsed += SpecialAbility.FrogOnSpecialAbility;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.VampireFrogStaff);
					break;
				case ProjectileID.Hornet:
					projFuncs.minionOnSpecialAbilityUsed += SpecialAbility.HornetOnSpecialAbility;
					projFuncs.minionPreAI += SpecialAbility.HornetPreAI;
					projFuncs.minionOnShootProjectile += SpecialAbility.HornetOnShootProjectile;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.HornetStaff);
					break;
				case ProjectileID.FlyingImp:
					projFuncs.minionOnSpecialAbilityUsed += SpecialAbility.ImpOnSpecialAbility;
					projFuncs.minionCustomAI = DefaultMinionAI.ImpCustomAI;
					projFuncs.minionOnShootProjectile += DefaultMinionAI.ImpOnShootProjectile;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.ImpStaff);
					break;
				case ProjectileID.VenomSpider:
				case ProjectileID.JumperSpider:
				case ProjectileID.DangerousSpider:
					projFuncs.minionOnSpecialAbilityUsed += SpecialAbility.SpiderOnSpecialAbility;
					projFuncs.minionCustomAI = SpecialAbility.SpiderCustomAI;
					projFuncs.minionCustomPreDraw += SpecialAbility.SpiderCustomDraw;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.SpiderStaff);
					break;
				case ProjectileID.SoulscourgePirate:
				case ProjectileID.PirateCaptain:
				case ProjectileID.OneEyedPirate:
					projFuncs.minionOnSpecialAbilityUsed += SpecialAbility.PirateOnSpecialAbility;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.PirateStaff);
					break;
				case ProjectileID.Smolstar:
					projFuncs.minionOnSpecialAbilityUsed += SpecialAbility.SmolChiunStarOnSpecialAbility;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.Smolstar);
					break;
				case ProjectileID.Tempest:
					projFuncs.minionOnSpecialAbilityUsed += SpecialAbility.SharknadoOnSpecialAbility;
					projFuncs.minionPreAI += SpecialAbility.SharknadoPreAI;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.TempestStaff);
					break;
				case ProjectileID.Retanimini:
					projFuncs.minionOnSpecialAbilityUsed = SpecialAbility.Ret_Spaz_OnSpecialAbility;
					projFuncs.minionTerminateSpecialAbility = SpecialAbility.Ret_Spaz_TerminateSpecialAbility;
					projFuncs.minionPreAI += SpecialAbility.RetaniminiPreAI;
					projFuncs.minionOnPlayerHitNPCWithProj = SpecialAbility.Ret_OnHitNPCWithProj;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.OpticStaff);
					break;
				case ProjectileID.Spazmamini:
					projFuncs.minionOnSpecialAbilityUsed += SpecialAbility.Ret_Spaz_OnSpecialAbility;
					projFuncs.minionTerminateSpecialAbility = SpecialAbility.Ret_Spaz_TerminateSpecialAbility;
					projFuncs.minionCustomAI = SpecialAbility.SpazmaminiAI;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.OpticStaff);
					break;
				case ProjectileID.Pygmy:
				case ProjectileID.Pygmy2:
				case ProjectileID.Pygmy3:
				case ProjectileID.Pygmy4:
					projFuncs.minionOnSpecialAbilityUsed = SpecialAbility.PygmySpecialAbility;
					projFuncs.minionPostAI += SpecialAbility.PygmyPostAI;
					projFuncs.minionTerminateSpecialAbility = SpecialAbility.PygmyTerminateSpecialAbility;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.PygmyStaff);
					break;
				case ProjectileID.Raven:
					projFuncs.minionOnSpecialAbilityUsed = DefaultMinionAI.RavenSpecialAbility;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.RavenStaff);
					break;
				case ProjectileID.DeadlySphere:
					projFuncs.minionOnSpecialAbilityUsed = SpecialAbility.DeadlySphereSpecialAbility;
					projFuncs.onPostCreation = SpecialAbility.DeadlySpherePostCreation;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.DeadlySphereStaff);
					break;
				case ProjectileID.UFOMinion:
					projFuncs.minionCustomPreDraw = DefaultMinionAI.UFOCustomDraw;
					projFuncs.minionOnSpecialAbilityUsed = DefaultMinionAI.UFOSpecialAbility;
					projFuncs.minionModifyColor = DefaultMinionAI.UFOModifyColor;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.XenoStaff);
					projFuncs.AddBuff(projectile, ProjectileBuffs.ProjectileBuffIDs.MartianSaucerBuff, projectile);
					break;
				case ProjectileID.StardustCellMinion:
					break;
				case ProjectileID.StardustDragon1:
					break;
				case ProjectileID.BatOfLight:
					projFuncs.minionOnSpecialAbilityUsed += SpecialAbility.BatOfLightOnSpecialAbility;
					projFuncs.minionPostAI += SpecialAbility.BatOfLighPostAI;
					projFuncs.minionOnHitNPC += SpecialAbility.BatOfLightOnHitNPC;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.SanguineStaff);
					break;
				case ProjectileID.EmpressBlade:
					break;
				case ProjectileID.StormTigerTier1:
				case ProjectileID.StormTigerTier2:
				case ProjectileID.StormTigerTier3:
					break;

				case ProjectileID.HoundiusShootius:
					projFuncs.minionOnSpecialAbilityUsed += DefaultMinionAI.HoundiusShootiusSpecialAbility;
					projFuncs.minionTerminateSpecialAbility += DefaultMinionAI.HoundiusShootiusTerminateSpecialAbility;
					projFuncs.minionPreAI += DefaultMinionAI.HoundiusShootiusPreAI;
					projFuncs.minionPostAI += DefaultMinionAI.HoundiusShootiusPostAI;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.HoundiusShootius);
					break;
				case ProjectileID.SpiderHiver:
					projFuncs.minionOnSpecialAbilityUsed += DefaultMinionAI.SpiderHiverSpecialAbility;
					projFuncs.minionOnShootProjectile += DefaultMinionAI.SpiderHiverOnShootProjectile;
					projData.maxEnergy = GetMinionPowerRechargeTime(ItemID.QueenSpiderStaff);
					break;

			}
		}

		static float GetMinionPowerRechargeTime(int ItemID)
		{
			if (ItemsAlreadyLoaded && ItemsLoadedStaticDefaults[ItemID])
				return ReworkMinion_Item.minionPowerRechargeTime[ItemID];
			return 0;
		}
		public static void ConfigRehookSourceItem(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
		{
			//special abilities
			HookSourceItem(projectile, projFuncs, projFuncs.OriginalSourceItem);
		}

		public static void HookSourceItem(Projectile projectile, ReworkMinion_Projectile projFuncs, int item)
		{
			projFuncs.OriginalSourceItem = item;
			projFuncs.SourceItemConfigOverride = BakedConfig.GetProjSourceItemType(projectile.type);
			if (projFuncs.SourceItem != -1)
			{
				DefaultSpecialAbility specialAbility = ReworkMinion_Item.defaultSpecialAbilityUsed[projFuncs.SourceItem];
				if (specialAbility != null)
					specialAbility.HookOnProjectileCreated(projectile, projFuncs);
			}
		}
		public static void ItemSetStaticDefaults()
		{
			ReworkMinion_Item.minionPowers = new minionPower[ItemLoader.ItemCount][];
			Func<Player, Vector2, Entity>[] specialAbilityFindTarget = new Func<Player, Vector2, Entity>[ItemLoader.ItemCount];
			Func<Item, ReworkMinion_Player, List<Projectile>>[] specialAbilityFindMinions = new Func<Item, ReworkMinion_Player, List<Projectile>>[ItemLoader.ItemCount];
			ReworkMinion_Item.specialAbilityFindTarget = specialAbilityFindTarget;
			ReworkMinion_Item.specialAbilityFindMinions = specialAbilityFindMinions;
			DefaultSpecialAbility[] defaultSpecialAbilityUsed = new DefaultSpecialAbility[ItemLoader.ItemCount];
			ReworkMinion_Item.defaultSpecialAbilityUsed = defaultSpecialAbilityUsed;

			int[] minionPowerRechargeTime = new int[ItemLoader.ItemCount];
			ReworkMinion_Item.minionPowerRechargeTime = minionPowerRechargeTime;

			if (ItemsLoadedStaticDefaults != null)
			{
				for (int x = 0; x < ItemLoader.ItemCount; x++)
				{
					if (ItemsLoadedStaticDefaults[x])
					{
						Item testItem = new Item();
						testItem.SetDefaults(x);
						ItemsLoadedStaticDefaults[x] = false;
						ItemSetDefaults(testItem);
					}
				}
			}
			else
				ItemsLoadedStaticDefaults = new MassBitsByte(ItemLoader.ItemCount);

			ItemsAlreadyLoaded = true;
			ModSupportItemStatics.queued.ForEach(i => i.Apply());
		}

		public static void ItemSetDefaults(Item item)
		{
			if (!ItemsAlreadyLoaded || ItemsLoadedStaticDefaults[item.type])
				return;
			ItemsLoadedStaticDefaults[item.type] = true;

			int[] minionPowerRechargeTime = ReworkMinion_Item.minionPowerRechargeTime;
			Func<Player, Vector2, Entity>[] specialAbilityFindTarget = ReworkMinion_Item.specialAbilityFindTarget;
			Func<Item, ReworkMinion_Player, List<Projectile>>[] specialAbilityFindMinions = ReworkMinion_Item.specialAbilityFindMinions;
			DefaultSpecialAbility[] defaultSpecialAbilityUsed = ReworkMinion_Item.defaultSpecialAbilityUsed;


			Projectile testProj = new();
			int x = item.type;
			minionPower[] minionPowers = new minionPower[2];

			ReworkMinion_Item.minionPowers[x] = minionPowers;
			bool specialActive = false;
			switch (x)
			{
				case ItemID.BabyBirdStaff:
					minionPowerRechargeTime[x] = 300;
					break;
				case ItemID.AbigailsFlower:
					minionPowerRechargeTime[x] = 600;
					break;
				case ItemID.SlimeStaff:
					minionPowerRechargeTime[x] = 300;
					break;
				case ItemID.FlinxStaff:
					minionPowerRechargeTime[x] = 480;
					break;
				case ItemID.VampireFrogStaff:
					minionPowerRechargeTime[x] = 3600;
					break;
				case ItemID.HornetStaff:
					minionPowerRechargeTime[x] = 1200;
					break;
				case ItemID.ImpStaff:
					minionPowerRechargeTime[x] = 300;
					break;
				case ItemID.SpiderStaff:
					minionPowerRechargeTime[x] = 300;
					break;
				case ItemID.PirateStaff:
					minionPowerRechargeTime[x] = 900;
					break;
				case ItemID.SanguineStaff:
					minionPowerRechargeTime[x] = 2400;
					break;
				case ItemID.Smolstar:
					minionPowerRechargeTime[x] = 1200;
					break;
				case ItemID.OpticStaff:
					minionPowerRechargeTime[x] = 600;
					break;
				case ItemID.PygmyStaff:
					minionPowerRechargeTime[x] = 300;
					break;
				case ItemID.DeadlySphereStaff:
					minionPowerRechargeTime[x] = 600;
					break;
				case ItemID.RavenStaff:
					minionPowerRechargeTime[x] = 600;
					break;
				case ItemID.TempestStaff:
					minionPowerRechargeTime[x] = 1800;
					break;
				case ItemID.XenoStaff:
					minionPowerRechargeTime[x] = 1800;
					break;
				case ItemID.StardustCellStaff:
					break;
				case ItemID.StardustDragonStaff:
					break;
				case ItemID.EmpressBlade:
					break;
				case ItemID.StormTigerStaff:
					break;


				case ItemID.HoundiusShootius:
					minionPowerRechargeTime[x] = 900;
					break;
				case ItemID.QueenSpiderStaff:
					minionPowerRechargeTime[x] = 300;
					break;
				case ItemID.StaffoftheFrostHydra:
					break;
				case ItemID.MoonlordTurretStaff:
					break;
				case ItemID.RainbowCrystalStaff:
					break;

				default:
					minionPowerRechargeTime[x] = 0;
					break;

			}

			if (BakedConfig.CustomSpecialPowersEnabled(x))
			{
				if (!specialActive)
					switch (x)
					{
						case ItemID.BabyBirdStaff:
							minionPowers[0] = minionPower.NewMP(10, roundingType: mpRoundingType.integer);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTargetPoint;
							specialAbilityFindMinions[x] = SpecialAbility.PreAbility_Toggle;
							specialActive = true;
							break;
						case ItemID.AbigailsFlower:
							minionPowers[0] = minionPower.NewMP(10);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTarget;
							specialAbilityFindMinions[x] = SpecialAbility.PreAbility_FindAllMinions;
							specialActive = true;
							break;
						case ItemID.SlimeStaff:
							minionPowers[0] = minionPower.NewMP(5);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTargetPoint;
							specialAbilityFindMinions[x] = SpecialAbility.PreAbility_FindClosestMinion;
							specialActive = true;
							break;
						case ItemID.FlinxStaff:
							minionPowers[0] = minionPower.NewMP(3);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTarget;
							specialAbilityFindMinions[x] = SpecialAbility.PreAbility_FindAllMinions;
							specialActive = true;
							break;
						case ItemID.VampireFrogStaff:
							minionPowers[0] = minionPower.NewMP(15);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTarget;
							specialAbilityFindMinions[x] = SpecialAbility.PreAbility_FindAllMinions;
							specialActive = true;
							break;
						case ItemID.HornetStaff:
							minionPowers[0] = minionPower.NewMP(2);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTarget;
							specialAbilityFindMinions[x] = SpecialAbility.PreAbility_FindAllMinions;
							specialActive = true;
							break;
						case ItemID.ImpStaff:
							minionPowers[0] = minionPower.NewMP(5, mpScalingType.multiply);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTarget;
							specialAbilityFindMinions[x] = SpecialAbility.PreAbility_Imp;
							specialActive = true;
							break;
						case ItemID.SpiderStaff:
							minionPowers[0] = minionPower.NewMP(10);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTarget;
							specialAbilityFindMinions[x] = SpecialAbility.PreAbility_FindClosestMinion;
							specialActive = true;
							break;
						case ItemID.PirateStaff:
							minionPowers[0] = minionPower.NewMP(40, roundingType: mpRoundingType.integer, DifficultyScale: true);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTargetPoint;
							specialAbilityFindMinions[x] = SpecialAbility.PreAbility_Pirate;
							specialActive = true;
							break;
						case ItemID.SanguineStaff:
							minionPowers[0] = minionPower.NewMP(50);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTarget;
							specialAbilityFindMinions[x] = SpecialAbility.PreAbility_FindAllMinions;
							specialActive = true;
							break;
						case ItemID.Smolstar:
							minionPowers[0] = minionPower.NewMP(25, roundingType: mpRoundingType.integer);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTarget;
							specialAbilityFindMinions[x] = SpecialAbility.PreAbility_FindAllMinions;
							specialActive = true;
							break;
						case ItemID.OpticStaff:
							minionPowers[0] = minionPower.NewMP(20, scalingType: mpScalingType.add, roundingType: mpRoundingType.integer);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTarget;
							specialAbilityFindMinions[x] = SpecialAbility.PreAbility_FindAllMinions;
							specialActive = true;
							break;
						case ItemID.PygmyStaff:
							minionPowers[0] = minionPower.NewMP(15, DifficultyScale: true);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTargetPoint;
							specialAbilityFindMinions[x] = SpecialAbility.PreAbility_Pygmy;
							specialActive = true;
							break;
						case ItemID.DeadlySphereStaff:
							minionPowers[0] = minionPower.NewMP(75, roundingType: mpRoundingType.integer);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTarget;
							specialAbilityFindMinions[x] = SpecialAbility.PreAbility_FindAllMinions;
							specialActive = true;
							break;
						case ItemID.RavenStaff:
							minionPowers[0] = minionPower.NewMP(5);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTargetPoint;
							specialAbilityFindMinions[x] = SpecialAbility.PreAbility_OneAtATime;
							specialActive = true;
							break;
						case ItemID.TempestStaff:
							minionPowers[0] = minionPower.NewMP(0.4f, mpScalingType.divide);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTargetPoint;
							specialAbilityFindMinions[x] = SpecialAbility.PreAbility_OneAtATime;
							specialActive = true;
							break;
						case ItemID.XenoStaff:
							minionPowers[0] = minionPower.NewMP(20);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTargetPoint;
							specialAbilityFindMinions[x] = SpecialAbility.PreAbility_FindAnyMinion;
							specialActive = true;
							break;
						case ItemID.StardustCellStaff:
							minionPowers[0] = minionPower.NewMP(15);
							break;
						case ItemID.StardustDragonStaff:
							minionPowers[0] = minionPower.NewMP(150, roundingType: mpRoundingType.integer);
							break;
						case ItemID.EmpressBlade:
							minionPowers[0] = minionPower.NewMP(10, roundingType: mpRoundingType.integer);
							break;
						case ItemID.StormTigerStaff:
							minionPowers[0] = minionPower.NewMP(150, roundingType: mpRoundingType.integer);
							break;


						case ItemID.HoundiusShootius:
							minionPowers[0] = minionPower.NewMP(3);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTargetPoint;
							specialAbilityFindMinions[x] = DefaultMinionAI.PreAbility_HoundiusShootius;
							specialActive = true;
							break;
						case ItemID.QueenSpiderStaff:
							minionPowers[0] = minionPower.NewMP(100);
							specialAbilityFindTarget[x] = SpecialAbility.FindSpecialAbilityTarget;
							specialAbilityFindMinions[x] = SpecialAbility.PreAbility_FindAllMinions;
							specialActive = true;
							break;
						case ItemID.StaffoftheFrostHydra:
							minionPowers[0] = minionPower.NewMP(10);
							break;
						case ItemID.MoonlordTurretStaff:
							minionPowers[0] = minionPower.NewMP(10);
							break;
						case ItemID.RainbowCrystalStaff:
							minionPowers[0] = minionPower.NewMP(10);
							break;


						case ItemID.BlandWhip:
							minionPowers[0] = minionPower.NewMP(5, roundingType: mpRoundingType.integer);
							break;
						case ItemID.ThornWhip:
							minionPowers[0] = minionPower.NewMP(100, roundingType: mpRoundingType.integer);
							break;
						case ItemID.BoneWhip:
							minionPowers[0] = minionPower.NewMP(30, roundingType: mpRoundingType.integer);
							break;
						case ItemID.FireWhip:
							minionPowers[0] = minionPower.NewMP(200, roundingType: mpRoundingType.integer);
							break;
						case ItemID.CoolWhip:
							minionPowers[0] = minionPower.NewMP(200, roundingType: mpRoundingType.integer);
							break;
						case ItemID.SwordWhip:
							minionPowers[0] = minionPower.NewMP(120, roundingType: mpRoundingType.integer);
							break;
						case ItemID.MaceWhip:
							minionPowers[0] = minionPower.NewMP(150, roundingType: mpRoundingType.integer);
							break;
						case ItemID.ScytheWhip:
							minionPowers[0] = minionPower.NewMP(70, roundingType: mpRoundingType.integer);
							break;
						case ItemID.RainbowWhip:
							minionPowers[0] = minionPower.NewMP(100, roundingType: mpRoundingType.integer);
							minionPowers[1] = minionPower.NewMP(200, roundingType: mpRoundingType.integer);
							break;

						case ItemID.DD2BallistraTowerT1Popper:
							minionPowers[0] = minionPower.NewMP(50);
							break;
						case ItemID.DD2BallistraTowerT2Popper:
							minionPowers[0] = minionPower.NewMP(60);
							break;
						case ItemID.DD2BallistraTowerT3Popper:
							minionPowers[0] = minionPower.NewMP(70);
							break;

						case ItemID.DD2LightningAuraT1Popper:
							minionPowers[0] = minionPower.NewMP(0.25f);
							break;
						case ItemID.DD2LightningAuraT2Popper:
							minionPowers[0] = minionPower.NewMP(0.35f);
							break;
						case ItemID.DD2LightningAuraT3Popper:
							minionPowers[0] = minionPower.NewMP(0.45f);
							break;

						case ItemID.DD2ExplosiveTrapT1Popper:
							minionPowers[0] = minionPower.NewMP(100);
							break;
						case ItemID.DD2ExplosiveTrapT2Popper:
							minionPowers[0] = minionPower.NewMP(125);
							break;
						case ItemID.DD2ExplosiveTrapT3Popper:
							minionPowers[0] = minionPower.NewMP(150);
							break;

						case ItemID.DD2FlameburstTowerT1Popper:
							minionPowers[0] = minionPower.NewMP(10);
							break;
						case ItemID.DD2FlameburstTowerT2Popper:
							minionPowers[0] = minionPower.NewMP(25);
							break;
						case ItemID.DD2FlameburstTowerT3Popper:
							minionPowers[0] = minionPower.NewMP(40);
							break;
						default:
							minionPowerRechargeTime[x] = 0;
							minionPowers[0] = minionPower.NewMP(0);
							break;

					}
			}

			if (DefaultSpecialAbility.Loaded && !specialActive)
			{
				testProj.SetDefaults(item.shoot);
				if (ReworkMinion_Item.IsSummon(item) || testProj.minion || testProj.sentry)
				{
					string totalString = item.Name;
					if (item.ModItem != null)
						totalString += item.ModItem.Mod.Name;
					totalString += current.DefaultAbilsSeed.ToString();
					Random deterministic = new Random(compute_hash(totalString));

					DefaultSpecialAbility abil = DefaultSpecialAbility.GetSpecial(item, testProj, deterministic);
					defaultSpecialAbilityUsed[x] = abil;
					abil.GetRandomMinionPower(minionPowers, deterministic);

					specialAbilityFindMinions[x] = SpecialAbility.PreAbility_EmptyList;

				}
				BakedConfig.SetConfigMinionPower(x);
			}
		}
		static int compute_hash(string s) {
			const int p = 31;
			const double m = 1e9 + 9;
			double hash_value = 0;
			double p_pow = 1;
			foreach (char c in s) {
				hash_value = (hash_value + (c - 'a' + 1) * p_pow) % m;
				p_pow = (p_pow * p) % m;
			}
			return (int)hash_value;
		}

		public static void ProjectileSetStaticDefaults()
		{
			ModSupportProjectiles = new ModSupportProjectile[ProjectileLoader.ProjectileCount];
			ProjectilesAlreadyLoaded = true;
			ModSupportProjectile.ModSupportProjectiles.ForEach(i => ModSupportProjectiles[i.ProjectileID] = i);
		}

		public static void BuffSetStaticDefaults()
		{
			ModSupportBuffs = new ModSupportBuff[BuffLoader.BuffCount];
			BuffsAlreadyLoaded = true;
			ModSupportBuff.ModSupportBuffs.ForEach(i => ModSupportBuffs[i.BuffID] = i);
		}

		public static void ModSupport_HookProjectile(int projID, int hookNum, object hook)
		{
			ModSupportProjectile newModSupportProj = ModSupportProjectile.Generate(projID);
			newModSupportProj.AssignHook(hookNum, hook);

			if (ProjectilesAlreadyLoaded)
				ModSupportProjectiles[projID] = newModSupportProj;
		}
		public static void ModSupport_HookBuff(int buffID, int hookNum, object hook)
		{
			ModSupportBuff newModSupportBuff = ModSupportBuff.Generate(buffID);
			newModSupportBuff.AssignHook(hookNum, hook);

			if (BuffsAlreadyLoaded)
				ModSupportBuffs[buffID] = newModSupportBuff;
		}

		public static void ModSupport_AddSpecialPowerDisplayData(object func) {
			ModdedGetDisplayData.Add((Func<int, int, Tuple<Texture2D, Rectangle>>)func);
		}

		public static void ModSupport_AddItemStatics(int summonItemID, Func<Player, Vector2, Entity> specialAbilityFindTarget, Func<Player, Item, List<Projectile>, List<Projectile>> specialAbilityFindMinions, Tuple<float, int, int, bool>[] minionPowers, int minionPowerRechargeTime, bool specialActive)
		{
			ModSupportItemStatics newStats = new ModSupportItemStatics(summonItemID, specialAbilityFindTarget, specialAbilityFindMinions, minionPowers, minionPowerRechargeTime, specialActive);

			ModSupportItemStatics.queued.Add(newStats);
			if (ItemsAlreadyLoaded)
				newStats.Apply();
		}

		public static void ModSupport_HookDefaultSpecialHandler(Mod mod, int hookNum, object hook)
		{
			ModSupportDefaultSpecialAbility handler = ModSupportDefaultSpecialAbility.Generate(mod);
			handler.AssignHook(hookNum, hook);
		}
	}
}
