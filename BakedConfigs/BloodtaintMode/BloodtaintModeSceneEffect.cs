using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace SummonersShine
{
    public class BloodtaintModeSceneEffect : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player)
        {
            bool rv = Config.current.IsBloodtaintMode() && !(Main.SceneMetrics.ActiveMonolithType != -1 || Main.player[Main.myPlayer].ZoneTowerNebula || Main.player[Main.myPlayer].ZoneTowerSolar || Main.player[Main.myPlayer].ZoneTowerStardust || Main.player[Main.myPlayer].ZoneTowerVortex);
            if(!rv)
                SkyManager.Instance.Deactivate("SummonersShine:BloodtaintModeSky");
            return rv;
        }
        /*public override void SpecialVisuals(Player player)
        {
                SkyManager.Instance.Activate("SummonersShine:BloodtaintModeSky");
        }*/
        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (isActive)
                SkyManager.Instance.Activate("SummonersShine:BloodtaintModeSky");
            else
                SkyManager.Instance.Deactivate("SummonersShine:BloodtaintModeSky");
        }
        public override SceneEffectPriority Priority
        {
            get
            {
                return SceneEffectPriority.None;
            }
        }
    }
}
