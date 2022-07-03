using SummonersShine.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine
{
    class ReworkMinion_World : ModSystem
    {
        public static bool WorldLoaded = false;
        public override void OnWorldLoad()
        {
            CustomProjectileDrawLayer.Clear();
            EnergyDisplay.AllDisplays.Clear();
            PlatformCollection.ClearPlatforms();
            Main.tooltipPrefixComparisonItem = null;
            WorldLoaded = true;
        }

        public override void PreSaveAndQuit()
        {
            
            //WorldLoaded = false;
        }
    }
}
