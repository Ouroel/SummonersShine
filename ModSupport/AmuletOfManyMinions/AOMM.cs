using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.ModSupport
{
    public class AOMMModSupport : AutoModSupport
    {
        public override string modName => "AmuletOfManyMinions";

        public override void Load(Mod mod)
        {
            ModProjectile proj = mod.Find<ModProjectile>("CharredChimeraMinionHead");
            SummonersShine.modInstance.Call(0, 5, proj.Type);

            proj = mod.Find<ModProjectile>("TerrarianEntMinion");
            SummonersShine.modInstance.Call(1, proj.Type, 23, (int)1);

            proj = mod.Find<ModProjectile>("LandChunkProjectile");
            SummonersShine.modInstance.Call(0, 5, proj.Type);
        }
    }
}
