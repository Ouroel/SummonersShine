using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine
{
    public static class EntityPlatformInteraction
    {
        /*public static void OnStandOnPlatform(this Entity entity)
        {
            Player player = entity as Player;
            if (player != null)
                OnStandOnPlatform(player);
        }*/
        public static bool CanStandOnPlatform(this Player player) {
            return player.active && !player.controlDown && player.grapCount == 0 && player.gravDir == 1 && !player.GetModPlayer<ReworkMinion_Player>().IsStanding && (!player.mount.Active || !player.mount.Cart);
        }
        public static void OnStandOnPlatform(this Player player)
        {
            player.GetModPlayer<ReworkMinion_Player>().UpdateFallStart = true;
        }

        public static bool IsMainPlayer(this Player player)
        {
            return !Main.dedServ && player.whoAmI == Main.myPlayer;
        }
    }

    public class Platform_AttachedToProj : Platform
    {
        public Projectile parent;

        public Platform_AttachedToProj(Vector2 pos, float width, Projectile parent) : base(pos, width) {
            this.parent = parent;
        }

        public override int Net_GetKey()
        {
            return GetEncodedKeyType(parent.identity, 0);
        }
    }

    public abstract class Platform {
        public Vector2 pos;
        public float width;
        public List<Entity> attachedEntityList = new List<Entity>();
        public Vector2 lastMove = Vector2.Zero;
        public OnEntityAddedOrRemoved OnEntityAdded;
        public OnEntityAddedOrRemoved OnEntityRemoved;
        public Action OnNetUpdate = delegate () { };
        public List<Projectile> grapples = new List<Projectile>();

        public delegate void OnEntityAddedOrRemoved(Entity entity, bool fromServer = false);

        List<Entity> nextAddVels = new();
        public bool Net_IsEqual(int key)
        {
            return key == Net_GetKey();
        }

        public abstract int Net_GetKey();

        public int GetEncodedKeyType(int key, int type) {
            return key * 8 + type;
        }

        public Platform(Vector2 pos, float width) {
            this.pos = pos;
            this.width = width;
            PlatformCollection.AddPlatform(this);
        }

        public void Destroy() {
            PlatformCollection.RemovePlatform(this);
            attachedEntityList.ForEach(i=>SafelyRemovePlayer(i));
            attachedEntityList.Clear();
            grapples.ForEach(i =>
            {
                i.GetGlobalProjectile<ReworkMinion_Projectile>().hookedPlatform = null;
            });
        }
        public void AddAttachedEntity(Entity entity)
        {
            if (attachedEntityList.Contains(entity))
                return;
            attachedEntityList.Add(entity);
            nextAddVels.Add(entity);

            if (OnEntityAdded != null)
                OnEntityAdded(entity);

            //sync player
            Player player = entity as Player;
            if (player == null)
                return;
            player.GetModPlayer<ReworkMinion_Player>().platform = this;
        }

        void SafelyRemovePlayer(Entity entity) {
            Player player = entity as Player;
            if (player != null)
            {
                ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
                if (playerFuncs.platform == this)
                    playerFuncs.platform = null;
            }
        }
        public void RemoveAttachedEntity(Entity entity)
        {
            if (!attachedEntityList.Remove(entity))
                return;

            SafelyRemovePlayer(entity);

            if (!(nextAddVels.Remove(entity) || lastMove == Vector2.Zero))
            {
                Vector2 vel = entity.velocity;
                float dot = Vector2.Dot(vel, lastMove);
                float lastMoveLenSqr = lastMove.LengthSquared();
                dot /= lastMoveLenSqr;
                Vector2 velProjected = vel * dot;
                Vector2 newVelProjected = velProjected;
                if (newVelProjected.LengthSquared() < lastMoveLenSqr)
                    newVelProjected = lastMove;

                entity.velocity += (newVelProjected - velProjected);
            }

            if (OnEntityRemoved != null)
                OnEntityRemoved(entity);
        }

        public void Move(Vector2 direction) {

            List<Tuple<Vector2, Entity>> CaughtEntities = new List<Tuple<Vector2, Entity>>();
            attachedEntityList.ForEach(i => CaughtEntities.Add(new Tuple<Vector2, Entity>(Vector2.Zero, i)));

            //players
            for (int x = 0; x < Main.maxPlayers; x++)
            {

                Player player = Main.player[x];
                Tuple<bool, Vector2> result = PlatformCollection.DetectPlatform(player.position, -direction, player.width, player.height, this, true);
                if (player.CanStandOnPlatform() && result.Item1 && !PlatformCollection.EntityAttachedToPlatform(player))
                {
                    if (!attachedEntityList.Contains(player))
                    {
                        CaughtEntities.Add(new Tuple<Vector2, Entity>(result.Item2, player));
                        player.OnStandOnPlatform();
                    }
                }
            }
            for (int x = 0; x < Main.maxProjectiles; x++)
            {

                Projectile projectile = Main.projectile[x];
                Tuple<bool, Vector2> result = PlatformCollection.DetectPlatform(projectile.position, -direction, projectile.width, projectile.height, this, true);
                if (projectile.type >= ProjectileID.OneEyedPirate && projectile.type <= ProjectileID.PirateCaptain && result.Item1 && !PlatformCollection.EntityAttachedToPlatform(projectile))
                {
                    if (!attachedEntityList.Contains(projectile))
                    {
                        CaughtEntities.Add(new Tuple<Vector2, Entity>(result.Item2, projectile));
                    }
                }
            }

            CaughtEntities.ForEach(i =>
            {
                Vector2 change = i.Item1 + direction;
                Entity entity = i.Item2;
                change = Collision.TileCollision(entity.position, change, entity.width, entity.height, true, true, 1);
                entity.position += change;
                PlatformCollection.GroundEntity(i.Item2);
            });
            pos += direction;
            lastMove = direction;

            if (direction != Vector2.Zero) {
                nextAddVels.ForEach(i =>
                {
                    Vector2 vel = i.velocity;
                    float dot = Vector2.Dot(vel, direction);
                    if (dot <= 0)
                        return;
                    dot /= direction.LengthSquared();
                    Vector2 velProjected = vel * dot;
                    Vector2 newVelProjected = velProjected - direction;
                    if (Vector2.Dot(velProjected, newVelProjected) < 0)
                        newVelProjected = Vector2.Zero;

                    i.velocity += (newVelProjected - velProjected);
                });
            }
            nextAddVels.Clear();

            grapples.ForEach(i =>
            {
                i.position += direction;
            });
        }
        public Vector2 center { get { return pos + new Vector2(width * 0.5f, 0); } set { pos = value - new Vector2(width * 0.5f, 0); } }
    }
    public static class PlatformCollection
    {

        public class PlatformCollection_Loader : ILoadable
        {
            public void Load(Mod mod)
            {
                platforms = new();
            }

            public void Unload()
            {
                platforms = null;
            }
        }

        static List<Platform> platforms;

        const float grapple_height = 16f;

        public static Platform FindPlatform(int key) {
            return platforms.Find(i => i.Net_IsEqual(key));
        }
        public static void AddPlatform(Platform platform)
        {
            if (platforms.Contains(platform))
                return;
            platforms.Add(platform);
        }

        public static void RemovePlatform(Platform platform)
        {
            platforms.Remove(platform);
        }

        public static void ClearPlatforms()
        {
            platforms.Clear();
        }

        public static Platform GetPlatform(int index) {
            return platforms[index];
        }

        public static Platform AttemptGrapple(Projectile grapple)
        {
            Platform lastHooked = grapple.GetGlobalProjectile<ReworkMinion_Projectile>().hookedPlatform;
            if (lastHooked != null) {
                return lastHooked;
            }

            Vector2 pos = grapple.Center;
            Platform rv = null;
            platforms.ForEach(platform =>
            {
                if (rv == null && platform != null)
                {
                    Vector2 platformPos = platform.pos;
                    if (pos.Y > platformPos.Y - grapple_height &&
                    pos.Y < platformPos.Y + grapple_height &&
                    pos.X > platformPos.X - grapple_height &&
                    pos.X < platformPos.X + grapple_height + platform.width)
                    {
                        platform.grapples.Add(grapple);
                        rv = platform;
                    }
                }
            });
            
            return rv;
        }
        public static void UnattachAllPlatforms(Entity entity)
        {
            platforms.ForEach(platform =>
            {
                if (platform != null)
                    platform.RemoveAttachedEntity(entity);
            });
        }

        public static void Net_ForcePlatform(Entity entity, Platform platform) {
            platforms.ForEach(i =>
            {
                if (i == platform)
                {
                    i.AddAttachedEntity(entity);
                }
                else
                    i.RemoveAttachedEntity(entity);
            });
        }

        public static bool EntityAttachedToPlatform(Entity entity)
        {
            bool rv = false;
            platforms.ForEach(platform =>
            {
                rv = rv || platform.attachedEntityList.Contains(entity);
            });
            return rv;
        }
        public static Tuple<bool, Vector2> TestPlatformCollision(Entity entity, Func<Platform, bool> customCheck = null) {
            bool rv = false;
            Vector2 vel = entity.velocity;
            Platform closestPlatform = null;
            List<Platform> platformsToUnattach = new List<Platform>();

            Vector2 smallestVel = vel;
            bool foundFavoritePlatform = false;

            platforms.ForEach(platform =>
            {
                if (platform != null)
                {
                    Tuple<bool, Vector2> result = DetectPlatform(entity.position, vel, entity.width, entity.height, platform);
                    if (result.Item1 && (customCheck == null || customCheck(platform)))
                    {
                        rv = true;

                        bool isFavoritePlatform = platform.attachedEntityList.Contains(entity);

                        if (isFavoritePlatform || (!foundFavoritePlatform && Math.Abs(smallestVel.Y) > Math.Abs(result.Item2.Y)))
                        {
                            closestPlatform = platform;

                            smallestVel = result.Item2;
                            if (smallestVel.Y > -0.1f && smallestVel.Y < 0.1f)
                            {
                                smallestVel.Y = 0;
                            }
                        }

                        foundFavoritePlatform = foundFavoritePlatform || isFavoritePlatform;
                    }
                    platformsToUnattach.Add(platform);
                }
            });

            vel = new Vector2(vel.X, smallestVel.Y);

            if (closestPlatform != null)
            {
                platformsToUnattach.Remove(closestPlatform);
                closestPlatform.AddAttachedEntity(entity);
            }
            platformsToUnattach.ForEach(i => i.RemoveAttachedEntity(entity));
            return new Tuple<bool, Vector2>(rv, vel);
        }

        public static Tuple<bool, Vector2> DetectPlatform(Vector2 pos, Vector2 vel, int width, int height, Platform platform, bool outputX = false) {
            
            //if will never collide
            if (vel.Y <= 0)
                return new Tuple<bool, Vector2>(false, default);
            float floor = pos.Y + height;
            float platformY = platform.pos.Y;

            //if moving away from platform
            if (platformY < floor)
                return new Tuple<bool, Vector2>(false, default);
            float destFloor = floor + vel.Y;

            //if dest doesn't even touch the platform
            if (platformY > destFloor)
                return new Tuple<bool, Vector2>(false, default);
            float platformX = platform.pos.X;

            float leftThreshold = platformX - pos.X - width;
            float rightThreshold = platformX + platform.width - pos.X;
            float relativePlatformY = platformY - floor;

            float compensation = (relativePlatformY > 0 ? -0.1f : 0.1f);
            relativePlatformY += compensation;


            //do collision

            float progress = relativePlatformY / vel.Y;
            float xPos = vel.X * progress;
            float xRV = vel.X;
            if (outputX)
                xRV = xPos;
            if (leftThreshold < xPos && rightThreshold > xPos)
            {
                return new Tuple<bool, Vector2>(true, new Vector2(xRV, relativePlatformY));
            }

            return new Tuple<bool, Vector2>(false, default);
        }

        public static void GroundEntity(Entity entity) {
            Player player = entity as Player;
            if (player == null)
                return;
            player.fallStart = (int)(player.position.Y / 16f);
            player.jump = 0;
            player.wings = 0;
        }

        public static Vector2 GetRealPlayerVelocity(this Player player)
        {
            Vector2 rv = player.velocity;
            bool slime = player.mount.Active && (player.mount.Type == 43 || (player.mount.IsConsideredASlimeMount && !player.SlimeDontHyperJump)) && player.velocity.Y != 0f;
            if (slime)
                rv.Y *= 2;
            return rv;
        }
        //use only if you changed player vel
        public static void ConvertRealVelocityBack(this Player player)
        {
            bool slime = player.mount.Active && (player.mount.Type == 43 || (player.mount.IsConsideredASlimeMount && !player.SlimeDontHyperJump)) && player.velocity.Y != 0f;
            if (slime)
                player.velocity.Y *= 0.5f;
        }
    }
}
