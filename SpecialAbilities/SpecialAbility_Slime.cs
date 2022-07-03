using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SummonersShine.DataStructures;
using SummonersShine.Effects;
using SummonersShine.ProjectileBuffs;
using SummonersShine.Projectiles;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace SummonersShine.SpecialAbilities
{
    public static partial class SpecialAbility
    {
        const float springSpeed = 10f;
        const float springGrav = SpringSlime.springGrav;
        const float minTime = 30f;
        const float maxTime = 40f;
        /*public static Vector2 FindAvailableSpot() { 
        
        }*/
        public static void SlimeSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity _target, int specialType, bool fromServer)
        {
            if (!fromServer)
            {
                Projectile newProj = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), projectile.position, Vector2.Zero, ProjectileModIDs.SpringSlime, projectile.damage, projectile.knockBack, projectile.owner);
                newProj.timeLeft = (int)projFuncs.GetMinionPower(projectile, 0) * 60;
                newProj.Center = projectile.Center;
                Vector2 velocity = _target.position - projectile.Center;
                float time = Math.Clamp(velocity.Length() / springSpeed, minTime, maxTime);
                velocity /= time;
                newProj.velocity = velocity;
                if(time > 0)
                    newProj.velocity.Y -= springGrav * time * 0.5f;
                newProj.netUpdate = true;
            }
            projData.energy = 0;
        }
    }
}
