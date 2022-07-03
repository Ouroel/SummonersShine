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
    public class ImpSigil : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.behindTiles = false;
            gore.timeLeft = 1;
            gore.rotation = Main.rand.NextFloat(MathF.PI * 2);
        }
        public override bool Update(Gore gore)
        {
            if (gore.frameCounter >= 15)
            {
                float sine = MathF.Sin((gore.frameCounter - 15) * 0.0262f);
                gore.alpha = (int)(255 * sine);
            }
            if (gore.frameCounter >= 75)
                gore.active = false;
            gore.frameCounter++;
            return false;
        }
    }
}
