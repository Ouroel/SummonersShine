using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;

namespace SummonersShine.Effects
{
    public abstract class TwinsAssembler : ModGore
    {
        Gore lights;
        public virtual int direction
        {
            get { return 1; }
        }
        public virtual int lightsID
        {
            get { return ModEffects.Spazmatiassembler_Lights; }
        }
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.behindTiles = false;
            lights = Main.gore[Gore.NewGore(source, gore.position, gore.velocity, lightsID, 1f)];
        }

        public override bool Update(Gore gore)
        {
            lights.rotation = gore.rotation;
            float sineParameter = gore.frameCounter * 0.0278f;
            float sine = MathF.Sin(MathF.Sin(sineParameter));
            if (sineParameter > Math.PI * 0.5f)
                sine = 1;
            gore.velocity = Vector2.UnitX.RotatedBy(gore.rotation) * direction * sine;
            gore.alpha = (int)(255 * sineParameter);
            if (gore.frameCounter >= 60)
            {
                gore.active = false;
            }
            gore.frameCounter++;
            return true;
        }
    }

    public class Retinassembler : TwinsAssembler {
        public override int direction
        {
            get { return -1; }
        }
        public override int lightsID
        {
            get { return ModEffects.Retinassembler_Lights; }
        }
    }
    public class Spazmatiassembler : TwinsAssembler { }

    public abstract class TwinsAssembler_Lights : ModGore
    {
        public virtual int direction
        {
            get { return 1; }
        }
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.behindTiles = false;
        }

        public override Color? GetAlpha(Gore gore, Color lightColor)
        {
            float alpha = 1 - gore.alpha * 0.015f;
            return Color.White * alpha;
        }
        public override bool Update(Gore gore)
        {
            gore.alpha++;
            if (gore.alpha > 67)
                gore.active = false;
            float sineParameter = gore.frameCounter * 0.0278f;
            float sine = MathF.Sin(MathF.Sin(sineParameter));
            if (sineParameter > Math.PI * 0.5f)
                sine = 1;
            gore.velocity = Vector2.UnitX.RotatedBy(gore.rotation) * direction * sine;
            gore.frameCounter++;
            return true;
        }
    }
    public class Retinassembler_Lights : TwinsAssembler_Lights {
        public override int direction
        {
            get { return -1; }
        }
    }
    public class Spazmatiassembler_Lights : TwinsAssembler_Lights { }
}
