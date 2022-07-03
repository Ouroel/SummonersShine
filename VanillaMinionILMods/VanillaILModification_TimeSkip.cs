using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {

        private static void UpdateClouds_DontAccelerateSurroundings(On.Terraria.Cloud.orig_UpdateClouds orig)
        {
            if (!SingleThreadExploitation.acceleratingTime)
            {
                orig();
                return;
            }
            double dayRate = Main.dayRate;
            Main.dayRate = SingleThreadExploitation.originalDayRate;
            orig();
            Main.dayRate = dayRate;
        }
        private static void UpdateSkyManager_DontAccelerateSurroundings(On.Terraria.Graphics.Effects.SkyManager.orig_Update orig, Terraria.Graphics.Effects.SkyManager self, GameTime gameTime)
        {
            if (!SingleThreadExploitation.acceleratingTime)
            {
                orig(self, gameTime);
                return;
            }
            double worldEventUpdateRate = Main.desiredWorldEventsUpdateRate;
            Main.desiredWorldEventsUpdateRate = SingleThreadExploitation.originalRate;
            orig(self, gameTime);
            Main.desiredWorldEventsUpdateRate = worldEventUpdateRate;
        }

        private static void UpdateTimeRate_ModifyTimeRate(On.Terraria.Main.orig_UpdateTimeRate orig)
        {
            orig();
            if (Config.current.IsBloodtaintMode() && Main.dayTime && Main.time > 3600 && Main.time < 50400 && !Main.slimeRain && !Main.eclipse)
            {
                bool accelerate = true;
                if (accelerate)
                    for (int i = 0; i < Main.npc.Length; i++)
                    {
                        NPC test = Main.npc[i];
                        if (test != null && test.active && test.boss)
                        {
                            accelerate = false;
                            break;
                        }
                    }
                if (accelerate)
                {
                    SingleThreadExploitation.originalDayRate = Main.dayRate;
                    SingleThreadExploitation.originalRate = Main.desiredWorldEventsUpdateRate;
                    Main.dayRate *= 40;
                    Main.desiredWorldTilesUpdateRate *= 40;
                    Main.desiredWorldEventsUpdateRate *= 40;
                    SingleThreadExploitation.acceleratingTime = true;
                }
                else
                    SingleThreadExploitation.acceleratingTime = false;
            }
            else
                SingleThreadExploitation.acceleratingTime = false;
        }
    }
}
