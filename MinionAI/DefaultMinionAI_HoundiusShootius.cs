using Microsoft.Xna.Framework;
using SummonersShine.Effects;
using SummonersShine.Projectiles;
using SummonersShine.SpecialAbilities;
using SummonersShine.SpecialData;
using SummonersShine.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.MinionAI
{
	public static partial class DefaultMinionAI
	{
		public static void HoundiusShootiusDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
		{
			HoundiusShootiusTPEffect(projectile);
			HoundiusShootiusStatCollection collection = player.GetModPlayer<ReworkMinion_Player>().GetSpecialData<HoundiusShootiusStatCollection>();

			HoundiusShootiusStat stats = projFuncs.GetSpecialData<HoundiusShootiusStat>();
			collection.Remove(stats);
			collection.DeleteHoundiusShootius(projectile);

			//HoundiusShootiusSpecialSummonEffect(projectile.Bottom);
		}

		static void HoundiusShootiusTPEffect(Projectile projectile)
		{
			int num425 = 30;
			int num430 = 25;
			int num431 = 30;
			int num3;
			for (int num432 = 0; num432 < num425; num432 = num3 + 1)
			{
				int num433 = Dust.NewDust(projectile.Center - new Vector2((float)num430, (float)num431), num430 * 2, num431 * 2, DustID.Fireworks, 0f, 0f, 0, default(Color), 1f);
				Dust dust92 = Main.dust[num433];
				Dust dust3 = dust92;
				dust3.velocity *= 2f;
				Main.dust[num433].noGravity = true;
				dust92 = Main.dust[num433];
				dust3 = dust92;
				dust3.scale *= 0.5f;
				num3 = num432;
			}
		}

		public static List<Projectile> PreAbility_HoundiusShootius(Item item, ReworkMinion_Player player)
		{
			HoundiusShootiusStatCollection collection = player.GetSpecialData<HoundiusShootiusStatCollection>();
			if (!collection.AddMegaMinion(player.Player, false, item.GetSource_ItemUse(item)))
				return player.GetSpecialData<HoundiusShootiusStatCollection>().GetHoundiusShootiusFromValidList(SpecialAbility.PreAbility_FindAllMinions(item, player));
			item.GetGlobalItem<ReworkMinion_Item>().UsingSpecialAbility = true;
			return null;
		}
		public static void HoundiusShootiusSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity _target, int specialType, bool fromServer)
		{
			projData.energy = 0;
			if (projData.castingSpecialAbilityTime != -1)
			{
				HoundiusShootiusTerminateSpecialAbility(projectile, projFuncs, projData, null, null);
			}
			HoundiusShootiusSetDrawType(projData, projData.castingSpecialAbilityType);
			HoundiusShootiusTPEffect(projectile);
			projectile.Bottom = _target.position;
			HoundiusShootiusTPEffect(projectile);
			HoundiusShootiusSpecialSummonEffect(projectile.Bottom);

			InitFreshlyLoadedHoundiusShootiusSpecial(projectile, projFuncs, projData);
			projData.specialCastPosition = projectile.Bottom;
			projectile.velocity.X = 0;
			projectile.timeLeft = 7200;
			projData.energyRegenRateMult = 4;
		}
		public static void HoundiusShootiusTerminateSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player, ReworkMinion_Player playerFuncs)
		{
			Platform platform = projFuncs.hookedPlatform;
			if (platform != null)
			{
				platform.Destroy();
			}

			CustomProjectileDrawLayer drawy = CustomProjectileDrawLayer.Find(projectile);
			if (drawy != null)
				(drawy as CustomPreDrawProjectile_HoundiusShootius).Terminate(projData.specialCastPosition, projectile.velocity);
		}
		public static void HoundiusShootiusPreAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
		{
			if (!ModUtils.IsCastingSpecialAbility(projData, ItemID.HoundiusShootius)) return;

			SingleThreadExploitation.houndiusStoredVelX = projectile.velocity.X;

			int life = (int)(projFuncs.GetMinionPower(projectile, 0) * 60);
			if (!HoundiusShootiusIsCausingEarthquake(projData, life))
				return;

			List<NPC> validNPCs = new();
			ModUtils.FindValidNPCsAndDoSomething(projectile, (NPC found) => {
				validNPCs.Add(found);
				return false;
			});
			if (validNPCs.Count == 0)
				return;
			int attempts = 0;
			validNPCs.ForEach(found =>
			{
				attempts++;
				RainDebris(projectile, found);
			});
			validNPCs.Sort((i, j) => { if (i.life > j.life) return 1; if (i.life < j.life) return -1; return 0; });
			while (attempts < 5)
			{
				validNPCs.ForEach(found =>
				{
					if (attempts >= 5) return;
					attempts++;
					RainDebris(projectile, found);
				});
			}
		}

		static void RainDebris(Projectile projectile, NPC found)
		{
			if (Main.rand.Next(0, 30) == 0)
			{
				int height = Main.rand.Next(320, 640);
				float time = MathF.Sqrt(height / 0.1f) / 5;
				Vector2 fullVel = found.GetRealNPCVelocity() * time;
				Vector2 pos = found.Top;
				fullVel = ModUtils.GetTileCollideModifier(pos, fullVel);
				pos = pos + fullVel;
				Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), pos + new Vector2(Main.rand.Next(-found.width / 2, found.width / 2), -height), Vector2.Zero, ProjectileModIDs.AGDebris, projectile.damage, projectile.knockBack, projectile.owner, pos.Y);
			}
		}
		public static void HoundiusShootiusPostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
		{
			if (!ModUtils.IsCastingSpecialAbility(projData, ItemID.HoundiusShootius)) return;

			projectile.velocity.X = SingleThreadExploitation.houndiusStoredVelX;
			CustomPreDrawProjectile_HoundiusShootius postDrawProjectile = CustomProjectileDrawLayer.Find(projectile) as CustomPreDrawProjectile_HoundiusShootius;
			int life = (int)(projFuncs.GetMinionPower(projectile, 0) * 60);
			if (postDrawProjectile == null)
			{
				InitFreshlyLoadedHoundiusShootiusSpecial(projectile, projFuncs, projData);
			}
			else
			{
				int currentSpecialTimer = HoundiusShootiusGetSpecialTimer(projData);
				if (currentSpecialTimer == 0)
					postDrawProjectile.targetGlowLight = 0.6f;
				else if (currentSpecialTimer > life)
					postDrawProjectile.targetGlowLight = 0.3f;
				else
					postDrawProjectile.targetGlowLight = 1f;
			}

			projectile.velocity = HoundiusShootiusFloatyPlatform(projectile.Bottom, projData.specialCastPosition, projectile.velocity);

			HoundiusShootiusIncrementSpecialTimer(projData, projectile.IsOnRealTick(projData), life);
		}

		public static Vector2 HoundiusShootiusFloatyPlatform(Vector2 pos, Vector2 castPos, Vector2 vel)
		{

			Vector2 diff = castPos - pos;
			Vector2 origDiff = diff;
			float lenSqr = diff.LengthSquared();
			diff *= 0.05f;
			vel += diff;
			if (diff.Y < 8 && vel.Y < 0)
			{
				vel.Y -= 0.01f;
			}

			float dot = 0;
			if (lenSqr > 0)
			{
				dot = Vector2.Dot(origDiff, vel) / origDiff.LengthSquared();
			}
			Vector2 projected = origDiff * -dot;
			Vector2 nonprojected = vel - projected;
			vel -= nonprojected;
			vel += nonprojected * 0.99f;
			if (lenSqr > 16 * 16 && dot < 0)
			{
				vel += projected * 1.75f;
			}
			return vel;
		}
		const int HOUNDIUS_PLATFORM_DISP = 4;
		const int HOUNDIUS_PLATFORM_OFFX = -26;
		const int HOUNDIUS_PLATFORM_WIDTH = HOUNDIUS_PLATFORM_OFFX * -2;
		static void InitFreshlyLoadedHoundiusShootiusSpecial(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
		{
			CustomPreDrawProjectile_HoundiusShootius customDraw = new CustomPreDrawProjectile_HoundiusShootius(projectile, 80, 288);
			Vector2 CustomOrigin = new Vector2(ModTextures.RuinsPillar.Width / 2, 112);
			customDraw.pos = projectile.Bottom - CustomOrigin;
			customDraw.DrawData = HoundiusShootiusGetDrawType(projData);
			projFuncs.hookedPlatform = new Platform_AttachedToProj(projectile.Bottom + new Vector2(HOUNDIUS_PLATFORM_OFFX, HOUNDIUS_PLATFORM_DISP), HOUNDIUS_PLATFORM_WIDTH, projectile);
		}
		public static void HoundiusShootiusOnMovement(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
		{
			//movement independent of speed mods
			if (projFuncs.hookedPlatform != null)
				if (projData.currentTick == 1 && projectile.numUpdates < 0)
				{
					projFuncs.hookedPlatform.Move(projectile.Bottom + new Vector2(HOUNDIUS_PLATFORM_OFFX, HOUNDIUS_PLATFORM_DISP) - projFuncs.hookedPlatform.pos);
				}
		}
		public static void HoundiusShootiusSpecialSummonEffect(Vector2 castPos)
		{
			Vector2 vec1 = new Vector2(0, -32);
			Vector2 vec2 = new Vector2(0, -64);
			Vector2 vec3 = new Vector2(0, -96);
			for (int x = 0; x < 20; x++)
			{
				Dust dust = Dust.NewDustDirect(castPos + Main.rand.NextVector2Circular(32, 32), 0, 0, DustID.Smoke, 0, 0, 50, Color.DarkSlateGray, 1);
			}
			for (int x = 0; x < 20; x++)
			{
				Dust dust = Dust.NewDustDirect(castPos + vec1 + Main.rand.NextVector2Circular(32, 32), 0, 0, DustID.Smoke, 0, 0, 50, Color.DarkSlateGray, 1);
			}
			for (int x = 0; x < 20; x++)
			{
				Dust dust = Dust.NewDustDirect(castPos + vec2 + Main.rand.NextVector2Circular(32, 32), 0, 0, DustID.Smoke, 0, 0, 50, Color.DarkSlateGray, 1);
			}
			for (int x = 0; x < 20; x++)
			{
				Dust dust = Dust.NewDustDirect(castPos + vec3 + Main.rand.NextVector2Circular(32, 32), 0, 0, DustID.Smoke, 0, 0, 50, Color.DarkSlateGray, 1);
			}
		}
		

		const int max_drawType = 2;

		public static void HoundiusShootiusSetDrawType(MinionProjectileData projData, int drawType)
        {
			if (projData.castingSpecialAbilityTime == -1)
			{
				projData.castingSpecialAbilityTime = drawType;
				return;
			}
			else
			{
				projData.castingSpecialAbilityTime -= (projData.castingSpecialAbilityTime % max_drawType);
				projData.castingSpecialAbilityTime += drawType;
			}
        }
		public static int HoundiusShootiusGetDrawType(MinionProjectileData projData)
		{
			return projData.castingSpecialAbilityTime % max_drawType;
		}
		public static void HoundiusShootiusSetSpecialTimer(MinionProjectileData projData, int value)
		{
			int remainder = projData.castingSpecialAbilityTime % max_drawType;
			projData.castingSpecialAbilityTime = value * max_drawType + remainder;
		}

		public static int HoundiusShootiusGetSpecialTimer(MinionProjectileData projData)
		{
			return (projData.castingSpecialAbilityTime - (projData.castingSpecialAbilityTime % max_drawType)) / max_drawType;
		}
		public static bool HoundiusShootiusIsCausingEarthquake(MinionProjectileData projData, int life) {
			int value = HoundiusShootiusGetSpecialTimer(projData);
			return value > 0 && value <= life;
		}
		public static bool HoundiusShootiusIncrementSpecialTimer(MinionProjectileData projData, bool realTick, int life)
		{
			const int cooldown = 900;
			bool rv;
			int remainder = projData.castingSpecialAbilityTime % max_drawType;
			int getVar = (projData.castingSpecialAbilityTime - remainder) / max_drawType;
			if (getVar == 0)
				rv = false;
			else
			{
				rv = getVar <= life;
				if (realTick)
				{
					getVar++;
					if (getVar > life && getVar > cooldown)
						getVar = 0;
					projData.castingSpecialAbilityTime = getVar * max_drawType + remainder;
				}
			}
			return rv;
		}
	}
}
