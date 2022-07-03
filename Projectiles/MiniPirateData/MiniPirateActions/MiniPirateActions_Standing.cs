using Terraria;

namespace SummonersShine.Projectiles.MiniPirateData
{
    public partial class MiniPirate
    {
        /* Mini Pirate Actions */

        class MiniPirateActions_Standing : MiniPirateActions
        {
            public MiniPirateActions_Standing(MiniPirate pirate)
            {
                LastEntitySpriteFacing = GetEntitySpriteDirection(pirate.parentEntity);
                pirate.frame = 0;
            }
            public override MiniPirateActions Update(MiniPirate pirate, float simRate)
            {
                //pirate.Move(GetWorldPosFromRelativePos(pirate));

                UpdateFrames(pirate, 10, MiniPirateFrames.StandStart, MiniPirateFrames.StandEnd, simRate);

                int newFacing = GetEntitySpriteDirection(pirate.parentEntity);
                if (newFacing != LastEntitySpriteFacing)
                    pirate.facing *= -1;
                LastEntitySpriteFacing = newFacing;

                ActionDuration += simRate;
                if (ActionDuration > 120 && Main.rand.Next(0, (int)(15 / simRate) + 1) == 0)
                    return new MiniPirateActions_Walking(pirate, Main.rand.Next(15, 120));
                return this;
            }
        }
        
    }
}
