using Microsoft.Xna.Framework;
using SummonersShine.BakedConfigs;
using SummonersShine.SpecialData;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.ProjectileBuffs
{
    public enum ProjectileBuffIDs {
        None,
        ModdedBuff,
        FlinxyFuryBuff,
        ReactionEnrageBuff,
        InstantAttackBuff,
        RavenSpeedBuff,
        MartianSaucerBuff,
    }
    public abstract partial class ProjectileBuff
    {
        public virtual ProjectileBuffIDs ID => ProjectileBuffIDs.None;
        public Projectile sourceProj;
        public int sourceProjIdentity; //net pointer to minion power

        public int moddedBuffID = 0;
        public Mod modID = null;
        public virtual void PreDraw(Projectile projectile, ReworkMinion_Projectile projFuncs, Color color) { }
        public virtual void PostDraw(Projectile projectile, ReworkMinion_Projectile projFuncs, Color color) { }
        public virtual float GetAttackSpeedBuff(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData) { return 1; }

        //public virtual void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) { }

        public virtual void LoadNetData(BinaryReader reader) { }

        public virtual void SaveNetData(BinaryWriter writer) { }

        //return true to not kill
        public abstract bool Update(Projectile projectile, ReworkMinion_Projectile projFuncs);

        public static ProjectileBuff NewBuff(ProjectileBuffIDs id, Projectile sourceProj, Mod modID, int moddedBuffID)
        {
            ProjectileBuff rv;
            switch (id)
            {
                case ProjectileBuffIDs.ModdedBuff:
                    rv = new ModdedBuff();
                    rv.modID = modID;
                    rv.moddedBuffID = moddedBuffID;
                    break;
                case ProjectileBuffIDs.FlinxyFuryBuff:
                    rv = new FlinxyFuryBuff();
                    break;
                case ProjectileBuffIDs.ReactionEnrageBuff:
                    rv = new ReactionEnrageBuff();
                    break;
                case ProjectileBuffIDs.InstantAttackBuff:
                    rv = new InstantAttackBuff();
                    break;
                case ProjectileBuffIDs.RavenSpeedBuff:
                    rv = new RavenSpeedBuff();
                    break;
                case ProjectileBuffIDs.MartianSaucerBuff:
                    rv = new MartianSaucerBuff();
                    break;
                default:
                    throw (new ArgumentException("Cannot create buff of ID " + id));

            }
            rv.sourceProj = sourceProj;
            if (sourceProj != null)
                rv.sourceProjIdentity = sourceProj.identity;
            else
                rv.sourceProjIdentity = -1;
            return rv;
        }

        public virtual bool IsEqual(ProjectileBuffIDs ID, Projectile sourceProj, Mod modID, int moddedBuffID)
        {
            return this.ID == ID && sourceProjIdentity == sourceProj.identity;
        }
    }
}
