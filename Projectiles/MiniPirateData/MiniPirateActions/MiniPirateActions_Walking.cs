using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;

namespace SummonersShine.Projectiles.MiniPirateData
{
    public partial class MiniPirate
    {
        class MiniPirateActions_Walking : MiniPirateActions
        {
            const float speed = 2;
            const float jumpDuration = 20;
            float desiredTime;

            List<PathfindingReturnData> pathfindingResult;
            float targetX;
            Vector2 startPoint;
            Vector2 endPoint;

            MiniPirate pirate;

            public override MiniPirateActions TransitionTarget => new MiniPirateActions_Standing(pirate);

            public MiniPirateActions_Walking(MiniPirate pirate, float desiredTime)
            {
                LastEntitySpriteFacing = GetEntitySpriteDirection(pirate.parentEntity);
                EntityTerrain terrain = pirate.terrain;
                EntityTerrainPlatform terrainPlatform = terrain.platforms[Main.rand.Next(0, terrain.platforms.Count)];
                float terrainPlatformX = Main.rand.NextFloat(terrainPlatform.minX, terrainPlatform.maxX);

                pathfindingResult = terrain.Pathfind(pirate.terrainPlatform, pirate.relativePosition.X, terrainPlatform, terrainPlatformX, 6, 6);
                pathfindingResult[0].Unpack(ref pirate.terrainPlatform, ref targetX, ref startPoint, ref endPoint);
                pirate.frame = 2;
                this.desiredTime = desiredTime;

                pirate.SetDesired(new Vector2(terrainPlatformX, terrainPlatform.Y), pirate.parentEntity, terrain, terrainPlatform);

                this.pirate = pirate;
            }
            public override MiniPirateActions Update(MiniPirate pirate, float simRate)
            {
                bool shouldRet = false;
                float diff = targetX - pirate.relativePosition.X;
                int sign = Math.Sign(diff);
                MiniPirateActions rv = this;

                if (MathF.Abs(diff) < speed * simRate)
                {
                    pirate.relativePosition.X = targetX;

                    pathfindingResult.RemoveAt(0);
                    if (pathfindingResult.Count == 0)
                        shouldRet = true;
                    else //leap
                    {
                        rv = new MiniPirateActions_Jumping_WithinEntity(this, startPoint, endPoint, 10, jumpDuration);
                        GeneralMiniPirateUpdate(pirate, simRate, sign, 10, MiniPirateFrames.WalkStart, MiniPirateFrames.WalkEnd);
                        pirate.relativePosition = endPoint;
                        pathfindingResult[0].Unpack(ref pirate.terrainPlatform, ref targetX, ref startPoint, ref endPoint);
                        return rv;
                    }
                }
                else
                    pirate.relativePosition.X += speed * simRate * sign;
                GeneralMiniPirateUpdate(pirate, simRate, sign, 10, MiniPirateFrames.WalkStart, MiniPirateFrames.WalkEnd);

                if (shouldRet)
                {
                    if (ActionDuration > desiredTime)
                        return TransitionTarget;
                    else
                    {
                        rv = new MiniPirateActions_Walking(pirate, desiredTime - ActionDuration);
                        return rv;
                    }
                }
                return rv;
            }
        }
    }
}
