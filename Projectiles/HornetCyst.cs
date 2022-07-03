using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.Projectiles
{
    public class HornetCyst : ModProjectile
    {
        const int oneByte = 4096;
        public Vector2 CenterOffset
        {
            get
            {
                int rawData = (int)Projectile.ai[1];
                int yOffset = rawData % oneByte;
                int xOffset = (rawData - yOffset) / oneByte;
                if (yOffset % 2 == 1)
                {
                    yOffset -= 1;
                    yOffset = -yOffset;
                }
                yOffset /= 2;
                if (xOffset % 2 == 1)
                {
                    xOffset -= 1;
                    xOffset = -xOffset;
                }
                xOffset /= 2;
                return new Vector2(xOffset, yOffset);
            }
            set
            {
                int xVal = (int)value.X * 2;
                if (xVal < 0) {
                    xVal = -xVal;
                    xVal += 1;
                }

                int yVal = (int)value.Y * 2;
                if (yVal < 0)
                {
                    yVal = -yVal;
                    yVal += 1;
                }

                Projectile.ai[1] = xVal * oneByte + yVal;
            }
        }
        public Entity LatchTarget { get => ModUtils.GetNPCProjOrPlayer_ID((int)Projectile.ai[0]); set => Projectile.ai[0] = ModUtils.ConvertToGlobalEntityID(value); }

        public override void SetStaticDefaults()
        {
            ProjectileModIDs.HornetCyst = Projectile.type;
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.knockBack = 2f;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool? CanDamage()
        {
            return LatchTarget == null;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            General_OnHit(target, damage, crit, target.spriteDirection, target.rotation);
            /*
            Projectile.penetrate = -1;
            LatchTarget = target;
            int maxCircle = target.width;
            if (maxCircle > target.height)
                maxCircle = target.height;
            maxCircle /= 2;
            maxCircle = (int)(maxCircle * Main.rand.NextFloat(0.25f, 1));
            int inversion = target.spriteDirection == -1 ? -1 : 1;
            Vector2 centerOffset = -Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(target.rotation * inversion) * maxCircle * inversion;
            CenterOffset = centerOffset;
            Projectile.timeLeft = Projectile.originalDamage;
            Projectile.netUpdate = true;*/
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            General_OnHit(target, damage, crit, target.direction);
        }

        void General_OnHit(Entity target, int damage, bool crit, int dir, float rot = 0)
        {
            Projectile.penetrate = -1;
            LatchTarget = target;
            int maxCircle = target.width;
            if (maxCircle > target.height)
                maxCircle = target.height;
            maxCircle /= 2;
            maxCircle = (int)(maxCircle * Main.rand.NextFloat(0.25f, 1));
            int inversion = dir == -1 ? -1 : 1;
            Vector2 centerOffset = -Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(rot * inversion) * maxCircle * inversion;
            CenterOffset = centerOffset;
            Projectile.timeLeft = Projectile.originalDamage;
            Projectile.netUpdate = true;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return LatchTarget == null;
        }

        public override bool ShouldUpdatePosition()
        {
            return LatchTarget == null;
        }
        public override void AI()
        {
            if (Projectile.localAI[1] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item171, Projectile.position);
                Projectile.localAI[1]++;
                LatchTarget = null;
            }
            else if (Projectile.localAI[1] < 30f)
                Projectile.localAI[1]++;
            bool latched = LatchTarget != null;
            int baseFrame = latched ? 2 : 0;
            int endFrame = latched ? 5 : 1;
            Projectile.frameCounter++;
            if (Projectile.frameCounter == 60)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame == endFrame)
                    Projectile.frame = baseFrame;
                else
                    Projectile.frame++;
            }
            Dust dust;
            float rot = 0;
            if (latched)
            {
                Entity target = LatchTarget;
                if (!target.active)
                {
                    Projectile.timeLeft = 0;
                }
                else
                {
                    float targetRot;
                    int inversion;
                    target.GetEntityRotSpriteDirection(out targetRot, out inversion);
                    rot += targetRot;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.localAI[0]++;
                        if (Projectile.localAI[0] >= 2)
                        {
                            Projectile.localAI[0] = 0;
                            Player player = Main.player[Projectile.owner];

                            bool should = Main.rand.NextBool(30);
                            if (should)
                            {
                                int numBees = Main.rand.Next(0, 3);
                                for (int x = 0; x < numBees; x++)
                                {
                                    Projectile bee = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(4, 4), player.beeType(), player.beeDamage(Projectile.damage), player.beeKB(Projectile.knockBack), Main.myPlayer);
                                    bee.usesLocalNPCImmunity = true;
                                    bee.localNPCHitCooldown = -1;
                                    bee.netUpdate = true;
                                }
                            }
                        }
                    }
                    Vector2 centerOffset = CenterOffset;
                    Projectile.Center = target.Center + (centerOffset * inversion).RotatedBy(-targetRot * inversion);
                    if (Main.rand.Next(0, 30) == 0)
                    {
                        dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Honey + Main.rand.Next(0, 2));
                        dust.velocity = Projectile.velocity * 0.5f;
                    }
                }
            }
            else
            {
                if (Main.rand.NextBool((int)(31 - Projectile.localAI[1]) / 10 + 1))
                    for (int x = 0; x < 3; x++)
                    {
                        dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Honey + Main.rand.Next(0, 2));
                        dust.velocity = Projectile.velocity * -0.25f;
                    }
            }
            rot += Projectile.velocity.ToRotation();
            Projectile.rotation = rot;
        }
    }
}
