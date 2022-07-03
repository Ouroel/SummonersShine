using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SummonersShine.Projectiles.MiniPirateData;
using Terraria;

namespace SummonersShine.Projectiles.MiniPirateData
{
    public partial class MiniPirate
    {
        abstract partial class MiniPirateActions
        {
            public float ActionDuration = 0;
            public int LastEntitySpriteFacing = 0;
            public virtual int Urgent => 0;
            public virtual MiniPirateActions TransitionTarget => null;
            public abstract MiniPirateActions Update(MiniPirate pirate, float simRate);

            public virtual MiniPirateActions ForceKill() { return this; }

            public virtual bool drawFullLight => false;

            public void UpdateFrames(MiniPirate pirate, int frameDelay, MiniPirateFrames minFrame, MiniPirateFrames maxFrame, float simRate)
            {
                pirate.frameCounter += simRate;
                while (pirate.frameCounter > frameDelay)
                {
                    pirate.frame++;
                    pirate.frameCounter -= frameDelay;
                    if (pirate.frame >= ((int)maxFrame + 1))
                        pirate.frame = (int)minFrame;
                }
            }

            public Vector2 GetRelativePos(MiniPirate pirate)
            {
                Entity parentEntity = pirate.parentEntity;
                if (parentEntity == null)
                    return pirate.feet;
                Vector2 rv = pirate.feet - parentEntity.position;
                if (GetEntitySpriteDirection(parentEntity) == -1)
                    rv.X = parentEntity.width - rv.X;
                return rv;
            }

            public void GeneralMiniPirateUpdate(MiniPirate pirate, float simRate, int sign, int frameDelay, MiniPirateFrames minFrame, MiniPirateFrames maxFrame)
            {
                pirate.facing = sign * GetEntitySpriteDirection(pirate.parentEntity);

                UpdateFrames(pirate, frameDelay, minFrame, maxFrame, simRate);
                ActionDuration += simRate;
            }
            public void GeneralMiniPirateUpdate_AbsFacing(MiniPirate pirate, float simRate, int sign, int frameDelay, MiniPirateFrames minFrame, MiniPirateFrames maxFrame)
            {
                pirate.facing = sign;

                UpdateFrames(pirate, frameDelay, minFrame, maxFrame, simRate);
                ActionDuration += simRate;
            }
        }
    }
}
