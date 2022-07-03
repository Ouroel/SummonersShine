using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace SummonersShine.Projectiles.MiniPirateData
{
    public partial class MiniPirate
    {
        class MiniPirateActions_BulletStorm : MiniPirateActions
        {
            const int missChance = 3;
            const float damageMod = 0.10f;

            public override int Urgent => 1;
            float duration;
            public MiniPirateActions next;
            int facing = 1;
            int soundCD = 0;
            bool firstTick = true;
            public override bool drawFullLight => true;
            public MiniPirateActions_BulletStorm(MiniPirateActions next, float duration)
            {
                this.duration = duration;
                this.next = next;
            }

            public override MiniPirateActions ForceKill()
            {
                return next;
            }
            public override MiniPirateActions Update(MiniPirate pirate, float simRate)
            {

                DastardlyDoubloon thisBoat = pirate.boat;
                Projectile thisBoatProjectile = thisBoat.Projectile;
                Player player = Main.player[thisBoatProjectile.owner];
                ArmorShaderData shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);

                if (Main.rand.Next(0, 5) == 0)
                    facing *= -1;
                GeneralMiniPirateUpdate(pirate, simRate, facing, 5, MiniPirateFrames.Gun1, MiniPirateFrames.Gun6);
                int x_ = 0;
                int bulletsThisTick = Main.rand.Next(0, 3);
                while (x_ < bulletsThisTick)
                {
                    NewBulletSpray(Main.rand.NextFloat(0, MathF.PI * 2), pirate, Main.rand.NextFloat(0.5f, 1.5f), shader);
                    x_++;
                }
                Lighting.AddLight(pirate.feet, TorchID.Yellow);

                Player owner = Main.player[thisBoatProjectile.owner];
                if (soundCD <= 0)
                {
                    SoundEngine.PlaySound(SoundID.Item41, pirate.Center);
                    soundCD = 5;
                }
                soundCD -= 1;

                int maxHitCount = 0;

                List<NPC> npcs = ModUtils.FindValidNPCsOrderedByDist(thisBoatProjectile, pirate.Center, 1400, true, pirate.Center);
                npcs.ForEach(i =>
                {
                    maxHitCount++;
                    if (maxHitCount >= 20)
                        return;
                    if (firstTick)
                    {
                        thisBoatProjectile.localNPCImmunity[i.whoAmI] = Main.rand.Next(0, 20);
                    }
                    if (thisBoatProjectile.localNPCImmunity[i.whoAmI] <= 0)
                    {
                        int hitCD = 5 + Main.rand.Next(-2, 2);
                        thisBoat.HitscanNPC(i, hitCD, damageMod);
                        BulletImpactEnemy(i.Center + new Vector2(Main.rand.NextFloat(-i.width / 3, i.width / 3), Main.rand.NextFloat(-i.height / 3, i.height / 3)), -(i.Center - pirate.Center), Main.rand.NextFloat(0.5f, 2), shader);
                    }
                });
                firstTick = false;

                if (ActionDuration >= duration)
                    return next;
                return this;
            }

            void NewBulletSpray(float angle, MiniPirate pirate, float velMult, ArmorShaderData shader)
            {
                Vector2 newOrigin = pirate.Center + new Vector2(Main.rand.NextFloat(10, 16), 0).RotatedBy(angle);
                Vector2 parentVel = Vector2.Zero;
                if (pirate.parentEntity != null)
                    parentVel = pirate.parentEntity.velocity;
                for (int x = 0; x < 5; x++)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(3, 5), 0).RotatedBy(angle + Main.rand.NextFloat(0.1f, -0.1f));
                    Dust dust = Dust.NewDustDirect(newOrigin, 1, 1, DustID.MartianSaucerSpark, 0, 0, 0, Color.White);
                    dust.position = newOrigin;
                    dust.velocity = vel * velMult + parentVel;
                    dust.noGravity = true;
                }
                for (int x = 0; x < 10; x++)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(2, 3), 0).RotatedBy(angle + Main.rand.NextFloat(0.2f, -0.2f));
                    Dust dust = Dust.NewDustDirect(newOrigin, 1, 1, DustID.SolarFlare, 0, 0, 0, Color.White);
                    dust.position = newOrigin;
                    dust.velocity = vel * velMult + parentVel;
                    dust.noGravity = true;
                    dust.shader = shader;
                }
                for (int x = 0; x < 3; x++)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(1, 2), 0).RotatedBy(angle + Main.rand.NextFloat(0.3f, -0.3f));
                    Dust dust = Dust.NewDustDirect(newOrigin, 1, 1, DustID.Smoke, 0, 0, 0, Color.LightGray);
                    dust.velocity = vel * velMult + parentVel;
                    dust.noGravity = true;
                    dust.shader = shader;
                }
                for (int x = 0; x < 3; x++)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(0.1f, 0.3f), 0).RotatedBy(angle + Main.rand.NextFloat(0.3f, -0.3f));
                    Dust dust = Dust.NewDustDirect(newOrigin, 1, 1, DustID.Smoke, 0, 0, 0, Color.DarkSlateGray);
                    dust.velocity = vel * velMult + parentVel;
                    dust.noGravity = true;
                    dust.shader = shader;
                }
            }

            void BulletImpactEnemy(Vector2 enemyPos, Vector2 dir, float power, ArmorShaderData shader)
            {

                float angle = dir.ToRotation();

                for (int x = 0; x < 5; x++)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(3, 5), 0).RotatedBy(angle + Main.rand.NextFloat(0.5f, -0.5f));
                    Dust dust = Dust.NewDustDirect(enemyPos, 1, 1, DustID.MarblePot, 0, 0, 0, Color.Gray);
                    dust.position = enemyPos;
                    dust.velocity = vel * power;
                    dust.noGravity = true;
                    dust.shader = shader;
                }
                for (int x = 0; x < 15; x++)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(2, 4), 0).RotatedBy(angle + Main.rand.NextFloat(1.3f, -1.3f));
                    Dust dust = Dust.NewDustDirect(enemyPos, 1, 1, DustID.MartianSaucerSpark, 0, 0, 0, Color.White);
                    dust.position = enemyPos;
                    dust.velocity = vel * power;
                    dust.noGravity = true;
                    dust.shader = shader;
                }
                for (int x = 0; x < 3; x++)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(1, 2), 0).RotatedBy(angle + Main.rand.NextFloat(0.5f, -0.5f));
                    Dust dust = Dust.NewDustDirect(enemyPos, 1, 1, DustID.SolarFlare, 0, 0, 0, Color.White);
                    dust.position = enemyPos;
                    dust.velocity = vel * power;
                    dust.noGravity = true;
                    dust.shader = shader;
                }
            }
        }
        
    }
}
