using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace SummonersShine.Effects
{
    public class PoisonPuff : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.behindTiles = false;
            gore.alpha = 255;
            gore.timeLeft = 1;
            gore.Frame = new SpriteFrame(1, 3, 0, (byte)Main.rand.Next(0, 3));
        }
        public override bool Update(Gore gore)
        {
            gore.frameCounter++;
            float sine = MathF.Sin(gore.frameCounter * 0.0524f);
            gore.alpha = (int)(255 * sine * sine * 0.4f + 102);
            gore.scale = MathF.Sin((0.25f + (float)gore.frameCounter * 0.0251f) * MathF.PI) * 0.7f;
            if (gore.frameCounter >= 30)
            {
                gore.active = false;
            }
            return false;
        }
    }
}
