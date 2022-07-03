using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.Projectiles
{
    public class ImpSuperFireball : ModProjectile
    {
        public int BlastRad { get => (int)Projectile.ai[1] * 16; set => Projectile.ai[1] = value; }
        //public float HomeTarget { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }
        bool updateVel = false;
        bool exploding = false;
        public override string Texture => ModUtils.GetEmptyTexture();

        public override void SetStaticDefaults()
        {
            ProjectileModIDs.ImpSuperFireball = Projectile.type;
            Main.projFrames[Projectile.type] = 1;
            //ProjectileID.Sets.CountsAsHoming[Projectile.type] = true;
            ProjectileID.Sets.CanDistortWater[Projectile.type] = true;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 5;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hide = true;
            Projectile.knockBack = 2f;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool? CanDamage()
        {
            return true;
        }
        public override bool? CanCutTiles()
        {
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.position += Projectile.velocity;
            Explode();
            return true;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (!exploding)
                return;
            hitbox.X -= BlastRad;
            hitbox.Y -= BlastRad;
            hitbox.Width += BlastRad;
            hitbox.Height += BlastRad;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (!exploding)
                return null;
            if((target.Center - Projectile.Center).LengthSquared() > BlastRad * BlastRad)
                return false;
            return null;
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * 1.25f);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
            if (!exploding)
            {
                Explode();
            }
        }

        void Explode()
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            exploding = true;
            int penetrate = Projectile.penetrate;
            Projectile.penetrate = -1;
            Projectile.Damage();
            Player player = Main.player[Projectile.owner];
            for (int x = 0; x < 20; x++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.SolarFlare, 0.2f, 0.2f, Scale: 2);
                dust.noGravity = true;
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                dust.position += new Vector2(Main.rand.NextFloat(0, BlastRad), 0).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
            }
            Projectile.penetrate = penetrate;
            exploding = false;
        }
        public override void AI()
        {
            if(updateVel)
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 8;
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
                Projectile.localAI[0]++;
            }
            Player player = Main.player[Projectile.owner];
            if (Projectile.velocity != Vector2.Zero)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, Scale: 2);
                dust.noGravity = true;
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                dust.velocity *= 0.3f;
                dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SolarFlare, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, Scale: 1);
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            else {
                Explode();
                Projectile.Kill();
            }
        }
    }
}
