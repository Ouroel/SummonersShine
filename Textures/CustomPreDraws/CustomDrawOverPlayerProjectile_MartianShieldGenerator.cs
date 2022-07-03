using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SummonersShine.Projectiles;
using System;
using Terraria;

namespace SummonersShine.Textures
{
    public class CustomDrawOverPlayerProjectile_MartianShieldGenerator : CustomProjectileDrawLayer
    {
        const float MaxProgress = 15;
        int Progress = 0;
        public int ShieldOpacityProgress = 0;
        public float Osci = 0;
        const float MaxOsci = 120;
        float rot;
        bool flip1;
        bool flip2;

        const int MartianBubbleDims = 144;
        public CustomDrawOverPlayerProjectile_MartianShieldGenerator(Projectile proj) : base(proj, MartianBubbleDims, MartianBubbleDims, DrawType.AbovePlayers)
        {
            flip1 = Main.rand.NextBool(2);
            flip2 = Main.rand.NextBool(2);
        }

        public override void Draw(Projectile projectile)
        {
            float osci = MathF.Sin(Osci * MathF.PI * 2 / MaxOsci);
            float osci2 = MathF.Sin((Osci + MaxOsci / 3) * MathF.PI * 2 / MaxOsci);
            float alpha = 200 + osci * 10f;
            float scale = MathF.Sqrt(Progress / MaxProgress) * 0.95f;
            scale += osci * 0.05f;
            float scale2 = scale + osci2 * 0.05f;
            int extraOpacity = ShieldOpacityProgress * 5;
            ModTextures.JustDraw_Projectile(ModTextures.MartianBubble, center, 1, 0, scale, false, Color.White, alpha - extraOpacity, flip1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, rot: rot);
            ModTextures.JustDraw_Projectile(ModTextures.MartianBubble, center, 1, 0, scale2, false, Color.White, alpha - extraOpacity, flip2 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, rot: rot);
        }

        public override void Update(Projectile projectile)
        {
            if (ShieldOpacityProgress > 0)
                ShieldOpacityProgress--;
            if (projectile != null)
            {
                rot = projectile.rotation;
                center = projectile.Center;
                if (Progress < MaxProgress)
                    Progress++;
                Osci += 1;
                Osci %= MaxOsci;
            }
            else
            {
                if (Progress == 0)
                    destroyThisNext = true;
                else
                    Progress--;
            }
        }
    }
}
