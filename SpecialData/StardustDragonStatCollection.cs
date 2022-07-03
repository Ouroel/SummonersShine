using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace SummonersShine.SpecialData
{
    public class StardustDragonStat : miniMinionStat { }
    public class StardustDragonStatCollection : SpecialDataBase, IMiniMinionStatCollection<StardustDragonStat>
    {

        List<miniMinionStat> stats = new List<miniMinionStat>();
        public List<Projectile> allDragonParts = new();
        public void Remove(miniMinionStat stat)
        {
            allDragonParts.Remove(stat.projectile);
            if (stat.projectile.type == ProjectileID.StardustDragon2)
                stats.Remove(stat);
            Update();
        }
        public void Add(miniMinionStat stat)
        {
            allDragonParts.Add(stat.projectile);
            if (stat.projectile.type == ProjectileID.StardustDragon2)
                stats.Add(stat);
            Update();
        }

        void Update() {

            if (stats.Count == 0)
                return;
            float totalAS = 0;
            float totalCrit = 0;
            float totalDamage = 0;
            float totalPrefixMP = 0;
            float totalKB = 0;
            stats.ForEach(stat =>
            {
                totalAS += stat.speed;
                totalCrit += stat.crit;
                totalDamage += stat.damage;
                totalPrefixMP += stat.prefixMinionPower;
                totalKB += stat.kb;
            });
            int numElements = stats.Count;
            totalAS /= numElements;
            totalCrit /= numElements;
            totalDamage /= numElements;
            totalPrefixMP /= numElements;
            totalKB /= numElements;
            allDragonParts.ForEach(i => {
                i.originalDamage = (int)totalDamage;
                i.knockBack = totalKB;
                ReworkMinion_Projectile dragonPartFuncs = i.GetGlobalProjectile<ReworkMinion_Projectile>();
                dragonPartFuncs.ProjectileCrit = (int)totalCrit;
                dragonPartFuncs.MinionASMod = totalAS;
                dragonPartFuncs.PrefixMinionPower = totalPrefixMP;
            });
        }
    }
}
