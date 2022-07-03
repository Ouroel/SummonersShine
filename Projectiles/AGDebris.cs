using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.Projectiles
{
    public class AGDebris : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileModIDs.AGDebris = Projectile.type;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.frame = Main.rand.Next(0, 3);
            Projectile.extraUpdates = 4;
            Projectile.GetGlobalProjectile<ReworkMinion_Projectile>().ArmorIgnoredPerc = 0.66f;
            Projectile.DamageType = DamageClass.Summon;
        }

        int dura = 0;
        public override void AI()
        {
            if (Main.rand.Next(0, 64 - dura) == 0)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), 0, 0, DustID.Sand, 0, 0, 50, Color.SandyBrown, 1);
                dust.velocity *= 0.1f;
                dust.velocity.Y = -Projectile.velocity.Y * 0.5f;
            }
            if (Main.rand.Next(0, 64 - dura) == 0)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), 0, 0, DustID.Smoke, 0, 0, 50, Color.DimGray, 1);
                dust.velocity *= 0.1f;
                dust.velocity.Y = -Projectile.velocity.Y * 0.5f;
            }
            if (Main.rand.Next(0, 64 - dura) == 0)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), 0, 0, DustID.Sand, 0, 0, 50, Color.SaddleBrown, 1);
                dust.velocity *= 0.1f;
                dust.velocity.Y = -Projectile.velocity.Y * 0.5f;
            }
            Projectile.velocity.Y += 0.1f;
            if (!Projectile.tileCollide && Projectile.Center.Y > Projectile.ai[0] && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.tileCollide = true;
            }
            if (dura < 60)
                dura += 1;
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage /= 3;
            hitDirection = Main.rand.Next(-1, 2);
            knockback /= 4;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return true;
        }

        public override void Kill(int timeLeft)
        {
            for (int x = 0; x < 4; x++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), 0, 0, DustID.Sand, Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, -1), 50, Color.SandyBrown, 1);
                
            }
            for (int x = 0; x < 4; x++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), 0, 0, DustID.Smoke, Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, -1), 50, Color.DimGray, 1);
                
            }
            for (int x = 0; x < 4; x++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), 0, 0, DustID.Sand, Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, -1), 50, Color.SaddleBrown, 1);
                
            }
        }
    }
}
