using Microsoft.Xna.Framework;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.Projectiles
{
    public abstract class CustomSpecialAbilitySourceMinion : CustomMinion
    {
        public override void minionTerminateSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player, ReworkMinion_Player playerFuncs)
        {
            if (!SingleThreadExploitation.TerminatingSpecialAbility)
            {
                SingleThreadExploitation.TerminatingSpecialAbility = true;
                projectile.Kill();
                SingleThreadExploitation.TerminatingSpecialAbility = false;
            }
        }
    }
    public abstract class CustomMinion : ModProjectile
    {
        //how fast the minion can accelerate to their target's velocity
        public virtual float minionTrackingAcceleration => 0;
        //the speed for which the minion's own code must compensate for. Set it to 5f for most minions' tracking to look natural.
        public virtual float minionTrackingImperfection => 5;
        public virtual MinionTracking_State trackingState => MinionTracking_State.normal;
        //normal - for velocity-based projectiles. stepped - for position lerping based projectiles (you must compensate for it yourself in the code). none - for projectiles that look pretty on your head.
        public virtual MinionSpeedModifier minionSpeedModType => MinionSpeedModifier.normal;

        public virtual int ArmorIgnoredPerc => 0;
        public virtual float energy => 0;
        public virtual float maxEnergy => 0;
        public virtual float energyRegenRate => 1f;
        public virtual bool minionNotSentry => true;
        public virtual float minionSlots => 1;
        //if your minion operates with smaller numbers, reduce this number
        public virtual float minionFlickeringThreshold => 0.1f;

        public virtual bool ComparativelySlowPlayer => true;

        public ReworkMinion_Projectile ProjFuncs;
        public MinionProjectileData ProjData;

        public static void Populate()
        {
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.minion = minionNotSentry;
            Projectile.sentry = !minionNotSentry;
            Projectile.minionSlots = minionSlots;
            Projectile.DamageType = DamageClass.Summon;

            ReworkMinion_Projectile projFuncs = Projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            MinionProjectileData projData = projFuncs.GetMinionProjData();

            this.ProjFuncs = projFuncs;
            this.ProjData = projData;

            projFuncs.minionPreAI += minionPreAI;
            projFuncs.minionEndOfAI += minionEndOfAI;
            projFuncs.minionPostAI += minionPostAI;
            projFuncs.minionPostAIPostRelativeVelocity += minionPostAIPostRelativeVelocity;
            projFuncs.minionSummonEffect += minionSummonEffect;
            projFuncs.minionDespawnEffect += minionDespawnEffect;
            projFuncs.minionOnSpecialAbilityUsed = minionOnSpecialAbilityUsed;
            projFuncs.minionTerminateSpecialAbility = minionTerminateSpecialAbility;
            projFuncs.minionCustomAI = minionCustomAI;
            projFuncs.minionCustomPreDraw = minionCustomPreDraw;
            projFuncs.minionCustomPostDraw = minionCustomPostDraw;
            projFuncs.onPostCreation = onPostCreation;
            projFuncs.minionOnMovement = minionOnMovement;
            projFuncs.minionOnCreation = minionOnCreation;

            projFuncs.minionOnHitNPC += minionOnHitNPC;
            projFuncs.minionOnHitNPC_Fake += minionOnHitNPC_Fake;

            projFuncs.minionOnSlopeCollide += minionOnSlopeCollide;

            projFuncs.ComparativelySlowPlayer = ComparativelySlowPlayer;
            projFuncs.ArmorIgnoredPerc = ArmorIgnoredPerc;

            projData.minionTrackingAcceleration = minionTrackingAcceleration;
            projData.minionTrackingImperfection = minionTrackingImperfection;
            projData.trackingState = trackingState;
            projData.minionSpeedModType = minionSpeedModType;
            projData.energy = energy;
            projData.energyRegenRate = energyRegenRate;
            projData.maxEnergy = maxEnergy;

            projData.minionFlickeringThreshold = minionFlickeringThreshold;
        }

        public virtual void minionOnSlopeCollide(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Vector4 newVel) { }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        //called at the start of PreAI after initialization
        public virtual void onPostCreation(Projectile projectile, Player player) { }
        //called after minion speed velocity changes have been accounted for.
        public virtual void minionPreAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData) { }
        //called after AI, assuming the tick is real and not duplicated.
        public virtual void minionEndOfAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData) { }
        //called before minion speed velocity changes are altered.
        public virtual void minionPostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData) { }
        //use this to turn the heads of dragons to the proper direction
        public virtual void minionPostAIPostRelativeVelocity(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData) { }
        //called at the start of PostAI after initialization
        public virtual void minionSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player) { }
        //called upon kill
        public virtual void minionDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player) { }
        public virtual void minionOnMovement(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData) { }
        //called when you right-click with a summon staff of SourceItem ID
        public virtual void minionOnSpecialAbilityUsed(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity target, int specialType, bool fromServer) { }
        //use this to override the minion's normal AI. I don't know why you need this.
        public virtual bool minionCustomAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData) { return true; }
        //this exists to hook into vanilla thingies. I don't know why you need this.
        public virtual bool minionCustomPreDraw(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Color lightColor) { return true; }
        //this exists to hook into vanilla thingies. I don't know why you need this.
        public virtual void minionCustomPostDraw(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Color lightColor) { }

        public virtual void minionOnCreation(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player) { }
        public virtual void minionOnHitNPC(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) { }
        public virtual void minionOnHitNPC_Fake(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) { }

        public virtual void minionTerminateSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player, ReworkMinion_Player playerFuncs)
        {
            projData.castingSpecialAbilityTime = -1;
        }
    }
}
