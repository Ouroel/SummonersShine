using Microsoft.Xna.Framework;
using SummonersShine.Projectiles;
using SummonersShine.SpecialData;
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
		public static void SpiderHiverDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
		{
			if (projFuncs.killedTicks == 0)
			{
				projData.alphaOverride = projectile.alpha;
				Projectile newProj = CloneProjForDespawnEffect(projectile, projFuncs, projData, projectile.velocity);
				newProj.netUpdate = true;
				ReworkMinion_Projectile newProjFuncs = newProj.GetGlobalProjectile<ReworkMinion_Projectile>();
				MinionProjectileData newProjData = newProjFuncs.GetMinionProjData();
				newProj.timeLeft = newProj.GetFadeTime(newProjData);
				if (newProjData.alphaOverride == 0)
					newProjData.alphaOverride = 1;
				int num425 = 40;
				int num3;
				for (int num428 = 0; num428 < num425; num428 = num3 + 1)
				{
					int num429 = Dust.NewDust(projectile.position + Vector2.UnitY * 16f, projectile.width, projectile.height - 16, DustID.Venom, 0f, 0f, 100, default(Color), 1f);
					Main.dust[num429].scale = (float)Main.rand.Next(1, 10) * 0.1f;
					Main.dust[num429].noGravity = true;
					Main.dust[num429].fadeIn = 1.5f;
					Dust dust91 = Main.dust[num429];
					Dust dust3 = dust91;
					dust3.velocity *= 0.75f;
					num3 = num428;
				}
			}
		}
		public static void SpiderHiverPostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
		{
			if (ModUtils.IsCastingSpecialAbility(projData, ItemID.QueenSpiderStaff) && projData.castingSpecialAbilityTime != -1)
			{
				ModUtils.IncrementSpecialAbilityTimer(projectile, projFuncs, projData, 600, 1);
			}
			HandleFadeInOut(projectile, projFuncs, projData, false, true);
		}

		public static void SpiderHiverSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity _target, int specialType, bool fromServer)
		{
			projData.energy = 0;
			projData.castingSpecialAbilityTime = 0;
			projData.energyRegenRateMult = 0;
			projectile.timeLeft = 7200;
		}
		public static void SpiderHiverOnShootProjectile(Projectile projectile, Projectile source, ReworkMinion_Projectile sourceFuncs, MinionProjectileData sourceData)
		{
			const float SPIDER_HIVER_VENOM_MAX = 20;
			const float SPIDER_HIVER_VENOM_MAXDIST = 8 * SPIDER_HIVER_VENOM_MAX;
			const float SPIDER_HIVER_VENOM_VEL = SPIDER_HIVER_VENOM_MAXDIST / SPIDER_HIVER_VENOM_MAX / 300;

			if (ModUtils.IsCastingSpecialAbility(sourceData, ItemID.QueenSpiderStaff) && sourceData.castingSpecialAbilityTime != -1)
			{
				float distRatio = sourceFuncs.GetMinionPower(source, 0) / 100;
				int count = (int)(distRatio * SPIDER_HIVER_VENOM_MAX);
				float spacing = SPIDER_HIVER_VENOM_VEL;
				Vector2 vel = projectile.velocity;
				vel.SafeNormalize(Vector2.Zero);
				Vector2 ovipositor;
				switch (source.frame)
				{
					case 0:
						ovipositor = new(32, 4);
						break;
					case 1:
						ovipositor = new(28, -8);
						break;
					case 2:
						ovipositor = new(24, -13);
						break;
					case 3:
						ovipositor = new(12, -16);
						break;
					default:
						ovipositor = new(0, -20);
						break;
				}
				ovipositor.X *= source.spriteDirection;
				Vector2 spawnPoint = source.Center + ovipositor;
				for (int x = 10; x < count; x++)
				{
					float iters = spacing * x;
					int numTicks = (int)(iters / 16) + 1;
					iters /= numTicks;
					Vector2 inVel = vel * iters + new Vector2(0, -3) + Main.rand.NextVector2Circular(4f, 4f);
					Projectile.NewProjectile(projectile.GetSource_FromThis(), spawnPoint, inVel / numTicks, ProjectileModIDs.SpiderHiverVenom, source.damage, 0, source.owner, numTicks, Main.rand.NextFloat(0.05f, 0.15f));
				}
			}
		}
	}
}
