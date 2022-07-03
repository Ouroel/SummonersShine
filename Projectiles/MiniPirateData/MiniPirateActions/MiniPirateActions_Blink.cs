using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace SummonersShine.Projectiles.MiniPirateData
{
    public partial class MiniPirate
    {
        class MiniPirateActions_Blink : MiniPirateActions
        {
            Entity newEnt;
            Vector2 endRelPos;

            public MiniPirateActions next;
            float chargeDuration, blinkDuration, flourishDuration;
            int lastState = 0;
            bool init = false;

            int sign;

            public override MiniPirateActions TransitionTarget => next;

            MiniPiratePreDraw_Trail trail;

            public override bool drawFullLight => true;

            public override MiniPirateActions ForceKill()
            {
                if (trail != null)
                    trail.Kill();
                return this;
            }
            public MiniPirateActions_Blink(Entity newEnt, Vector2 end, MiniPirateActions next, float chargeDuration, float blinkDuration, float flourishDuration)
            {
                this.newEnt = newEnt;
                endRelPos = end;
                this.next = next;
                this.chargeDuration = chargeDuration;
                this.blinkDuration = blinkDuration + chargeDuration;
                this.flourishDuration = this.blinkDuration + flourishDuration;
            }
            public override MiniPirateActions Update(MiniPirate pirate, float simRate)
            {
                if (!init)
                {
                    init = true;
                }
                if (newEnt == null || newEnt.active == false)
                    return next;

                Player player = Main.player[pirate.boat.Projectile.owner];
                Vector2 absDest = pirate.GetWorldPosFromRelativePos(endRelPos, newEnt);
                Vector2 diff = absDest - pirate.Center;

                int state = 1;


                if (ActionDuration >= blinkDuration)
                {
                    state = 3;
                    if (state != lastState)
                    {
                        pirate.SetToDesired();
                    }
                    GeneralMiniPirateUpdate(pirate, simRate, sign, 100, MiniPirateFrames.BlinkEnd, MiniPirateFrames.BlinkEnd);
                    if (ActionDuration >= flourishDuration)
                    {
                        trail.Kill();
                        return next;
                    }

                }
                else if (ActionDuration >= chargeDuration)
                {
                    sign = diff.X < 0 ? -1 : 1;
                    state = 2;
                    if (state != lastState)
                    {
                        pirate.Decouple();
                    }
                    float target = blinkDuration - ActionDuration;
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
                    GeneralMiniPirateUpdate_AbsFacing(pirate, simRate, sign, 100, MiniPirateFrames.BlinkTransition, MiniPirateFrames.BlinkTransition);
                }
                else
                {
                    sign = diff.X < 0 ? -1 : 1;
                    GeneralMiniPirateUpdate_AbsFacing(pirate, simRate, sign, 100, MiniPirateFrames.BlinkStart, MiniPirateFrames.BlinkStart);
                }

                if (state != lastState)
                {
                    switch (state)
                    {
                        case 1:
                            pirate.frame = (int)MiniPirateFrames.BlinkStart;
                            break;
                        case 2:
                            trail = MiniPiratePreDraw_Trail.New(pirate);
                            pirate.frame = (int)MiniPirateFrames.BlinkTransition;
                            break;
                        case 3:
                            pirate.frame = (int)MiniPirateFrames.BlinkEnd;
                            break;

                    }
                    lastState = state;
                }
                return this;
            }
        }
        
    }
}
