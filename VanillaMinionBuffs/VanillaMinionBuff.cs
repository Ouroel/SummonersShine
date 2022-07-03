using SummonersShine.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.VanillaMinionBuffs
{
    public abstract class DD2Buff : MinionBuff
    {
        public virtual int projectileIDt2 => -1;
        public virtual int projectileIDt3 => -1;
        public virtual int itemIDt2 => -1;
        public virtual int itemIDt3 => -1;
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[projectileID] + player.ownedProjectileCounts[projectileIDt2] + player.ownedProjectileCounts[projectileIDt3] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
        public override bool RightClick(int buffIndex)
        {
            List<Projectile> projectiles = new();
            ReworkMinion_Player playerFuncs = Main.player[Main.myPlayer].GetModPlayer<ReworkMinion_Player>();
            playerFuncs.GetMinionCollection(itemID).minions.ForEach(i => projectiles.Add(i));
            playerFuncs.GetMinionCollection(itemIDt2).minions.ForEach(i => projectiles.Add(i));
            playerFuncs.GetMinionCollection(itemIDt3).minions.ForEach(i => projectiles.Add(i));
            projectiles.ForEach(i => i.Kill());
            return true;
        }
    }

    public abstract class MinionBuffAutoClear : MinionBuff
    {
        public override bool RightClick(int buffIndex)
        {
            List<Projectile> projectiles = new();
            Main.player[Main.myPlayer].GetModPlayer<ReworkMinion_Player>().GetMinionCollection(itemID).minions.ForEach(i => projectiles.Add(i));
            projectiles.ForEach(i => i.Kill());
            return false;
        }
    }
    public abstract class MinionBuff : ModBuff
    {
        public virtual int projectileID => -1;
        public virtual int itemID => -1;
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Main.persistentBuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[projectileID] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.ClearBuff(Type);
                buffIndex--;
            }
        }

        public override bool RightClick(int buffIndex)
        {
            List<Projectile> projectiles = new();
            Main.player[Main.myPlayer].GetModPlayer<ReworkMinion_Player>().GetMinionCollection(itemID).minions.ForEach(i => { if (VanillaMinionBuff.ProjectileAllowed(i.type)) projectiles.Add(i); });
            projectiles.ForEach(i => i.Kill());
            return true;
        }
    }
    public static class VanillaMinionBuff
    {
        public static void AddVanillaSentryBuff(Player player, Item item) {
            switch (item.type)
            {
                case ItemID.HoundiusShootius:
                    player.AddBuff(ModContent.BuffType<HoundiusShootiusBuff>(), 2);
                    break;
                case ItemID.QueenSpiderStaff:
                    player.AddBuff(ModContent.BuffType<QueenSpiderBuff>(), 2);
                    break;
                case ItemID.StaffoftheFrostHydra:
                    player.AddBuff(ModContent.BuffType<FrostHydraBuff>(), 2);
                    break;
                case ItemID.MoonlordTurretStaff:
                    player.AddBuff(ModContent.BuffType<MoonlordTurretBuff>(), 2);
                    break;
                case ItemID.RainbowCrystalStaff:
                    player.AddBuff(ModContent.BuffType<RainbowCrystalBuff>(), 2);
                    break;
                case ItemID.DD2FlameburstTowerT1Popper:
                case ItemID.DD2FlameburstTowerT2Popper:
                case ItemID.DD2FlameburstTowerT3Popper:
                    player.AddBuff(ModContent.BuffType<ApprenticeBuff>(), 2);
                    break;
                case ItemID.DD2BallistraTowerT1Popper:
                case ItemID.DD2BallistraTowerT2Popper:
                case ItemID.DD2BallistraTowerT3Popper:
                    player.AddBuff(ModContent.BuffType<SquireBuff>(), 2);
                    break;
                case ItemID.DD2ExplosiveTrapT1Popper:
                case ItemID.DD2ExplosiveTrapT2Popper:
                case ItemID.DD2ExplosiveTrapT3Popper:
                    player.AddBuff(ModContent.BuffType<HuntressBuff>(), 2);
                    break;
                case ItemID.DD2LightningAuraT1Popper:
                case ItemID.DD2LightningAuraT2Popper:
                case ItemID.DD2LightningAuraT3Popper:
                    player.AddBuff(ModContent.BuffType<MonkBuff>(), 2);
                    break;
            }
        }

        public static int[] GetBuffItemType_DD2(int buffType)
        {
            if (buffType == ModContent.BuffType<ApprenticeBuff>())
            {
                return new int[] { ItemID.DD2FlameburstTowerT1Popper, ItemID.DD2FlameburstTowerT2Popper, ItemID.DD2FlameburstTowerT3Popper };
            }
            if (buffType == ModContent.BuffType<SquireBuff>())
            {
                return new int[] { ItemID.DD2BallistraTowerT1Popper, ItemID.DD2BallistraTowerT2Popper, ItemID.DD2BallistraTowerT3Popper };
            }
            if (buffType == ModContent.BuffType<HuntressBuff>())
            {
                return new int[] { ItemID.DD2ExplosiveTrapT1Popper, ItemID.DD2ExplosiveTrapT2Popper, ItemID.DD2ExplosiveTrapT3Popper };
            }
            if (buffType == ModContent.BuffType<MonkBuff>())
            {
                return new int[] { ItemID.DD2LightningAuraT1Popper, ItemID.DD2LightningAuraT2Popper, ItemID.DD2LightningAuraT3Popper };
            }
            return null;
        }

        public static bool ProjectileAllowed(int projectileType)
        {
            if (projectileType == ProjectileModIDs.AncientGuardian)
            {
                return false;
            }
            return true;
        }
    }
}
