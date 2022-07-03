using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace SummonersShine.Effects
{
    public class ChiunStar : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.behindTiles = false;
            gore.rotation = Main.rand.NextFloat(MathF.PI * 2);
        }
        public override bool Update(Gore gore)
        {
            int current = gore.frameCounter % 10;
            gore.alpha = 255 - current * 25;
            if (current == 0)
            {
                gore.scale = Main.rand.NextFloat(0.5f, 1f);
                gore.rotation = Main.rand.NextFloat(0, MathF.PI * 2);
            }
            if (gore.frameCounter > 10)
                gore.active = false;
            gore.frameCounter++;
            return false;
        }

        public override Color? GetAlpha(Gore gore, Color lightColor)
        {
            return Color.White * (1 - gore.alpha * 0.003922f);
        }
    }
}
