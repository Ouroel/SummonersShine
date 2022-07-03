using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SummonersShine.Projectiles;
using System;
using System.Collections.Generic;
using Terraria;

namespace SummonersShine.Textures
{
    public class CustomPreDrawProjectile_Shadowraze : CustomProjectileDrawLayer
    {
        public Vector2 castPos;
        public int timeLeft;
        public int radius;

        public float[] initialRot;
        public float[] rotSpeed;
        public bool[] flip;

        public bool[] npcFlip;
        public Vector2[] npcCenter;
        public float[] npcInitialRot;

        public int numSouls;

        public float[] soulInitialRot;
        public bool[] soulFlip;
        public float[] soulSpeed;

        public float[] soulAverageDist;
        public float[] soulSineDisp;
        public float[] soulDistSpeed;

        const int maxTime = 20;
        const float maxTimeFloat = maxTime;
        const float realRad = 80;
        public CustomPreDrawProjectile_Shadowraze(Vector2 position, int radius, int numSouls, List<NPC> npcs) : base(null, radius * 2, radius * 2)
        {
            castPos = position;
            timeLeft = maxTime;
            this.radius = radius;
            pos = position - new Vector2(radius, radius);

            initialRot = new float[]
            {
                Main.rand.NextFloat(0, MathF.PI * 2),
                Main.rand.NextFloat(0, MathF.PI * 2),
                Main.rand.NextFloat(0, MathF.PI * 2),
            };
            rotSpeed = new float[3]
            {
                Main.rand.NextFloat(0.5f, 1f),
                Main.rand.NextFloat(1f, 2f),
                Main.rand.NextFloat(1.5f, 3f),
            };
            flip = new bool[3]
            {
                Main.rand.NextBool(2),
                Main.rand.NextBool(2),
                Main.rand.NextBool(2),
            };
            npcFlip = new bool[npcs.Count];
            npcCenter = new Vector2[npcs.Count];
            npcInitialRot = new float[npcs.Count];

            this.numSouls = numSouls;
            int npcSoulCount = npcs.Count * numSouls;
            soulInitialRot = new float[npcSoulCount];
            soulFlip = new bool[npcSoulCount];
            soulSpeed = new float[npcSoulCount];

            soulAverageDist = new float[npcSoulCount];
            soulSineDisp = new float[npcSoulCount];
            soulDistSpeed = new float[npcSoulCount];

            for (int x = 0; x < npcs.Count; x++)
            {
                npcFlip[x] = Main.rand.NextBool(2);
                npcCenter[x] = npcs[x].Center;
                npcInitialRot[x] = Main.rand.NextFloat(0, MathF.PI * 2);
                for (int j = 0; j < numSouls; j++)
                {
                    int index = x * numSouls + j;
                    float totalScale = (j + 1) * 1.2f / numSouls;
                    totalScale = Main.rand.NextFloat(totalScale * 0.6f, totalScale);
                    soulInitialRot[index] = Main.rand.NextFloat(MathF.PI * 2);
                    soulFlip[index] = Main.rand.NextBool(2);
                    soulAverageDist[index] = Main.rand.NextFloat(totalScale);
                    soulSineDisp[index] = Main.rand.NextFloat(totalScale - soulAverageDist[index]);
                    soulDistSpeed[index] = Main.rand.NextFloat(0.2f, 2);
                    soulSpeed[index] = Main.rand.NextFloat(0.3f, 3f);
                }
            }
        }

        public override void Draw(Projectile projectile)
        {
            Rectangle rectangle = ModTextures.Shadowraze.Frame(1, 1, 0, 0, 0, 0);
            Vector2 rectCenter = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
            Rectangle rectangle2 = ModTextures.ShadowrazeSoul.Frame(1, 1, 0, 0, 0, 0);
            Vector2 rectCenter2 = new Vector2(rectangle2.Width / 2, rectangle2.Height / 2);
            Vector2 projPos = this.castPos - Main.screenPosition;
            float scale = this.radius / realRad;
            //ModTextures.JustDraw_Projectile(ModTextures.Shadowraze, this.castPos, 1, 0, (float)this.radius / realRad, false, Color.White, 0);
            float timeProgress = (this.timeLeft / maxTimeFloat);
            float sinProgress = MathF.Sin(timeProgress * MathF.PI);
            for (int x = 0; x < 3; x++)
            {
                float thisScale;
                float alpha;
                switch (x)
                {
                    case 0:
                        thisScale = 0.8f + 0.2f * sinProgress;
                        alpha = 255 - 128 * sinProgress;
                        break;
                    case 1:
                        thisScale = 0.3f + 0.2f * sinProgress;
                        alpha = 200 - 100 * sinProgress;
                        break;
                    default:
                        thisScale = 0.25f + 0.2f * sinProgress;
                        alpha = 100 - 50 * sinProgress;
                        break;
                }
                SpriteEffects effect;
                int rotDir;
                if (this.flip[x])
                {
                    effect = SpriteEffects.FlipHorizontally;
                    rotDir = -2;
                }
                else
                {
                    effect = SpriteEffects.None;
                    rotDir = 2;
                }

                Color color = Color.White * ((255 - alpha) * 0.00392157f);

                Main.EntitySpriteDraw(ModTextures.Shadowraze, projPos, rectangle, color, this.initialRot[x] + timeProgress * rotDir * this.rotSpeed[x] * -MathF.PI, rectCenter, scale * thisScale, effect, 0);
            }
            /*
            for (int x = 0; x < this.npcCenter.Length; x++)
            {
                float thisScale = 0.2f + 0.3f * sinProgress;
                float alpha = 50 - 25 * sinProgress;
                int rot;
                SpriteEffects effect;

                Color color = Color.White * ((255 - alpha) * 0.00392157f);

                if (this.npcFlip[x])
                {
                    effect = SpriteEffects.FlipHorizontally;
                    rot = -2;
                }
                else
                {
                    effect = SpriteEffects.None;
                    rot = 2;
                }

                Main.EntitySpriteDraw(ModTextures.Shadowraze, this.npcCenter[x] - Main.screenPosition, rectangle, color, this.npcInitialRot[x] + timeProgress * rot * -MathF.PI, rectCenter, thisScale, effect, 0);

                
            }*/
            for (int j = 0; j < this.numSouls; j++)
            {
                for (int x = 0; x < this.npcCenter.Length; x++)
                {
                    int index = x * this.numSouls + j;
                    float thisScale = this.soulAverageDist[index] + this.soulSineDisp[index] * MathF.Sin(timeProgress * MathF.PI * this.soulDistSpeed[index]);
                    float alpha = 50 - 25 * sinProgress;
                    float rot;
                    SpriteEffects effect;

                    Color color = Color.White * ((255 - alpha) * 0.00392157f);

                    if (this.soulFlip[index])
                    {
                        effect = SpriteEffects.FlipHorizontally;
                        rot = 1f;
                    }
                    else
                    {
                        effect = SpriteEffects.None;
                        rot = -1f;
                    }
                    Main.EntitySpriteDraw(ModTextures.ShadowrazeSoul, this.npcCenter[x] - Main.screenPosition, rectangle2, color, this.soulInitialRot[index] + timeProgress * this.soulSpeed[index] * rot * -MathF.PI, rectCenter2, thisScale * scale, effect, 0);

                }
            }
        }

        public override void Update(Projectile projectile)
        {
            this.timeLeft -= 1;
            this.radius = (int)(this.radius * 0.999f);
            if (this.timeLeft <= 0)
                this.destroyThisNext = true;
        }
    }
}
