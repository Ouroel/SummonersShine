using Microsoft.Xna.Framework;
using Terraria;

namespace SummonersShine.SpecialData
{
    public class UFOSpecialDrawData : SpecialDataBase
    {
        public Vector2 location;
        public float progress = 0;

        public UFOSpecialDrawData() {
            location = Vector2.Zero;
        }
        public const float maxProgress = 30;
        public void LerpTarget(Vector2 TargetVel) {
            Vector2 diff = TargetVel - location;
            location += diff * progress / maxProgress;
        }
    }
}
