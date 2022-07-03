using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SummonersShine.Projectiles;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace SummonersShine.SpecialAbilities
{
    public static partial class SpecialAbility
    {
        public static void PirateOnSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity _target, int specialType, bool fromServer)
        {
            Player player = Main.player[projectile.owner];
            projData.energy = 0;
            ReworkMinion_Player ownerData = player.GetModPlayer<ReworkMinion_Player>();
            PirateStatCollection collection = ownerData.GetSpecialData<PirateStatCollection>();
            (collection.megaMinionBody.ModProjectile as DastardlyDoubloon).RechargeShield((int)projFuncs.GetMinionPower(projectile, 0));
        }
        public static bool PirateCustomAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            return true;
        }
        public static List<Projectile> PreAbility_Pirate(Item summonItem, ReworkMinion_Player player)
        {
            PirateStatCollection collection = player.GetSpecialData<PirateStatCollection>();
            if (!collection.AddMegaMinion(player.Player, false, summonItem.GetSource_ItemUse(summonItem)))
                return PreAbility_FindAnyMinion(summonItem, player);
            summonItem.GetGlobalItem<ReworkMinion_Item>().UsingSpecialAbility = true;
            return null;
        }
    }
}
