using SummonersShine.BakedConfigs;
using SummonersShine.ModSupport;
using SummonersShine.Projectiles;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.SpecialAbilities.DefaultSpecialAbility
{
    public abstract class DefaultSpecialAbility
    {

        static DefaultSpecialAbility notImplemented;
        public static DefaultSpecialAbility[] abilities;
        public static List<DefaultSpecialAbility> megaBakedAbilities;
        public static bool Loaded;

        public class DefaultSpecialAbility_Loader : ILoadable
        {
            public void Load(Mod mod)
            {
                abilities = new DefaultSpecialAbility[]{
                    new SA_Multishot(),
                    //new SA_Reaction(),
                    new SA_ReactionWhipless(),
                    new SA_Instastrike(),
                    new SA_Instastrike_Melee(),
                    //new SA_Uplift(),
                    //new SA_Subversion(),
                    //new SA_Vengeance(),
                    new SA_Kvetch(),
                    new SA_Reach(),
                    new SA_Lacerate(),
                };
                notImplemented = new SA_NotImplemented();
                Loaded = false;
            }

            public void Unload()
            {
                abilities = null;
                megaBakedAbilities = null;
                notImplemented = null;
                Loaded = false;
            }
        }

        public virtual string GetFullLocalizationPath(string midPath)
        {
            return SummonersShine.LocalizationPath + midPath + ToString();
        }

        public static void BakeDefaultSpecialList()
        {
            int count = abilities.Length;
            count += DefaultSpecialAbility_ModdedCustom.modAbils.Count;
            count += DefaultWhipSpecialAbility_ModdedCustom.modAbils.Count;
            megaBakedAbilities = new();
            count = 0;
            for (int x = 0; x < abilities.Length; x++)
            {
                megaBakedAbilities.Add(abilities[x]);
            }
            DefaultSpecialAbility_ModdedCustom.modAbils.Reverse();
            DefaultWhipSpecialAbility_ModdedCustom.modAbils.Reverse();
            megaBakedAbilities.AddRange(DefaultSpecialAbility_ModdedCustom.modAbils);
            megaBakedAbilities.AddRange(DefaultWhipSpecialAbility_ModdedCustom.modAbils);
            Loaded = true;
        }

        public static DefaultSpecialAbility GetSpecial(Item item, Projectile projectile, Random rand) {
            List<DefaultSpecialAbility> validList = new List<DefaultSpecialAbility>();
            int x = 0;
            megaBakedAbilities.ForEach(i =>
            {
                DefaultSpecialAbility test = i;
                bool? configValid = BakedConfig.IsDefaultSpecialAbilityWhitelisted(item.type, x);
                if (configValid == true || (configValid == null && test.ValidForItem(item, projectile)))
                {
                    validList.Add(test);
                }
                x++;
            });
            if(validList.Count == 0)
                return notImplemented;
            DefaultSpecialAbility rv = validList[rand.Next(0, validList.Count)];
            return rv;
        }

        public abstract void GetRandomMinionPower(minionPower[] minionPowers, Random rand);
        public virtual void GetMinionPower(minionPower[] minionPowers, float val1, float val2) {
            minionPowers[0] = minionPower.NewMP(val1, mpScalingType.add);
            minionPowers[1] = minionPower.NewMP(val2, mpScalingType.add);
        }

        public abstract bool ValidForItem(Item item, Projectile projectile);

        public void HookOnProjectileCreated(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            if (!BakedConfig.BlacklistedProjectiles[projectile.type])
            {
                OnProjectileCreated(projectile, projFuncs);
            }
        }

        public static void UnhookSpecialPower(Projectile projectile)
        {
            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            if (ReworkMinion_Item.defaultSpecialAbilityUsed != null && projFuncs.SourceItem != -1 && projFuncs.SourceItem < ReworkMinion_Item.defaultSpecialAbilityUsed.Length)
            {
                DefaultSpecialAbility abil = ReworkMinion_Item.defaultSpecialAbilityUsed[projFuncs.SourceItem];
                if (abil != null)
                    abil.UnhookMinionPower(projectile, projFuncs);
            }
        }

        public static void HookSpecialPower(Projectile projectile)
        {

            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            if (ReworkMinion_Item.defaultSpecialAbilityUsed != null && projFuncs.SourceItem != -1 && projFuncs.SourceItem < ReworkMinion_Item.defaultSpecialAbilityUsed.Length)
            {
                if (!BakedConfig.BlacklistedProjectiles[projectile.type])
                {
                    DefaultSpecialAbility abil = ReworkMinion_Item.defaultSpecialAbilityUsed[projFuncs.SourceItem];
                    if (abil != null)
                        abil.HookOnProjectileCreated(projectile, projFuncs);
                }
            }
        }

        public static void MassUnhook() {
            for (int x = 0; x < Main.projectile.Length; x++)
            {
                Projectile proj = Main.projectile[x];
                if (proj != null && proj.active)
                {
                    UnhookSpecialPower(proj);
                }
            }
        }

        public static void MassHook() {
            for (int x = 0; x < Main.projectile.Length; x++)
            {
                Projectile proj = Main.projectile[x];
                if (proj != null && proj.active)
                {
                    HookSpecialPower(proj);
                }
            }
        }

        public virtual bool GetArbitraryBuffGaugeData(List<Projectile> projList, out float topGauge, out float bottomGauge)
        {
            topGauge = 0;
            bottomGauge = 0;
            return false;
        }
        public virtual void OnRightClick(ReworkMinion_Player playerFuncs) { }
        public virtual void OnProjectileCreated(Projectile projectile, ReworkMinion_Projectile projFuncs) { }
        public virtual void UnhookMinionPower(Projectile projectile, ReworkMinion_Projectile projFuncs) { }

        public virtual bool OnWhipUsed(Player player, ReworkMinion_Player playerFuncs, Item whip, ReworkMinion_Item whipFuncs) { return false; }

        public bool IsRanged(Projectile projectile)
        {
            bool? rvMod = ModSupportDefaultSpecialAbility.HandleIsRangedOverride(projectile);
            if(rvMod != null)
                return rvMod.Value;
            bool rv = ((projectile.ModProjectile != null && ProjectileLoader.CanDamage(projectile) != true)) || !(projectile.usesIDStaticNPCImmunity || projectile.usesLocalNPCImmunity);
            if (rv)
                return true;
            switch (projectile.type)
            {
                case ProjectileID.Retanimini:
                case ProjectileID.Sharknado:
                    return true;
            }
            return false;
        }

        public bool IsNotWhip(Projectile projectile)
        {
            return projectile.DamageType != DamageClass.SummonMeleeSpeed;
        }

        public bool IsModdedSafe(Item item, Projectile projectile)
        {
            if (Config.current.SafeModdedMinionDefaultAbilities) {
                if (item.ModItem != null && item.ModItem.Mod != SummonersShine.modInstance)
                    return false;
                if (projectile.ModProjectile != null && projectile.ModProjectile.Mod != SummonersShine.modInstance)
                    return false;
            }
            return true;
        }
    }
    public abstract class DefaultSpecialAbility_Whip : DefaultSpecialAbility {

        public virtual int id => 0;
        public override bool ValidForItem(Item item, Projectile projectile)
        {
            return !IsNotWhip(projectile);
        }

        public override bool OnWhipUsed(Player player, ReworkMinion_Player playerFuncs, Item whip, ReworkMinion_Item whipFuncs)
        {
            float mp0 = whipFuncs.GetMinionPower(player, whip.type, 0);
            float mp1 = whipFuncs.GetMinionPower(player, whip.type, 1);
            WhipSpecialAbility.WhipSpecialAbility lastWhipSpecial = playerFuncs.lastWhipSpecial;

            if (lastWhipSpecial.id != id || lastWhipSpecial.minionPower0 != mp0 || lastWhipSpecial.minionPower1 != mp1)
            {
                playerFuncs.lastWhipSpecial.Kill();
                playerFuncs.lastWhipSpecial = WhipSpecialAbility.WhipSpecialAbility.GetWhipSpecialAbility(id, mp0, mp1);
            }
            else
                playerFuncs.lastWhipSpecial.duration = playerFuncs.lastWhipSpecial.maxDuration;

            return true;
        }
    }

    public class SA_Kvetch : DefaultSpecialAbility_Whip
    {
        public override string ToString()
        {
            return "Kvetch";
        }
        public override void GetRandomMinionPower(minionPower[] minionPowers, Random rand)
        {
            GetMinionPower(minionPowers, rand.Next(1, 4) * 5, 0);
        }
        public override void GetMinionPower(minionPower[] minionPowers, float val1, float val2)
        {
            minionPowers[0] = minionPower.NewMP(val1, mpScalingType.add);
            minionPowers[1] = minionPower.NewMP(val2, mpScalingType.add);
        }

        public override int id => 1;
    }
    public class SA_Reach : DefaultSpecialAbility_Whip
    {
        public override string ToString()
        {
            return "Reach";
        }
        public override void GetRandomMinionPower(minionPower[] minionPowers, Random rand)
        {
            GetMinionPower(minionPowers, rand.Next(4, 7) * 5, 0);
        }
        public override void GetMinionPower(minionPower[] minionPowers, float val1, float val2)
        {
            minionPowers[0] = minionPower.NewMP(val1, mpScalingType.add);
            minionPowers[1] = minionPower.NewMP(val2, mpScalingType.add);
        }

        public override int id => 2;
    }
    public class SA_Lacerate : DefaultSpecialAbility_Whip
    {
        public override string ToString()
        {
            return "Lacerate";
        }
        public override void GetRandomMinionPower(minionPower[] minionPowers, Random rand)
        {
            GetMinionPower(minionPowers, rand.Next(1, 3) * 5, 0);
        }

        public override void GetMinionPower(minionPower[] minionPowers, float val1, float val2)
        {
            minionPowers[0] = minionPower.NewMP(val1, mpScalingType.add);
            minionPowers[1] = minionPower.NewMP(val2, mpScalingType.add);
        }

        public override int id => 3;
    }

    class SA_NotImplemented : DefaultSpecialAbility
    {
        public override void GetRandomMinionPower(minionPower[] minionPowers, Random rand)
        {
        }

        public override void OnProjectileCreated(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
        }

        public override bool ValidForItem(Item item, Projectile projectile)
        {
            return true;
        }
        public override string ToString()
        {
            return "NotImplemented";
        }
    }

    class SA_Uplift : DefaultSpecialAbility
    {
        public override string ToString()
        {
            return "Uplift";
        }
        public override void GetRandomMinionPower(minionPower[] minionPowers, Random rand)
        {
            minionPowers[0] = minionPower.NewMP(rand.Next(10, 20));
            minionPowers[1] = minionPower.NewMP(rand.Next(10, 20));
        }

        public override bool ValidForItem(Item item, Projectile projectile)
        {
            return IsNotWhip(projectile);
        }
        public override void OnProjectileCreated(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            throw new NotImplementedException();
        }
    }

    class SA_Subversion : DefaultSpecialAbility
    {
        public override string ToString()
        {
            return "Subversion";
        }
        public override void GetRandomMinionPower(minionPower[] minionPowers, Random rand)
        {
            minionPowers[0] = minionPower.NewMP(rand.Next(10, 20));
            minionPowers[1] = minionPower.NewMP(rand.Next(10, 20));
        }
        public override bool ValidForItem(Item item, Projectile projectile)
        {
            return IsNotWhip(projectile);
        }
        public override void OnProjectileCreated(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            throw new NotImplementedException();
        }
    }

    class SA_Vengeance : DefaultSpecialAbility
    {
        public override string ToString()
        {
            return "Vengeance";
        }
        public override void GetRandomMinionPower(minionPower[] minionPowers, Random rand)
        {
            minionPowers[0] = minionPower.NewMP(rand.Next(10, 20));
            minionPowers[1] = minionPower.NewMP(rand.Next(10, 20));
        }

        public override bool ValidForItem(Item item, Projectile projectile)
        {
            return true;
        }
        public override void OnProjectileCreated(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            throw new NotImplementedException();
        }
    }
}
