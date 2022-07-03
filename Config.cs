using SummonersShine.BakedConfigs;
using SummonersShine.SpecialAbilities.DefaultSpecialAbility;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.IO;

namespace SummonersShine
{
    /*public class SummonersShineRavenConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        [DefaultValue(0.1)]
        public float ravenBaseSpeed;
        [DefaultValue(0.1f)]
        public float ravenSpeedPerEnemyVel;
        [Range(0.1f, 10f)]
        [DefaultValue(4)]
        public float ravenSpeedCap;
    }*/

    [Label("$" + SummonersShine.LocalizationPath + "Config.ClientConfig")]
    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public static ClientConfig current;

        [Label("$" + SummonersShine.LocalizationPath + "Config.CompressSpecialAbilityDescription")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.CompressSpecialAbilityDescriptionDesc")]
        [DefaultValue(true)]
        public bool CompressSpecialAbilityDescription;

        [DefaultValue(false)]
        [Label("$" + SummonersShine.LocalizationPath + "Config.PrintMinionDeaths")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.PrintMinionDeathsDesc")]
        public bool PrintMinionDeaths;

        public override bool NeedsReload(ModConfig pendingConfig)
        {
            current = pendingConfig as ClientConfig;
            return false;
        }

        public class ReworkMinion_Config_Loader : ILoadable
        {
            void ILoadable.Load(Mod mod)
            {
                current = ModContent.GetInstance<ClientConfig>();
            }

            void ILoadable.Unload()
            {
                current = null;
            }
        }
    }

    [Label("$" + SummonersShine.LocalizationPath + "Config.Config")]
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;


        public static Config current;

        [Header("$" + SummonersShine.LocalizationPath + "Config.Events")]

        [DefaultValue(false)]
        [Label("$" + SummonersShine.LocalizationPath + "Config.BloodtaintMode")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.BloodtaintModeDesc")]
        public bool BloodtaintMode = false;

        /* bool BloodtaintMode = false;*/

        public bool IsBloodtaintMode() { return BloodtaintMode; }


        [Header("$" + SummonersShine.LocalizationPath + "Config.General")]

        [Range(int.MinValue, int.MaxValue)]
        [DefaultValue(886326188)]
        [Label("$" + SummonersShine.LocalizationPath + "Config.DefaultAbilsSeed")]
        public int DefaultAbilsSeed;

        [Label("$" + SummonersShine.LocalizationPath + "Config.DisableCustomMinionSpecialPowers")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.DisableCustomMinionSpecialPowersDesc")]
        [DefaultValue(false)]
        public bool DisableCustomMinionSpecialPowers;


        [Label("$" + SummonersShine.LocalizationPath + "Config.ItemIgnoresCustomSpecialPower")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.ItemIgnoresCustomSpecialPowerDesc")]
        public List<ItemDefinition> ItemIgnoresCustomSpecialPower = new();

        [Label("$" + SummonersShine.LocalizationPath + "Config.RemoveManaCost")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.RemoveManaCostDesc")]
        [DefaultValue(true)]
        public bool RemoveManaCost;

        [Label("$" + SummonersShine.LocalizationPath + "Config.ItemsRetainManaCost")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.ItemsRetainManaCostDesc")]
        public List<ItemDefinition> ItemsRetainManaCost = new();

        [Label("$" + SummonersShine.LocalizationPath + "Config.CustomDefaultAbils")]
        public List<DefaultAbilMap> CustomDefaultAbils = new();


        [Range(0f, 100f)]
        [Increment(1f)]
        [DefaultValue(80)]
        [Label("$" + SummonersShine.LocalizationPath + "Config.MinionOutgoingDamage")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.MinionOutgoingDamageDesc")]
        public float MinionOutgoingDamage;

        [Label("$" + SummonersShine.LocalizationPath + "Config.MinionOutgoingDamageMod")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.MinionOutgoingDamageModDesc")]
        public List<MinionOutgoingDamageModMap> MinionOutgoingDamageMod = new();

        //[Label("$" + SummonersShine.LocalizationPath + "Config.StopPrintingByteFixer")]
        //[Tooltip("$" + SummonersShine.LocalizationPath + "Config.StopPrintingByteFixerDesc")]
        //[DefaultValue(false)]
        //public bool StopPrintingByteFixer;

        [Header("$" + SummonersShine.LocalizationPath + "Config.CompatConfig")]

        [Label("$" + SummonersShine.LocalizationPath + "Config.DisableMinionCrits")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.DisableMinionCritsDesc")]
        [DefaultValue(false)]
        public bool DisableMinionCrits;

        [Label("$" + SummonersShine.LocalizationPath + "Config.DisableDynamicProjectileCacheUpdate")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.DisableDynamicProjectileCacheUpdateDesc")]
        [DefaultValue(false)]
        public bool DisableDynamicProjectileCacheUpdate;

        [Label("$" + SummonersShine.LocalizationPath + "Config.DisableModdedMinionTracking")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.DisableModdedMinionTrackingDesc")]
        [DefaultValue(true)]
        public bool DisableModdedMinionTracking;

        [Label("$" + SummonersShine.LocalizationPath + "Config.SafeModdedMinionDefaultAbilities")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.SafeModdedMinionDefaultAbilitiesDesc")]
        [DefaultValue(true)]
        public bool SafeModdedMinionDefaultAbilities;

        //[Label("$" + SummonersShine.LocalizationPath + "Config.BuffSourceItems")]
        //[Tooltip("$" + SummonersShine.LocalizationPath + "Config.BuffSourceItemsDesc")]
        List<BuffSourceItemMap> BuffSourceItems = new();

        [Label("$" + SummonersShine.LocalizationPath + "Config.BlacklistedProjectiles")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.BlacklistedProjectilesDesc")]
        public List<ProjectileDefinition> BlacklistedProjectiles = new();

        [Label("$" + SummonersShine.LocalizationPath + "Config.ProjectileSourceItemType")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.ProjectileSourceItemTypeDesc")]
        public List<DefaultItemSourceMap> ProjectileSourceItemType = new();

        [Label("$" + SummonersShine.LocalizationPath + "Config.ProjectilesIgnoreTracking")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.ProjectilesIgnoreTrackingDesc")]
        public List<ProjectileDefinition> MinionsIgnoreTracking = new();

        [Label("$" + SummonersShine.LocalizationPath + "Config.ProjectileCountedAsMinion")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.ProjectileCountedAsMinionDesc")]
        public List<ProjectileDefinition> ProjectilesCountedAsMinion = new();

        [Label("$" + SummonersShine.LocalizationPath + "Config.ProjectilesNotCountedAsMinion")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.ProjectilesNotCountedAsMinionDesc")]
        public List<ProjectileDefinition> ProjectilesNotCountedAsMinion = new();

        [Label("$" + SummonersShine.LocalizationPath + "Config.ProjectilesForceAimbot")]
        [Tooltip("$" + SummonersShine.LocalizationPath + "Config.ProjectilesForceAimbotDesc")]
        public List<DefaultProjectileForceAimbotMap> ProjectilesForceAimbot = new();

        public static void CopyContent(Config pending)
        {
            for (int i = 0; i < ProjectileLoader.ProjectileCount; i++)
            {
                ModProjectile projFuncs = ProjectileLoader.GetProjectile(i);
                if (projFuncs == null || !BakedConfig.IsModWhitelisted(projFuncs.Mod, 1))
                    BakedConfig.MinionOutgoingDamageMod[i] = pending.MinionOutgoingDamage / 100;
                else
                    BakedConfig.MinionOutgoingDamageMod[i] = 1;
            }
            pending.MinionOutgoingDamageMod.ForEach(i => i.Bake());

            if (pending.IsBloodtaintMode()) {
                BakedConfigs.BloodtaintMode.Activate();
            }

            if (pending.DisableCustomMinionSpecialPowers)
            {
                for (int i = 0; i < ItemLoader.ItemCount; i++)
                    if (i < BakedConfig.ItemIgnoresCustomSpecialPower.Length && i > -1)
                        BakedConfig.ItemIgnoresCustomSpecialPower[i] = true;
            }
            else
                pending.ItemIgnoresCustomSpecialPower.ForEach(i => { if (BakedConfig.ItemIgnoresCustomSpecialPower.Length > i.Type && i.Type > -1) BakedConfig.ItemIgnoresCustomSpecialPower[i.Type] = true; });
            if (!pending.RemoveManaCost)
            {
                for (int i = 0; i < ItemLoader.ItemCount; i++)
                    if (i < BakedConfig.ItemRetainManaCost.Length && i > -1)
                        BakedConfig.ItemRetainManaCost[i] = true;
            }
            else
                pending.ItemsRetainManaCost.ForEach(i => { if (BakedConfig.ItemRetainManaCost.Length > i.Type && i.Type > -1) BakedConfig.ItemRetainManaCost[i.Type] = true; });

            pending.MinionsIgnoreTracking.ForEach(i => { if (BakedConfig.MinionsIgnoreTracking.Length > i.Type && i.Type > -1) BakedConfig.MinionsIgnoreTracking[i.Type] = true; });
            pending.ProjectilesNotCountedAsMinion.ForEach(i => { if (BakedConfig.ProjectilesNotCountedAsMinion.Length > i.Type && i.Type > -1) BakedConfig.ProjectilesNotCountedAsMinion[i.Type] = true; });
            pending.ProjectilesCountedAsMinion.ForEach(i => { if (BakedConfig.ProjectilesCountedAsMinion.Length > i.Type && i.Type > -1) BakedConfig.ProjectilesCountedAsMinion[i.Type] = true; });
            pending.BlacklistedProjectiles.ForEach(i => { if (BakedConfig.BlacklistedProjectiles.Length > i.Type && i.Type > -1) BakedConfig.BlacklistedProjectiles[i.Type] = true; });
            //pending.ProjectilesForceAimbot.ForEach(i => BakedConfig.ProjectilesForceAimbot[i.Type] = true);
            pending.CustomDefaultAbils.ForEach(i =>
            {
                if (i.def == null)
                    return;
                int type = i.def.Type;
                if (BakedConfig.DefaultAbilityType.Length > type && type > -1)
                {
                    BakedConfig.DefaultAbilityType[type] = i.DefaultAbilityType + 1;
                    BakedConfig.DefaultAbility_MinionPower0[type] = i.MinionPower0;
                    BakedConfig.DefaultAbility_MinionPower1[type] = i.MinionPower1;
                }
            });

            pending.ProjectileSourceItemType.ForEach(i =>
            {
                BakedConfig.SetProjSourceItemType(i.proj.Type, i.item.Type);
            });
            pending.BuffSourceItems.ForEach(i => i.Bake());

        }

        public override void OnChanged()
        {
            if (!BakedConfig.initialized)
                return;
            current = ModContent.GetInstance<Config>();
            BakedConfig.Instantiate(current);

            if (ReworkMinion_World.WorldLoaded)
            {
                DefaultSpecialAbility.MassUnhook();
                MassProjectileUnminion();
            }
            MinionDataHandler.ItemSetStaticDefaults();
            if (ReworkMinion_World.WorldLoaded)
                MassProjectileReset();
        }
        /*public override bool NeedsReload(ModConfig pendingConfig)
        {
            Config pending = pendingConfig as Config;
            BakedConfig.Instantiate(pending);

            current = pendingConfig as Config;

            if (ReworkMinion_World.WorldLoaded)
            {
                DefaultSpecialAbility.MassUnhook();
                MassProjectileUnminion();
            }
            MinionDataHandler.ItemSetStaticDefaults();
            if (ReworkMinion_World.WorldLoaded)
                MassProjectileReset()
            //DefaultSpecialAbility.MassHook();
            return false;
        };*/
        static void MassProjectileUnminion()
        {
            for (int x = 0; x < Main.projectile.Length; x++)
            {
                Projectile proj = Main.projectile[x];
                if (proj != null && proj.active)
                {
                    ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
                    Player player = Main.player[proj.owner];
                    //unadd
                    if (projFuncs.IsMinion == ProjMinionRelation.isMinion && player != null)
                    {
                        if (projFuncs.SourceItem != -1)
                        {
                            MinionEnergyCounter counter = player.GetModPlayer<ReworkMinion_Player>().TryGetMinionCollection(projFuncs.SourceItem);
                            if(counter != null)
                                counter.minions.Remove(proj);
                        }
                    }
                }
            }
        }

        static void MassProjectileReset()
        {
            for (int x = 0; x < Main.projectile.Length; x++)
            {
                Projectile proj = Main.projectile[x];
                if (proj != null && proj.active)
                {
                    ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
                    Player player = Main.player[proj.owner];
                    if (projFuncs.IsMinion == ProjMinionRelation.isMinion && player != null)
                    {
                        MinionProjectileData projData = projFuncs.GetMinionProjData();
                        projFuncs.minionTerminateSpecialAbility(proj, projFuncs, projData, player, player.GetModPlayer<ReworkMinion_Player>());

                        projData.castingSpecialAbilityTime = -1;
                        projData.energyRegenRateMult = 1;
                        projData.specialCastPosition = Microsoft.Xna.Framework.Vector2.Zero;
                        projData.specialCastTarget = null;

                        MinionDataHandler.ConfigRehookSourceItem(proj, projFuncs, projData);

                        //readd
                        if (projFuncs.SourceItem != -1)
                        {
                            MinionEnergyCounter counter = player.GetModPlayer<ReworkMinion_Player>().TryGetMinionCollection(projFuncs.SourceItem);
                            if (counter != null)
                            {
                                List<Projectile> projList = counter.minions;
                                if (!projList.Contains(proj))
                                    projList.Add(proj);
                            }

                        }
                    }
                }
            }
        }

        public class MinionOutgoingDamageModMap
        {
            [Label("$" + SummonersShine.LocalizationPath + "Config.Projectile")]
            public ProjectileDefinition projectile;
            [Label("$" + SummonersShine.LocalizationPath + "Config.OutgoingDamage")]
            [Range(0f, 100f)]
            [Increment(1f)]
            [DefaultValue(80)]
            public float outgoingDamage;

            public void Bake() {
                if (projectile.Type < 0 || projectile.Type >= ProjectileLoader.ProjectileCount)
                    return;
                BakedConfig.MinionOutgoingDamageMod[projectile.Type] = outgoingDamage / 100f;
            }
        }

        /// <summary>
        /// ItemDefinition represents an Item identity. A typical use for this class is usage in ModConfig, perhapse to facilitate an Item tweaking mod.
        /// </summary>
        // JSONItemConverter should allow this to be used as a dictionary key.
        [TypeConverter(typeof(ToFromStringConverter<BuffDefinition>))]
        public class BuffDefinition : EntityDefinition
        {
            public static readonly Func<TagCompound, BuffDefinition> DESERIALIZER = Load;

            public override int Type => BuffID.Search.TryGetId(Mod != "Terraria" ? $"{Mod}/{Name}" : Name, out int id) ? id : -1;

            public BuffDefinition() : base() { }
            public BuffDefinition(int type) : base(BuffID.Search.GetName(type)) { }
            public BuffDefinition(string key) : base(key) { }
            public BuffDefinition(string mod, string name) : base(mod, name) { }

            public static BuffDefinition FromString(string s)
                => new(s);

            public static BuffDefinition Load(TagCompound tag)
                => new(tag.GetString("mod"), tag.GetString("name"));
        }

        public class BuffSourceItemMap
        {
            [Label("$" + SummonersShine.LocalizationPath + "Config.Buff")]
            public BuffDefinition buff;

            [Label("$" + SummonersShine.LocalizationPath + "Config.BuffSourceItems")]
            public List<ItemDefinition> items;

            public void Bake()
            {
                if (buff.Type < 0 || buff.Type >= BuffLoader.BuffCount)
                    return;
                int[] rv = new int[items.Count];
                int x = 0;
                bool terminate = false;
                items.ForEach(i =>
                {
                    if (i.Type < 0 || i.Type >= ItemLoader.ItemCount)
                        terminate = true;
                    rv[x] = i.Type;
                });
                if (terminate)
                    return;
                BakedConfig.SetBuffSourceItems(buff.Type, rv);
            }

        }
        public class DefaultProjectileForceAimbotMap
        {

            [Label("$" + SummonersShine.LocalizationPath + "Config.Projectile")]
            public ProjectileDefinition proj;
            [Label("$" + SummonersShine.LocalizationPath + "Config.StartAttackRange")]
            [DefaultValue(1400)]
            public int startAttackRange;
            [Label("$" + SummonersShine.LocalizationPath + "Config.SkipIfCannotHit")]
            [DefaultValue(false)]
            public bool skipIfCannotHit;
        }

        public class DefaultAbilMap
        {
            [Label("$" + SummonersShine.LocalizationPath + "Config.def")]
            public ItemDefinition def;
            [Range(0, 100)]
            [Label("$" + SummonersShine.LocalizationPath + "Config.DefaultAbilityType")]
            public int DefaultAbilityType;
            [Range(int.MinValue, int.MaxValue)]
            [Label("$" + SummonersShine.LocalizationPath + "Config.MinionPower0")]
            public int MinionPower0;
            [Range(int.MinValue, int.MaxValue)]
            [Label("$" + SummonersShine.LocalizationPath + "Config.MinionPower1")]
            public int MinionPower1;
        }

        public class DefaultItemSourceMap
        {
            [Label("$" + SummonersShine.LocalizationPath + "Config.Projectile")]
            public ProjectileDefinition proj;
            [Label("$" + SummonersShine.LocalizationPath + "Config.ItemSource")]
            public ItemDefinition item;
        }

        public void SetConfigMinionPowers() {
            DefaultSpecialAbility[] defaultSpecialAbilityUsed = ReworkMinion_Item.defaultSpecialAbilityUsed;
            CustomDefaultAbils.ForEach(i => {
                if (i.def == null || i.def.IsUnloaded)
                    return;
                int def = i.def.Type;
                if (BakedConfig.CustomSpecialPowersEnabled(def) && defaultSpecialAbilityUsed[def] == null)
                    return;
                minionPower[] minionPowers = ReworkMinion_Item.minionPowers[def];
                defaultSpecialAbilityUsed[def] = DefaultSpecialAbility.abilities[(int)i.DefaultAbilityType];
                defaultSpecialAbilityUsed[def].GetMinionPower(minionPowers, i.MinionPower0, i.MinionPower1);
            });
        }
        public class ReworkMinion_Config_Loader : ILoadable
        {
            void ILoadable.Load(Mod mod)
            {
                current = ModContent.GetInstance<Config>();
            }

            void ILoadable.Unload()
            {
                current = null;
            }
        }
    }
}