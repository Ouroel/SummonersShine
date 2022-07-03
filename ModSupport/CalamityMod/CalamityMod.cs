using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.ModSupport
{
    public class CalamityModSupport : AutoModSupport
    {
        public override string modName => "CalamityMod";

        public override void Load(Mod mod)
        {
        }

        void LinkItemBuff(Mod mod, string BuffName, string ItemName)
        {
            ModItem item = mod.Find<ModItem>(ItemName);
            ModBuff buff = mod.Find<ModBuff>(BuffName);
            SummonersShine.modInstance.Call(0, 9, buff.Type, item.Type);
        }
        void LinkItemBuff(Mod mod, string BuffName, string[] ItemName)
        {
            int[] itemTypes = new int[ItemName.Length];
            for (int x = 0; x < ItemName.Length; x++)
            {
                ModItem item = mod.Find<ModItem>(ItemName[x]);
                itemTypes[x] = item.Type;
            }
            ModBuff buff = mod.Find<ModBuff>(BuffName);
            SummonersShine.modInstance.Call(0, 9, buff.Type, itemTypes);
        }
        void ForceProjectileUnminion(Mod mod, string name)
        {
            ModProjectile proj = mod.Find<ModProjectile>(name);
            SummonersShine.modInstance.Call(0, 4, proj.Type);
        }
    }
}
