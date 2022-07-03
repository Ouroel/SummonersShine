using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace SummonersShine.Effects
{
    public class Gem : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLightEmittence = false;
            dust.frame = new Rectangle(0, 0, 14, 14);
            dust.customData = 0;
            dust.noLight = false;
        }
        public override bool Update(Dust dust)
        {
            if (dust.alpha % 2 == 0)
            {
                dust.frame.Y = 14 * Main.rand.Next(0, 4);
            }
            if (dust.alpha == 255) {
                dust.active = false;
            }
            dust.velocity.Y += 0.15f;
            dust.alpha++;
            return true;
        }
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color * (1 - dust.alpha * 0.003922f);
        }
    }
}
