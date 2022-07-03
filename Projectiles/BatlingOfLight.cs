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
    public class BatlingOfLight : ModProjectile
    {
        public Entity target
        {
            get
            {
                return ModUtils.GetNPCProjOrPlayer_ID((int)Projectile.ai[0]);
            }
            set
            {
                Projectile.ai[0] = ModUtils.ConvertToGlobalEntityID(value);
            }
        }
        public Entity bat
        {
            get
            {
                return ModUtils.GetNPCProjOrPlayer_ID((int)Projectile.originalDamage);
            }
            set
            {
                Projectile.originalDamage = ModUtils.ConvertToGlobalEntityID(value);
            }
        }
        float time
        {
            get
            {
                return (int)(Projectile.ai[1] / 2);
            }
            set
            {
                Projectile.ai[1] = value * 2 + Projectile.ai[1] % 2;
            }
        }
        bool canLifeleech { get => Projectile.ai[1] % 2 == 1; }
        public override void SetDefaults()
        {
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = 120;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.DamageType = DamageClass.Summon;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override void SetStaticDefaults()
        {
            ProjectileModIDs.BatlingOfLight = Projectile.type;
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return time < 31 && target == this.target;
        }
        public override bool CanHitPvp(Player target)
        {
            return time < 31 && target == this.target;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            time = 31;
        }
        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            time = 31;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            return true;
        }
        public override void AI()
        {
            Projectile.frameCounter = (Projectile.frameCounter + 1) % 3;
            if (Projectile.frameCounter == 0)
                Projectile.frame = (Projectile.frame + 1) % 4;
            Entity moveTarget;
            Vector2 sourcePos;
            Player player = Main.player[Projectile.owner];
            float segTime;
            if (time > 30) {
                moveTarget = player;
                sourcePos = target.Center;
                segTime = time - 30;
            }
            else {
                moveTarget = target;
                sourcePos = bat.Center;
                segTime = time;
            }

            Vector2 dir = moveTarget.Center - sourcePos;

            Vector2 normal = new Vector2(dir.Y, -dir.X);
            if (Vector2.Dot(normal, Projectile.Center - sourcePos) < 0)
                normal = -normal;

            Projectile.Center = sourcePos + dir * segTime / 30 + normal * MathF.Sin(segTime * MathF.PI / 30) * 0.1f;
            if (Main.rand.Next(0, 10) == 0)
                Dust.NewDust(Projectile.Center, 1, 1, DustID.BloodWater);

            if (time != 30)
                time++;
            if (time == 60)
            {
                if (Main.rand.NextBool(6) && canLifeleech)
                {
                    int heal = Main.rand.Next(1, 4);
                    player.HealEffect(heal);
                    player.statLife = Math.Min(player.statLife + heal, player.statLifeMax2);
                }
                Projectile.Kill();
            }
            if(target.active == false && time < 31)
                Projectile.Kill();
        }
    }
}
