using Microsoft.Xna.Framework;
using SummonersShine.Projectiles;
using System;
using Terraria;

namespace SummonersShine.Textures
{
    public class CustomPreDrawProjectile_NevermorePillar : CustomProjectileDrawLayer
    {
        bool flip;
        int rot;
        const int ShadowrazeRadius = 80;
        const int ShadowrazeWidth = ShadowrazeRadius + ShadowrazeRadius;
        public CustomPreDrawProjectile_NevermorePillar(Projectile proj, DrawType drawType = DrawType.BehindProjectiles) : base(proj, ShadowrazeWidth, ShadowrazeWidth, drawType)
        {
            pos = proj.Center - new Vector2(ShadowrazeRadius, ShadowrazeRadius);
            flip = Main.rand.NextBool(2);
            if (flip)
                rot = -1;
            else
                rot = 1;
        }

        public override void Draw(Projectile projectile)
        {
            const float cycle = 240;
            int progress1 = (int)(proj.localAI[1] % cycle);
            int progress2 = (int)((proj.localAI[1] + cycle / 2) % cycle);
            float startScale = 0.5f + 0.05f * MathF.Sin(progress1 * 2 * MathF.PI / cycle);
            float alphaRatio = NevermorePillar.GetAlphaRatio(proj.timeLeft);
            float remainder = 0.65f - startScale;
            ModTextures.JustDraw_Projectile(ModTextures.Shadowraze, proj.Center, 1, 0, startScale + remainder * (1 - MathF.Sin(MathF.PI * 2 * progress1 / cycle)), flip, Color.White, 255 - 255 * alphaRatio, rot: MathF.PI * 2 * progress1 / cycle * rot);
            ModTextures.JustDraw_Projectile(ModTextures.Shadowraze, proj.Center, 1, 0, startScale + remainder * (1 - MathF.Sin(MathF.PI * 2 * progress2 / cycle)), !flip, Color.White, 255 - 255 * alphaRatio, rot: MathF.PI * 4 * progress2 / cycle * -rot);
            ModTextures.JustDraw_Projectile(ModTextures.Shadowraze, proj.Center, 1, 0, startScale, flip, Color.White, 255 - 255 * alphaRatio, rot: MathF.PI * 8 * progress1 / cycle * rot);
        }

        public override void Update(Projectile projectile)
        {
        }
    }
}
