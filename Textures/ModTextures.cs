using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SummonersShine.Textures
{
    public static class ModTextures
    {
        public static Texture2D ThoughtBubble;
        public static Texture2D BuffMinionPowerGauge;
        public static Texture2D BuffMinionPowerPin;
        public static Texture2D BuffMinionPowerPinBottom;
        public static Texture2D PirateQueen;
        public static Texture2D TempestWind;
        public static Texture2D MagenChiunPlatform;
        public static Texture2D PygmyPlatform;
        public static Texture2D PygmyLivingTree;
        public static Texture2D MourningGlory;
        public static Texture2D RuinsPillar;
        public static Texture2D NevermorePillarSigils;
        public static Texture2D Shadowraze;
        public static Texture2D ShadowrazeSoul;
        public static Texture2D TractorBeam;
        public static Texture2D MartianBubble;

        public static void Populate()
        {
            if (!Main.dedServ)
            {
                ThoughtBubble = ModContent.Request<Texture2D>("SummonersShine/Textures/ThoughtBubble", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                BuffMinionPowerGauge = ModContent.Request<Texture2D>("SummonersShine/Textures/BuffMinionPowerGauge", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                BuffMinionPowerPin = ModContent.Request<Texture2D>("SummonersShine/Textures/BuffMinionPowerPin", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                BuffMinionPowerPinBottom = ModContent.Request<Texture2D>("SummonersShine/Textures/BuffMinionPowerPinBottom", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                PirateQueen = ModContent.Request<Texture2D>("SummonersShine/Textures/PirateQueen", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                TempestWind = ModContent.Request<Texture2D>("SummonersShine/Textures/TempestWind", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                MagenChiunPlatform = ModContent.Request<Texture2D>("SummonersShine/Textures/MagenChiunPlatform", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                PygmyPlatform = ModContent.Request<Texture2D>("SummonersShine/Textures/PygmyPlatform", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                PygmyLivingTree = ModContent.Request<Texture2D>("SummonersShine/Textures/PygmyLivingTree", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                MourningGlory = ModContent.Request<Texture2D>("SummonersShine/Textures/MourningGlory", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                RuinsPillar = ModContent.Request<Texture2D>("SummonersShine/Textures/RuinsPillar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                NevermorePillarSigils = ModContent.Request<Texture2D>("SummonersShine/Textures/NevermorePillarSigils", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Shadowraze = ModContent.Request<Texture2D>("SummonersShine/Textures/Shadowraze", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                ShadowrazeSoul = ModContent.Request<Texture2D>("SummonersShine/Textures/ShadowrazeSoul", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                TractorBeam = ModContent.Request<Texture2D>("SummonersShine/Textures/TractorBeam", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                MartianBubble = ModContent.Request<Texture2D>("SummonersShine/Textures/MartianBubble", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            }
        }

        public static void JustDraw_Player(Texture2D tex, Player attachedPlayer, Vector2 worldPos, int numFrames, int frame, float scale, ref PlayerDrawSet drawInfo, Color color, int shader = 0)
        {
            Rectangle rectangle = tex.Frame(1, numFrames, 0, frame, 0, 0);

            Vector2 origin = new Vector2(rectangle.Width * 0.5f, rectangle.Height * 0.5f);

            worldPos -= Main.screenPosition;

            SpriteEffects effect = SpriteEffects_ReverseGravityAndFlipHorizontally(attachedPlayer, ref origin, ref worldPos);

            DrawData newDrawData = new DrawData(tex, worldPos, new Rectangle?(rectangle), color, 0f, origin, scale, effect, 0);
            newDrawData.shader = shader;

            drawInfo.DrawDataCache.Add(newDrawData);
        }

        public static void JustDraw_Projectile(Texture2D tex, Vector2 worldPos, Rectangle frame, Vector2 scale, bool flip, Color lightColor, float alpha, SpriteEffects effect = SpriteEffects.None, bool UseCustomOrigin = false, Vector2 CustomOrigin = default(Vector2), float rot = 0)
        {
            lightColor *= (255 - alpha) * 0.00392157f;

            Vector2 projPos = worldPos - Main.screenPosition;
            Vector2 origin;

            if (UseCustomOrigin)
                origin = CustomOrigin;
            else
                origin = new Vector2(frame.Width / 2, frame.Height / 2);

            if (flip)
                effect |= SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(tex, projPos, frame, lightColor, rot, origin, scale, effect, 0);
        }
        public static void JustDraw_Projectile(Texture2D tex, Vector2 worldPos, int numFrames, int frame, float scale, bool flip, Color lightColor, float alpha, SpriteEffects effect = SpriteEffects.None, bool UseCustomOrigin = false, Vector2 CustomOrigin = default(Vector2), float rot = 0)
        {
            lightColor *= (255 - alpha) * 0.00392157f;

            Rectangle rectangle = tex.Frame(1, numFrames, 0, frame, 0, 0);

            Vector2 projPos = worldPos - Main.screenPosition;
            Vector2 origin;

            if (UseCustomOrigin)
                origin = CustomOrigin;
            else
                origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);

            if (flip)
                effect |= SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(tex, projPos, new Rectangle?(rectangle), lightColor, rot, origin, scale, effect, 0);
        }

        public static SpriteEffects SpriteEffects_ReverseGravityAndFlipHorizontally(Player player, ref Vector2 origin, ref Vector2 drawPos) {
            SpriteEffects effect = SpriteEffects.None;
            if (player != null)
            {
                if (player.gravDir == -1f)
                {
                    origin.Y = 0;
                    effect |= SpriteEffects.FlipVertically;
                    drawPos = Main.ReverseGravitySupport(drawPos, 0f);
                }
                if (player.direction == -1)
                    effect |= SpriteEffects.FlipHorizontally;
            }
            return effect;
        }
    }
}
