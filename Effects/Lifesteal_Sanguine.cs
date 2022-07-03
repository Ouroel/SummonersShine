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
    public class Lifesteal_Sanguine : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLightEmittence = false;
            dust.frame = new Rectangle(0, 0, 10, 20);
            dust.customData = 0;
            dust.noLight = false;
            dust.scale = 0;
            dust.customData = 0;
        }
        public override bool Update(Dust dust)
        {
            dust.alpha++;
            if (dust.alpha % 10 == 0)
            {
                dust.frame.X += 10;
            }
            if (dust.frame.X == 20)
            {
                dust.frame.X = 0;
            }
            if((int)dust.customData >= 30)
                dust.active = false;
            dust.scale = MathF.Sin((int)(dust.customData) * 0.0333f * MathF.PI);
            dust.velocity.Y += 0.15f;
            dust.customData = (int)dust.customData + 1;
            Lighting.AddLight(dust.position, TorchID.Crimson);
            return false;
        }
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color * (1 - dust.alpha * 0.003922f);
        }
    }
}