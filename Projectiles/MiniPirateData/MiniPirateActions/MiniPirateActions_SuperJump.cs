using Microsoft.Xna.Framework;
using SummonersShine.DataStructures;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace SummonersShine.Projectiles.MiniPirateData
{
    public partial class MiniPirate
    {
        class MiniPirateActions_SuperJump : MiniPirateActions
        {
            public MiniPirateActions next;
            public MiniPirateActions peak;
            Parabola parabola;
            float boostDuration;
            float halfDuration;
            float jumpDuration;
            float deboostDuration;
            int lastState = 0;
            public override int Urgent => 1;

            bool fullLight = false;
            public override bool drawFullLight => fullLight;
            public MiniPirateActions_SuperJump(MiniPirateActions next, MiniPirateActions peak, float jumpHeight, float boostDuration, float jumpDuration, Vector2 relativePos)
            {
                this.next = next;
                this.peak = peak;
                this.jumpDuration = boostDuration + jumpDuration;
                this.halfDuration = boostDuration + jumpDuration * 0.5f;
                this.boostDuration = boostDuration;
                this.deboostDuration = boostDuration + jumpDuration + boostDuration;
                parabola = Parabola.CreateParabola(new Vector2(this.boostDuration, relativePos.Y), new Vector2(this.jumpDuration, relativePos.Y), jumpHeight);
            }
            public override MiniPirateActions Update(MiniPirate pirate, float simRate)
            {
                Player player = Main.player[pirate.boat.Projectile.owner];
                int newState = 0;
                if (ActionDuration >= deboostDuration)
                {
                    pirate.relativePosition.Y = parabola.CalculateY(jumpDuration);
                    GeneralMiniPirateUpdate(pirate, simRate, 1, 1, MiniPirateFrames.SuperSlam, MiniPirateFrames.SuperSlam);
                    return next;
                }
                else if (ActionDuration > jumpDuration)
                {
                    newState = 3;
                    GeneralMiniPirateUpdate(pirate, simRate, 1, 1, MiniPirateFrames.SuperSlam, MiniPirateFrames.SuperSlam);
                    Dust dust = Dust.NewDustDirect(pirate.feet + new Vector2(Main.rand.NextFloat(8, -8), 0), 1, 1, DustID.Smoke);
                    dust.velocity *= 0.1f;
                    dust.velocity += pirate.parentEntity.velocity;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                }
                else if (ActionDuration > boostDuration)
                {
                    pirate.relativePosition.Y = parabola.CalculateY(Math.Min(ActionDuration, jumpDuration));
                    if (ActionDuration > halfDuration)
                    {
                        newState = 2;
                        if (lastState != newState)
                        {
                            pirate.relativePosition.Y = parabola.CalculateY(halfDuration);
                            GeneralMiniPirateUpdate(pirate, simRate, 1, 5, MiniPirateFrames.SuperDiveStart, MiniPirateFrames.SuperDiveEnd);
                            lastState = 2;
                            fullLight = true;
                            return peak;
                        }
                        GeneralMiniPirateUpdate(pirate, simRate, 1, 5, MiniPirateFrames.SuperDiveStart, MiniPirateFrames.SuperDiveEnd);
                    }
                    else
                    {
                        newState = 1;
                        GeneralMiniPirateUpdate(pirate, simRate, 1, 5, MiniPirateFrames.SuperJumpStart, MiniPirateFrames.SuperJumpEnd);
                        Dust dust = Dust.NewDustDirect(pirate.feet, 1, 1, DustID.Smoke);
                        dust.velocity *= 0.01f;
                        dust.velocity += pirate.parentEntity.velocity;
                        dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                    }
                    fullLight = true;
                    Dust dust2 = Dust.NewDustDirect(pirate.feet, 1, 1, DustID.GoldFlame);
                    dust2.velocity *= 0.01f;
                    dust2.velocity += pirate.parentEntity.velocity;
                    dust2.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                    Lighting.AddLight(pirate.feet, TorchID.Yellow);
                }
                else
                {
                    GeneralMiniPirateUpdate(pirate, simRate, 1, 1, MiniPirateFrames.Boost, MiniPirateFrames.Boost);
                    Dust dust = Dust.NewDustDirect(pirate.feet + new Vector2(Main.rand.NextFloat(8, -8), 0), 1, 1, DustID.Smoke);
                    dust.velocity *= 0.1f;
                    dust.velocity += pirate.parentEntity.velocity;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                }
                if (lastState != newState)
                {
                    lastState = newState;
                    switch (lastState)
                    {
                        case 0:
                            pirate.frame = (int)MiniPirateFrames.Boost;
                            break;
                        case 1:
                            pirate.frame = (int)MiniPirateFrames.SuperJumpStart;
                            break;
                        case 2:
                            pirate.frame = (int)MiniPirateFrames.SuperDiveStart;
                            break;
                        case 3:
                            pirate.frame = (int)MiniPirateFrames.SuperSlam;
                            break;
                    }
                    pirate.frameCounter = 0;
                }
                return this;
            }
        }
        
    }
}
