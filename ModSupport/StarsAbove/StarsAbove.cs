using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.ModSupport
{
    public class StarsAboveModSupport : AutoModSupport
    {
        public override string modName => "StarsAbove";

        public override void Load(Mod mod)
        {
            ModProjectile proj = mod.Find<ModProjectile>("TemporalTimepiece2");
            SummonersShine.modInstance.Call(1, proj.Type, 23, (int)1);

            proj = mod.Find<ModProjectile>("TemporalTimepiece3");
            SummonersShine.modInstance.Call(1, proj.Type, 23, (int)1);

            proj = mod.Find<ModProjectile>("Melusine");
            SummonersShine.modInstance.Call(1, proj.Type, 23, (int)1);

            proj = mod.Find<ModProjectile>("Arondight");
            SummonersShine.modInstance.Call(1, proj.Type, 23, (int)1);

            proj = mod.Find<ModProjectile>("MagicCircle");
            SummonersShine.modInstance.Call(0, 1, proj.Type);
            proj = mod.Find<ModProjectile>("TentacleCircle");
            SummonersShine.modInstance.Call(0, 1, proj.Type);
            proj = mod.Find<ModProjectile>("TakonomiconLaser");
            SummonersShine.modInstance.Call(0, 1, proj.Type);
            proj = mod.Find<ModProjectile>("TakodachiRound");
            SummonersShine.modInstance.Call(0, 4, proj.Type);
            proj = mod.Find<ModProjectile>("Tentacle1");
            SummonersShine.modInstance.Call(0, 4, proj.Type);
            proj = mod.Find<ModProjectile>("Tentacle2");
            SummonersShine.modInstance.Call(0, 4, proj.Type);
            proj = mod.Find<ModProjectile>("Tentacle3");
            SummonersShine.modInstance.Call(0, 4, proj.Type);

            proj = mod.Find<ModProjectile>("YoumuRound");
            SummonersShine.modInstance.Call(0, 4, proj.Type);
            proj = mod.Find<ModProjectile>("IrysBolt");
            SummonersShine.modInstance.Call(0, 4, proj.Type);
            proj = mod.Find<ModProjectile>("BlueStarBit");
            SummonersShine.modInstance.Call(0, 4, proj.Type);
            proj = mod.Find<ModProjectile>("GreenStarBit");
            SummonersShine.modInstance.Call(0, 4, proj.Type);
            proj = mod.Find<ModProjectile>("OrangeStarBit");
            SummonersShine.modInstance.Call(0, 4, proj.Type);
            proj = mod.Find<ModProjectile>("RedStarBit");
            SummonersShine.modInstance.Call(0, 4, proj.Type);
            proj = mod.Find<ModProjectile>("PurpleStarBit");
            SummonersShine.modInstance.Call(0, 4, proj.Type);
            proj = mod.Find<ModProjectile>("YellowStarBit");
            SummonersShine.modInstance.Call(0, 4, proj.Type);
            proj = mod.Find<ModProjectile>("MelusineBeam");
            SummonersShine.modInstance.Call(0, 4, proj.Type);
            proj = mod.Find<ModProjectile>("ArondightBeam");
            SummonersShine.modInstance.Call(0, 4, proj.Type);
            proj = mod.Find<ModProjectile>("Icebolt");
            SummonersShine.modInstance.Call(0, 4, proj.Type);
            proj = mod.Find<ModProjectile>("Icebolt2");
            SummonersShine.modInstance.Call(0, 4, proj.Type);
            proj = mod.Find<ModProjectile>("SatanaelRound");
            SummonersShine.modInstance.Call(0, 4, proj.Type);

            ModBuff buff = mod.Find<ModBuff>("StarchildBuff");
            ModItem item = mod.Find<ModItem>("LuminaryWand");
            SummonersShine.modInstance.Call(0, 9, buff.Type, item.Type);

            buff = mod.Find<ModBuff>("IrysBuff");
            item = mod.Find<ModItem>("CaesuraOfDespair");
            SummonersShine.modInstance.Call(0, 9, buff.Type, item.Type);

            buff = mod.Find<ModBuff>("PhantomMinion");
            item = mod.Find<ModItem>("KonpakuKatana");
            SummonersShine.modInstance.Call(0, 9, buff.Type, item.Type);

            item = mod.Find<ModItem>("Kifrosse");
            buff = mod.Find<ModBuff>("KifrosseBuff1");
            SummonersShine.modInstance.Call(0, 9, buff.Type, item.Type);
            buff = mod.Find<ModBuff>("KifrosseBuff2");
            SummonersShine.modInstance.Call(0, 9, buff.Type, item.Type);
            buff = mod.Find<ModBuff>("KifrosseBuff3");
            SummonersShine.modInstance.Call(0, 9, buff.Type, item.Type);
            buff = mod.Find<ModBuff>("KifrosseBuff4");
            SummonersShine.modInstance.Call(0, 9, buff.Type, item.Type);
            buff = mod.Find<ModBuff>("KifrosseBuff5");
            SummonersShine.modInstance.Call(0, 9, buff.Type, item.Type);
            buff = mod.Find<ModBuff>("KifrosseBuff6");
            SummonersShine.modInstance.Call(0, 9, buff.Type, item.Type);
            buff = mod.Find<ModBuff>("KifrosseBuff7");
            SummonersShine.modInstance.Call(0, 9, buff.Type, item.Type);
            buff = mod.Find<ModBuff>("KifrosseBuff8");
            SummonersShine.modInstance.Call(0, 9, buff.Type, item.Type);
            buff = mod.Find<ModBuff>("KifrosseBuff9");
            SummonersShine.modInstance.Call(0, 9, buff.Type, item.Type);
        }
    }
}
