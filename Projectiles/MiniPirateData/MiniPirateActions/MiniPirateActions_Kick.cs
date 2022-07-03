using Microsoft.Xna.Framework;
using SummonersShine.SpecialData;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace SummonersShine.Projectiles.MiniPirateData
{
    public partial class MiniPirate
    {
        class MiniPirateActions_Kick : MiniPirateActions
        {
            Entity entToBoot;
            float travelTime, bootTime;
            MiniPirateActions next;
            bool init = false;
            bool kicked = false;
            int sign;
            Vector2 startPoint;

            MiniPiratePreDraw_Trail trail;

            public override int Urgent => 1;

            public override MiniPirateActions ForceKill()
            {
                return next;
            }

            public MiniPirateActions_Kick(Entity entToBoot, MiniPirateActions next, float travelTime, float bootTime)
            {
                this.travelTime = travelTime;
                this.bootTime = bootTime + travelTime;
                this.next = next;
                this.entToBoot = entToBoot;
            }
            public override MiniPirateActions Update(MiniPirate pirate, float simRate)
            {
                if (entToBoot == null || !entToBoot.active)
                    return next;
                if (!init)
                {
                    pirate.frame = (int)MiniPirateFrames.KickStart;
                    pirate.Decouple();
                    init = true;
                    trail = MiniPiratePreDraw_Trail.New(pirate);
                    startPoint = pirate.feet;
                }
                if (ActionDuration < travelTime)
                {
                    Vector2 endPos = entToBoot.Center;
                    Vector2 diff = endPos - pirate.feet;
                    Player player = Main.player[pirate.boat.Projectile.owner];

                    sign = diff.X < 0 ? -1 : 1;
                    float target = travelTime - ActionDuration;
                    float ratio = simRate / target;
                    if (ratio > 1)
                        ratio = 1;
                    ratio = MathHelper.SmoothStep(0, 1, ratio);

                    Dust dust = Dust.NewDustDirect(pirate.Center, 1, 1, DustID.GoldFlame);
                    Vector2 step = diff * ratio;
                    dust.velocity *= 0.01f;
                    dust.velocity += step * 0.1f;
                    dust.noGravity = true;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);

                    pirate.relativePosition += step;

                    GeneralMiniPirateUpdate_AbsFacing(pirate, simRate, sign, 5, MiniPirateFrames.KickStart, MiniPirateFrames.KickEnd);
                }
                else if (ActionDuration < bootTime)
                {
                    Player player = Main.player[pirate.boat.Projectile.owner];
                    if (!kicked)
                    {
                        Projectile proj = entToBoot as Projectile;

                        int targetIndex = pirate.AnyTargets();
                        Vector2 direction = (pirate.Center - startPoint + new Vector2(0, -4));
                        NPC target = null;
                        if (targetIndex != -1)
                        {
                            target = Main.npc[targetIndex];
                            Vector2 diff = target.Center - entToBoot.Center;
                            direction = diff;

                            if (target != null)
                            {
                                direction /= PirateStat.kickTime;
                            }
                        }
                        else if (direction != Vector2.Zero)
                        {
                            direction.Normalize();
                            direction *= 160 / PirateStat.kickTime;
                        }

                        KickEffect(player, pirate.Center + direction * 2, direction);
                        if (proj != null)
                        {
                            ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
                            MinionProjectileData stats = projFuncs.GetMinionProjData();
                            stats.castingSpecialAbilityTime = 0;
                            stats.specialCastPosition = direction;
                            if (target != null)
                            {
                                proj.ai[0] = 0;
                                stats.specialCastTarget = target;
                            }
                        }
                        else
                            entToBoot.velocity = direction;
                        kicked = true;
                    }
                    GeneralMiniPirateUpdate_AbsFacing(pirate, simRate, sign, 5, MiniPirateFrames.KickStart, MiniPirateFrames.KickEnd);
                }
                else
                {
                    trail.Kill();
                    return next;
                }
                return this;
            }

            void KickEffect(Player player, Vector2 origin, Vector2 direction)
            {
                for (int x = 0; x < 3; x++)
                {
                    Vector2 pos = origin + Main.rand.NextVector2Circular(8, 8);
                    Dust dust = Dust.NewDustDirect(pos, 1, 1, DustID.Smoke);
                    dust.velocity *= 0.01f;
                    dust.velocity += direction.RotatedBy(Main.rand.NextFloat(-1, 1)) * Main.rand.NextFloat(0, 0.05f);
                    dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                }
            }
        }
        
    }
}
