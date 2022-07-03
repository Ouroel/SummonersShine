using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.ModSupport
{
    public class ModSupportBuff
    {
        public static List<ModSupportBuff> ModSupportBuffs = new();
        public Func<int, int, List<Projectile>, Tuple<bool, float, float>> OverrideGetPinPositions;
        public Func<int, int[]> OverrideGetLinkedItems;

        public int BuffID;
        //public delegate Tuple<bool, float, float> GetPinPositions(int type, int itemType, List<Projectile> valid);
        public static ModSupportBuff Generate(int BuffID)
        {
            ModSupportBuff rv = ModSupportBuffs.Find(i => i.BuffID == BuffID);
            if (rv == null)
            {
                rv = new(BuffID);
                ModSupportBuffs.Add(rv);
            }
            return rv;
        }
        public class ModSupportBuff_Loader : ILoadable
        {
            public void Load(Mod mod)
            {
                ModSupportBuffs = new();

            }

            public void Unload()
            {
                ModSupportBuffs = null;

            }
        }
        public ModSupportBuff(int BuffID)
        {
            this.BuffID = BuffID;
        }
        public void AssignHook(int hookNum, object hook)
        {
            switch (hookNum)
            {
                case 0:
                    OverrideGetPinPositions = (Func<int, int, List<Projectile>, Tuple<bool, float, float>>)hook;
                    return;
                case 1:
                    OverrideGetLinkedItems = (Func<int, int[]>)hook;
                    return;
            }
        }
    }
}
