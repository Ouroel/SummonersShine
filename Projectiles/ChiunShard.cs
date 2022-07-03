using Microsoft.Xna.Framework;
using SummonersShine.SpecialAbilities;
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

namespace SummonersShine.Projectiles
{
    public class ChiunShard : CustomSpecialAbilitySourceMinion
    {
        Vector2 newVelAdd;
        Entity targetStuckIn
        {
            get => ModUtils.GetNPCProjOrPlayer_ID((int)Projectile.ai[0]);
            set => Projectile.ai[0] = ModUtils.ConvertToGlobalEntityID(value);
        }
        int chargeTime
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override void SetStaticDefaults()
        {
            ProjectileModIDs.ChiunShard = Projectile.type;
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.CanDistortWater[Projectile.type] = true;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override float minionSlots => 0;
        public override void SetDefaults()
        {
            //Projectile.extraUpdates = 0;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.frame = Main.rand.Next(0, 4);
            Projectile.timeLeft = 480;
            Projectile.tileCollide = false;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Summon;

            base.SetDefaults();

            ProjFuncs.LimitedLife = true;
        }
        public override MinionTracking_State trackingState => MinionTracking_State.normal;
        public override float minionTrackingImperfection => 0;
        public override float minionTrackingAcceleration => 10f;
        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool? CanDamage()
        {
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            newVelAdd = Projectile.velocity - oldVelocity;
            return false;
        }
        public override void minionOnMovement(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (newVelAdd == Vector2.Zero)
                return;
            Projectile.velocity += newVelAdd;
            newVelAdd = Vector2.Zero;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (chargeTime != 17)
                return false;
            return target == targetStuckIn;
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            damage = Main.DamageVar(Math.Min(target.statDefense, 500) * (1 + Projectile.originalDamage * 0.02f), Main.player[Projectile.owner].luck);
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = Main.DamageVar(target.checkArmorPenetration(500) * (1 + Projectile.originalDamage * 0.02f), Main.player[Projectile.owner].luck);
        }

        int GetChiunShardParticleCount(int chargeTime) {
            if (chargeTime > 15) return 0;
            chargeTime = chargeTime % 5;
            if (chargeTime == 0 || chargeTime == 4)
                return 8;
            return 1;
        }

        void ChiunSpray(Player player, Vector2 center, Vector2 vel, int enemyArmor)
        {
            vel.Normalize();
            vel *= 10;
            int numParticles = 5 + Math.Min(enemyArmor / 2, 20);
            for (int x = 0; x < numParticles; x++)
            {
                {
                    Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.BlueCrystalShard, Scale: 1.5f);
                    dust.velocity += vel;
                    dust.noGravity = true;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                }
                {
                    Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.PinkCrystalShard, Scale: 1.5f);
                    dust.velocity += vel;
                    dust.noGravity = true;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                }
                {
                    Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.PurpleCrystalShard, Scale: 1.5f);
                    dust.velocity += vel;
                    dust.noGravity = true;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                }
            }
        }
        void ChiunLerp(Player player, Vector2 center, Vector2 vel, int numCount)
        {
            if (numCount == 0)
                return;

            if (Main.rand.Next(5) == 0)
                for (int x = 1; x < numCount + 1; x++)
                {
                    Vector2 pos = center + vel * x / numCount;
                    {
                        Dust dust = Dust.NewDustDirect(pos, 1, 1, DustID.BlueCrystalShard);
                        dust.velocity *= 0.5f;
                        dust.noGravity = true;
                        dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                    }
                    {
                        Dust dust = Dust.NewDustDirect(pos, 1, 1, DustID.PinkCrystalShard);
                        dust.velocity *= 0.5f;
                        dust.noGravity = true;
                        dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                    }
                    {
                        Dust dust = Dust.NewDustDirect(pos, 1, 1, DustID.PurpleCrystalShard);
                        dust.velocity *= 0.5f;
                        dust.noGravity = true;
                        dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                    }
                }
        }

        public override void AI()
        {
            Entity target = targetStuckIn;
            if (Projectile.localAI[0] == 0)
            {
                Projectile.ArmorPenetration = 0;
                Projectile.localAI[0] = 1;
            }

            if (target != null)
            {
                bool nullTarget = !target.active;
                NPC targetNPC = target as NPC;
                if (targetNPC != null && !targetNPC.CanBeChasedBy(Projectile))
                    nullTarget = true;
                if (nullTarget)
                {
                    target = null;
                }
            }
            if (target == null)
            {
                Projectile.velocity.Y += 0.1f;
                Projectile.rotation += Projectile.velocity.X;
                if (Main.rand.Next(160) == 0)
                    Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.BlueCrystalShard);
                if (Main.rand.Next(160) == 0)
                    Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.PinkCrystalShard);
                if (Main.rand.Next(160) == 0)
                    Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.PurpleCrystalShard);
                int newNPC = Projectile.RandomMinionTarget();
                if (newNPC != -1)
                    targetStuckIn = Main.npc[newNPC];
                else
                    Projectile.tileCollide = true;
                ProjData.trackingState = MinionTracking_State.noTracking;
            }
            else
            {
                Projectile.tileCollide = false;
                ReworkMinion_Projectile.SetMoveTarget(Projectile, target);
                Vector2 diff = target.Center - Projectile.Center;
                
                if (chargeTime == 0)
                {
                    Projectile.spriteDirection = -Projectile.spriteDirection;
                }
                chargeTime++;
                int sign = Projectile.spriteDirection;
                float progress;
                Vector2 disp = new Vector2(0, -120) * sign;
                if (chargeTime < 5)
                {
                    disp = disp.RotatedBy(Math.PI * 0.66 * (0 + Projectile.frame));
                    progress = 1 / (5f - chargeTime);
                }
                else if (chargeTime < 10)
                {
                    disp = disp.RotatedBy(Math.PI * 0.66 * (1 + Projectile.frame));
                    progress = 1 / (10f - chargeTime);
                }
                else if (chargeTime < 15)
                {
                    disp = disp.RotatedBy(Math.PI * 0.66 * (2 + Projectile.frame));
                    progress = 1 / (15f - chargeTime);
                }
                else if (chargeTime < 18)
                {
                    disp = Vector2.Zero;
                    progress = 1 / (18f - chargeTime);
                }
                else
                {
                    disp = -disp.RotatedBy(Math.PI * 0.66 * (2 + Projectile.frame));
                    progress = 1 / (21f - chargeTime);

                    if (chargeTime >= 20f)
                    {
                        chargeTime = 0;
                    }
                }
                Projectile.velocity = (disp + diff) * progress;

                Player player = Main.player[Projectile.owner];

                if (chargeTime == 17) {
                    int armor = 0;
                    NPC targetNPC = target as NPC;
                    Player targetPlayer = target as Player;
                    if (targetNPC != null)
                        armor = targetNPC.checkArmorPenetration(500) * 2;
                    else if (targetPlayer != null)
                        armor = Math.Min(targetPlayer.statDefense, 500);
                    ChiunSpray(player, Projectile.Center + Projectile.velocity, Projectile.velocity / 4, armor);


                    int test = Projectile.RandomMinionTarget(target.whoAmI);
                    if (test != -1)
                        targetStuckIn = Main.npc[test];
                }

                Projectile.rotation += Projectile.velocity.X;
                int particleChance = GetChiunShardParticleCount(chargeTime);
                ChiunLerp(player, Projectile.Center, Projectile.velocity, particleChance);
                ProjData.trackingState = MinionTracking_State.normal;
            }
        }
    }
}
