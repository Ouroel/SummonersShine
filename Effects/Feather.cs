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
    public abstract class Feather : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLightEmittence = false;
            dust.frame = new Rectangle(0, 0, 16, 12);
            dust.customData = 0;
            dust.noLight = false;
        }
        public override bool Update(Dust dust)
        {
            int unboxed = (int)dust.customData;
            int frame = (int)dust.customData % 30;
            int direction = frame > 10 ? -1 : 1;
            if (frame % 5 == 0)
            {
                dust.frame.Y += 12 * direction;
            }
            if (unboxed == 60)
            {
                dust.active = false;
            }
            dust.velocity.X += -0.015f * direction;
            dust.velocity.Y += -0.01f;
            dust.customData = unboxed + 1;
            return true;
        }
    }

    public class RavenFeather : Feather { }
    public class FinchFeather : Feather { }
}
