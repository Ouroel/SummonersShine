using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using System;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.Utilities;
using SummonersShine.Prefixes;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using SummonersShine.SpecialAbilities;
using SummonersShine.SpecialData;
using SummonersShine.SpecialAbilities.DefaultSpecialAbility;
using SummonersShine.BakedConfigs;
using Terraria.Graphics.Capture;
using Terraria.Audio;
using Terraria.UI;
using Terraria.GameInput;
using SummonersShine.SpecialAbilities.WhipSpecialAbility;
using SummonersShine.Projectiles;

namespace SummonersShine
{
	public enum mpScalingType
	{
		add,
		subtract,
		multiply,
		divide,
	}
	public enum mpRoundingType
	{
		dp2,
		integer,
	}

	public struct minionPower
	{
		public float power;
		public mpScalingType scalingType;
		public mpRoundingType roundingType;
		public bool DifficultyScale;

		public static minionPower NewMP(float power, mpScalingType scalingType = mpScalingType.multiply, mpRoundingType roundingType = mpRoundingType.dp2, bool DifficultyScale = false)
		{

			return new minionPower
			{
				power = power,
				scalingType = scalingType,
				roundingType = roundingType,
				DifficultyScale = DifficultyScale
			};
		}
    }
	public class ReworkMinion_Item : GlobalItem
	{

		public override bool InstancePerEntity => true;
		public const int num_vanilla_prefixes = 24;
		public static minionPower[][] minionPowers;
		public static int[] minionPowerRechargeTime;
		public static DefaultSpecialAbility[] defaultSpecialAbilityUsed;
		static int[] vanillaTypelessPrefixes_Weapons = new int[num_vanilla_prefixes];
		public static List<ModPrefix> prefixes_weapons;

		public float prefixMinionPower = 1f;
		float originalUseTime = 0;
		public bool UsingSpecialAbility = false;
		bool IsAnItemWhichUsedToHaveMana = false;

		public static Func<Player, Vector2, Entity>[] specialAbilityFindTarget;
		public static Func<Item, ReworkMinion_Player, List<Projectile>>[] specialAbilityFindMinions;

		public class ReworkMinion_Item_Loader : ILoadable
		{
			public void Load(Mod mod)
            {
				prefixes_weapons = new();
				prefixes_weapons.Add(ModContent.GetInstance<SummonerPrefix_Benzona>());
				prefixes_weapons.Add(ModContent.GetInstance<SummonerPrefix_Bloodbound>());
				prefixes_weapons.Add(ModContent.GetInstance<SummonerPrefix_Devoted>());
				prefixes_weapons.Add(ModContent.GetInstance<SummonerPrefix_Fanatical>());
				prefixes_weapons.Add(ModContent.GetInstance<SummonerPrefix_Heretical>());
				prefixes_weapons.Add(ModContent.GetInstance<SummonerPrefix_Initiated>());
				prefixes_weapons.Add(ModContent.GetInstance<SummonerPrefix_Divine>());
				prefixes_weapons.Add(ModContent.GetInstance<SummonerPrefix_Lunatic>());
				prefixes_weapons.Add(ModContent.GetInstance<SummonerPrefix_Remphanic>());
				prefixes_weapons.Add(ModContent.GetInstance<SummonerPrefix_Righteous>());
				prefixes_weapons.Add(ModContent.GetInstance<SummonerPrefix_Shining>());
				prefixes_weapons.Add(ModContent.GetInstance<SummonerPrefix_Unholy>());

				//PrintPrefixesToTable();

				prefixes_weapons.AddRange(PrefixLoader.GetPrefixesInCategory(PrefixCategory.AnyWeapon));

				int[] vanillaTypelessPrefixes = ReworkMinion_Item.vanillaTypelessPrefixes_Weapons;
				vanillaTypelessPrefixes[0] = 36;
				vanillaTypelessPrefixes[1] = 37;
				vanillaTypelessPrefixes[2] = 38;
				vanillaTypelessPrefixes[3] = 39;
				vanillaTypelessPrefixes[4] = 40;
				vanillaTypelessPrefixes[5] = 41;
				vanillaTypelessPrefixes[6] = 42;
				vanillaTypelessPrefixes[7] = 43;
				vanillaTypelessPrefixes[8] = 44;
				vanillaTypelessPrefixes[9] = 45;
				vanillaTypelessPrefixes[10] = 46;
				vanillaTypelessPrefixes[11] = 47;
				vanillaTypelessPrefixes[12] = 48;
				vanillaTypelessPrefixes[13] = 49;
				vanillaTypelessPrefixes[14] = 50;
				vanillaTypelessPrefixes[15] = 51;
				vanillaTypelessPrefixes[16] = 53;
				vanillaTypelessPrefixes[17] = 54;
				vanillaTypelessPrefixes[18] = 55;
				vanillaTypelessPrefixes[19] = 56;
				vanillaTypelessPrefixes[20] = 57;
				vanillaTypelessPrefixes[21] = 59;
				vanillaTypelessPrefixes[22] = 60;
				vanillaTypelessPrefixes[23] = 61;
			}

			public void Unload() {
				vanillaTypelessPrefixes_Weapons = null;
				prefixes_weapons = null;
				minionPowers = null;
				defaultSpecialAbilityUsed = null;
				specialAbilityFindTarget = null;
				specialAbilityFindMinions = null;
				minionPowerRechargeTime = null;
			}
			static void PrintPrefixesToTable()
			{
				prefixes_weapons.Sort((i, j) =>
				{
					float vali = (i as SummonerPrefix).GetRealValueMult();
					float valj = (j as SummonerPrefix).GetRealValueMult();
					if (vali > valj)
						return -1;
					else if (vali < valj)
						return 1;
					return 0;
				});
				string print = "[table][tr][th]Modifier[/th][th]Damage[/th][th]Speed[/th][th]Knockback[/th][th]Critical strike chance[/th][th]Ability Power[/th][th]Tier[/th][th]Value[/th][/tr]";
				prefixes_weapons.ForEach(i => print += (i as Prefixes.SummonerPrefix).PrintToTable());
				print += "[/table]";
				Logging.PublicLogger.Info(print);
			}
		}

		public override float UseAnimationMultiplier(Item item, Player player)
		{
			if (ProjectileID.Sets.MinionTargettingFeature[item.shoot] && player.altFunctionUse == 2)
				return 0.25f;
			return 1f;
		}

        public override float UseTimeMultiplier(Item item, Player player)
        {
			if (ProjectileID.Sets.MinionTargettingFeature[item.shoot] && player.altFunctionUse == 2)
				return 0.25f;
			return 1f;
		}

        public override void UseAnimation(Item item, Player player)
        {
            base.UseAnimation(item, player);
        }
		public bool UseSpecialAbility(Item item, Player player, bool IgnoreFlag = false)
		{
			bool flag = player.ItemTimeIsZero && player.selectedItem != 58 && player.controlUseTile && player.releaseUseItem && !player.controlUseItem && !player.mouseInterface && !CaptureManager.Instance.Active && !Main.HoveringOverAnNPC;

			if (!IgnoreFlag)
			{
				if (player.itemTime <= 1)
				{
					UsingSpecialAbility = false;
				}
				else
				{
					return UsingSpecialAbility;
				}
			}
			if (IgnoreFlag || flag)
			{
				if (BakedConfig.CustomSpecialPowersEnabled(item.type))
				{
					ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
					if (specialAbilityFindMinions[item.type] == null)
						return false;
					List<Projectile> projectiles = specialAbilityFindMinions[item.type](item, playerFuncs);
					if (projectiles != null && projectiles.Count > 0)
					{
						Projectile[] targets = projectiles.ToArray();
						Vector2 mouseworld = Main.MouseWorld;
						PacketHandler.WritePacket_SendSpecialAbility(mouseworld, item.type, targets);
						bool usedMinionAbilities = UseMinionAbilities(player, mouseworld, item.type, targets, false);
						PacketHandler.WritePacket_SendRightClick();
						UsingSpecialAbility = UsingSpecialAbility || usedMinionAbilities;
						return UsingSpecialAbility;
					}
					return UsingSpecialAbility;
				}
			}
			return false;
		}

        public override bool? UseItem(Item item, Player player)
		{
			if (ProjectileID.Sets.MinionTargettingFeature[item.shoot] && player.altFunctionUse == 2)
			{
				if (!UsingSpecialAbility)
				{
					player.MinionNPCTargetAim(false);
				}
			}
			return null;
        }

		public static bool UseMinionAbilities(Player player, Vector2 mouseWorld, int itemID, Projectile[] viableMinions, bool fromServer = false)
		{
			Entity target = specialAbilityFindTarget[itemID](player, mouseWorld);
			if (target != null)
			{
				for (int i = 0; i < viableMinions.Length; i++)
				{
					Projectile projectile = viableMinions[i];
					ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
					MinionProjectileData projData = projFuncs.GetMinionProjData();
					projFuncs.minionOnSpecialAbilityUsed(projectile, projFuncs, projData, target, projData.castingSpecialAbilityType, fromServer);
					projData.castingSpecialAbilityType = 0;
				}
				return true;
			}
			return false;
		}

        public float GetMinionPower(Player player, int itemID, int index)
        {
			return player.GetModPlayer<ReworkMinion_Player>().GetMinionPower(index, minionPowers[itemID], prefixMinionPower);
		}

		public static bool IsSummon(Item item) {
			return item.DamageType == DamageClass.Summon || item.DamageType == DamageClass.SummonMeleeSpeed;
		}
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			bool compressDesc = ClientConfig.current.CompressSpecialAbilityDescription;
			if (item.CountsAsClass(DamageClass.Summon))
			{
				MinionDataHandler.ItemSetDefaults(item);
				ReworkMinion_Item globalItem = item.GetGlobalItem<ReworkMinion_Item>();

				int index;
				bool iter;
				TooltipLine line;

				bool defaultNotNull = defaultSpecialAbilityUsed[item.netID] != null;
				bool CanPrefix = !defaultNotNull || (item.damage > 0 && !BakedConfig.ItemNonPrefixable[item.type]);
				if (CanPrefix)
				{
					string toolTip;
					string text;



					Player player = Main.LocalPlayer;
					int numMinionPowers = minionPowers[item.type].Length;
					object[] textData = new object[numMinionPowers + 1];
					textData[0] = minionPowerRechargeTime[item.type] / 60;
					for (int x = 0; x < numMinionPowers; x++)
					{
						textData[x + 1] = globalItem.GetMinionPower(player, item.type, x);
					}

					bool tried = false;
					if (defaultNotNull)
					{
						ReIterate_Default:
						if (compressDesc ^ tried)
						{
							toolTip = defaultSpecialAbilityUsed[item.netID].GetFullLocalizationPath("SummonSpecialDefaultCompressed.");
							text = Language.GetTextValue(toolTip, textData);
						}
						else
						{
							toolTip = defaultSpecialAbilityUsed[item.netID].GetFullLocalizationPath("SummonSpecialDefault.");
							text = Language.GetTextValue(toolTip, textData);
						}
						tried = !tried && toolTip == text;
						if (tried)
							goto ReIterate_Default;
					}
					else
					{
						ReIterate_RightClick:
						if (compressDesc ^ tried)
						{
							toolTip = SummonersShine._minionPowerTooltipCache_Compressed[item.netID];
							text = Language.GetTextValue(toolTip, textData);
						}
						else
						{
							toolTip = SummonersShine._minionPowerTooltipCache[item.netID];
							text = Language.GetTextValue(toolTip, textData);
						}
						tried = !tried && toolTip == text;
						if (tried)
							goto ReIterate_RightClick;
					}
					text = RightClickTextReplacer(text);


					line = new TooltipLine(Mod, "SummonSpecial", text);
					index = 0;
					iter = true;
					while (iter)
					{
						if (index >= tooltips.Count)
							break;
						switch (tooltips[index].Name)
						{
							case "ItemName":
							case "Favorite":
							case "FavoriteDesc":
							case "NoTransfer":
							case "Social":
							case "SocialDesc":
							case "Damage":
							case "CritChance":
							case "Speed":
							case "Knockback":
								index++;
								break;
							default:
								iter = false;
								break;
						}
					}
					tooltips.Insert(index, line);
				}

				index = tooltips.Count - 1;
				iter = true;
				while (iter)
				{
					if (index <= 0)
						break;
					switch (tooltips[index].Name)
					{
						case "ItemName":
						case "Favorite":
						case "FavoriteDesc":
						case "NoTransfer":
						case "Social":
						case "SocialDesc":
						case "Damage":
						case "CritChance":
						case "Speed":
						case "Knockback":
						case "PickPower":
						case "AxePower":
						case "HammerPower":
						case "UseMana":
						case "Consumable":
						case "Material":
						case "PrefixDamage":
						case "PrefixSpeed":
						case "PrefixCritChance":
							iter = false;
							break;
						default:
							if (tooltips[index].Name.StartsWith("Tooltip"))
							{
								iter = false;
								break;
							}
							else
							{
								index--;
							}
							break;
					}
				}

				if (!item.social && prefixMinionPower != 1)
				{
					string power = Math.Round(100 * (prefixMinionPower - 1)).ToString();
					bool modifierBad = prefixMinionPower < 1;
					if (!modifierBad)
						power = '+' + power;
					line = new TooltipLine(Mod, "SummonPrefix", Language.GetTextValue(SummonersShine.LocalizationPath + "SummonSpecialDefault.MinionPower", power))
					{
						IsModifier = true,
						IsModifierBad = modifierBad
					};
					tooltips.Insert(index + 1, line);
				}

				index = tooltips.Count - 1;
				while (tooltips[index].Name != "Damage")
				{
					index--;
					if (index <= 0 || tooltips[index].Name == "CritChance")
						return;
				}

				int weaponCrit = Main.player[Main.myPlayer].GetWeaponCrit(item);
				line = new TooltipLine(Mod, "CritChance", weaponCrit.ToString() + Lang.tip[5].Value);
				tooltips.Insert(index + 1, line);
			}
		}

		public static string RightClickTextReplacer(string tooltip) {
			if (tooltip.Contains("<right>"))
			{
				InputMode inputMode = InputMode.Keyboard;
				if (PlayerInput.UsingGamepad)
				{
					inputMode = InputMode.XBoxGamepad;
				}
				if (inputMode == InputMode.XBoxGamepad)
				{
					KeyConfiguration keyConfiguration = PlayerInput.CurrentProfile.InputModes[inputMode];
					string text = PlayerInput.BuildCommand("", true, new List<string>[]
					{
							keyConfiguration.KeyStatus["MouseRight"]
					});
					text = text.Replace(": ", "");
					tooltip = tooltip.Replace("<right>", text);
				}
				else
				{
					KeyConfiguration keyConfiguration = PlayerInput.CurrentProfile.InputModes[inputMode];
					string text = PlayerInput.BuildCommand("", true, new List<string>[]
					{
							keyConfiguration.KeyStatus["MouseRight"]
					});
					text = text.Replace(": ", "");
					text = text.Replace("Mouse2", "right click");
					text = text.Replace("Mouse1", "left click");
					tooltip = tooltip.Replace("<right>", text);
				}
			}
			return tooltip;
		}
        public override int ChoosePrefix(Item item, UnifiedRandom rand)
		{
			if (IsSummon(item) && item.damage > 0 && (BakedConfig.ItemNonPrefixable == null || !BakedConfig.ItemNonPrefixable[item.type]))
			{
				int rng = rand.Next(0, prefixes_weapons.Count + num_vanilla_prefixes);
				if (rng < num_vanilla_prefixes) {
					return vanillaTypelessPrefixes_Weapons[rng];
				}
				return prefixes_weapons[rng - num_vanilla_prefixes].Type;
			}
			return -1;
		}

		public override GlobalItem Clone(Item item, Item itemClone)
        {
			ReworkMinion_Item rv = (ReworkMinion_Item)base.Clone(item, itemClone);
			if (itemClone.prefix == 0)
			{
				rv.prefixMinionPower = 1;
			}
			return rv;
        }
		public float GetUseTimeModifier(Item item)
		{
			if(originalUseTime != 0)
				return item.useTime / originalUseTime;
			Item compItem = ModUtils.GetPrefixComparisonItem(item.netID);
			originalUseTime = compItem.useTime;
			if (originalUseTime != 0)
				return item.useTime / originalUseTime;
			else
				return 1;
		}
        public override void SetDefaults(Item item)
		{
			if (item.CountsAsClass(DamageClass.Summon))
			{
				MinionDataHandler.ItemSetDefaults(item);
				if (BakedConfig.initialized && !BakedConfig.ItemRetainManaCost[item.type])
				{
					if (item.mana != 0)
						IsAnItemWhichUsedToHaveMana = true;
					item.mana = 0;
				}
				prefixMinionPower = 1;
			}
			else if (Config.current.IsBloodtaintMode() && (item.pick > 0 || item.axe > 0 || item.hammer > 0))
			{
				item.damage = 0;
			}
		}
        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
		}

		public override bool CanUseItem(Item item, Player player)
		{
			if (Config.current.IsBloodtaintMode() && item.damage > 0 && item.pick == 0 && item.axe == 0 && item.hammer == 0 && !IsSummon(item))
			{
				return false;
			}
			if (player.altFunctionUse != 2)
			{
				VanillaMinionBuffs.VanillaMinionBuff.AddVanillaSentryBuff(player, item);
			}
			if (item.CountsAsClass(DamageClass.Summon))
			{
				MinionDataHandler.ItemSetDefaults(item);
			}
			DefaultSpecialAbility special = defaultSpecialAbilityUsed[item.type];
			if (special != null)
			{
				ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
				bool isWhip = special.OnWhipUsed(player, playerFuncs, item, this);
				if (isWhip)
					playerFuncs.OnWhipUsed(item, this);
			}
			return true;
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (IsAnItemWhichUsedToHaveMana && !player.HasBuff(item.buffType))
			{
				player.AddBuff(item.buffType, 2);
			}
			return true;
        }
    }
}