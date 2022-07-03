using MonoMod.Cil;
using SummonersShine.Prefixes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {

        private static void DrawInventory_PostReforgeEncapsulate(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (!c.TryGotoNext(i => i.MatchCall("Terraria.ModLoader.ItemLoader", "PostReforge")))
            {
                SummonersShine.logger.Error("[DrawInventory_PostReforgeEncapsulate] ItemLoader.PostReforge not found!");
                return;
            }
            c.EmitDelegate(SingleThreadExploitation.SetReforgingTrue);
            c.Index++;
            c.EmitDelegate(SingleThreadExploitation.SetReforgingFalse);
        }
        private static bool Prefix_ConvertCalaPrefixes(On.Terraria.Item.orig_Prefix orig, Item self, int pre)
        {
            if (SingleThreadExploitation.Reforging && self.CountsAsClass(DamageClass.Summon))
            {
                //retier mythical and ruthless
                if (pre == PrefixID.Mythical)
                    pre = PrefixID.Ruthless;
                else if (pre == PrefixID.Ruthless)
                    pre = PrefixID.Mythical;

                switch (pre)
                {
                    case PrefixID.Legendary:
                    case PrefixID.Legendary2:
                    case PrefixID.Mythical:
                        pre = SummonerPrefix.Divine;
                        break;
                    case PrefixID.Savage:
                    case PrefixID.Masterful:
                        pre = SummonerPrefix.Remphanic;
                        break;
                    case PrefixID.Massive:
                    case PrefixID.Sharp:
                    case PrefixID.Mystic:
                        pre = SummonerPrefix.Bloodbound;
                        break;
                    case PrefixID.Dangerous:
                    case PrefixID.Celestial:
                        pre = SummonerPrefix.Shining;
                        break;
                    case PrefixID.Large:
                    case PrefixID.Adept:
                        pre = SummonerPrefix.Initiated;
                        break;
                    case PrefixID.Pointy:
                    case PrefixID.Bulky:
                    case PrefixID.Taboo:
                        pre = SummonerPrefix.Devoted;
                        break;
                    case PrefixID.Heavy:
                    case PrefixID.Manic:
                        pre = SummonerPrefix.Righteous;
                        break;
                    case PrefixID.Light:
                    case PrefixID.Furious:
                        pre = SummonerPrefix.Fanatical;
                        break;
                    case PrefixID.Small:
                    case PrefixID.Dull:
                    case PrefixID.Intense:
                        pre = SummonerPrefix.Heretical;
                        break;
                    case PrefixID.Tiny:
                    case PrefixID.Inept:
                        pre = SummonerPrefix.Lunatic;
                        break;
                    case PrefixID.Shameful:
                    case PrefixID.Deranged:
                        pre = SummonerPrefix.Unholy;
                        break;
                    case PrefixID.Unhappy:
                    case PrefixID.Terrible:
                    case PrefixID.Ignorant:
                        pre = SummonerPrefix.Benzona;
                        break;
                }
            }
            return orig(self, pre);
        }
    }
}
