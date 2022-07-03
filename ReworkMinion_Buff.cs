using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SummonersShine.BakedConfigs;
using SummonersShine.ModSupport;
using SummonersShine.SpecialAbilities.DefaultSpecialAbility;
using SummonersShine.Textures;
using SummonersShine.VanillaMinionBuffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine
{
    public class ReworkMinion_Buff : GlobalBuff
    {
        public override void SetStaticDefaults()
        {
            MinionDataHandler.BuffSetStaticDefaults();
        }
        public override void Update(int type, NPC npc, ref int buffIndex)
        {
            if (type == BuffID.Slow)
            {
                npc.GetGlobalNPC<ReworkMinion_NPC>().slowAmount = 0.5f;
            }
        }

        public override bool RightClick(int type, int buffIndex)
        {
            int[] itemTypes = GetItemTypes(type);
            if (itemTypes == null) return true;

            Player player = Main.player[Main.myPlayer];
            ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
            for (int x = 0; x < itemTypes.Length; x++)
            {
                MinionEnergyCounter counter = playerFuncs.GetMinionCollection(itemTypes[x]);
                List<Projectile> counters = counter.minions.ToList();
                counters.ForEach(i => i.Kill());
            }
            return true;
        }

        public static int[] GetItemTypes(int type)
        {
            int[] itemTypes = null;
            ModSupportBuff modSupportBuff = MinionDataHandler.ModSupportBuffs[type];
            if (modSupportBuff != null)
            {
                if (modSupportBuff.OverrideGetLinkedItems != null)
                    itemTypes = modSupportBuff.OverrideGetLinkedItems(type);
            }

            if (itemTypes == null)
            {
                itemTypes = BakedConfig.GetBuffSourceItemTypes(type);
            }
            return itemTypes;
        }

        public static void DrawSummonerPin(SpriteBatch spriteBatch, int type, int buffIndex, BuffDrawParams drawParams)
        {
            ModSupportBuff modSupportBuff = MinionDataHandler.ModSupportBuffs[type];
            int[] itemTypes = GetItemTypes(type);
            if (itemTypes == null) return;

            float topPos = -1;
            float bottomPos = -1;
            Player player = Main.player[Main.myPlayer];
            ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
            for (int x = 0; x < itemTypes.Length; x++)
            {
                MinionEnergyCounter counter = playerFuncs.GetMinionCollection(itemTypes[x]);
                DefaultSpecialAbility defaultSpecial = ReworkMinion_Item.defaultSpecialAbilityUsed[itemTypes[x]];
                float currentTopPos = -1;
                float currentBottomPos = -1;
                if (defaultSpecial != null)
                {
                    if (!defaultSpecial.GetArbitraryBuffGaugeData(counter.minions, out currentTopPos, out currentBottomPos))
                        continue;
                }
                else
                {
                    if (modSupportBuff != null && modSupportBuff.OverrideGetPinPositions != null)
                    {
                        Tuple<bool, float, float> result = modSupportBuff.OverrideGetPinPositions(type, itemTypes[x], counter.minions);
                        if (!result.Item1)
                            continue;
                        currentTopPos = result.Item2;
                        currentBottomPos = result.Item3;
                    }
                    else
                    {
                        if (counter.minionFullPercentage.Count == 0)
                            continue;
                        currentBottomPos = counter.minionFullPercentage[0];
                        currentTopPos = counter.minionFullPercentage[counter.minionFullPercentage.Count - 1];
                    }
                }
                if (topPos == -1 || topPos < currentTopPos)
                    topPos = currentTopPos;
                if (bottomPos == -1 || bottomPos > currentBottomPos)
                    bottomPos = currentBottomPos;
            }
            if (topPos == -1)
                return;

            Vector2 drawPos = drawParams.Position;
            drawPos.Y -= 2;

            spriteBatch.Draw(ModTextures.BuffMinionPowerGauge, drawPos, Color.White);

            float yDisp = (1 - bottomPos) * 30;
            Color pinColor = Color.White;
            if (bottomPos != 1)
            {
                pinColor *= 0.9f;
                pinColor.A = 255;
            }
            Vector2 pos = drawPos + new Vector2(0, yDisp);
            pos.X = (int)pos.X;
            pos.Y = (int)pos.Y;
            spriteBatch.Draw(ModTextures.BuffMinionPowerPinBottom, pos, pinColor);


            yDisp = (1 - topPos) * 30;
            pinColor = Color.White;
            if (topPos != 1)
            {
                pinColor *= 0.9f;
                pinColor.A = 255;
            }
            pos = drawPos + new Vector2(0, yDisp);
            pos.X = (int)pos.X;
            pos.Y = (int)pos.Y;
            spriteBatch.Draw(ModTextures.BuffMinionPowerPin, pos, pinColor);
        }
    }
}
