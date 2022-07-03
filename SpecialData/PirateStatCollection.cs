using Microsoft.Xna.Framework;
using SummonersShine.Projectiles;
using SummonersShine.Projectiles.MiniPirateData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace SummonersShine.SpecialData
{
    public class PirateStat : miniMinionStat
    {
        public const float kickTime = 15;
        public const float falloffTime = 5;

        /*public Entity forcedTarget;
        public int kickDuration = -1;
        public int invadeShipCooldown = 0;*/

        Vector2 lastPos;
        bool posSaved;

        public Vector2 GetKickVel(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData) {
            projData.castingSpecialAbilityTime++;
            int numUpdates = projectile.extraUpdates + 1;

            float speed = projFuncs.GetSpeed(projectile);

            int lastCastingSpecialTime = projData.castingSpecialAbilityTime;

            bool terminate = lastCastingSpecialTime >= kickTime + falloffTime;
            bool massiveDrag = lastCastingSpecialTime > kickTime;


            if (terminate)
            {
                projData.specialCastTarget = null;
                projData.castingSpecialAbilityTime = -1 - Main.rand.Next(300, 1000);
                projectile.netUpdate = true; //sync away kickVel
            }
            Vector2 targetVel;
            if (projData.specialCastTarget != null)
                targetVel = projData.specialCastTarget.velocity * speed / numUpdates;
            else
                targetVel = Vector2.Zero;
            Vector2 rv = projData.specialCastPosition;
            if (terminate)
                projData.specialCastPosition = Vector2.Zero;
            rv = rv + new Vector2(0, 5f * (lastCastingSpecialTime - kickTime / 2)) + targetVel;
            if (massiveDrag)
                return rv * (falloffTime - lastCastingSpecialTime + kickTime) / falloffTime;
            return rv;
        }

        public void SaveProjectileData(Projectile projectile)
        {
            lastPos = projectile.position;
            posSaved = true;
        }
        public void LoadProjectileData(Projectile projectile)
        {
            if (!posSaved)
                return;
            projectile.position = lastPos;
            posSaved = false;
        }
    }

    public abstract class SummonersShineStatCollection : SpecialDataBase
    {
        protected List<miniMinionStat> stats = new();
        public ReworkMinion_Projectile megaMinion;
        public Projectile megaMinionBody;
        public float avgDmg = 0;
        public float avgSpd = 1;
        public float avgCrit = 0;
        public void Add(miniMinionStat stat)
        {
            stats.Add(stat);
            Update();
        }
        public void Remove(miniMinionStat stat)
        {
            stats.Remove(stat);
            Update();
        }
        void Update()
        {
            avgDmg = 0;
            avgSpd = 0;
            avgCrit = 0;
            if (stats.Count == 0)
            {
                if (megaMinionBody != null && megaMinionBody.active)
                {
                    Deactivate();
                }
                avgSpd = 1;
                return;
            }
            stats.ForEach(i =>
            {
                avgDmg += i.damage;
                avgSpd += i.speed;
                avgCrit += i.crit;
            });
            avgSpd /= stats.Count;
            avgCrit /= stats.Count;

            if (megaMinionBody != null && megaMinionBody.active)
            {
                megaMinionBody.originalDamage = (int)avgDmg;
                megaMinion.ProjectileCrit = (int)avgCrit;
                megaMinion.MinionASMod = avgSpd;
            }
        }
        public void ForceInsertMegaMinion(Projectile projectile)
        {
            if (megaMinionBody != null && megaMinionBody.active)
            {
                megaMinionBody.Kill();
            }

            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            megaMinionBody = projectile;
            megaMinion = projFuncs;
            projectile.originalDamage = (int)avgDmg;
            projFuncs.ProjectileCrit = (int)avgCrit;
            projFuncs.MinionASMod = avgSpd;
        }
        public bool AddMegaMinion(Player player, bool fromServer, IEntitySource projSource)
        {
            if (stats.Count == 0)
                return false;
            if (megaMinionBody != null)
            {
                if (!megaMinionBody.active || megaMinionBody.GetGlobalProjectile<ReworkMinion_Projectile>() != megaMinion)
                {
                    KillMegaMinion();
                    if (player.whoAmI == Main.myPlayer)
                        SpawnMegaMinion(player, projSource);
                }
                else
                {
                    return Reactivate();
                }
            }
            else
            {
                if (player.whoAmI == Main.myPlayer)
                    SpawnMegaMinion(player, projSource);
            }
            return true;
        }
        public void KillMegaMinion()
        {
            megaMinionBody = null;
            megaMinion = null;
        }

        public virtual void Deactivate() { }
        public virtual bool Reactivate() { return false; }
        public virtual void SpawnMegaMinion(Player player, IEntitySource projSource) { }
    }
    public class PirateStatCollection : SummonersShineStatCollection, IMiniMinionStatCollection<PirateStat>
    {
        public MiniPirate.MiniPirateEventWrapper_KickPirate kickPirateWrapper;

        public bool CanFindEnemyTarget = false;

        public override void Deactivate()
        {
            DastardlyDoubloon ship = (DastardlyDoubloon)megaMinionBody.ModProjectile;
            ship.Deactivate();
        }

        public override bool Reactivate()
        {
            DastardlyDoubloon ship = (DastardlyDoubloon)megaMinionBody.ModProjectile;
            return ship.Reactivate();
        }

        public override void SpawnMegaMinion(Player player, IEntitySource projSource)
        {
            player.SpawnMinionOnCursor(projSource, player.whoAmI, ProjectileModIDs.DastardlyDoubloon, (int)avgDmg, 0);
        }
    }
}
