using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SummonersShine.SpecialData
{
    public class RightClickReactionCollection : SpecialDataBase
    {
        public List<ProjStuff> reactionCollection = new();

        public void Add(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData) {
            reactionCollection.Add(new ProjStuff(projectile, projFuncs, projData));
        }

        public void Remove(Projectile projectile) {
            reactionCollection.RemoveAll(i => i.proj == projectile);
        }

        public class ProjStuff
        {
            public Projectile proj;
            public ReworkMinion_Projectile projFuncs;
            public MinionProjectileData projData;

            public ProjStuff(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
            {
                this.proj = proj; this.projFuncs = projFuncs; this.projData = projData;
            }
        }
    }
}
