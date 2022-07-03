using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace SummonersShine.ModSupport
{
    public abstract class AutoModSupport
    {
        public virtual string modName { get { throw new NotImplementedException(); } }
        public abstract void Load(Mod mod);

        public void Autoload() {

            Mod mod;
            bool success = ModLoader.TryGetMod(modName, out mod);
            if (success)
            {
                Load(mod);
            }
        }

        public static void AutoloadAll()
        {
            new StarsAboveModSupport().Autoload();
            new AOMMModSupport().Autoload();
            new CalamityModSupport().Autoload();
        }
    }
}
