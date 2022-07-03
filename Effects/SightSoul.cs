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
{    public class SightSoul : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLightEmittence = false;
            dust.frame = new Rectangle(0, 0, 18, 18);
            dust.customData = 0;
            dust.noGravity = true;
            dust.customData = 0;
        }
        public override bool Update(Dust dust)
        {
            if ((int)dust.customData % 5 == 0)
            {
                dust.frame.Y += 18;
            }
            if (dust.frame.Y == 108)
            {
                dust.frame.Y = 0;
            }
            if ((int)dust.customData > 15)
                dust.alpha += 15;
            dust.velocity.Y *= 1.05f;
            dust.position += dust.velocity;
            dust.rotation = dust.velocity.ToRotation() + MathF.PI * 0.5f;
            if (dust.alpha > 255)
                dust.active = false;
            dust.customData = (int)dust.customData + 1;
            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color rvColor = Color.White * (1 - dust.alpha * 0.003922f);
            rvColor.A = (byte)(rvColor.A * 0.7f);
            return rvColor;
        }
    }
}
