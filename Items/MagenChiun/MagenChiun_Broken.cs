using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SummonersShine.SpecialData;
using SummonersShine.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.Items.MagenChiun
{
    public class MagenChiun_Broken : ModItem
    {
        public override string Name => "CorrodedShield_Broken";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ReworkMinion_Player>().HasMagenChiunEquipped = true;
        }

        public static void MagenChiun_Draw(Vector2 origin, Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Color lightColor)
        {
            Texture2D value = ModTextures.MagenChiunPlatform;

            Rectangle rectangle = value.Frame(1, 2, 0, 0, 0, 0);
            Rectangle rectangle2 = value.Frame(1, 2, 0, 1, 0, 0);

            Vector2 drawPos = origin - Main.screenPosition;
            Vector2 drawOrigin = new Vector2(rectangle.Width * 0.5f, rectangle.Height * 0.5f);
            float alpha = 1 - proj.alpha / 255f;

            Main.EntitySpriteDraw(value, drawPos, new Rectangle?(rectangle2), Color.White * alpha, 0f, drawOrigin, proj.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(value, drawPos, new Rectangle?(rectangle), lightColor * alpha, 0f, drawOrigin, proj.scale, SpriteEffects.None, 0);
        }
    }
}
