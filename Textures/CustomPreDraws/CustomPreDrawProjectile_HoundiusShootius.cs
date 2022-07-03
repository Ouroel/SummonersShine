using Microsoft.Xna.Framework;
using SummonersShine.MinionAI;
using SummonersShine.SpecialAbilities;
using System;
using Terraria;

namespace SummonersShine.Textures
{
    public class CustomPreDrawProjectile_HoundiusShootius : CustomProjectileDrawLayer
    {
        public int DrawData = 0;
        public Vector2 castPos;
        public Vector2 vel;
        public int alphaOverride = 255;
        public int minTime = 50;
        public bool terminated = false;
        public float glowLight = 0.6f;
        public float targetGlowLight = 0.6f;
        public CustomPreDrawProjectile_HoundiusShootius(Projectile proj, int width, int height) : base(proj, width, height) { }

        public override void Update(Projectile projectile)
        {
			Vector2 CustomOrigin = new Vector2(ModTextures.RuinsPillar.Width / 2, 112);
			if (terminated)
			{
				vel.Y += 0.13f;
				vel = DefaultMinionAI.HoundiusShootiusFloatyPlatform(pos + CustomOrigin, castPos, vel);
				pos += vel;
				if (minTime < alphaOverride)
				{
					alphaOverride -= 8;
					if (minTime >= alphaOverride)
						minTime = 255;
				}
				else
				{
					alphaOverride += 8;
				}
				if (alphaOverride > 255)
				{
					destroyThisNext = true;
					DefaultMinionAI.HoundiusShootiusSpecialSummonEffect(pos + CustomOrigin);
				}
			}
			else
			{
				alphaOverride = Math.Max(alphaOverride - 8, 0);
			}

			if (targetGlowLight > glowLight)
				glowLight += 0.005f;
			else if (targetGlowLight < glowLight)
				glowLight -= 0.005f;

		}
        public override void Draw(Projectile projectile)
        {
			Vector2 CustomOrigin = new Vector2(ModTextures.RuinsPillar.Width / 2, 112);
			if (!terminated)
			{
				pos = projectile.Bottom - CustomOrigin;
			}

			float black = (255 - alphaOverride) / 255f;
			Color color1 = Lighting.GetColor(new Point((int)pos.X / 16, (int)pos.Y / 16)) * black;
			Color color2 = Color.White * glowLight * black;
			color1.A = 255;
			color2.A = 255;
			bool flip = DrawData == 1;
			Vector2 drawOrigin = pos + CustomOrigin;
			ModTextures.JustDraw_Projectile(ModTextures.RuinsPillar, drawOrigin, 2, 0, 1, flip, color1, 0, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, true, CustomOrigin);
			ModTextures.JustDraw_Projectile(ModTextures.RuinsPillar, drawOrigin, 2, 1, 1, flip, color2, 0, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, true, CustomOrigin);

		}

		public void Terminate(Vector2 castPos, Vector2 vel)
        {
            terminated = true;
            proj = null;

            this.castPos = castPos;
            this.vel = vel;
        }
    }
}
