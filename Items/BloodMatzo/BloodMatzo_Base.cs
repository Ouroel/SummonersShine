using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.Items.BloodMatzo
{
    //unfinished item: use it to focus minions on a target
    public abstract class BloodMatzo_Base : ModItem
    {
        public virtual void OnHitNPCWithProj(Projectile projectile, Entity ent, ref int damage, ref bool crit, Vector2 cursorPos)
        {

        }

        public override void SetDefaults()
        {
            Item.holdStyle = 1;
            Item.useTime = 1;
            Item.useAnimation = 0;
        }

        public override bool? UseItem(Player player)
        {
            player.GetModPlayer<ReworkMinion_Player>().currentBloodMatzo = this;
            return null;
        }
    }
}
