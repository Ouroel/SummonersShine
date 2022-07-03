using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SummonersShine.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace SummonersShine.BakedConfigs
{
    public class BloodtaintModeSky : CustomSky
    {
        private bool isActive;
        private float intensity = 0;
        private float brightness = 180;

        Asset<Texture2D> BloodtaintMoon;
        Asset<Texture2D> BloodtaintMoonBG;

        public override void OnLoad()
        {
            BloodtaintMoon = ModContent.Request<Texture2D>("SummonersShine/BakedConfigs/BloodtaintMode/bloodmoon");
            BloodtaintMoonBG = ModContent.Request<Texture2D>("SummonersShine/BakedConfigs/BloodtaintMode/bloodmoonBG");
        }
        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                int brightness = (int)this.brightness;
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * intensity);

                spriteBatch.Draw(BloodtaintMoonBG.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(255, 255, 255) * intensity * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f));
                spriteBatch.Draw(BloodtaintMoon.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(brightness, brightness, brightness) * intensity);
            }
        }
        public override float GetCloudAlpha()
        {
            return 1 - intensity;
        }

        public override bool IsActive()
        {
            return isActive || intensity > 0f;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (isActive)
            {
                int WorldBrightness;
                if (Main.bloodMoon)
                    WorldBrightness = 220;
                else if (Main.eclipse)
                    WorldBrightness = 200;
                else if (Main.dayTime)
                    WorldBrightness = 130;
                else
                    WorldBrightness = 180;
                if (brightness < WorldBrightness)
                    brightness += 0.1f;
                else if (brightness > WorldBrightness)
                    brightness -= 0.1f;
                if (intensity < 1f)
                    intensity += 0.01f;
            }
            else if (intensity > 0f)
            {
                intensity -= 0.01f;
            }
        }
    }
}
