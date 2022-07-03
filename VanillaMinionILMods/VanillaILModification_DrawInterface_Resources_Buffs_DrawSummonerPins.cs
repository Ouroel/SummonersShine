using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {
        private static void DrawInterface_Resources_Buffs_DrawSummonerPins(On.Terraria.Main.orig_DrawInterface_Resources_Buffs orig, Main self)
        {
            orig(self);
			DrawAllSummonerPins();
		}

		static void DrawAllSummonerPins() {

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(0, null, SamplerState.LinearClamp, DepthStencilState.None, null, null, Main.UIScaleMatrix);
			int num2 = 11;
			for (int i = 0; i < Player.MaxBuffs; i++)
			{
				if (Main.player[Main.myPlayer].buffType[i] > 0)
				{
					int x = 32 + i * 38;
					int num4 = 76;
					if (i >= num2)
					{
						x = 32 + Math.Abs(i % 11) * 38;
						num4 += 50 * (i / 11);
					}
					PreDrawSummonerPin(i, x, num4);
				}
			}
		}

		static void PreDrawSummonerPin(int buffSlotOnPlayer, int x, int y) {
			int num = Main.player[Main.myPlayer].buffType[buffSlotOnPlayer];
			if (num == 0)
			{
				return;
			}
			Color color = new Color(Main.buffAlpha[buffSlotOnPlayer], Main.buffAlpha[buffSlotOnPlayer], Main.buffAlpha[buffSlotOnPlayer], Main.buffAlpha[buffSlotOnPlayer]);
			Asset<Texture2D> asset = TextureAssets.Buff[num];
			Texture2D texture2D = asset.Value;
			Vector2 vector = new Vector2((float)x, (float)y);
			int num2 = asset.Width();
			int num3 = asset.Height();
			Vector2 vector2 = new Vector2((float)x, (float)(y + num3));
			Rectangle rectangle = new Rectangle(0, 0, num2, num3);
			Rectangle mouseRectangle = new Rectangle(x, y, num2, num3);
			BuffDrawParams buffDrawParams = new BuffDrawParams(texture2D, vector, vector2, rectangle, mouseRectangle, color);
			ReworkMinion_Buff.DrawSummonerPin(Main.spriteBatch, num, buffSlotOnPlayer, buffDrawParams);
		}
    }
}
