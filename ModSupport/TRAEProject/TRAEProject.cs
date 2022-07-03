using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.ModSupport
{
    public class TRAEModSupport : AutoModSupport
    {
        public override string modName => "TRAEProject";

        public override void Load(Mod mod)
        {
            SummonersShine.modInstance.Call(0, 3, ProjectileID.Retanimini);
        }
    }
}
