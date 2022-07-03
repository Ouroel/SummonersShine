using Microsoft.Xna.Framework;
using SummonersShine.Effects;
using SummonersShine.Projectiles;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.MinionAI
{
    public static partial class DefaultMinionAI
    {
        public static void RavenSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            for (int i = 0; i < 20; i++)
                Dust.NewDustDirect(projectile.Center, projectile.width, projectile.height, ModEffects.RavenFeather).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
        }
        public static void RavenDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            for (int i = 0; i < 20; i++)
                Dust.NewDustDirect(projectile.Center, projectile.width, projectile.height, ModEffects.RavenFeather).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
        }
        public static void Raven_PostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (projectile.ai[0] == 0)
                return;
            int attackTarget = -1;
            projectile.Minion_FindTargetInRange(1400, ref attackTarget, false, null);
            if (attackTarget != -1)
                projectile.ai[0] = 0;
        }

        public static void RavenSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity _target, int specialType, bool fromServer)
        {
            projData.energy = 0;

            if (Main.myPlayer == projectile.owner)
            {
                Projectile newProj = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), _target.position, Vector2.Zero, ProjectileModIDs.NevermorePillar, 0, projectile.knockBack, projectile.owner);
                newProj.originalDamage = projectile.originalDamage;
                newProj.netUpdate = true;
            }
        }
    }
}
