using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.Buffs
{
    public class Bloodtaint : ModBuff
    {
        public override void SetStaticDefaults()
        {
            //Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Main.persistentBuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
            playerFuncs.minionASIgnoreMainWeapon *= 4f;
            //playerFuncs.minionASRetreating *= 0.5f;
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}
