using Microsoft.Xna.Framework;
using SummonersShine.BakedConfigs;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace SummonersShine
{
    public static class ModUtils
    {
        public static bool NextVelOutOfBounds(this Projectile projectile) {
            Vector2 newPos = projectile.position + projectile.velocity;
            newPos /= 16;
            if (newPos.X < 0 || newPos.Y < 0 || newPos.X > Main.maxTilesX || newPos.Y > Main.maxTilesY)
                return true;
            return false;
        }
        public static Vector2 GetRealNPCVelocity(this NPC npc)
        {
            ReworkMinion_NPC npcFuncs = npc.GetGlobalNPC<ReworkMinion_NPC>();
            if (npcFuncs.teleported)
            {
                return npc.position - npcFuncs.lastPos;
            }
            return npc.velocity;
        }

        public static Vector2 GetTileCollideModifier(Vector2 center, Vector2 vel, float give = 16)
        {
            if (vel == Vector2.Zero)
                return vel;
            float velMag = vel.Length();
            Vector2 workingVel = vel * (velMag + give) / velMag;
            Vector2 newLine = center + workingVel;
            center /= 16;
            center.X = (int)center.X;
            center.Y = (int)center.Y;
            newLine /= 16;
            newLine.X = (int)newLine.X;
            newLine.Y = (int)newLine.Y;
            Tuple<int, int> col;
            Collision.TupleHitLine((int)center.X, (int)center.Y, (int)newLine.X, (int)newLine.Y, 0, 0, new(), out col);
            Vector2 collided = new Vector2(col.Item1, col.Item2);
            if (newLine.X == collided.X && newLine.Y == collided.Y)
            {
                return vel;
            }
            collided -= center;
            collided *= 16;
            float collidedLen = collided.Length();
            if (collidedLen <= give)
                return Vector2.Zero;

            Vector2 endDist = collided * (collidedLen - give) / collidedLen;
            return endDist;
        }
        public static void GetEntityRotSpriteDirection(this Entity entity, out float rot, out int dir)
        {
            NPC npc = entity as NPC;
            if (npc != null)
            {
                rot = npc.rotation;
                dir = npc.spriteDirection;
                return;
            }
            Player player = entity as Player;
            if (player != null)
            {
                rot = 0;
                dir = player.direction;
                return;
            }
            Projectile projectile = entity as Projectile;
            if (projectile != null)
            {
                rot = projectile.rotation;
                dir = projectile.spriteDirection;
                return;
            }
            throw new ArgumentOutOfRangeException("[GetEntityRotSpriteDirection] entity is not an NPC, player, or projectile.");
        }
        public static Projectile FindProjectileWithIdentity(int id)
        {
            for (int x = 0; x < Main.projectile.Length; x++)
            {
                Projectile rv = Main.projectile[x];
                if (rv.identity == id)
                {
                    return rv;
                }
            }
            return null;
        }
        public static Entity GetNPCProjOrPlayer_ID(int id)
        {
            if (id == -1)
                return null;
            int mod4 = id % 4;
            int actualID = (id - mod4) / 4;
            switch (mod4)
            {
                case 0:
                    return Main.npc[(int)actualID];
                case 1:
                    return Main.player[(int)actualID];
                case 2:
                    return FindProjectileWithIdentity((int)actualID);
                default:
                    throw new ArgumentOutOfRangeException("[GetNPCProjOrPlayer_ID] unclear type");
            }
        }
        public static int ConvertToGlobalEntityID(this Entity entity)
        {
            if (entity == null)
                return -1;
            NPC npc = entity as NPC;
            if (npc != null)
            {
                return npc.whoAmI * 4;
            }
            Projectile proj = entity as Projectile;
            if (proj != null)
            {
                return proj.identity * 4 + 2;
            }
            return entity.whoAmI * 4 + 1;
        }

        public static bool IsProjectileStationary(int id, int parent)
        {
            return BakedConfig.NoTrackingProjectiles[id] || BakedConfig.MinionsIgnoreTracking[parent];
        }
        public static Item GetPrefixComparisonItem(int netID)
        {
            if (Main.tooltipPrefixComparisonItem == null)
            {
                Main.tooltipPrefixComparisonItem = new Item();
            }
            Item compItem = Main.tooltipPrefixComparisonItem;
            if (compItem.netID != netID)
                compItem.netDefaults(netID);
            return compItem;
        }
        public static bool IncrementSpecialAbilityTimer(this Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, int maxTicks, float defaultEnergyRegenMult = 1)
        {
            if (projectile.IsOnRealTick(projData))
            {
                projData.castingSpecialAbilityTime++;
                if (projData.castingSpecialAbilityTime >= maxTicks)
                {
                    projData.castingSpecialAbilityTime = -1;
                    projData.energyRegenRateMult = defaultEnergyRegenMult;
                    Player player = Main.player[projectile.owner];
                    ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
                    projFuncs.minionTerminateSpecialAbility(projectile, projFuncs, projData, player, playerFuncs);
                    return true;
                }
            }
            return false;
        }
        public static Vector2 GetLineCollision(Vector2 start, Vector2 line, out bool tooCramped, float give = 16)
        {
            Vector2 newLine = start + line;
            newLine /= 16;
            Tuple<int, int> col;
            if (!Collision.TupleHitLine((int)start.X / 16, (int)start.Y / 16, (int)newLine.X, (int)newLine.Y, 0, 0, new(), out col))
            {
                tooCramped = false;
                return start + line;
            }

            Vector2 rv = new Vector2(col.Item1, col.Item2) * 16;
            rv -= start;
            float len = rv.Length();
            if (len < give)
            {
                tooCramped = true;
                return start;
            }
            tooCramped = false;
            rv *= (len - give) / len;
            rv += start;
            return rv;

        }

        //code ripped from Collision.CanHitLine to ensure compatibility with vanilla behavior
        public static Vector2 FurthestCanHitLine(Vector2 start, Vector2 line)
        {

            int center1x = (int)((start.X) / 16f);
            int center1y = (int)((start.Y) / 16f);
            int center2x = (int)((start + line).X / 16f);
            int center2y = (int)((start + line).Y / 16f);

            Vector2 farthestCanHit = start;

            Math.Clamp(center1x, 1, Main.maxTilesX - 1);
            Math.Clamp(center1y, 1, Main.maxTilesY - 1);
            Math.Clamp(center2x, 1, Main.maxTilesX - 1);
            Math.Clamp(center2y, 1, Main.maxTilesY - 1);
            float xDiff = (float)Math.Abs(center1x - center2x);
            float yDiff = (float)Math.Abs(center1y - center2y);
            //same location
            if (xDiff == 0f && yDiff == 0f)
            {
                return farthestCanHit;
            }
            float xOverY = 1f;
            float yOverX = 1f;
            if (xDiff == 0f || yDiff == 0f)
            {
                if (xDiff == 0f)
                {
                    xOverY = 0f;
                }
                if (yDiff == 0f)
                {
                    yOverX = 0f;
                }
            }
            else if (xDiff > yDiff)
            {
                xOverY = xDiff / yDiff;
            }
            else
            {
                yOverX = yDiff / xDiff;
            }
            float num9 = 0f;
            float num10 = 0f;
            int centerOneUnderCenter2 = 1;
            if (center1y < center2y)
            {
                centerOneUnderCenter2 = 2;
            }
            int xDiffInt = (int)xDiff;
            int yDiffInt = (int)yDiff;
            int stepX = Math.Sign(center2x - center1x);
            int stepY = Math.Sign(center2y - center1y);
            bool flag = false;
            bool flag2 = false;

            try
            {
                for (; ; )
                {
                    if (centerOneUnderCenter2 != 1)
                    {
                        if (centerOneUnderCenter2 == 2)
                        {
                            num9 += xOverY;
                            int num16 = (int)num9;
                            num9 %= 1f;
                            for (int i = 0; i < num16; i++)
                            {
                                if (Main.tile[center1x, center1y - 1] == null
                                    || Main.tile[center1x, center1y] == null
                                    || Main.tile[center1x, center1y + 1] == null)
                                {
                                    return farthestCanHit;
                                }
                                Tile tile = Main.tile[center1x, center1y - 1];
                                Tile tile2 = Main.tile[center1x, center1y + 1];
                                Tile tile3 = Main.tile[center1x, center1y];
                                if ((!tile.IsActuated && tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]) || (!tile2.IsActuated && tile2.HasTile && Main.tileSolid[tile2.TileType] && !Main.tileSolidTop[tile2.TileType]) || (!tile3.IsActuated && tile3.HasTile && Main.tileSolid[tile3.TileType] && !Main.tileSolidTop[tile3.TileType]))
                                {
                                    return farthestCanHit;
                                }
                                if (xDiffInt == 0 && yDiffInt == 0)
                                {
                                    flag = true;
                                    break;
                                }
                                farthestCanHit = new Vector2(center1x, center1y) * 16;
                                center1x += stepX;
                                xDiffInt--;
                                if (xDiffInt == 0 && yDiffInt == 0 && num16 == 1)
                                {
                                    flag2 = true;
                                }
                            }
                            if (yDiffInt != 0)
                            {
                                centerOneUnderCenter2 = 1;
                            }
                        }
                    }
                    else
                    {
                        num10 += yOverX;
                        int numSteps = (int)num10;
                        num10 %= 1f;
                        for (int j = 0; j < numSteps; j++)
                        {
                            if (Main.tile[center1x - 1, center1y] == null
                                || Main.tile[center1x, center1y] == null
                                || Main.tile[center1x + 1, center1y] == null)
                            {
                                return farthestCanHit;
                            }
                            Tile tile4 = Main.tile[center1x - 1, center1y];
                            Tile tile5 = Main.tile[center1x + 1, center1y];
                            Tile tile6 = Main.tile[center1x, center1y];
                            if ((!tile4.IsActuated && tile4.HasTile && Main.tileSolid[tile4.TileType] && !Main.tileSolidTop[tile4.TileType]) || (!tile5.IsActuated && tile5.HasTile && Main.tileSolid[tile5.TileType] && !Main.tileSolidTop[tile5.TileType]) || (!tile6.IsActuated && tile6.HasTile && Main.tileSolid[tile6.TileType] && !Main.tileSolidTop[tile6.TileType]))
                            {
                                return farthestCanHit;
                            }
                            if (xDiffInt == 0 && yDiffInt == 0)
                            {
                                flag = true;
                                break;
                            }
                            farthestCanHit = new Vector2(center1x, center1y) * 16;
                            center1y += stepY;
                            yDiffInt--;
                            if (xDiffInt == 0 && yDiffInt == 0 && numSteps == 1)
                            {
                                flag2 = true;
                            }
                        }
                        if (xDiffInt != 0)
                        {
                            centerOneUnderCenter2 = 2;
                        }
                    }
                    if (Main.tile[center1x, center1y] == null)
                    {
                        return farthestCanHit;
                    }
                    Tile tile7 = Main.tile[center1x, center1y];
                    if (!tile7.IsActuated && tile7.HasTile && Main.tileSolid[tile7.TileType] && !Main.tileSolidTop[tile7.TileType])
                    {
                        return farthestCanHit;
                    }
                    if (flag || flag2)
                    {
                        farthestCanHit = start + line;
                        return farthestCanHit;
                    }
                }
            }
            catch
            {
                return farthestCanHit;
            }
        }
        public static bool ClassicMinionCheck(Projectile minion, NPC npc, Vector2 projectileOrigin, float len, ref float returnDist, float minLen = 0)
        {

            if (npc.CanBeChasedBy(minion, false))
            {
                Vector2 disp = npc.Center - projectileOrigin;
                float dist = disp.Length();
                bool canHit = dist < len && dist >= minLen && Collision.CanHitLine(projectileOrigin, 0, 0, npc.position, npc.width, npc.height);
                if (canHit)
                {
                    returnDist = dist;
                    return true;
                }
            }
            return false;
        }
        public static List<NPC> FindValidNPCsOrderedByDist(Projectile projectile, Vector2 pos, float maxDist = 1400, bool useCustomOrigin = false, Vector2 origin = default, float minDist = 0)
        {
            if (!useCustomOrigin)
                origin = projectile.Center;
            List<NPC> rv = new();
            for (int x = 0; x < Main.npc.Length; x++)
            {
                NPC npc = Main.npc[x];
                float refNum = 0;
                if (ClassicMinionCheck(projectile, npc, origin, maxDist, ref refNum, minDist))
                {
                    rv.Add(npc);
                }
            }
            rv.Sort(delegate (NPC i, NPC j)
            {
                float diff = i.Center.DistanceSQ(pos) - j.Center.DistanceSQ(pos);
                if (diff > 0)
                    return 1;
                else if (diff < 0)
                    return -1;
                return 0;
            });
            return rv;
        }

        public static int FindValidNPCsAndDoSomething(Projectile projectile, Func<NPC, bool> onFound, float maxDist = 1400, bool useCustomOrigin = false, Vector2 origin = default)
        {
            if (!useCustomOrigin)
                origin = projectile.Center;
            int rv = -1;
            for (int x = 0; x < Main.npc.Length; x++)
            {
                NPC npc = Main.npc[x];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    Vector2 disp = npc.Center - origin;
                    float dist = disp.Length();
                    bool canHit = dist < maxDist && Collision.CanHitLine(projectile.Center, 0, 0, npc.position, npc.width, npc.height);
                    if (canHit)
                    {
                        rv = x;
                        if (onFound(npc))
                            return rv;
                    }
                }
            }
            return rv;
        }
        public static Projectile FindProjWithIdentity(int owner, int identity)
        {

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile test = Main.projectile[i];
                if (test != null && test.active && test.owner == owner && test.identity == identity)
                {
                    return test;
                }
            }
            return null;
        }

        public static void Write7BitEncodedSignedInt(this BinaryWriter writer, int number)
        {
            number *= 2;
            if (number < 0)
            {
                number *= -1;
                number += 1;
            }
            writer.Write7BitEncodedInt(number);
        }
        public static int Read7BitEncodedSignedInt(this BinaryReader reader)
        {
            int number = reader.Read7BitEncodedInt();
            int sign = number % 2;
            number -= sign;
            number /= 2;
            if (sign == 1)
            {
                number *= -1;
            }
            return number;
        }

        /*public static void SetStardustDragonChild(Projectile projectile, Projectile child)
        {
            int value = -1;
            if (child != null)
                value = child.projUUID;
            projectile.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData().specialCastPosition.X = value;
        }
        public static int GetStardustDragonChild(Projectile projectile)
        {
            int val = (int)projectile.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData().specialCastPosition.X;
            return Projectile.GetByUUID(projectile.owner, val);
        }*/

        public static List<Projectile> GetWholeStardustDragon(int playerID)
        {
            List<Projectile> allUpdated = new();
            for (int x = 0; x < Main.projectile.Length; x++)
            {
                Projectile test = Main.projectile[x];
                if (test.active && test.owner == playerID && test.type >= ProjectileID.StardustDragon1 && test.type <= ProjectileID.StardustDragon4)
                {
                    allUpdated.Add(test);
                }
            }
            return allUpdated;
        }

        public static List<Projectile> GetWholeStardustDragon_Sorted(int playerID)
        {
            List<int> allUpdated = new();
            for (int x = 0; x < Main.projectile.Length; x++)
            {
                Projectile test = Main.projectile[x];
                if (test.active && test.owner == playerID && test.type >= ProjectileID.StardustDragon1 && test.type <= ProjectileID.StardustDragon4)
                {
                    allUpdated.Add(x);
                }
            }
            SortStardustDragonProjectiles(allUpdated);
            List<Projectile> rv = new();
            allUpdated.ForEach(i => rv.Add(Main.projectile[i]));
            return rv;
        }

        static void SortStardustDragonProjectiles(List<int> list)
        {
            List<List<int>> list2 = new List<List<int>>();
            for (int i = 0; i < list.Count; i++)
            {
                int num = list[i];
                if (Main.projectile[num].type == 628)
                {
                    list.Remove(num);
                    List<int> list3 = new List<int>();
                    list3.Insert(0, num);
                    int byUUID = Projectile.GetByUUID(Main.projectile[num].owner, Main.projectile[num].ai[0]);
                    while (byUUID >= 0 && !list3.Contains(byUUID) && Main.projectile[byUUID].active && Main.projectile[byUUID].type >= 625 && Main.projectile[byUUID].type <= 627)
                    {
                        list3.Add(byUUID);
                        list.Remove(byUUID);
                        byUUID = Projectile.GetByUUID(Main.projectile[byUUID].owner, Main.projectile[byUUID].ai[0]);
                    }
                    List<int> list4 = new List<int>();
                    for (int j = list3.Count - 2; j >= 0; j--)
                    {
                        list4.Add(list3[j]);
                    }
                    list4.Add(list3[list3.Count - 1]);
                    list2.Add(list4);
                    i = -1;
                }
            }
            List<int> list5 = new List<int>(list);
            list2.Add(list5);
            list.Clear();
            for (int k = 0; k < list2.Count; k++)
            {
                for (int l = 0; l < list2[k].Count; l++)
                {
                    list.Add(list2[k][l]);
                }
            }
        }
        public static List<Player> DetectPlayersWithinBox(Vector2 Position, int width, int height)
        {
            return DetectPlayersWithinBox((int)Position.X, (int)Position.X + width, (int)Position.Y, (int)Position.Y + height);
        }
        public static List<Player> DetectPlayersWithinBox(int x1, int x2, int y1, int y2)
        {
            List<Player> rv = new();
            for (int x = 0; x < Main.maxPlayers; x++)
            {
                Player test = Main.player[x];
                if (test != null && test.active && test.position.X + test.width > x1 && test.position.X < x2 && test.position.Y + test.height > y1 && test.position.Y < y2)
                {
                    rv.Add(test);
                }
            }
            return rv;
        }

        public static List<Player> DetectPlayersWithinCircle(Vector2 center, int width)
        {
            List<Player> rv = DetectPlayersWithinBox((int)center.X - width, (int)center.X + width, (int)center.Y - width, (int)center.Y + width);
            rv.RemoveAll(i =>
            {
                return i.Center.DistanceSQ(center) > width * width;
            });
            return rv;
        }

        public static void SyncedAddVelocityToNPC(NPC npc, Vector2 velocity)
        {
            if (npc.HasBuff(BuffID.Slow))
            {
                if(npc.noGravity)
                    velocity /= 2;
                else
                    velocity.X /= 2;
            }
            SyncedApplyVelocityToNPC(npc, velocity - npc.velocity);
        }

        public static void SyncedApplyPositionToNPC(NPC npc, Vector2 position)
        {
            PacketHandler.WritePacket_SyncApplyPositionToNPC(npc.whoAmI, position);
            npc.position = position;
        }
        public static void SyncedApplyVelocityToNPC(NPC npc, Vector2 velocity)
        {
            PacketHandler.WritePacket_SyncApplyVelocityToNPC(npc.whoAmI, velocity);
            npc.velocity += velocity;
        }

        public static void LightningAura_MagenChiun(this Projectile proj)
        {


            int num = 10;
            int num2 = 999;
            int num3 = 30;
            int num5 = 4;
            if (Main.player[proj.owner].setMonkT2)
            {
                num3 -= 5;
            }
            if (Main.player[proj.owner].setMonkT3)
            {
                num = 14;
                num5 = 8;
            }

            proj.localAI[0] = 1f;
            proj.velocity = Vector2.Zero;
            Point point = proj.Center.ToTileCoordinates();
            bool flag2 = true;
            Point point2 = proj.Bottom.ToTileCoordinates();
            Point p;
            if (!WorldUtils.Find(new Point(point2.X, point2.Y - 1), Searches.Chain(new Searches.Up(num), new GenCondition[]
            {
            new Conditions.NotNull(),
            new Conditions.IsSolid()
            }), out p))
            {
                p = new Point(point.X, point.Y - num - 1);
            }
            int num6 = 0;
            if (flag2 && Main.tile[point2.X, point2.Y] != null && Main.tile[point2.X, point2.Y].IsHalfBlock)
            {
                num6 += 8;
            }
            Vector2 vector = point2.ToWorldCoordinates(8f, (float)num6);
            Vector2 vector2 = p.ToWorldCoordinates(8f, 0f);
            proj.Size = new Vector2(1f, vector.Y - vector2.Y);
            if (proj.height > num * 16)
            {
                proj.height = num * 16;
            }
            if (proj.height < num5 * 16)
            {
                proj.height = num5 * 16;
            }
            proj.height *= 2;
            proj.width = (int)((float)proj.height * 1f);
            if (proj.width > num2)
            {
                proj.width = num2;
            }
            proj.Center = vector;
        }

        public static string GetEmptyTexture() { return "Terraria/Images/Projectile_" + ProjectileID.StormTigerGem; }

        public static bool IsCastingSpecialAbility(MinionProjectileData projData, int itemID) {
            return projData.castingSpecialAbilityTime != -1 && BakedConfig.CustomSpecialPowersEnabled(itemID);
        }
        public static int GetBuffCount(this Player player)
        {
            for (int x = 0; x < player.buffTime.Length; x++)
            {
                if (player.buffTime[x] == 0 || player.buffType[x] == 0)
                    return x;
            }
            return player.buffTime.Length;
        }
    }
}
