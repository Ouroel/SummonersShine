using SummonersShine.Projectiles;
using SummonersShine.SpecialAbilities.DefaultSpecialAbility;
using SummonersShine.VanillaMinionBuffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace SummonersShine.BakedConfigs
{
    public class MassBitsByte {
        BitsByte[] bitsbytes;

        public bool this[int key] {
            get {
                int pos = key % 8;
                return bitsbytes[key / 8][pos];
            }
            set {
                int pos = key % 8;
                bitsbytes[key / 8][pos] = value;
            }
        }

        public MassBitsByte(int max) {
            bitsbytes = new BitsByte[(max - 1) / 8 + 1];
        }

        public int Length => bitsbytes.Length * 8;
    }

    public class ModWhitelistData
    {
        public Mod mod;
        bool tracking = false;
        bool balanced = false;
        public ModWhitelistData(Mod mod)
        {
            this.mod = mod;
        }

        public bool GetWhitelist(int type) {
            switch (type) {
                case 0:
                    return tracking;
                case 1:
                    return balanced;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public void SetWhitelist(int type)
        {
            switch (type)
            {
                case -1:
                    tracking = true;
                    balanced = true;
                    break;
                case 0:
                    tracking = true;
                    break;
                case 1:
                    balanced = true;
                    break;
            }
        }
    }
    public static class BakedConfig
    {
        public static int DefaultAbilsSeed;

        //public static bool DisableCustomMinionSpecialPowers; bake inside

        public static MassBitsByte ItemIgnoresCustomSpecialPower;
        public static MassBitsByte ItemNonPrefixable;
        public static MassBitsByte ItemRetainManaCost;

        public static float[] MinionOutgoingDamageMod;

        public static int[] ProjSourceItem;
        public static int[][] BuffSourceItem;
        public static int[] MinionItemBuff;
        public static MassBitsByte ItemHasBuffLinked;
        //CustomDefaultAbils
        public static int[] DefaultAbilityType;
        public static int[] DefaultAbility_MinionPower0;
        public static int[] DefaultAbility_MinionPower1;

        /*public static bool StopPrintingByteFixer;
        public static bool DisableMinionCrits;
        public static bool DisableDynamicProjectileCacheUpdate;*/

        public static MassBitsByte BlacklistedProjectiles;

        //DisableModdedMinionTracking
        public static MassBitsByte NoTrackingProjectiles;
        public static MassBitsByte MinionsIgnoreTracking;

        public static MassBitsByte ProjectilesNotCountedAsMinion;
        public static MassBitsByte ProjectilesCountedAsMinion;

        public static MassBitsByte DefaultSpecialAbilityWhitelist;
        const int num_default_bits = 8;

        public static MassBitsByte ItemCountAsWhip;

        public static bool initialized = false;

        static List<BakedConfigQueueData> queueList = new();

        public static List<ModWhitelistData> WhitelistedMods = new List<ModWhitelistData>();

        public class BakedConfigs_Loader : ILoadable {
            public void Load(Mod mod)
            {
                queueList = new();
                WhitelistedMods = new();
            }

            public void Unload()
            {
                queueList = null;
                WhitelistedMods = null;

                NoTrackingProjectiles = null;
                MinionsIgnoreTracking = null;
                ProjectilesNotCountedAsMinion = null;
                ProjectilesCountedAsMinion = null;
                BlacklistedProjectiles = null;

                ProjSourceItem = null;
                BuffSourceItem = null;
                MinionItemBuff = null;
                ItemHasBuffLinked = null;
                MinionOutgoingDamageMod = null;

                ItemRetainManaCost = null;
                ItemIgnoresCustomSpecialPower = null;
                ItemNonPrefixable = null;
                DefaultAbilityType = null;
                DefaultAbility_MinionPower0 = null;
                DefaultAbility_MinionPower1 = null;

                DefaultSpecialAbilityWhitelist = null;

                ItemCountAsWhip = null;
            }
        }
        public static void Instantiate(Config pendingConfig)
        {
            int ProjectileCount = ProjectileLoader.ProjectileCount;
            int ItemCount = ItemLoader.ItemCount;
            int BuffCount = BuffLoader.BuffCount;

            NoTrackingProjectiles = new(ProjectileCount);
            MinionsIgnoreTracking = new(ProjectileCount);
            ProjectilesNotCountedAsMinion = new(ProjectileCount);
            ProjectilesCountedAsMinion = new(ProjectileCount);
            BlacklistedProjectiles = new(ProjectileCount);

            ProjSourceItem = new int[ProjectileCount];
            BuffSourceItem = new int[BuffCount][];
            MinionItemBuff = new int[ItemCount];
            ItemHasBuffLinked = new(ItemCount);
            MinionOutgoingDamageMod = new float[ProjectileCount];

            ItemRetainManaCost = new(ItemCount);
            ItemIgnoresCustomSpecialPower = new(ItemCount);
            ItemNonPrefixable = new(ItemCount);
            DefaultAbilityType = new int[ItemCount];
            DefaultAbility_MinionPower0 = new int[ItemCount];
            DefaultAbility_MinionPower1 = new int[ItemCount];

            DefaultSpecialAbilityWhitelist = new(ItemCount * num_default_bits);

            ItemCountAsWhip = new(ItemCount);

            InitDefaults();
            Config.CopyContent(pendingConfig);

            initialized = true;
            queueList.ForEach(i => PushModdedConfig(i.type, i.id, i.value));
        }

        public static bool? IsDefaultSpecialAbilityWhitelisted(int itemID, int defaultSpecialID)
        {
            if (defaultSpecialID >= (num_default_bits - 1) || DefaultSpecialAbilityWhitelist[itemID * num_default_bits] == false)
                return null;
            return DefaultSpecialAbilityWhitelist[itemID * num_default_bits + 1 + defaultSpecialID];
        }

        public static void SetDefaultSpecialAbilityWhitelist(int itemID, BitsByte[] bytes)
        {
            int pos = itemID * num_default_bits;
            DefaultSpecialAbilityWhitelist[pos] = true;
            int count = 0;
            for (int x = 0; x < bytes.Length; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    count++;
                    pos++;
                    if (count == num_default_bits)
                        continue;
                    DefaultSpecialAbilityWhitelist[pos] = bytes[x][y];
                }
                if (count == num_default_bits)
                    continue;
            }
        }

        public static void WhitelistMod(Mod mod, int whitelistType)
        {
            ModWhitelistData data = WhitelistedMods.Find(i=>i.mod == mod);
            if (data == null)
            {
                data = new ModWhitelistData(mod);
                WhitelistedMods.Add(data);
            }
            data.SetWhitelist(whitelistType);
        }
        public static bool IsModWhitelisted(Mod mod, int whitelistType)
        {
            if (mod == SummonersShine.modInstance)
                return true;
            ModWhitelistData data = WhitelistedMods.Find(i => i.mod == mod);
            if (data == null)
                return false;
            return data.GetWhitelist(whitelistType);
        }

        public static int[] GetBuffSourceItemTypes(int buffID)
        {
            return BuffSourceItem[buffID];
        }
        public static int GetMinionItemBuff(int itemID)
        {
            return MinionItemBuff[itemID] - 1;
        }

        public static void AddBuffSourceItem(int buffID, int itemID)
        {
            int[] origList = BuffSourceItem[buffID];
            if (origList == null)
            {
                SetBuffSourceItem(buffID, itemID);
                return;
            }
            int[] newList = new int[origList.Length + 1];
            for (int i = 0; i < origList.Length; i++)
                newList[i] = origList[i];
            newList[origList.Length] = itemID;
            BuffSourceItem[buffID] = newList;
            ItemHasBuffLinked[itemID] = true;
        } 

        public static void SetBuffSourceItem(int buffID, int itemID, bool alsoTrackItemBuff = false)
        {
            if (buffID == 0)
                return;
            BuffSourceItem[buffID] = new int[] { itemID };
            if (alsoTrackItemBuff)
                MinionItemBuff[itemID] = buffID + 1;
            ItemHasBuffLinked[itemID] = true;
        }

        public static void SetBuffSourceItems(int buffID, params int[] itemID)
        {
            if (buffID == 0)
                return;
            BuffSourceItem[buffID] = itemID;
            for(int x = 0; x < itemID.Length; x++)
                ItemHasBuffLinked[itemID[x]] = true;
        }

        static readonly BitsByte[] meleeMinionDefaultSpecials = new BitsByte[] {
            new BitsByte(false, true, false, true)
        };

        static readonly BitsByte[] rangedMinionDefaultsNoMultishot = new BitsByte[] {
            new BitsByte(false, true, true)
        };

        static void InitDefaults()
        {
            BlacklistedProjectiles[ProjectileID.AbigailCounter] = true;
            BlacklistedProjectiles[ProjectileID.StormTigerGem] = true;
            BlacklistedProjectiles[ProjectileID.StardustDragon2] = true;
            BlacklistedProjectiles[ProjectileID.StardustDragon3] = true;
            BlacklistedProjectiles[ProjectileID.StardustDragon4] = true;

            SetDefaultSpecialAbilityWhitelist(ItemID.AbigailsFlower, meleeMinionDefaultSpecials);
            SetDefaultSpecialAbilityWhitelist(ItemID.StormTigerStaff, meleeMinionDefaultSpecials);

            SetDefaultSpecialAbilityWhitelist(ItemID.MoonlordTurretStaff, rangedMinionDefaultsNoMultishot);
            SetDefaultSpecialAbilityWhitelist(ItemID.RainbowCrystalStaff, rangedMinionDefaultsNoMultishot);

            NoTrackingProjectiles[ProjectileID.WhiteTigerPounce] = true;
            NoTrackingProjectiles[ProjectileID.UFOLaser] = true;
            NoTrackingProjectiles[ProjectileID.MoonlordTurretLaser] = true;
            NoTrackingProjectiles[ProjectileModIDs.GoldenFruit] = true;
            NoTrackingProjectiles[ProjectileModIDs.NightmareFuel] = true;
            NoTrackingProjectiles[ProjectileModIDs.SpiderHiverVenom] = true;

            SetProjSourceItemType(ProjectileID.StormTigerTier1, ItemID.StormTigerStaff);
            SetProjSourceItemType(ProjectileID.StormTigerTier2, ItemID.StormTigerStaff);
            SetProjSourceItemType(ProjectileID.StormTigerTier3, ItemID.StormTigerStaff);

            SetProjSourceItemType(ProjectileID.AbigailMinion, ItemID.AbigailsFlower);

            BakeAllItemBuffs();
            SetBuffSourceItem(BuffID.BabyBird, ItemID.BabyBirdStaff, true); //AoMM Fix
            SetBuffSourceItem(BuffID.AbigailMinion, ItemID.AbigailsFlower, true);
            SetBuffSourceItem(BuffID.StormTiger, ItemID.StormTigerStaff, true);
            SetBuffSourceItem(BuffID.BatOfLight, ItemID.SanguineStaff, true);
            SetBuffSourceItem(BuffID.SharknadoMinion, ItemID.TempestStaff, true);
            SetBuffSourceItem(BuffID.EmpressBlade, ItemID.EmpressBlade, true);

            SetBuffSourceItem(ModContent.BuffType<HoundiusShootiusBuff>(), ItemID.HoundiusShootius);
            SetBuffSourceItem(ModContent.BuffType<QueenSpiderBuff>(), ItemID.QueenSpiderStaff, true);
            SetBuffSourceItem(ModContent.BuffType<FrostHydraBuff>(), ItemID.StaffoftheFrostHydra);
            SetBuffSourceItem(ModContent.BuffType<MoonlordTurretBuff>(), ItemID.MoonlordTurretStaff, true);
            SetBuffSourceItem(ModContent.BuffType<RainbowCrystalBuff>(), ItemID.RainbowCrystalStaff, true);

            SetBuffSourceItems(ModContent.BuffType<ApprenticeBuff>(), ItemID.DD2FlameburstTowerT1Popper, ItemID.DD2FlameburstTowerT2Popper, ItemID.DD2FlameburstTowerT3Popper);
            SetBuffSourceItems(ModContent.BuffType<SquireBuff>(), ItemID.DD2BallistraTowerT1Popper, ItemID.DD2BallistraTowerT2Popper, ItemID.DD2BallistraTowerT3Popper);
            SetBuffSourceItems(ModContent.BuffType<HuntressBuff>(), ItemID.DD2ExplosiveTrapT1Popper, ItemID.DD2ExplosiveTrapT2Popper, ItemID.DD2ExplosiveTrapT3Popper );
            SetBuffSourceItems(ModContent.BuffType<MonkBuff>(), ItemID.DD2LightningAuraT1Popper, ItemID.DD2LightningAuraT2Popper, ItemID.DD2LightningAuraT3Popper);
        }

        static void BakeAllItemBuffs()
        {
            Item testItem = new();
            for (int x = 0; x < ItemLoader.ItemCount; x++)
            {
                testItem.SetDefaults(x);
                if (ReworkMinion_Item.IsSummon(testItem)) {
                    SetBuffSourceItem(testItem.buffType, testItem.type);
                }
            }
        }

        public static void SetConfigMinionPower(int itemID) {

            int abilType = DefaultAbilityType[itemID]; //1 index this so 0 is null
            if (abilType == 0)
                return;
            DefaultSpecialAbility[] defaultSpecialAbilityUsed = ReworkMinion_Item.defaultSpecialAbilityUsed;
            minionPower[] minionPowers = ReworkMinion_Item.minionPowers[itemID];
            DefaultSpecialAbility special = DefaultSpecialAbility.abilities[abilType - 1];
            special.GetMinionPower(minionPowers, DefaultAbility_MinionPower0[itemID], DefaultAbility_MinionPower1[itemID]);
            defaultSpecialAbilityUsed[itemID] = special;
        }

        public static bool Get_ProjectileCountedAsMinion(int itemID)
        {
            if (initialized)
                return ProjectilesCountedAsMinion[itemID];
            else
                return false;
			
        }
        public static bool Get_ProjectileNotCountedAsMinion(int itemID)
        {
            if (initialized)
                return ProjectilesNotCountedAsMinion[itemID];
            else
                return false;
        }

        public static bool IgnoreTracking(Projectile projectile)
        {
            return (Config.current.DisableModdedMinionTracking &&
                projectile.ModProjectile != null &&
                !IsModWhitelisted(projectile.ModProjectile.Mod, 0));
        }

        struct BakedConfigQueueData
        {
            public int type;
            public object id;
            public object value;
            public BakedConfigQueueData(int type, object id, object value) { this.type = type; this.id = id; this.value = value; }
        }

        public static void AddModdedConfig_ThroughCode(int type, object id, object value)
        {
            queueList.Add(new BakedConfigQueueData(type, id, value));
            if (initialized) {
                PushModdedConfig(type, id, value);
            }
        }
        public static bool CustomSpecialPowersEnabled(int itemID)
        {
            return !ItemIgnoresCustomSpecialPower[itemID];
        }

        //Key
        //0 - default
        //1 - no item
        //2+ - item (n - 2) e.g. 3 => 1, 4 => 2, etc.
        public static int GetProjSourceItemType(int projectileType) {
            int rv = ProjSourceItem[projectileType];
            return rv - 2;
        }

        public static void SetProjSourceItemType(int projectiletype, int itemType)
        {
            if (projectiletype < ProjSourceItem.Length)
                ProjSourceItem[projectiletype] = itemType + 2;
        }
        static void PushModdedConfig(int type, object id, object value)
        {
            switch (type)
            {
                case 0:
                    ItemIgnoresCustomSpecialPower[(int)id] = true;
                    return;
                case 1:
                    BlacklistedProjectiles[(int)id] = true;
                    return;
                case 2:
                    NoTrackingProjectiles[(int)id] = true;
                    return;
                case 3:
                    MinionsIgnoreTracking[(int)id] = true;
                    return;
                case 4:
                    ProjectilesNotCountedAsMinion[(int)id] = true;
                    return;
                case 5:
                    ProjectilesCountedAsMinion[(int)id] = true;
                    return;
                case 6:
                    int whitelistType;
                    if (value == null)
                        whitelistType = -1;
                    else
                        whitelistType = (int)value;
                    WhitelistMod((Mod)id, whitelistType);
                    return;
                case 7:
                    ItemNonPrefixable[(int)id] = true;
                    return;
                case 8:
                    SetProjSourceItemType((int)id, (int)value);
                    return;
                case 9:
                    SetBuffSourceItem((int)id, (int)value);
                    return;
                case 10:
                    ItemRetainManaCost[(int)id] = true;
                    return;
                case 11:
                    SetBuffSourceItems((int)id, (int[])value);
                    return;
                case 12:
                    MinionOutgoingDamageMod[(int)id] = (float)value;
                    return;
                case 13:
                    SetDefaultSpecialAbilityWhitelist((int)id, (BitsByte[])value);
                    return;
                case 14:
                    ItemCountAsWhip[(int)id] = true;
                    break;
            }
        }
    }
}
