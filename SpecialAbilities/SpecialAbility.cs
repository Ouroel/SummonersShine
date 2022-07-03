using Microsoft.Xna.Framework;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SummonersShine.SpecialAbilities
{
    public class DummyEntity : Entity {
        public DummyEntity(Vector2 position) {
            this.position = position;
        }
    }
    public static partial class SpecialAbility
    {
        public static void DefaultOnSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity _target, int specialType, bool fromServer)
        {
            projData.energy = 0;
            projData.castingSpecialAbilityTime = 0;
            projData.energyRegenRateMult = 0;
        }
        public static Entity FindSpecialAbilityTarget(Player player, Vector2 mouseWorld) {
            int num = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                bool flag = Main.npc[i].CanBeChasedBy(player, false) && Main.npc[i].Hitbox.Distance(player.Center) <= 1400 && (num == -1 || Main.npc[i].Hitbox.Distance(mouseWorld) < Main.npc[num].Hitbox.Distance(mouseWorld));
                if (flag)
                {
                    num = i;
                }
            }
            if (num == -1)
                return null;
            return Main.npc[num];
        }

        public static Entity FindSpecialAbilityTargetPoint(Player player, Vector2 mouseWorld) {
            return new DummyEntity(mouseWorld);
        }

        public static List<Projectile> PreAbility_OneAtATime(Item summonItem, ReworkMinion_Player player)
        {
            Projectile shouldReturn = player.GetMinionCollection(summonItem.type).minions.Find(i =>
            {
                MinionProjectileData projData = i.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData();
                return projData.castingSpecialAbilityTime != -1;
            });
            if (shouldReturn == null)
                return PreAbility_FindAnyMinion(summonItem, player);
            return new List<Projectile>();
        }
        public static List<Projectile> PreAbility_FindAnyMinion(Item summonItem, ReworkMinion_Player player)
        {
            Projectile rvProj = player.GetMinionCollection(summonItem.type).minions.Find(i =>
            {
                MinionProjectileData projData = i.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData();
                return projData.maxEnergy > 0 && projData.energy >= projData.maxEnergy;
            });
            List<Projectile> rv = new List<Projectile>();
            if (rvProj != null)
            {
                rv.Add(rvProj);
            }
            return rv;

        }
        public static List<Projectile> PreAbility_FindClosestMinion(Item summonItem, ReworkMinion_Player player)
        {
            Projectile closest = null;
            MinionProjectileData closestProjData = null;

            Vector2 playerPos = player.Player.Center;
            player.GetMinionCollection(summonItem.type).minions.ForEach(i =>
            {
                MinionProjectileData projData = i.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData();
                if (projData.maxEnergy == 0 || projData.energy < projData.maxEnergy)
                    return;
                //pay cost here
                if (closest == null || (i.Center - playerPos).LengthSquared() < (closest.Center - playerPos).LengthSquared())
                {
                    closest = i;
                    closestProjData = projData;
                }
            });
            List<Projectile> rv = new List<Projectile>();
            if (closest != null)
            {
                rv.Add(closest);
            }
            return rv;
        }

        public static List<Projectile> PreAbility_Toggle(Item summonItem, ReworkMinion_Player player)
        {
            List<Projectile> rv = PreAbility_FindAllMinions(summonItem, player);
            List<Projectile> notCasting = rv.FindAll(i => i.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData().castingSpecialAbilityTime == -1);
            if (notCasting.Any())
                return notCasting;
            return rv;
        }

        public static List<Projectile> PreAbility_FindAllMinions(Item summonItem, ReworkMinion_Player player)
        {
            return player.GetMinionCollection(summonItem.type).minions.FindAll(i =>
            {
                MinionProjectileData projData = i.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData();
                return projData.maxEnergy != 0 && projData.energy == projData.maxEnergy;
            });
        }
        public static List<Projectile> PreAbility_EmptyList(Item summonItem, ReworkMinion_Player player)
        {
            return new();
        }

        public static bool NoCustomAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            return true;
        }
        public static void NoOnSpecialAbilityUsed(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity target, int specialType, bool fromServer)
        {
        }
        
        public static bool NoCustomPreDraw(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Color lightColor)
        {
            return true;
        }
        public static void NoCustomPostDraw(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Color lightColor)
        {
        }
        public static Color NoCustomModifyColor(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Color lightColor)
        {
            return lightColor;
        }

        public static int RandomMinionTarget(this Projectile projectile, int prefersNot = -1, float range = 1400, Vector2 offset = default(Vector2))
        {
            List<int> rv = new();
            int lastResort = -1;
            Vector2 testCenter = projectile.Center + offset;

            for (int x = 0; x < Main.npc.Length; x++)
            {
                NPC npc = Main.npc[x];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    Vector2 disp = npc.Center - testCenter;
                    float dist = disp.Length();
                    bool canHit = dist < range && Collision.CanHitLine(testCenter, 0, 0, npc.position, npc.width, npc.height);
                    if (canHit)
                    {
                        if (x == prefersNot)
                            lastResort = x;
                        else
                            rv.Add(x);
                    }
                }
            }
            if (rv.Count > 0)
                return rv[Main.rand.Next(0, rv.Count)];
            return lastResort;
            ;
        }
    }
}
