using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SummonersShine.SpecialData
{
    public class DesertTigerStat : miniMinionStat{ }
    public class DesertTigerStatCollection : miniMinionStatCollection, IMiniMinionStatCollection<DesertTigerStat> { }

    public class AbigailCounterStat : miniMinionStat{ }
    public class AbigailStatCollection : miniMinionStatCollection, IMiniMinionStatCollection<AbigailCounterStat> { }

    public abstract class miniMinionStat : SpecialDataBase
    {
        public float speed;
        public float damage;
        public int crit;
        public float prefixMinionPower;
        public float kb;
        public Projectile projectile;

        public miniMinionStat init(Projectile proj, ReworkMinion_Projectile projFuncs) {
            projectile = proj;
            speed = projFuncs.MinionASMod;
            damage = proj.originalDamage;
            crit = projFuncs.ProjectileCrit;
            kb = proj.knockBack;
            prefixMinionPower = projFuncs.PrefixMinionPower;
            return this;
        }
    }
    public interface IMiniMinionStatCollection<T> where T : miniMinionStat
    {
        public void Add(miniMinionStat stat) { }
    }
    public abstract class miniMinionStatCollection : SpecialDataBase, IMiniMinionStatCollection<miniMinionStat>
    {
        List<miniMinionStat> stats = new List<miniMinionStat>();
        public miniMinionStat greatest;
        public ReworkMinion_Projectile megaMinion;
        public Projectile megaMinionBody;
        public float Energy;
        public float EnergyRegenRate;
        public float MaxEnergy;
        public bool inactive = true;

        public void TransferEnergyInCaseOfDeath(float Energy, float EnergyRegenRate, float MaxEnergy)
        {
            if (!inactive)
            {
                this.Energy = Energy;
                this.EnergyRegenRate = EnergyRegenRate;
                this.MaxEnergy = MaxEnergy;
            }
        }
        public virtual void Remove(miniMinionStat stat)
        {
            stats.Remove(stat);
            if (stat == greatest)
            {
                greatest = null;
                stats.ForEach(i =>
                {
                    overthrow(i);
                });
            }
            if (stats.Count == 0)
            {
                Energy = 0;
                EnergyRegenRate = 0;
                MaxEnergy = 0;
                inactive = true;
            }
        }
        public void Add(miniMinionStat stat)
        {
            stats.Add(stat);
            overthrow(stat);
        }

        void overthrow (miniMinionStat stat) {
            float speeda = stat.speed;
            int crita = stat.crit;
            float prefixPowera = stat.prefixMinionPower;
            float kba = stat.kb;
            if (greatest == null)
            {
                greatest = stat;
                FullBake(stat, speeda, crita, prefixPowera, kba);
            }
            float dmga = stat.damage;

            float dmgb = greatest.damage;
            if (dmga > dmgb)
                FullBake(stat, speeda, crita, prefixPowera, kba);
            if (dmga != dmgb)
                return;

            float speedb = greatest.speed;
            if (speeda < speedb)
                FullBake(stat, speeda, crita, prefixPowera, kba);
            if (speeda != speedb)
                return;

            float critb = greatest.crit;
            if (crita > critb)
                FullBake(stat, speeda, crita, prefixPowera, kba);
            if (crita != critb)
                return;

            float prefixPowerb = greatest.prefixMinionPower;
            if (prefixPowera > prefixPowerb)
                FullBake(stat, speeda, crita, prefixPowera, kba);
            if (prefixPowera != prefixPowerb)
                return;

            if (kba > greatest.kb)
                FullBake(stat, speeda, crita, prefixPowera, kba);

        }
        void FullBake(miniMinionStat greatest, float speed, int crit, float prefixPower, float kb)
        {

            this.greatest = greatest;
            if (megaMinion != null)
            {
                megaMinionBody.knockBack = kb;
                megaMinion.MinionASMod = speed;
                megaMinion.ProjectileCrit = crit;
                megaMinion.PrefixMinionPower = prefixPower;
            }
        }

        public void InitMegaMinion(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {

            megaMinionBody = proj;
            megaMinion = projFuncs;

            if (!inactive)
            {
                projData.energy = Energy;
                projData.energyRegenRate = EnergyRegenRate;
                projData.maxEnergy = MaxEnergy;
            }
            inactive = false;

            if (greatest != null)
            {
                proj.knockBack = greatest.kb;
                projFuncs.MinionASMod = greatest.speed;
                projFuncs.ProjectileCrit = greatest.crit;
                projFuncs.PrefixMinionPower = greatest.prefixMinionPower;
            }
            else
                proj.Kill();
        }
    }
}
