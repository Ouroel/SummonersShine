using SummonersShine.SpecialAbilities.DefaultSpecialAbility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.BakedConfigs
{
    public static class BloodtaintMode
    {
        public static void Activate()
        {
            const float BLOODTAINT_MODE_OUTGOINGDMGMOD = 0.7f;
            const int INSTASTRIKE = 2;

            Item testItem = new();
            Projectile testProj = new();
            for (int x = 0; x < ItemLoader.ItemCount; x++)
            {

                testItem.SetDefaults(x);
                testProj.SetDefaults(testItem.shoot);
                bool? valid = BakedConfig.IsDefaultSpecialAbilityWhitelisted(testItem.type, INSTASTRIKE);
                if (valid == null)
                {
                    valid = DefaultSpecialAbility.abilities[INSTASTRIKE].ValidForItem(testItem, testProj);
                }

                bool validForItem = valid.Value;
                if (validForItem)
                {
                    BakedConfig.DefaultAbilityType[x] = INSTASTRIKE + 1;
                    BakedConfig.DefaultAbility_MinionPower0[x] = 70;
                    BakedConfig.ItemIgnoresCustomSpecialPower[x] = true;
                }
            }
            for (int i = 0; i < ProjectileLoader.ProjectileCount; i++)
            {
                BakedConfig.MinionOutgoingDamageMod[i] = BLOODTAINT_MODE_OUTGOINGDMGMOD;
            }

            BakedConfig.DefaultAbility_MinionPower0[ItemID.AbigailsFlower] = 50;
            BakedConfig.DefaultAbility_MinionPower0[ItemID.StormTigerStaff] = 40;
            BakedConfig.DefaultAbility_MinionPower0[ItemID.StardustDragonStaff] = 40;
        }
    }
}
