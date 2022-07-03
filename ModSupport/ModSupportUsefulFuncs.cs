using SummonersShine.BakedConfigs;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SummonersShine.ModSupport
{
    public static class ModSupportUsefulFuncs
    {
        public static float Projectile_GetSpeed(Projectile projectile)
        {
            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            return projFuncs.GetSpeed(projectile);
        }
        public static float Projectile_GetSimulationRate(Projectile projectile)
        {
            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            return projFuncs.GetSimulationRate(projectile);
        }
        public static float Projectile_GetInternalSimRate(Projectile projectile)
        {
            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            return projFuncs.GetInternalSimRate(projectile);
        }

        public static float Item_GetMinionPower(Player player, Item item, int index)
        {
            return item.GetGlobalItem<ReworkMinion_Item>().GetMinionPower(player, item.type, index);
        }
        public static float Projectile_GetMinionPower(Projectile proj, int index) {
            return proj.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionPower(proj, index);
        }

        public static bool Projectile_IsOnRealTick(Projectile proj)
        {
            return ProjectileMethods.IsOnRealTick(proj, proj.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData());
        }

        public static bool Projectile_IsCastingSpecialAbility(Projectile projectile, int ItemID)
        {
            return ModUtils.IsCastingSpecialAbility(projectile.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData(), ItemID);
        }

        public static bool Item_IsMinionPowerEnabled(int ItemID)
        {
            return BakedConfig.CustomSpecialPowersEnabled(ItemID);
        }
        public static bool Projectile_IsMinionPowerEnabled(Projectile projectile)
        {
            return BakedConfig.CustomSpecialPowersEnabled(projectile.GetGlobalProjectile<ReworkMinion_Projectile>().SourceItem);
        }

        public static Tuple<float, float, int, int, bool> Projectile_GetAllMinionPowerData(Projectile proj, int index) {
            ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
            float rv = projFuncs.GetMinionPower(proj, index);
            minionPower powers = ReworkMinion_Item.minionPowers[projFuncs.SourceItem][index];
            return new Tuple<float, float, int, int, bool>(rv, powers.power, (int)powers.scalingType, (int)powers.roundingType, powers.DifficultyScale);
        }

        public static Tuple<float, float, int, int, bool> Item_GetAllMinionPowerData(Player player, Item item, int index)
        {
            ReworkMinion_Item itemFuncs = item.GetGlobalItem<ReworkMinion_Item>();
            float rv = itemFuncs.GetMinionPower(player, item.type, index);
            minionPower powers = ReworkMinion_Item.minionPowers[item.type][index];
            return new Tuple<float, float, int, int, bool>(rv, powers.power, (int)powers.scalingType, (int)powers.roundingType, powers.DifficultyScale);
        }

        public static void Projectile_OverrideSourceItem(Projectile proj, int sourceItem)
        {
            ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
            projFuncs.OverrideSourceItem(proj, sourceItem);
        }
        public static object RunUsefulFunction(int funcIndex, object[] args)
        {
            switch(funcIndex)
            {
                case 0:
                    ReworkMinion_Projectile.SetMoveTarget((Projectile)args[0], (Entity)args[1]);
                    return null;
                case 1:
                    ReworkMinion_Projectile.SetMoveTarget_FromID((Projectile)args[0], (int)args[1]);
                    return null;
                case 2:
                    return Projectile_IsOnRealTick((Projectile)args[0]);
                case 3:
                    return Projectile_GetMinionPower((Projectile)args[0], (int)args[1]);
                case 4:
                    return Projectile_GetSpeed(args[0] as Projectile);
                case 5:
                    return Projectile_GetSimulationRate(args[0] as Projectile);
                case 6:
                    return Projectile_GetInternalSimRate(args[0] as Projectile);
                case 7:
                    return ReworkMinion_Projectile.GetTotalProjectileVelocity((Microsoft.Xna.Framework.Vector2)args[0], (float)args[1], (Projectile)args[2], (NPC)args[3]);
                case 8:
                    return Projectile_IsCastingSpecialAbility(args[0] as Projectile, (int)args[1]);
                case 9:
                    Projectile proj = args[0] as Projectile;
                    ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
                    return proj.IncrementSpecialAbilityTimer(projFuncs, projFuncs.GetMinionProjData(), (int)args[1], (float)args[2]);
                case 10:
                    return Projectile_GetAllMinionPowerData((Projectile)args[0], (int)args[1]);
                case 11:
                    Projectile_OverrideSourceItem((Projectile)args[0], (int)args[1]);
                    return null;
                case 12:
                    return Item_IsMinionPowerEnabled((int)args[0]);
                case 13:
                    return Projectile_IsMinionPowerEnabled((Projectile)args[0]);
                case 14:
                    MinionEnergyCounter counter = ((Player)args[0]).GetModPlayer<ReworkMinion_Player>().GetMinionCollection((int)args[1]);
                    return new Tuple<List<Projectile>, List<float>>(counter.minions, counter.minionFullPercentage);
                case 15:
                    Item item = ((Item)args[0]);
                    ReworkMinion_Item itemFuncs = item.GetGlobalItem<ReworkMinion_Item>();
                    bool usingSpecial = itemFuncs.UsingSpecialAbility;
                    itemFuncs.UseSpecialAbility(item, (Player)args[1], true);
                    itemFuncs.UsingSpecialAbility = usingSpecial;
                    break;
                case 16:
                    return Item_GetMinionPower((Player)args[0], (Item)args[1], (int)args[2]);
                case 17:
                    return Item_GetAllMinionPowerData((Player)args[0], (Item)args[1], (int)args[2]);
                case 18:
                    ((Player)args[0]).GetModPlayer<ReworkMinion_Player>().AddNewThought((int)args[1], (bool)args[2], (bool)args[3]);
                    break;

            }
            return null;
        }
    }
}
