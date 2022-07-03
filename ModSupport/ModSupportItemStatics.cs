using Microsoft.Xna.Framework;
using SummonersShine.BakedConfigs;
using SummonersShine.SpecialAbilities;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.ModSupport
{
    public class ModSupportItemStatics
    {
        public static List<ModSupportItemStatics> queued = new();

        int summonItemID;

        Func<Player, Vector2, Entity> specialAbilityFindTarget;
        Func<Player, Item, List<Projectile>, List<Projectile>> specialAbilityFindMinions;
        int minionPowerRechargeTime = 0;
        Tuple<float, int, int, bool>[] minionPowers;

        bool specialActive;

        public class ModSupportItemStatics_Loader : ILoadable
        {
            public void Load(Mod mod)
            {
                queued = new();

            }

            public void Unload()
            {
                queued = null;

            }
        }

        public List<Projectile> PreAbility_Modded(Item summonItem, ReworkMinion_Player player)
        {
            if (specialAbilityFindMinions == null)
                return null;
            List<Projectile> rv = player.GetMinionCollection(summonItem.type).minions.FindAll(i =>
            {
                MinionProjectileData projData = i.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData();
                return projData.energy == projData.maxEnergy;
            });
            return specialAbilityFindMinions(player.Player, summonItem, rv);
        }

        public ModSupportItemStatics(int summonItemID, Func<Player, Vector2, Entity> specialAbilityFindTarget, Func<Player, Item, List<Projectile>, List<Projectile>> specialAbilityFindMinions, Tuple<float, int, int, bool>[] minionPowers, int minionPowerRechargeTime, bool specialActive) {
            this.specialAbilityFindMinions = specialAbilityFindMinions;
            this.specialAbilityFindTarget = specialAbilityFindTarget;
            this.minionPowers = minionPowers;
            this.minionPowerRechargeTime = minionPowerRechargeTime;
            this.summonItemID = summonItemID;
            this.specialActive = specialActive;
        }

        public void Apply() {
            MinionDataHandler.ItemsLoadedStaticDefaults[summonItemID] = true;

            ReworkMinion_Item.minionPowerRechargeTime[summonItemID] = minionPowerRechargeTime;
            if (!BakedConfig.CustomSpecialPowersEnabled(summonItemID))
                return;
            if (specialActive)
                ReworkMinion_Item.defaultSpecialAbilityUsed[summonItemID] = null;
            if (specialAbilityFindTarget != null)
                ReworkMinion_Item.specialAbilityFindTarget[summonItemID] = specialAbilityFindTarget;
            else
                ReworkMinion_Item.specialAbilityFindTarget[summonItemID] = SpecialAbility.FindSpecialAbilityTargetPoint;
            ReworkMinion_Item.specialAbilityFindMinions[summonItemID] = PreAbility_Modded;
            if (minionPowers != null)
            {
                minionPower[] minionPowers_baked = new minionPower[minionPowers.Length];
                ReworkMinion_Item.minionPowers[summonItemID] = minionPowers_baked;
                for (int x = 0; x < minionPowers.Length; x++)
                {
                    Tuple<float, int, int, bool> data = minionPowers[x];
                    minionPowers_baked[x] = minionPower.NewMP(data.Item1, (mpScalingType)data.Item2, (mpRoundingType)data.Item3, data.Item4);
                }
            }
        }
    }
}
