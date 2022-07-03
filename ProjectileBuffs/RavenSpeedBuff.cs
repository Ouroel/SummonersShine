using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SummonersShine.ProjectileBuffs
{
    public abstract partial class ProjectileBuff
    {
        class RavenSpeedBuff : ProjectileBuff
        {
            public override ProjectileBuffIDs ID => ProjectileBuffIDs.RavenSpeedBuff;
            public override bool Update(Projectile projectile, ReworkMinion_Projectile projFuncs)
            {
                return true;
            }

            public override float GetAttackSpeedBuff(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
            {
                /*NPC targetNPC = projData.moveTarget as NPC;
                 SummonersShineRavenConfig config = (SummonersShineRavenConfig)SummonersShine.modInstance.GetConfig("SummonersShineRavenConfig");
                 float speed;
                 if (targetNPC != null)
                     speed = config.ravenBaseSpeed + targetNPC.velocity.Length() * config.ravenSpeedPerEnemyVel;
                 else
                     speed = 1;
                 if (speed > config.ravenSpeedCap)
                     speed = config.ravenSpeedCap;
                 return 1 / speed;*/
                return 1;
            }
        }
    }
}
