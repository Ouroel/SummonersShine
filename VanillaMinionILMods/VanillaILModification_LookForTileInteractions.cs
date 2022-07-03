using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Capture;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {

        private static void LookForTileInteractions_PrioritizeRightClickSpecial(On.Terraria.Player.orig_LookForTileInteractions orig, Player self)
        {

            Item item = self.inventory[self.selectedItem];
            if (item != null && item.active && !item.IsAir)
            {
                ReworkMinion_Item itemFuncs = item.GetGlobalItem<ReworkMinion_Item>();
                bool canGoAhead = itemFuncs.UseSpecialAbility(item, self);
                if (!canGoAhead)
                {
                    orig(self);
                }
                else
                {
                    self.controlUseItem = true;
                }
            }
            else
                orig(self);
        }

        private static void ItemCheck_Inner_SetAltUse(On.Terraria.Player.orig_ItemCheck_Inner orig, Player self, int i)
        {
            Item item = self.inventory[self.selectedItem];
            if (item != null && item.active && !item.IsAir)
            {
                ReworkMinion_Item itemFuncs = item.GetGlobalItem<ReworkMinion_Item>();
                if (itemFuncs.UsingSpecialAbility)
                    self.altFunctionUse = 1;
            }
            orig(self, i);
        }

        private static void ManageRightClickFeatures_DontOpenChesterWhenCastingSpecial(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (!c.TryGotoNext(i => i.MatchLdfld<Player>("altFunctionUse")))
            {
                SummonersShine.logger.Error("[ManageRightClickFeatures_DontOpenChesterWhenCastingSpecial] Player.altFunctionUse not found!");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(UsingSpecialAbilityExclusion);
        }

        static bool UsingSpecialAbilityExclusion(int altFunctionUse, Player player)
        {
            if (altFunctionUse != 0)
                return true;
            Item item = player.inventory[player.selectedItem];
            if (item != null && item.active && !item.IsAir)
            {
                ReworkMinion_Item itemFuncs = item.GetGlobalItem<ReworkMinion_Item>();
                return itemFuncs.UsingSpecialAbility;
            }
            return false;
        }
        private static void Main_TryInteractingWithX_DontOpenChesterWhenCastingSpecial(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.Index++;
            c.EmitDelegate(UsingSpecialAbilityExclusion_TryInteractingWithX);
        }
        static bool UsingSpecialAbilityExclusion_TryInteractingWithX(bool orig)
        {
            if (orig)
                return true;
            Player player = Main.player[Main.myPlayer];
            Item item = player.inventory[player.selectedItem];
            if (item != null && item.active && !item.IsAir)
            {
                ReworkMinion_Item itemFuncs = item.GetGlobalItem<ReworkMinion_Item>();
                return itemFuncs.UsingSpecialAbility;
            }
            return false;
        }
    }
}
