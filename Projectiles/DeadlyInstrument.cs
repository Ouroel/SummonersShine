using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.Projectiles
{
    public class DeadlyInstrument : ModProjectile
    {
        const int deadlyInstrumentSpeed = 32;
        const int deadlyInstrumentSpeedSqr = deadlyInstrumentSpeed * deadlyInstrumentSpeed;
        Entity attachedEntity { get { return ModUtils.GetNPCProjOrPlayer_ID((int)Projectile.ai[0]); } set { Projectile.ai[0] = ModUtils.ConvertToGlobalEntityID(value); } }
        ReworkMinion_Projectile ProjFuncs;
        public override void SetStaticDefaults()
        {
            ProjectileModIDs.DeadlyInstrument = Projectile.type;
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.CanDistortWater[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.scale = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.frame = Main.rand.Next(0, 4);
            ProjFuncs = Projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            Projectile.DamageType = DamageClass.Summon;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            return true;
        }

        public override void Kill(int timeLeft)
        {
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (attachedEntity == null)
                return null;
            return target == attachedEntity;
        }
        public override bool CanHitPvp(Player target)
        {
            return target == attachedEntity;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.ArmorPenetration += (int)(Projectile.damage * (1 - ProjFuncs.GetMinionPower(Projectile, 0) * 0.02f));
            }
            if (attachedEntity != null)
            {
                NPC entNPC = attachedEntity as NPC;
                if (!attachedEntity.active || (entNPC != null && !entNPC.CanBeChasedBy(Projectile)))
                    attachedEntity = null;
            }
            if (attachedEntity != null)
            {
                Vector2 diff = attachedEntity.Center - Projectile.Center;
                if (diff.LengthSquared() > deadlyInstrumentSpeedSqr)
                {
                    diff.Normalize();
                    diff *= deadlyInstrumentSpeed;
                }
                Projectile.velocity = diff;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            int rand = Main.rand.Next(0, 3);
            short id;
            switch (rand)
            {
                case 0:
                    id = DustID.Electric;
                    break;
                case 1:
                    id = DustID.GoldFlame;
                    break;
                default:
                    id = DustID.CursedTorch;
                    break;
            }
            Dust dust;
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                for (int x = 0; x < 10; x++)
                {
                    Vector2 dustVel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.1f, 0.3f);
                    
                    dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, id, dustVel.X, dustVel.Y);
                    dust.noGravity = true;
                }
            }
            dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, id);
            dust.noGravity = true;
            dust.velocity *= 0.1f;
            dust = Dust.NewDustDirect(Projectile.position + Projectile.velocity * 0.5f, Projectile.width, Projectile.height, id);
            dust.noGravity = true;
            dust.velocity *= 0.1f;
        }
    }
}
