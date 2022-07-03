using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SummonersShine.ModdedMinionILMods
{
    public partial class ModdedMinionILMods
    {
        //This portion specifically makes modded minions work properly. UNDER CONSTRUCTION.
        //EDIT: Seems to be mathematically impossible


        /* This is for the proper loading and unloading of IL mods */
        /*List<Tuple<MethodBase, Delegate>> ModdedILMods = new();

        public void LoadILMod(MethodBase method, Delegate ilMod) {
            ModdedILMods.Add(new Tuple<MethodBase, Delegate>(method, ilMod));
            HookEndpointManager.Modify(method, ilMod);
        }

        public void Unload()
        {
            ModdedILMods.ForEach(i =>
            {
                HookEndpointManager.Unmodify(i.Item1, i.Item2);
            });
            ModdedILMods.Clear();
        }*/
    }
}
