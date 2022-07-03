using Microsoft.Xna.Framework;
using SummonersShine.SpecialAbilities;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SummonersShine.MinionAI
{
    public static partial class DefaultMinionAI
    {
        public static void Empty(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
        }
        public static void Empty(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
        }
        public static void NoEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
        }
        public static void AI_067_ResetAggro(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            Player player = Main.player[projectile.owner];
            if (player.rocketDelay2 > 0)
            {
                Vector2 pos = Main.player[projectile.owner].Bottom / 16;
                Vector2 pos2 = pos + new Vector2(0, 16);
                bool foundTarget = false;
                for (int x = 0; x < Main.maxNPCs; x++) {
                    NPC test = Main.npc[x];
                    if (test != null && test.active && test.CanBeChasedBy(projectile))
                    {
                        float dist = 800 + 40 * projectile.minionPos;
                        foundTarget = SpecialAbility.PygmyMinionCheck(projectile, test, projectile.Center, dist, ref dist);
                        if (foundTarget)
                            break;
                    }
                }
                if (foundTarget)
                {
                    SingleThreadExploitation.PlayerRocketDelay = player.rocketDelay2;
                    player.rocketDelay2 = 0;
                    projectile.ai[0] = 0;
                }
            }
        }

        public static void AI_067_ResetRocketDelay(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (SingleThreadExploitation.PlayerRocketDelay != 0)
            {
                Player player = Main.player[projectile.owner];
                player.rocketDelay2 = SingleThreadExploitation.PlayerRocketDelay;
                SingleThreadExploitation.PlayerRocketDelay = 0;
            }
        }
    }
}
