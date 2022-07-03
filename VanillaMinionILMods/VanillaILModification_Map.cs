/*using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SummonersShine.MinionAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {
        const int energyDisplayHeight = 40;

        const int originalOffset_Map = 90;
        const int originalOffset_InfoAccWithoutInv = -32;
        const int originalOffset_InfoAccWithInv = 94;

        const int originalShouldDrawAccessoriesHorizontallyThreshold = 820;
        const int originalMinScreenHeightWithMap = 950;

        const int originalAccessoriesDisplay = 174;

        const int originalSettingsButtonTheshold = 620;
        const int originalSettingsButtonThesholdWithMap = 870;

        static int[] screenHeightComparisonNumbers = {
            630,
            680,
            730,
        };
        public static void UpdateMinimapAngles_ChangeValue(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            for (int x = 0; x < 3; x++)
            {
                if (!c.TryGotoNext(i => i.MatchLdcI4(originalOffset_Map)))
                {
                    SummonersShine.logger.Error("[UpdateMinimapAngles_ChangeValue] Hook failed! Can't find LdcI4(originalOffset_Map)");
                    return;
                }
            }
            c.Index++;
            c.EmitDelegate<Func<int, int>>(AddEnergyDisplay);
        }
        public static void GetInfoAccPosition_ChangeValue(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (!c.TryGotoNext(i => i.MatchLdcI4(originalOffset_InfoAccWithoutInv)))
            {
                SummonersShine.logger.Error("[UpdateMinimapAngles_ChangeValue] Hook failed! Can't find LdcI4(originalOffset_InfoAccWithoutInv)");
                return;
            }
            c.Index++;
            c.EmitDelegate<Func<int, int>>(AddEnergyDisplay);

            if (!c.TryGotoNext(i => i.MatchLdcI4(originalOffset_InfoAccWithInv)))
            {
                SummonersShine.logger.Error("[UpdateMinimapAngles_ChangeValue] Hook failed! Can't find LdcI4(originalOffset_InfoAccWithInv)");
                return;
            }
            c.Index++;
            c.EmitDelegate<Func<int, int>>(AddEnergyDisplay);
        }

        static BasicILModification del_get_ShouldDrawInfoIconsHorizontally_ChangeValue = get_ShouldDrawInfoIconsHorizontally_ChangeValue;
        public static void get_ShouldDrawInfoIconsHorizontally_ChangeValue(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (!c.TryGotoNext(i => i.MatchLdcI4(originalShouldDrawAccessoriesHorizontallyThreshold)))
            {
                SummonersShine.logger.Error("[ShouldDrawInfoIconsHorizontally_get_ChangeValue] Hook failed! Can't find LdcI4(originalMinScreenHeightNoMap)");
                return;
            }
            c.Index++;
            c.EmitDelegate<Func<int, int>>(AddEnergyDisplay);
        }
        public static void DrawInventory_ChangeValue(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchLdcI4(originalAccessoriesDisplay)))
            {
                SummonersShine.logger.Error("[DrawInventory_ChangeValue] Hook failed! Can't find LdcI4(originalAccessoriesDisplay)");
                return;
            }
            c.Index++;
            c.EmitDelegate<Func<int, int>>(AddEnergyDisplay);
           
            if (!c.TryGotoNext(i => i.MatchLdcI4(originalMinScreenHeightWithMap)))
            {
                SummonersShine.logger.Error("[DrawInventory_ChangeValue] Hook failed! Can't find LdcI4(originalMinScreenHeightWithMap)");
                return;
            }
            c.Index++;
            c.EmitDelegate<Func<int, int>>(AddEnergyDisplay);

            if (!c.TryGotoNext(i => i.MatchLdloc(9)))
           {
               SummonersShine.logger.Error("[DrawInventory_ChangeValue] Hook failed! Can't find MatchLdloc(9)");
               return;
           }
           c.Index++;
           c.Remove();
           c.Emit(OpCodes.Ldc_I4, 9);
           c.Index++;

           if (!c.TryGotoNext(i => i.MatchLdloc(9)))
           {
               SummonersShine.logger.Error("[DrawInventory_ChangeValue] Hook failed! Can't find MatchLdloc(9)");
               return;
           }
           c.Index++;
           c.Remove();
           c.Emit(OpCodes.Ldc_I4, 8);

            c.Index++;

            if (!c.TryGotoNext(i => i.MatchLdcI4(originalAccessoriesDisplay)))
            {
                SummonersShine.logger.Error("[DrawInventory_ChangeValue] Hook failed! Can't find LdcI4(originalAccessoriesDisplay)");
                return;
            }
            c.Index++;
            c.EmitDelegate<Func<int, int>>(AddEnergyDisplay);

            for (int x = 0; x < screenHeightComparisonNumbers.Length; x++) {

                if (!c.TryGotoNext(i => i.MatchLdcI4(screenHeightComparisonNumbers[x])))
                {
                    SummonersShine.logger.Error("[DrawInventory_ChangeValue] Hook failed! Can't find LdcI4(originalAccessoriesDisplay)");
                    return;
                }
                c.Index++;
                c.EmitDelegate<Func<int, int>>(AddEnergyDisplay);
            }
        }
        public static void DrawNPCHousesInUI_ChangeValue(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            for (int i = 0; i < 5; i++)
            {
                if (!c.TryGotoNext(i => i.MatchLdcI4(originalAccessoriesDisplay)))
                {
                    SummonersShine.logger.Error("[DrawNPCHousesInUI_ChangeValue] Hook failed! Can't find LdcI4(originalAccessoriesDisplay)");
                    return;
                }
                c.Index++;
                c.EmitDelegate<Func<int, int>>(AddEnergyDisplay);
            }
        }

        public static void DrawInterface_29_SettingsButton_ChangeValue(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchLdcI4(originalSettingsButtonTheshold)))
            {
                SummonersShine.logger.Error("[DrawInterface_29_SettingsButton_ChangeValue] Hook failed! Can't find LdcI4(originalSettingsButtonTheshold)");
                return;
            }
            c.Index++;
            c.EmitDelegate<Func<int, int>>(AddEnergyDisplay);
            c.Index++;

            if (!c.TryGotoNext(i => i.MatchLdcI4(originalSettingsButtonThesholdWithMap)))
            {
                SummonersShine.logger.Error("[DrawInterface_29_SettingsButton_ChangeValue] Hook failed! Can't find LdcI4(originalSettingsButtonThesholdWithMap)");
                return;
            }
            c.Index++;
            c.EmitDelegate<Func<int, int>>(AddEnergyDisplay);
        }
        public static int AddEnergyDisplay(int originalThreshold)
        {
            return originalThreshold + energyDisplayHeight;
        }

        static BasicILModification del_get_UIScaleMax_ModifyValues = get_UIScaleMax_ModifyValues;
        public static void get_UIScaleMax_ModifyValues(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchLdcR4(600)))
            {
                SummonersShine.logger.Error("[get_UIScaleMax_ModifyValues] Hook failed! Can't find LdcI4(600)");
                return;
            }
            c.Remove();
            c.Emit(OpCodes.Ldc_R4, (float)(600 + energyDisplayHeight));
        }
    }
}*/