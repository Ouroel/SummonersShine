using Microsoft.Xna.Framework;
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
        public static void MoonlordTurretPostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (projFuncs.killedTicks > 0)
            {
                Player player = Main.player[projectile.owner];
                HandleFadeInOut(projectile, projFuncs, projData, true, true);
				//animate
				projectile.rotation -= (float)projectile.direction * 6.2831855f / 120f;
				Lighting.AddLight(projectile.Center, new Vector3(0.3f, 0.9f, 0.7f) * projectile.Opacity);
				if (Main.rand.Next(2) == 0)
				{
					Vector2 vector176 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
					Dust dust180 = Main.dust[Dust.NewDust(projectile.Center - vector176 * 30f, 0, 0, DustID.Vortex, 0f, 0f, 0, default(Color), 1f)];
					dust180.noGravity = true;
					dust180.position = projectile.Center - vector176 * (float)Main.rand.Next(10, 21);
					dust180.velocity = vector176.RotatedBy(1.5707963705062866, default(Vector2)) * 6f;
					dust180.scale = 0.5f + Main.rand.NextFloat();
					dust180.fadeIn = 0.5f;
					dust180.customData = projectile.Center;
				}
				if (Main.rand.Next(2) == 0)
				{
					Vector2 vector177 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
					Dust dust181 = Main.dust[Dust.NewDust(projectile.Center - vector177 * 30f, 0, 0, DustID.Granite, 0f, 0f, 0, default(Color), 1f)];
					dust181.noGravity = true;
					dust181.position = projectile.Center - vector177 * 30f;
					dust181.velocity = vector177.RotatedBy(-1.5707963705062866, default(Vector2)) * 3f;
					dust181.scale = 0.5f + Main.rand.NextFloat();
					dust181.fadeIn = 0.5f;
					dust181.customData = projectile.Center;
				}
				if (projectile.ai[0] < 0f)
				{
					Vector2 center14 = projectile.Center;
					int num940 = Dust.NewDust(center14 - Vector2.One * 8f, 16, 16, DustID.Vortex, projectile.velocity.X / 2f, projectile.velocity.Y / 2f, 0, default(Color), 1f);
					Dust dust182 = Main.dust[num940];
					Dust dust3 = dust182;
					dust3.velocity *= 2f;
					Main.dust[num940].noGravity = true;
					Main.dust[num940].scale = Utils.SelectRandom<float>(Main.rand, new float[]
					{
																																0.8f,
																																1.65f
					});
					Main.dust[num940].customData = projectile;
				}
			}
        }
    }
}
