using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SummonersShine.MinionAI;
using SummonersShine.SpecialData;
using SummonersShine.Textures;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.Projectiles.MiniPirateData
{
    public partial class MiniPirate
    {
        public DastardlyDoubloon boat;

        public Entity parentEntity; //latch position to this
        public EntityTerrain terrain;
        public EntityTerrainPlatform terrainPlatform;
        Vector2 relativePosition;

        /* desired for transitions */
        public Entity desiredParentEntity;
        public EntityTerrain desiredTerrain;
        public EntityTerrainPlatform desiredTerrainPlatform;
        Vector2 desiredRelativePosition;

        Vector2 _position = Vector2.Zero;
        MiniPirateActions currentAction;
        int facing = 1;

        float attackCooldown = 0;

        const float BulletStormHeight = 80;
        const float BulletStormDuration = 180;
        const float BulletStormCooldown = 600;

        List<MiniPiratePreDraw> preDraws = new();
        List<MiniPirateEvent> events = new();

        public readonly Texture2D spriteSheet;

        Vector2 feet
        {
            get
            {
                return _position + new Vector2(32, 48);
            }
            set
            {
                _position = value - new Vector2(32, 48);
            }
        }
        Vector2 Center
        {
            get
            {
                return _position + new Vector2(32, 40);
            }
            set
            {
                _position = value - new Vector2(32, 40);
            }
        }

        public MiniPirate(DastardlyDoubloon boat)
        {
            this.boat = boat;
            parentEntity = boat.Projectile;

            terrain = EntityTerrain.GetEntityTerrain(parentEntity);
            terrainPlatform = terrain.platforms[0];

            relativePosition = new Vector2(104, 44);
            SetDesired(new Vector2(104, 44), parentEntity, terrain, terrainPlatform);
            feet = parentEntity.position + relativePosition;

            currentAction = new MiniPirateActions_Standing(this);

            spriteSheet = ModTextures.PirateQueen;
        }

        float frameCounter = 0;
        int frame = 0;

        public void Move(Vector2 newPos)
        {
            feet = newPos;
        }

        public void SetDesired(Vector2 desiredRelativePosition, Entity desiredParentEntity, EntityTerrain desiredTerrain, EntityTerrainPlatform desiredTerrainPlatform)
        {
            this.desiredRelativePosition = desiredRelativePosition;
            this.desiredParentEntity = desiredParentEntity;
            this.desiredTerrain = desiredTerrain;
            this.desiredTerrainPlatform = desiredTerrainPlatform;
        }
        public void SetToDesired()
        {
            relativePosition = desiredRelativePosition;
            parentEntity = desiredParentEntity;
            terrain = desiredTerrain;
            terrainPlatform = desiredTerrainPlatform;
        }

        public void Update(float simRate, bool forceKill)
        {
            if (forceKill)
                currentAction = currentAction.ForceKill();
            events.RemoveAll(i =>
            {
                if (i.CanHook(this, currentAction))
                {
                    currentAction = i.HookEvent(this, currentAction);
                    return i.CanDestroy;
                }
                return false;
            });

            currentAction = TryAttacking(simRate).Update(this, simRate);

            preDraws.ForEach(i => i.Update(this));
            preDraws.RemoveAll(i => i.ShouldDestroy == true);
        }

        void AddEvent(MiniPirateEvent item) {
            events.Add(item);
        }

        MiniPirateActions TryAttacking(float simRate)
        {
            if (currentAction.Urgent == 0 && parentEntity != null)
            {
                int targets = AnyTargets(new Vector2(0, -BulletStormHeight));
                if (attackCooldown <= 0 && targets != -1)
                {
                    attackCooldown += BulletStormCooldown;
                    MiniPirateActions_SuperJump rv = new MiniPirateActions_SuperJump(currentAction, null, BulletStormHeight, 10, 20, relativePosition);
                    rv.peak = new MiniPirateActions_BulletStorm(rv, BulletStormDuration);
                    return rv;
                }
            }
            if (attackCooldown > 0)
            {
                attackCooldown -= Main.rand.NextFloat(0, 2) * simRate;
            }

            return currentAction;
        }

        public int AnyTargets(Vector2 offset = default(Vector2))
        {
            int rv = -1;
            Projectile thisBoatProjectile = boat.Projectile;
            Vector2 testCenter = Center + offset;

            NPC ownerMinionAttackTargetNPC = thisBoatProjectile.OwnerMinionAttackTargetNPC;
            if (ownerMinionAttackTargetNPC != null && ownerMinionAttackTargetNPC.CanBeChasedBy(thisBoatProjectile, false))
            {
                Vector2 disp = ownerMinionAttackTargetNPC.Center - testCenter;
                float dist = disp.Length();
                bool canHit = dist < 1400 && Collision.CanHitLine(testCenter, 0, 0, ownerMinionAttackTargetNPC.position, ownerMinionAttackTargetNPC.width, ownerMinionAttackTargetNPC.height);
                if (canHit)
                {
                    return ownerMinionAttackTargetNPC.whoAmI;
                }
            }
            for (int x = 0; x < Main.npc.Length; x++)
            {
                NPC npc = Main.npc[x];
                if (npc.CanBeChasedBy(thisBoatProjectile, false))
                {
                    Vector2 disp = npc.Center - testCenter;
                    float dist = disp.Length();
                    bool canHit = dist < 1400 && Collision.CanHitLine(testCenter, 0, 0, npc.position, npc.width, npc.height);
                    if (canHit)
                    {
                        rv = x;
                    }
                }
            }
            return rv;
        }

        public void Draw(Color lightColor)
        {
            Move(GetWorldPosFromRelativePos());
            preDraws.ForEach(i => i.Draw(this, lightColor));

            Color rvColor;
            float alpha;
            if (currentAction.drawFullLight)
            {

                rvColor = Color.White;
                alpha = 0;
            }
            else {
                rvColor = lightColor;
                alpha = boat.Projectile.alpha;
            }


            ModTextures.JustDraw_Projectile(spriteSheet, _position, 32, frame, 1, facing == 1, rvColor, alpha, UseCustomOrigin: true);

            /*Color rvColor = lightColor;
            if (currentAction.drawFullLight)
                rvColor = Color.White;
            rvColor *= (255 - boat.Projectile.alpha) * 0.00392157f;

            Texture2D sprites = spriteSheet;
            Rectangle rectangle = sprites.Frame(1, 32, 0, frame, 0, 0);

            Vector2 captainPos = _position - Main.screenPosition;
            Vector2 origin = Vector2.Zero;
            SpriteEffects effect = SpriteEffects.None;

            if (facing == 1)
                effect |= SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(sprites, captainPos, new Rectangle?(rectangle), rvColor, 0f, origin, 1, effect, 0);*/
        }

        public void Decouple()
        {
            relativePosition = GetWorldPosFromRelativePos();
            parentEntity = null;
        }

        public Vector2 GetWorldPosFromRelativePos(Vector2 rvRelativePos, Entity parentEntity)
        {
            if (parentEntity == null)
                return rvRelativePos;
            if (GetEntitySpriteDirection(parentEntity) == -1)
                rvRelativePos.X = parentEntity.width - rvRelativePos.X;
            return parentEntity.position + rvRelativePos;
        }
        public Vector2 GetWorldPosFromRelativePos()
        {
            return GetWorldPosFromRelativePos(relativePosition, parentEntity);
        }

        public static int GetEntitySpriteDirection(Entity entity)
        {
            Projectile projectile = entity as Projectile;
            if (projectile != null)
                return projectile.spriteDirection;
            return entity.direction;
        }

        public enum MiniPirateFrames
        {
            StandStart,
            StandEnd,
            WalkStart,
            WalkEnd,
            Boost,
            SuperJumpStart,
            SuperJumpEnd,
            Gun1,
            Gun2,
            Gun3,
            Gun4,
            Gun5,
            Gun6,
            SuperDiveStart,
            SuperDiveEnd,
            SuperSlam,
            BlinkStart,
            BlinkTransition,
            BlinkEnd,
            KickStart,
            KickEnd
        }
    }

    /* Entity Terrain */

    public struct EntityTerrainPlatform_LeapPoint
    {
        public readonly EntityTerrainPlatform newPlatform;
        public readonly float entryPoint;
        public readonly float leavePoint;
        public EntityTerrainPlatform_LeapPoint(EntityTerrainPlatform newPlatform, float enterPoint, float leavePoint)
        {
            this.newPlatform = newPlatform;
            this.entryPoint = enterPoint;
            this.leavePoint = leavePoint;
        }
    }
    public class EntityTerrainPlatform
    {
        public readonly float minX;
        public readonly float maxX;
        public readonly float Y;
        public List<EntityTerrainPlatform_LeapPoint> leapPoints = new List<EntityTerrainPlatform_LeapPoint>();

        public EntityTerrainPlatform(float minX, float maxX, float Y)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.Y = Y;
        }

        public void Link(EntityTerrainPlatform other, float X_this, float X_other)
        {
            leapPoints.Add(new EntityTerrainPlatform_LeapPoint(other, X_this, X_other));
            other.leapPoints.Add(new EntityTerrainPlatform_LeapPoint(this, X_other, X_this));
        }
    }

    public class EntityTerrain
    {
        public static EntityTerrain DastardlyDoubloonTerrain;

        public List<EntityTerrainPlatform> platforms = new();

        static EntityTerrain()
        {
        }
        public class EntityTerrain_Loader : ILoadable
        {
            public void Load(Mod mod)
            {
                /* doubloon */

                DastardlyDoubloonTerrain = new EntityTerrain();
                EntityTerrainPlatform platform = new(6, 114, 44);
                EntityTerrainPlatform platform2 = new(38, 42, 38);
                EntityTerrainPlatform platform3 = new(6, 38, 34);
                DastardlyDoubloonTerrain.platforms.Add(platform);
                DastardlyDoubloonTerrain.platforms.Add(platform2);
                DastardlyDoubloonTerrain.platforms.Add(platform3);
                platform.Link(platform2, 52, 44);
                platform2.Link(platform3, 36, 30);
            }

            public void Unload()
            {
                DastardlyDoubloonTerrain = null;
            }
        }

        public static EntityTerrain GetEntityTerrain(Entity entity)
        {

            Projectile proj = entity as Projectile;
            if (proj != null)
            {
                //yandev reporting REE CANT SWITCH REE
                if (proj.type == ProjectileModIDs.DastardlyDoubloon)
                    return DastardlyDoubloonTerrain;
            }
            EntityTerrain rv = new EntityTerrain();
            rv.platforms.Add(new EntityTerrainPlatform(0, entity.width, 0));

            return null;
        }

        class PathfindingData
        {
            public EntityTerrainPlatform platform;
            public float cost;
            public float entryPoint;
            public float leavePoint;
            public PathfindingData last;

            public PathfindingData(EntityTerrainPlatform platform, float cost, float entryPoint, float leavePoint, PathfindingData last)
            {
                this.platform = platform;
                this.cost = cost;
                this.leavePoint = leavePoint;
                this.entryPoint = entryPoint;
                this.last = last;
            }
        }

        public List<PathfindingReturnData> Pathfind(EntityTerrainPlatform platform, float xPos, EntityTerrainPlatform targetPlatform, float targetXPos, float leapCostPerUnit = 0, float xLeapSavings = 6, float minLeapCost = 0.1f)
        { //pray it works! never tried before
            List<PathfindingReturnData> rv = new List<PathfindingReturnData>();
            rv.Add(new PathfindingReturnData(targetPlatform, targetXPos, default, default));
            if (platform == targetPlatform)
            {
                return rv;
            }

            PathfindingData currentData = new PathfindingData(platform, 0, xPos, xPos, null);

            List<PathfindingData> exploredPlatforms = new List<PathfindingData>();
            List<PathfindingData> candidatePlatforms = new List<PathfindingData>();

            while (currentData.platform != targetPlatform) //dijkstra
            {
                //adds all candidates
                currentData.platform.leapPoints.ForEach(i => {

                    int index = exploredPlatforms.FindIndex(j => j.platform == i.newPlatform);
                    if (index != -1)
                        return;
                    index = candidatePlatforms.FindIndex(j => j.platform == i.newPlatform);
                    if (index != -1)
                        return;

                    float dist = i.entryPoint - currentData.leavePoint;
                    PathfindingData newCandidate = new PathfindingData(
                        i.newPlatform,
                        currentData.cost + Math.Abs(dist) + MathF.Max(minLeapCost, leapCostPerUnit - xLeapSavings),
                        Math.Max(0, Math.Abs(dist) - xLeapSavings) * Math.Sign(dist) + currentData.leavePoint,
                        i.leavePoint,
                        currentData
                        );

                    candidatePlatforms.Add(newCandidate);
                });

                //find greatest candidate
                PathfindingData greatest = null;

                if (candidatePlatforms.Count == 0)
                    return rv;

                candidatePlatforms.ForEach(i =>
                {
                    if (greatest == null || greatest.cost > i.cost)
                        greatest = i;
                });

                candidatePlatforms.Remove(greatest);
                exploredPlatforms.Add(currentData);
                currentData = greatest;
            }

            //compile

            while (currentData.last != null)
            {
                rv.Add(new PathfindingReturnData(currentData.platform, currentData.entryPoint, new Vector2(currentData.entryPoint, currentData.last.platform.Y), new Vector2(currentData.leavePoint, currentData.platform.Y)));
                currentData = currentData.last;
            }
            rv.Reverse();

            return rv;
        }
    }
    public class PathfindingReturnData
    {
        float nextTarget;
        Vector2 jumpStart;
        Vector2 jumpEnd;
        EntityTerrainPlatform platform;
        public PathfindingReturnData(EntityTerrainPlatform platform, float nextTarget, Vector2 jumpStart, Vector2 jumpEnd)
        {
            this.platform = platform;
            this.nextTarget = nextTarget;
            this.jumpStart = jumpStart;
            this.jumpEnd = jumpEnd;
        }

        public void Unpack(ref EntityTerrainPlatform platform, ref float targetX, ref Vector2 jumpStart, ref Vector2 jumpEnd)
        {
            targetX = nextTarget;
            jumpStart = this.jumpStart;
            jumpEnd = this.jumpEnd;
            platform = this.platform;
        }
    }
}
