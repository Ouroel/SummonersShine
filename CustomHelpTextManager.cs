using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SummonersShine
{
    public static class CustomHelpTextManager
    {

        public static Func<bool>[] helpText;

        public class CustomHelpTextManager_Loader : ILoadable
        {
            public void Load(Mod mod)
            {
                helpText = new Func<bool>[]
                {
                    GuideHelpText_Summon,
                };
            }

            public void Unload()
            {
                helpText = null;
            }
        }
        public static bool GetHelpText(int helpTextNumber, int startNumber)
        {
            helpTextNumber -= startNumber;
            if (helpTextNumber >= 0 && helpTextNumber < helpText.Length)
                return helpText[helpTextNumber]();
            return false;
        }

        static bool GuideHelpText_Summon()
        {
            object subObj = Lang.CreateDialogSubstitutionObject(null);

            bool rv = false;
            for (int i = 0; i < 58; i++)
            {
                if (Main.player[Main.myPlayer].inventory[i].DamageType == Terraria.ModLoader.DamageClass.Summon)
                {
                    rv = true;
                    break;
                }
            }
            if (rv)
                Main.npcChatText = ReworkMinion_Item.RightClickTextReplacer(Language.FindAll(Lang.CreateDialogFilter(SummonersShine.NPCDialogPath + "Guide_Help", subObj))[0].Value);
            return rv;
        }
    }
}
