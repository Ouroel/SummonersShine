using Microsoft.Xna.Framework;
using SummonersShine.DataStructures;
using System;

namespace SummonersShine.Projectiles.MiniPirateData
{
    public partial class MiniPirate
    {
        //jump from platform to platform
        class MiniPirateActions_Jumping_WithinEntity : MiniPirateActions
        {
            MiniPirateActions next;

            Parabola jumpParabola;
            float duration;
            float xStart;
            float xEnd;
            public override int Urgent => 1;

            public MiniPirateActions_Jumping_WithinEntity(MiniPirateActions next, Vector2 jumpStart, Vector2 jumpEnd, float boingHeight, float duration)
            {
                this.duration = duration;
                this.next = next;
                jumpParabola = Parabola.CreateParabola(new Vector2(0, jumpStart.Y), new Vector2(duration, jumpEnd.Y), boingHeight);
                xStart = jumpStart.X;
                xEnd = jumpEnd.X;
            }

            public override MiniPirateActions Update(MiniPirate pirate, float simRate)
            {
                int sign = Math.Sign(xEnd - xStart);
                pirate.relativePosition.X = MathHelper.Lerp(xStart, xEnd, ActionDuration / duration);
                pirate.relativePosition.Y = jumpParabola.CalculateY(Math.Min(ActionDuration, duration));

                GeneralMiniPirateUpdate(pirate, simRate, sign, 10, MiniPirateFrames.WalkStart, MiniPirateFrames.WalkEnd);

                if (ActionDuration >= duration) {
                    pirate.relativePosition.Y = jumpParabola.CalculateY(ActionDuration);
                    return next;
                }
                return this;
            }
        }
        
    }
}
