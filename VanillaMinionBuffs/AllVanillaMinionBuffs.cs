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
    public class HoundiusShootiusBuff : MinionBuff
    {
        public override int projectileID => ProjectileID.HoundiusShootius;
        public override int itemID => ItemID.HoundiusShootius;
    }
    public class QueenSpiderBuff : MinionBuffAutoClear
    {
        public override int projectileID => ProjectileID.SpiderHiver;
        public override int itemID => ItemID.QueenSpiderStaff;
    }
    public class FrostHydraBuff : MinionBuff
    {
        public override int projectileID => ProjectileID.FrostHydra;
        public override int itemID => ItemID.StaffoftheFrostHydra;
    }
    public class MoonlordTurretBuff : MinionBuffAutoClear
    {
        public override string Name => GetTraeCompat();
        public override int projectileID => ProjectileID.MoonlordTurret;
        public override int itemID => ItemID.MoonlordTurretStaff;

        static string GetTraeCompat() {
            if (ModLoader.TryGetMod("TRAEProject", out Mod _))
            {
                return "StardustTurretBuff";
            }
            return "MoonlordTurretBuff";
        }
    }
    public class RainbowCrystalBuff : MinionBuffAutoClear
    {
        public override int projectileID => ProjectileID.RainbowCrystal;
        public override int itemID => ItemID.RainbowCrystalStaff;
    }
    public class HuntressBuff : DD2Buff
    {
        public override int projectileID => ProjectileID.DD2ExplosiveTrapT1;
        public override int projectileIDt2 => ProjectileID.DD2ExplosiveTrapT2;
        public override int projectileIDt3 => ProjectileID.DD2ExplosiveTrapT3;
        public override int itemID => ItemID.DD2ExplosiveTrapT1Popper;
        public override int itemIDt2 => ItemID.DD2ExplosiveTrapT2Popper;
        public override int itemIDt3 => ItemID.DD2ExplosiveTrapT3Popper;
    }
    public class SquireBuff : DD2Buff
    {
        public override int projectileID => ProjectileID.DD2BallistraTowerT1;
        public override int projectileIDt2 => ProjectileID.DD2BallistraTowerT2;
        public override int projectileIDt3 => ProjectileID.DD2BallistraTowerT3;
        public override int itemID => ItemID.DD2BallistraTowerT1Popper;
        public override int itemIDt2 => ItemID.DD2BallistraTowerT2Popper;
        public override int itemIDt3 => ItemID.DD2BallistraTowerT3Popper;
    }
    public class ApprenticeBuff : DD2Buff
    {
        public override int projectileID => ProjectileID.DD2FlameBurstTowerT1;
        public override int projectileIDt2 => ProjectileID.DD2FlameBurstTowerT2;
        public override int projectileIDt3 => ProjectileID.DD2FlameBurstTowerT3;
        public override int itemID => ItemID.DD2FlameburstTowerT1Popper;
        public override int itemIDt2 => ItemID.DD2FlameburstTowerT2Popper;
        public override int itemIDt3 => ItemID.DD2FlameburstTowerT3Popper;
    }
    public class MonkBuff : DD2Buff
    {
        public override int projectileID => ProjectileID.DD2LightningAuraT1;
        public override int projectileIDt2 => ProjectileID.DD2LightningAuraT2;
        public override int projectileIDt3 => ProjectileID.DD2LightningAuraT3;
        public override int itemID => ItemID.DD2LightningAuraT1Popper;
        public override int itemIDt2 => ItemID.DD2LightningAuraT2Popper;
        public override int itemIDt3 => ItemID.DD2LightningAuraT3Popper;
    }
}
