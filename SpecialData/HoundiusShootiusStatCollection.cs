using SummonersShine.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace SummonersShine.SpecialData
{
    public class HoundiusShootiusStat : miniMinionStat { }
    public class HoundiusShootiusStatCollection : SummonersShineStatCollection, IMiniMinionStatCollection<HoundiusShootiusStat>
    {
        List<Projectile> houndiusCasted = new();
        public override void SpawnMegaMinion(Player player, IEntitySource projSource)
        {
            player.SpawnMinionOnCursor(projSource, player.whoAmI, ProjectileModIDs.AncientGuardian, (int)avgDmg, 0);
        }
        public override void Deactivate()
        {
            (megaMinionBody.ModProjectile as AncientGuardian).Deactivate();
        }

        public override bool Reactivate()
        {
            return (megaMinionBody.ModProjectile as AncientGuardian).Reactivate();
        }

        int facing = 0;

        public void DeleteHoundiusShootius(Projectile projectile)
        {
            houndiusCasted.Remove(projectile);
        }
        public List<Projectile> GetHoundiusShootiusFromValidList(List<Projectile> projectiles)
        {
            if (projectiles.Count == 0)
                return projectiles;
            Projectile rv = projectiles.Find(i => !houndiusCasted.Contains(i));
            if (rv == null)
            {
                rv = houndiusCasted[0];
                houndiusCasted.RemoveAt(0);
            }
            houndiusCasted.Add(rv);
            projectiles.Clear();
            projectiles.Add(rv);
            rv.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData().castingSpecialAbilityType = facing;
            facing++;
            if (facing > 1)
                facing = 0;
            return projectiles;
        }
    }
}
