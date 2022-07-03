using log4net;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SummonersShine.BakedConfigs;
using SummonersShine.Effects;
using SummonersShine.MinionAI;
using SummonersShine.ModSupport;
using SummonersShine.Prefixes;
using SummonersShine.Projectiles;
using SummonersShine.SpecialAbilities.DefaultSpecialAbility;
using SummonersShine.SpecialData;
using SummonersShine.Textures;
using SummonersShine.VanillaMinionILMods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace SummonersShine
{
    public class SummonersShine : Mod
    {
        public const string LocalizationPath = "Mods.SummonersShine.";
        public const string SummonSpecialPath = LocalizationPath + "SummonSpecial.";
        public const string SummonSpecialCompressedPath = LocalizationPath + "SummonSpecialCompressed.";
        public const string NPCDialogPath = LocalizationPath + "NPCDialog.";
        public const int StartOfModdedItems = 5125;

        public static string[] _minionPowerTooltipCache;
        public static string[] _minionPowerTooltipCache_Compressed;
        public static ILog logger;
        public static SummonersShine modInstance;
        public static Config modConfig;

        public const bool printData = false;
        public override void Load()
        {
            logger = Logger;
            modInstance = this;

            //register all IL mods
            VanillaILModification.RegisterAll();
            
            SkyManager.Instance["SummonersShine:BloodtaintModeSky"] = new BloodtaintModeSky();
        }

        public override void PostSetupContent()
        {
            AutoModSupport.AutoloadAll();
            if (!Main.dedServ)
            {
                //cache minion power tooltips;
                Array.Resize(ref _minionPowerTooltipCache, ItemLoader.ItemCount);
                Array.Resize(ref _minionPowerTooltipCache_Compressed, ItemLoader.ItemCount);
                (from f in typeof(ItemID).GetFields(BindingFlags.Static | BindingFlags.Public)
                 where f.FieldType == typeof(short)
                 select f).ToList<FieldInfo>().ForEach(delegate (FieldInfo field)
                 {
                     short i = (short)field.GetValue(null);
                     bool flag = i > 0 && (int)i < _minionPowerTooltipCache.Length;
                     if (flag)
                     {
                         _minionPowerTooltipCache[(int)i] = SummonSpecialPath + field.Name;
                         _minionPowerTooltipCache_Compressed[(int)i] = SummonSpecialCompressedPath + field.Name;
                     }
                 });

                for (int i = StartOfModdedItems; i < _minionPowerTooltipCache.Length; i++)
                {
                    ModItem item = ItemLoader.GetItem(i);
                    _minionPowerTooltipCache[(int)i] = "Mods." + item.Mod.Name + ".SummonSpecial." + item.Name;
                    _minionPowerTooltipCache_Compressed[(int)i] = "Mods." + item.Mod.Name + ".SummonSpecialCompressed." + item.Name;
                }

                ModEffects.Populate();
                ModTextures.Populate();
            }
            BakedConfig.Instantiate(Config.current);

            MinionDataHandler.ItemSetStaticDefaults();
            CustomSpecialAbilitySourceMinion.Populate();
            DefaultSpecialAbility.BakeDefaultSpecialList();
            base.PostSetupContent();
            SummonerPrefix.PostSetupContent();
            //if (printData)
            //PrintMinionPowerTable();
        }

        public override void Unload()
        {
            //OnNewProjectile = null;
            logger = null;
            modInstance = null;
            _minionPowerTooltipCache = null;

            Main.tooltipPrefixComparisonItem = null;
            Main.CurrentDrawnEntity = null;

            for (int x = 0; x < Main.maxPlayers; x++)
            {
                Player player = Main.player[x];
                if (player != null && player.active)
                {
                    ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
                    playerFuncs.PreSaveAndQuit();
                }

            }
            

            //VanillaILModification.UnregisterAll();
        }
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PacketHandler.HandlePacket(reader, whoAmI);
        }

        void PrintMinionPowerTable()
        {
            string rv = "[table][tr][th]Item[/th][th]Description[/th][th]Cooldown[/th][/tr]";
            rv += PrintItemMinionPowerInfo(ItemID.AbigailsFlower);
            rv += PrintItemMinionPowerInfo(ItemID.BabyBirdStaff);
            rv += PrintItemMinionPowerInfo(ItemID.SlimeStaff);
            rv += PrintItemMinionPowerInfo(ItemID.FlinxStaff);
            rv += PrintItemMinionPowerInfo(ItemID.VampireFrogStaff);
            rv += PrintItemMinionPowerInfo(ItemID.HornetStaff);
            rv += PrintItemMinionPowerInfo(ItemID.ImpStaff);
            rv += PrintItemMinionPowerInfo(ItemID.SpiderStaff);
            rv += PrintItemMinionPowerInfo(ItemID.PirateStaff);
            rv += PrintItemMinionPowerInfo(ItemID.Smolstar);
            rv += PrintItemMinionPowerInfo(ItemID.SanguineStaff);
            rv += PrintItemMinionPowerInfo(ItemID.OpticStaff);
            rv += PrintItemMinionPowerInfo(ItemID.PygmyStaff);
            rv += PrintItemMinionPowerInfo(ItemID.DeadlySphereStaff);
            rv += PrintItemMinionPowerInfo(ItemID.RavenStaff);
            rv += PrintItemMinionPowerInfo(ItemID.XenoStaff);
            rv += PrintItemMinionPowerInfo(ItemID.TempestStaff);
            rv += PrintItemMinionPowerInfo(ItemID.HoundiusShootius);
            rv += PrintItemMinionPowerInfo(ItemID.QueenSpiderStaff);
            rv += "[/table]";
            Logger.Info(rv);
        }

        static string PrintItemMinionPowerInfo(int itemID) {
            string toolTip = SummonersShine._minionPowerTooltipCache[itemID];

            Item item = new Item();
            Projectile projectile = new Projectile();
            item.SetDefaults(itemID);
            projectile.SetDefaults(item.shoot);

            string rv = "[tr]";
            rv += "[td]" + Lang.GetItemName(itemID) + "[/td]";
            minionPower mp1 = ReworkMinion_Item.minionPowers[itemID][0];
            minionPower mp2 = ReworkMinion_Item.minionPowers[itemID][1];
            string mp1NotesString = mp1.DifficultyScale ? " (doubles in Expert, triples in Master)" : "";
            string mp2NotesString = mp2.DifficultyScale ? " (doubles in Expert, triples in Master)" : "";
            MinionProjectileData projData = projectile.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData();
            rv += "[td]" + ReworkMinion_Item.RightClickTextReplacer(Language.GetTextValue(toolTip, Math.Round(ReworkMinion_Item.minionPowerRechargeTime[itemID] / 60f, 2), "[base " + mp1.power + mp1NotesString + "]", "[base " + mp2.power + mp2NotesString + "]")) + "[/td]";
            rv += "[td]" + Math.Round(ReworkMinion_Item.minionPowerRechargeTime[itemID] / 60f, 2) + "[/td]";
            return rv + "[/tr]";
        }
        static Projectile proj_whoamIer(object arg)
        {
            if(arg as Projectile == null)
            {
                return Main.projectile[(int)arg];
            }
            return (Projectile)arg;
        }
        public override object Call(params object[] args)
        {
            switch ((int)args[0]) {
                //Add mod configs
                case 0:
                    object valueArg = args.Length == 4 ? args[3] : null;
                    BakedConfig.AddModdedConfig_ThroughCode((int)args[1], args[2], valueArg);
                    return null;
                case 1:
                    MinionDataHandler.ModSupport_HookProjectile((int)args[1], (int)args[2], args[3]);
                    return null;
                case 2:
                    MinionDataHandler.ModSupport_AddItemStatics((int)args[1], (Func<Player, Vector2, Entity>)args[2], (Func<Player, Item, List<Projectile>, List<Projectile>>)args[3], (Tuple<float, int, int, bool>[])args[4], (int)args[5], (bool)args[6]);
                    return null;
                case 3:
                    MinionDataHandler.ModSupport_AddSpecialPowerDisplayData(args[1]);
                    return null;
                case 4:
                    ModSupportUtil.SetVariable_projFuncs(proj_whoamIer(args[1]), (int)args[2], args[3]);
                    return null;
                case 5:
                    ModSupportUtil.SetVariable_projData(proj_whoamIer(args[1]), (int)args[2], args[3]);
                    return null;
                case 6:
                    return ModSupportUtil.ExtractVariable_projFuncs(proj_whoamIer(args[1]), (int)args[2]);
                case 7:
                    return ModSupportUtil.ExtractVariable_projData(proj_whoamIer(args[1]), (int)args[2]);
                case 8:
                    ModSupportUtil.SetVariable_playerFuncs((Player)args[1], (int)args[2], args[3]);
                    return null;
                case 9:
                    return ModSupportUtil.ExtractVariable_playerFuncs((Player)args[1], (int)args[2]);
                case 10:
                    object[] argsList = new object[args.Length - 2];
                    for(int x = 2; x < args.Length; x++)
                    {
                        argsList[x - 2] = args[x];
                    }
                    return ModSupportUsefulFuncs.RunUsefulFunction((int)args[1], argsList);
                case 11:
                    ModSupportProjectileBuff.AddModdedBuff((Projectile)args[1], (Projectile)args[2], (Mod)args[3], (int)args[4], (Func<Projectile, bool>)args[5], (Func<Projectile, float>)args[6], (Action<Projectile, Color>)args[7], (Action<Projectile, Color>)args[8]);
                    return null;
                case 12:
                    ModSupportProjectileBuff.RemoveModdedBuff((Projectile)args[1], (Projectile)args[2], (Mod)args[3], (int)args[4]);
                    return null;
                case 13:
                    ModSupportProjectileBuff.AddProjectileBuff((Projectile)args[1], (Projectile)args[2], (int)args[3]);
                    return null;
                case 14:
                    ModSupportProjectileBuff.RemoveProjectileBuff((Projectile)args[1], (Projectile)args[2], (int)args[3]);
                    return null;
                case 15:
                    ModSupportUtil.SetVariable_itemFuncs((Item)args[1], (int)args[2], args[3]);
                    return null;
                case 16:
                    return ModSupportUtil.ExtractVariable_itemFuncs((Item)args[1], (int)args[2]);
                case 17:
                    MinionDataHandler.ModSupport_HookBuff((int)args[1], (int)args[2], args[3]);
                    return null;
                case 18:
                    MinionDataHandler.ModSupport_HookDefaultSpecialHandler((Mod)args[1], (int)args[2], args[3]);
                    return null;
                case 19:
                    DefaultSpecialAbility_ModdedCustom.AssignHook((Mod)args[1], (string)args[2], (int)args[3], args[4]);
                    return null;
                case 20:
                    DefaultWhipSpecialAbility_ModdedCustom.AssignHook((Mod)args[1], (string)args[2], (int)args[3], args[4]);
                    return null;
                default:
                    throw new ArgumentOutOfRangeException("[Call] args[0] out of range! (Range 0-20)");
            }
        }
    }
}