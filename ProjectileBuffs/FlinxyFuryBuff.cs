using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SummonersShine.ProjectileBuffs
{
    public abstract partial class ProjectileBuff
    {
        class FlinxyFuryBuff : ProjectileBuff
        {
            public override ProjectileBuffIDs ID => ProjectileBuffIDs.FlinxyFuryBuff;
            int max = 0;
            int interval = 0;
            Queue<Tuple<Vector2, float, int, int>> oldData = new();
            public override bool Update(Projectile projectile, ReworkMinion_Projectile projFuncs)
            {
                int maxInterval = (int)(0.25f * (projectile.extraUpdates + 1));
                if (maxInterval == 0 || interval % maxInterval == 0)
                {
                    oldData.Enqueue(new(projectile.position, projectile.rotation, projectile.spriteDirection, projectile.frame));
                    max++;
                    if (max > largestQueueSize)
                    {
                        oldData.Dequeue();
                        max = largestQueueSize;
                    }
                    interval = 0;
                }
                interval++;
                return true;
            }
            int largestQueueSize => 20;
            float drawOffsetX => 0f;
            float drawOriginOffsetX => 0f;
            float drawOriginOffsetY => -4f;

            float alphaStart => 0.5f;
            float alphaDecay => 0.001f;
            public override void PreDraw(Projectile projectile, ReworkMinion_Projectile projFuncs, Color color)
            {
                Tuple<Vector2, float, int, int>[] data = oldData.ToArray();
                Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
                for (int i = 0; i < data.Length; i++)
                {
                    Color newColor = projectile.GetAlpha(color) * (alphaStart - alphaDecay * i);
                    Tuple<Vector2, float, int, int> item = data[i];
                    Vector2 pos = item.Item1 - Main.screenPosition + new Vector2((float)projectile.width, (float)projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY + new Vector2(drawOffsetX, 0);
                    float rot = item.Item2;
                    int dir = item.Item3;
                    Rectangle frame = texture.Frame(1, Main.projFrames[projectile.type], 0, item.Item4, 0, 0);
                    Vector2 origin = frame.Size() * new Vector2(0.5f, 0.5f) + new Vector2(drawOriginOffsetX, -drawOriginOffsetY);
                    SpriteEffects effect = SpriteEffects.None;

                    if (dir == -1)
                        effect |= SpriteEffects.FlipHorizontally;
                    Main.EntitySpriteDraw(texture, pos, new Rectangle?(frame), newColor, rot, origin, 1, effect, 0);
                }
            }
        }
    }
}
