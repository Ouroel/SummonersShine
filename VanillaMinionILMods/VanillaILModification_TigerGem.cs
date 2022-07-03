using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SummonersShine.MinionAI;
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
        const int fullTurban = 6;
        const int fullHalfTurban = 2;

        public static void AI164_ReplaceHomeLocation(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchCall<Projectile>("AI_164_GetHomeLocation")))
            {
                SummonersShine.logger.Error("Hook failed! (AI164_ReplaceHomeLocation, Cannot find GetHomeLocation!)");
                return;
            }
            c.Remove();
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(AI164_NewHomeLocation);
        }
        public static Vector2 AI164_NewHomeLocation(Player player, int stackedIndex, int totalIndexes, Projectile self) {
            if(self.type == ProjectileID.StormTigerGem)
                return DefaultMinionAI.StormTigerGem_GetHomeLocation(player, stackedIndex, totalIndexes);
            return Projectile.AI_164_GetHomeLocation(player, stackedIndex, totalIndexes);
        }

        public static void UpdateBuffs_FixUpdateStormTigerStatus(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchCall<Player>("UpdateStormTigerStatus")))
            {
                SummonersShine.logger.Error("Hook failed! (UpdateBuffs_FixUpdateStormTigerStatus, Cannot find UpdateStormTigerStatus!)");
                return;
            }
            c.Remove();
            c.EmitDelegate<Action<Player>>(UpdateStormTigerStatus);

        }
        private static void UpdateStormTigerStatus(this Player player)
        {
            int num = player.ownedProjectileCounts[831];

            if (num > 6)
            {
                num = 835;
            }
            else if (num > 3)
            {
                num = 834;
            }
            else if (num > 0)
            {
                num = 833;
            }
            else {
                num = -1;
            }

            DesertTigerStatCollection collection = player.GetModPlayer<ReworkMinion_Player>().GetSpecialData<DesertTigerStatCollection>();
            Projectile tigerBody = collection.megaMinionBody;
            Vector2 center = player.Center;
            if (tigerBody != null)
            {
                if (tigerBody.type != num)
                {
                    center = tigerBody.Center;
                    tigerBody.Kill();
                }
                else return;
            }
            if (num == -1)
                return;
            Projectile.NewProjectile(player.GetSource_Misc("StormTigerTierSwap"), center, Vector2.Zero, num, 0, 0f, player.whoAmI, 0f, 0f);
        }
    }
}
