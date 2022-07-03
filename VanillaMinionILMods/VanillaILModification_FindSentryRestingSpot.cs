using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {
        public static void FindSentryRestingSpot_EmitMagenChiunCheck(On.Terraria.Player.orig_FindSentryRestingSpot func, Player self, int checkProj, out int worldX, out int worldY, out int pushYUp)
        {
            func(self, checkProj, out worldX, out worldY, out pushYUp);
            if (!PlayerHasMagenChiun(self))
                return;
            Vector2 mouseWorld = Main.MouseWorld;
            self.LimitPointToPlayerReachableArea(ref mouseWorld);
            worldY = (int)mouseWorld.Y;
        }

        static bool PlayerHasMagenChiun(Player player) {
            return player.GetModPlayer<ReworkMinion_Player>().HasMagenChiunEquipped;
        }
    }
}
