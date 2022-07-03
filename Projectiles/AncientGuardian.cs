using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SummonersShine.MinionAI;
using SummonersShine.Sounds;
using SummonersShine.SpecialAbilities;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.Projectiles
{
    public class AncientGuardian : CustomSpecialAbilitySourceMinion
    {
        public override float minionFlickeringThreshold => 0;
        enum AncientGuardianFrames {
            Squish,
            Normal,
            Jump1,
            Jump2,
            Run1,
            Run2,
            Run3,
            Run4,
            Run5,
            Run6,
        }

        enum AncientGuardianActionTypes {
            Idle,
            Jump,
            Hop,
            Charge,
            Stunned,
            Leap,
            ChargePillar,
        }
        const int drawOffsetX = -24;
        const int drawOffsetY = -32;
        public override void SetStaticDefaults()
        {
            ProjectileModIDs.AncientGuardian = Projectile.type;
            Main.projFrames[Projectile.type] = 10;
        }
        public override void minionTerminateSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player, ReworkMinion_Player playerFuncs)
        {
            Deactivate();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(ProjData.moveTarget.ConvertToGlobalEntityID());
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            int moveTarget = reader.Read7BitEncodedInt();
            ProjData.moveTarget = ModUtils.GetNPCProjOrPlayer_ID(moveTarget);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = 2;
            Projectile.width = 16;
            Projectile.height = 32;
            DrawOffsetX = drawOffsetX;
            DrawOriginOffsetY = drawOffsetY;
            Projectile.minionSlots = 0;
            Projectile.manualDirectionChange = true;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.X -= 8;
            hitbox.Width += 30;
            if (Projectile.spriteDirection == -1)
                hitbox.X -= 14;
            hitbox.Y -= 24;
            hitbox.Height += 24;
        }
        public override void minionOnCreation(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            ReworkMinion_Player ownerData = Main.player[Projectile.owner].GetModPlayer<ReworkMinion_Player>();
            HoundiusShootiusStatCollection collection = ownerData.GetSpecialData<HoundiusShootiusStatCollection>();
            collection.ForceInsertMegaMinion(Projectile);
        }

        public void Deactivate()
        {
            if (ProjFuncs.killedTicks == 0)
            {
                Projectile.timeLeft = Projectile.GetFadeTime(ProjData);
                if (ProjData.alphaOverride == 0)
                    ProjData.alphaOverride = 1;
                ProjFuncs.killedTicks = 1;
            }
        }
        public bool Reactivate()
        {
            if (ProjFuncs.killedTicks > 0)
            {
                ProjFuncs.killedTicks = 0;
                return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float ratio = (255 - ProjData.alphaOverride) / 255f;
            lightColor *= ratio;
            lightColor.A = 255;
            return true;
        }
        public override void Kill(int timeLeft)
        {
            ReworkMinion_Player ownerData = Main.player[Projectile.owner].GetModPlayer<ReworkMinion_Player>();
            HoundiusShootiusStatCollection collection = ownerData.GetSpecialData<HoundiusShootiusStatCollection>();
            MinionSummonEffect();
            collection.KillMegaMinion();

            ModSounds.PlayAGDeath(Projectile.Center);
        }
        public override bool? CanCutTiles()
        {
            return false;
        }

        void tileCollideHandler(Vector2 oldVelocity, Vector2 newVelocity) {
            float totalMag = newVelocity.Length() * oldVelocity.Length();
            if (totalMag != 0)
            {
                float dot = Vector2.Dot(newVelocity, oldVelocity);
                if (dot / totalMag < 0.8f)
                    if (actionType == AncientGuardianActionTypes.Charge || actionType == AncientGuardianActionTypes.ChargePillar)
                        actionDuration = -1;
            }
            else if (actionType == AncientGuardianActionTypes.Charge || actionType == AncientGuardianActionTypes.ChargePillar)
                actionDuration = -1;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            tileCollideHandler(oldVelocity, Projectile.velocity);
            return false;
        }
        public override void minionOnSlopeCollide(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Vector4 newVel)
        {
            tileCollideHandler(Projectile.velocity, new Vector2(newVel.Z, newVel.W));
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target == ProjData.moveTarget)
                if (actionType == AncientGuardianActionTypes.Charge && actionDuration == run_chargeUpTime)
                    actionDuration = run_chargeUpTime + 1;
        }

        AncientGuardianActionTypes actionType
        {
            get
            {
                return (AncientGuardianActionTypes)Projectile.ai[0];
            }
            set
            {
                Projectile.ai[0] = (float)value;
            }
        }
        int actionDuration {
            get
            {
                return (int)Projectile.ai[1];
            }
            set
            {
                Projectile.ai[1] = value;
            }
        }
        public override void minionPostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            DefaultMinionAI.HandleFadeInOut(Projectile, projFuncs, projData, false, false);
            if (projData.alphaOverride == 255)
                Projectile.Kill();

            if (projFuncs.killedTicks > 0)
                AI();
        }
        public override bool? CanDamage()
        {
            if (ProjFuncs.killedTicks > 0)
                return false;
            return null;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active)
                return;
            Projectile.timeLeft = 2;
            bool changeAction = false;

            //reset all stats
            ProjData.trackingState = MinionTracking_State.noTracking;
            if (!Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                Projectile.tileCollide = true;
            ProjData.minionTrackingAcceleration = 0;
            ProjData.minionTrackingImperfection = 0;
            Projectile.extraUpdates = 0;

            Vector2 restPos = player.Center - Vector2.UnitX * player.direction * 48;


            int xPos = (int)(restPos.X / 16);
            int yPos = (int)(restPos.Y / 16) - 1;
            List<Tuple<int, int>> def = new();
            Collision.TupleHitLine(xPos, yPos, xPos, yPos + 20, 0, 0, def, out Tuple<int, int> bottomResult);
            if (bottomResult.Item2 != yPos + 20)
            {
                restPos.Y = bottomResult.Item2 * 16 - 16;
            }
            float projDistFromPlayer = Projectile.Center.DistanceSQ(restPos);

            if (projDistFromPlayer > 4000 * 4000)
            {
                Projectile.Center = restPos;
                projDistFromPlayer = 0;
                changeAction = true;
            }
            else
            {
                switch (actionType)
                {
                    case AncientGuardianActionTypes.Stunned:
                        AnimateStunned(actionDuration, 6);
                        if (actionDuration > 60)
                            changeAction = true;
                        actionDuration++;
                        break;
                    case AncientGuardianActionTypes.Idle:
                        changeAction = true;
                        AnimateIdle(Projectile.frameCounter);
                        Projectile.velocity.X = 0;
                        Projectile.velocity.Y += 0.5f;
                        break;
                    case AncientGuardianActionTypes.Jump:
                        ProjData.minionTrackingAcceleration = 10f;
                        ProjData.minionTrackingImperfection = 4f;
                        Vector2 target = player.Center - Vector2.UnitX * Projectile.direction * 48;
                        changeAction = Leap(target);
                        break;
                    case AncientGuardianActionTypes.Hop:
                        target = restPos;
                        changeAction = Hop(target, player.GetRealPlayerVelocity());
                        break;
                    case AncientGuardianActionTypes.ChargePillar:
                        if (actionDuration == -1)
                            changeAction = true;
                        else
                            changeAction = ChargePillar();
                        break;
                    case AncientGuardianActionTypes.Charge:
                        if (actionDuration == -1)
                            changeAction = true;
                        else
                            changeAction = Charge();
                        break;
                    case AncientGuardianActionTypes.Leap:
                        NPC AttackTarget = ProjData.moveTarget as NPC;
                        if (AttackTarget != null)
                        {
                            Vector2 center = AttackTarget.Bottom + new Vector2(0, -16);
                            changeAction = Leap(center + ModUtils.GetTileCollideModifier(center, AttackTarget.GetRealNPCVelocity() * 60, 24), true, false);
                        }
                        else
                            changeAction = Leap(Projectile.position + Projectile.velocity, true, true);
                        break;
                }
            }

            if (Projectile.velocity.X > 0.1f) Projectile.spriteDirection = 1;
            else if (Projectile.velocity.X < -0.1f) Projectile.spriteDirection = -1;
            Projectile.frameCounter++;

            bool outOfBouds = Projectile.NextVelOutOfBounds();

            if (outOfBouds || actionType != AncientGuardianActionTypes.Jump && projDistFromPlayer > 1400 * 1400)
            {
                ReworkMinion_Projectile.SetMoveTarget_FromID(Projectile, -1);
                actionType = AncientGuardianActionTypes.Jump;
                actionDuration = 0;
                SpawnShadowParticle(true, Projectile.velocity, 1);
                SpawnShadowParticle(false, Projectile.velocity, 1);
                changeAction = false;
                if (outOfBouds)
                    Projectile.velocity = Vector2.Zero;
            }

            if (changeAction)
            {
                Projectile.netUpdate = true;
                int attackTarget = -1;
                Projectile.Minion_FindTargetInRange(1400, ref attackTarget, false);
                float magnitude = 1;
                bool pillarFound;
                FindPillarPos(out pillarFound);
                if ((actionType == AncientGuardianActionTypes.Charge || actionType == AncientGuardianActionTypes.ChargePillar) && actionDuration == -2)
                {
                    ReworkMinion_Projectile.SetMoveTarget_FromID(Projectile, -1);
                    actionType = AncientGuardianActionTypes.Stunned;
                    Projectile.friendly = false;
                }
                else if (pillarFound && actionType != AncientGuardianActionTypes.Stunned && actionType != AncientGuardianActionTypes.ChargePillar)
                {
                    ReworkMinion_Projectile.SetMoveTarget_FromID(Projectile, -1);
                    actionType = AncientGuardianActionTypes.ChargePillar;
                    Projectile.friendly = true;
                }
                else if (attackTarget != -1 && Projectile.tileCollide)
                {
                    if ((actionType != AncientGuardianActionTypes.Leap && Main.rand.Next(0, 3) == 0) ||
                        !Projectile.CanHitWithOwnBody(ProjData.moveTarget as NPC) ||
                        actionType == AncientGuardianActionTypes.Stunned ||
                        ((actionType == AncientGuardianActionTypes.Charge || actionType == AncientGuardianActionTypes.ChargePillar) && actionDuration == -1))
                    {
                        actionType = AncientGuardianActionTypes.Leap;
                        Projectile.friendly = true;
                    }
                    else
                    {
                        actionType = AncientGuardianActionTypes.Charge;
                        Projectile.frameCounter = 0;
                        Projectile.friendly = true;
                    }
                }
                else
                {
                    if (projDistFromPlayer > 16 * 16 * 30 * 30)
                    {
                        actionType = AncientGuardianActionTypes.Jump;
                        ReworkMinion_Projectile.SetMoveTarget_FromID(Projectile, -1);
                        Projectile.friendly = true;
                    }
                    else if (projDistFromPlayer > 16 * 16)
                    {
                        actionType = AncientGuardianActionTypes.Hop;
                        ReworkMinion_Projectile.SetMoveTarget_FromID(Projectile, -1);
                        ModSounds.PlayAGStep(Projectile.Center, 0.2f);
                        Projectile.friendly = false;
                    }
                    else
                    {
                        if (Projectile.tileCollide)
                            actionType = AncientGuardianActionTypes.Idle;
                        else
                            actionType = AncientGuardianActionTypes.Hop;
                        Projectile.friendly = false;
                        ReworkMinion_Projectile.SetMoveTarget_FromID(Projectile, -1);
                    }
                }
                if (actionType != AncientGuardianActionTypes.Idle)
                {
                    SpawnShadowParticle(true, Projectile.velocity, magnitude);
                    SpawnShadowParticle(false, Projectile.velocity, magnitude);
                }
                actionDuration = 0;
            }
        }
        const int run_chargeUpTime = 8 * 6;
        const int runThrough = 30;
        const int run_end = run_chargeUpTime + runThrough;
        const float runSpeed = 8;

        bool Charge()
        {
            NPC AttackTarget = ProjData.moveTarget as NPC;
            bool targetNull = AttackTarget == null;
            Vector2 target = Vector2.Zero;
            if(!targetNull)
                target = AttackTarget.Center;
            return Run(target, targetNull);
        }

        bool ChargePillar()
        {
            bool foundPillar;
            Vector2 target = FindPillarPos(out foundPillar);
            return Run(target, !foundPillar);
        }
        bool Run(Vector2 target, bool targetNull)
        {
            bool stop = true;
            if (actionDuration < run_chargeUpTime)
            {
                AnimateTaunt(actionDuration, 4);
                if (actionDuration + 1 == run_chargeUpTime)
                {
                    Projectile.frameCounter = 0;
                    if (targetNull)
                        return true;
                }
                if (!targetNull)
                    Projectile.spriteDirection = Math.Sign(target.X - Projectile.Center.X);
            }
            else stop = false;

            if (actionDuration > run_end)
                return true;

            if (stop)
            {
                Projectile.velocity = Vector2.Zero;
                actionDuration++;
            }
            else
            {
                if (PillarCollide())
                {
                    actionDuration = -2;
                    Projectile.velocity = Vector2.Zero;
                    ModSounds.PlayAGHitPillar(Projectile.Center);
                    return true;
                }

                Projectile.extraUpdates = 1;
                AnimateRun(Projectile.frameCounter);
                if (!targetNull && actionDuration == run_chargeUpTime)
                {
                    ProjData.trackingState = MinionTracking_State.normal;
                    ProjData.minionTrackingAcceleration = 0.1f;
                    ProjData.minionTrackingImperfection = 4f;
                    Vector2 diff = target - Projectile.Center;
                    if (diff == Vector2.Zero)
                        diff = Vector2.UnitX;
                    float len = diff.Length();
                    diff *= runSpeed / len;
                    Projectile.velocity = diff;
                    if (len < runSpeed)
                        actionDuration++;
                }
                else
                {
                    if (Projectile.velocity == Vector2.Zero)
                        return true;
                    else
                        actionDuration++;
                }
            }

            return false;
        }
        bool Hop(Vector2 target, Vector2 targetVel)
        {

            actionDuration++;
            const int leap_chargeuptime = 12;
            const int leap_leaptime = 30;
            const int leap_halfleaptime = leap_leaptime / 2;
            const float leap_yvelchangepertick = 0.5f;

            if (targetVel != Vector2.Zero) {
                float targetMag = targetVel.Length();
                if (targetMag <= 4)
                {
                    targetVel = Vector2.Zero;
                }
                else
                {
                    targetVel *= (targetMag - 4) / targetMag;
                }
            }


            if (actionDuration < leap_chargeuptime)
                AnimateHop(actionDuration);

            if (actionDuration == 1)
            {
                Projectile.velocity = Vector2.Zero;

                Vector2 dist = target - Projectile.Center;
                float magnitude = dist.Length();
                float speed = 4;
                if (magnitude > 0)
                {
                    float remainder = leap_leaptime - actionDuration + 1;
                    if (remainder > 0)
                    {
                        speed = (magnitude - 1) / remainder;
                        if (speed > 4)
                            speed = 4;
                    }
                }
                if (magnitude > 4)
                {
                    dist *= speed / magnitude;
                }
                dist += new Vector2(0, leap_yvelchangepertick * -leap_halfleaptime) + targetVel;
                Projectile.velocity = dist;
            }
            else
                Projectile.velocity.Y += leap_yvelchangepertick;
            if (actionDuration > leap_leaptime)
                return true;

            return false;
        }
        bool Leap(Vector2 target, bool extrapolateTargetPosInstead = false, bool targetNull = false) {
            actionDuration++;
            const int leap_chargeuptime = 24;
            const int leap_leaptime = 60;
            const int leap_halfleaptime = leap_leaptime / 2;
            const int leap_endleaptime = leap_chargeuptime + leap_leaptime;
            const int leap_recovertime = 6;
            const int leap_returntime = leap_recovertime + leap_endleaptime;
            const float leap_yvelchangepertick = 0.5f;
            const float leap_yvelchangepertick_offset = leap_yvelchangepertick * leap_halfleaptime * leap_halfleaptime / 2;

            if (actionDuration < leap_chargeuptime)
            {
                Projectile.velocity = Vector2.Zero;
                if (actionDuration == leap_chargeuptime - 1)
                {
                    SpawnShadowParticle(true, Projectile.velocity, 2);
                    SpawnShadowParticle(false, Projectile.velocity, 2);
                    ModSounds.PlayAGStep(Projectile.Center, 0.5f);
                }
                AnimateLeap(actionDuration);
                Projectile.spriteDirection = Math.Sign(target.X - Projectile.Center.X);
            }
            else if (actionDuration > leap_endleaptime)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.frame = 0;
            }
            else
            {

                Projectile.tileCollide = false;

                if (extrapolateTargetPosInstead)
                {

                    if (actionDuration == leap_chargeuptime)
                    {
                        if (targetNull)
                            return true;
                        Projectile.velocity = Vector2.Zero;
                        Vector2 dist = target - Projectile.Center;
                        Projectile.velocity = dist / leap_leaptime + new Vector2(0, leap_yvelchangepertick * -leap_halfleaptime);
                    }
                    else
                    {
                        Projectile.velocity += new Vector2(0, leap_yvelchangepertick);
                    }
                }
                else
                {
                    Projectile.velocity = Vector2.Zero;
                    ProjData.trackingState = MinionTracking_State.normal;
                    bounceOrLeap(target, leap_chargeuptime, leap_halfleaptime, leap_leaptime, leap_yvelchangepertick, leap_yvelchangepertick_offset);
                }
            }
            if (actionDuration > leap_returntime)
            {
                LeapSlam();
                SpawnShadowParticle(true, Projectile.velocity, 2);
                SpawnShadowParticle(false, Projectile.velocity, 2);
                ModSounds.PlayAGStep(Projectile.Center, 0.7f);
                return true;
            }
            return false;
        }

        void bounceOrLeap(Vector2 target, int chargeuptime, int halfLeapTime, int leapTime, float yVelChangePerTick, float yVelChangePerTickOffset)
        {
            float jumpTime = actionDuration - chargeuptime;
            float jumpProgress = jumpTime - halfLeapTime;

            float totalYCovered = yVelChangePerTick * jumpProgress * jumpProgress / 2 - yVelChangePerTickOffset;


            Vector2 dist = target - (Projectile.Center - new Vector2(0, totalYCovered));

            float totalLeap = leapTime - jumpTime + 1;
            if (totalLeap <= 0)
                totalLeap = 1;

            Projectile.velocity = 1 / totalLeap * dist + new Vector2(0, yVelChangePerTick * jumpProgress);

        }
        void AnimateHop(int frameCounter)
        {
            int frame = frameCounter % 12;
            frame /= 4;
            switch (frame)
            {
                case 0:
                    Projectile.frame = 1;
                    return;
                case 1:
                    Projectile.frame = 2;
                    return;
                case 2:
                    Projectile.frame = 3;
                    return;
            }
        }
        void AnimateLeap(int frameCounter) {
            int frame = frameCounter % 24;
            frame /= 6;
            switch (frame)
            {
                case 0:
                    Projectile.frame = 0;
                    return;
                case 1:
                    Projectile.frame = 1;
                    return;
                case 2:
                    Projectile.frame = 2;
                    return;
                case 3:
                    Projectile.frame = 3;
                    return;
            }
        }
        void AnimateIdle(int time) {
            int frame = time % 12;
            frame /= 6;
            switch (frame)
            {
                case 0:
                    Projectile.frame = 1;
                    return;
                case 1:
                    Projectile.frame = 2;
                    return;
            }
        }

        void AnimateStunned(int time, int interval = 6)
        {
            int frame = time % (interval * 10);
            bool wholeNumber = frame % interval == 0;
            frame /= interval;

            switch (frame)
            {
                case 0:
                    Projectile.frame = 0;
                    if (wholeNumber)
                    {
                        ModSounds.PlayAGRecover(Projectile.Center);
                        SpawnShadowParticle(true, Projectile.velocity, 1);
                        SpawnShadowParticle(false, Projectile.velocity, 1);
                    }
                    return;
                case 1:
                    Projectile.frame = 0;
                    return;
                case 2:
                    Projectile.frame = 1;
                    return;
                case 3:
                    if (wholeNumber)
                    {
                        ModSounds.PlayAGRecover(Projectile.Center);
                        SpawnShadowParticle(true, Projectile.velocity, 1);
                        SpawnShadowParticle(false, Projectile.velocity, 1);
                    }
                    Projectile.frame = 0;
                    return;
                case 4:
                    Projectile.frame = 1;
                    return;
                case 5:
                    Projectile.frame = 1;
                    return;
                case 6:
                    Projectile.frame = 2;
                    return;
                case 7:
                    if (wholeNumber)
                    {
                        ModSounds.PlayAGRecover(Projectile.Center);
                        SpawnShadowParticle(true, Projectile.velocity, 1);
                        SpawnShadowParticle(false, Projectile.velocity, 1);
                    }
                    Projectile.frame = 0;
                    return;
                case 8:
                    Projectile.frame = 1;
                    return;
                case 9:
                    Projectile.frame = 2;
                    return;
            }
        }
        void AnimateTaunt(int time, int interval = 6) {

            int frame = time % (interval * 6);
            if (frame % 6 == 0)
            {
                SpawnShadowParticle(true);
                SpawnShadowParticle(false);
            }
            if (frame == 0)
                ModSounds.PlayAGTaunt(Projectile.Center);
            frame /= interval;
            switch (frame)
            {
                case 0:
                    Projectile.frame = 0;
                    return;
                case 1:
                    Projectile.frame = 1;
                    return;
                case 2:
                    Projectile.frame = 2;
                    return;
                case 3:
                    Projectile.frame = 3;
                    return;
                case 4:
                    Projectile.frame = 2;
                    return;
                case 5:
                    Projectile.frame = 1;
                    return;
            }
        }

        void AnimateRun(int duration)
        {

            int frame = duration % 24;
            if (frame == 0 || frame == 12)
                ModSounds.PlayAGStep(Projectile.Center);
            frame /= 4;
            Projectile.frame = (int)AncientGuardianFrames.Run1 + frame;
            SpawnShadowTrail(true);
            SpawnShadowTrail(false);
        }

        public override void minionSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            MinionSummonEffect();
        }
        void MinionSummonEffect()
        {
            for (int x = 0; x < 80; x++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), 0, 0, DustID.Smoke, 0, 0, 50, Color.DarkSlateGray, 1);
            }
        }
        void SpawnShadowTrail(bool RightFoot)
        {
            Vector2 footPos = GetFootPos(RightFoot);

            for (int x = 0; x < 2; x++)
            {
                Dust dust = Dust.NewDustDirect(footPos + new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-1, 1)), 0, 0, DustID.Smoke, 0, 0, 50, Color.DarkSlateGray, 1);
                dust.velocity = -Projectile.velocity / 2 + Main.rand.NextVector2Circular(1, 1);
            }
        }

        void LeapSlam()
        {
            if (ProjFuncs.killedTicks == 0 && Main.myPlayer == Projectile.owner)
            {
                for (int x = 0; x < 15; x++) {
                    Vector2 shootVel = new Vector2(0, Main.rand.NextFloat(-1, -2)).RotatedBy(Main.rand.NextFloat(-1, 1));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ProjectileModIDs.NightmareFuel, Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                }
            }
        }
        void SpawnShadowParticle(bool RightFoot, Vector2 disp = default(Vector2), float magnitude = 1)
        {
            Vector2 footPos = GetFootPos(RightFoot) + disp;
            for (int x = 0; x < 10 * magnitude * magnitude; x++)
            {
                Dust dust = Dust.NewDustDirect(footPos + new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-1, 1)), 0, 0, DustID.Smoke, 0, 0, 50, Color.DarkSlateGray, 1);
                if (dust.velocity.Y < 0)
                    dust.velocity.Y = -dust.velocity.Y;
                dust.velocity *= magnitude;
            }
        }

        Vector2 GetFootPos(bool RightFoot)
        {
            Vector2 footPos = Projectile.Bottom;
            float offset;
            if (RightFoot)
                offset = 4;
            else
                offset = -16;
            footPos.X += offset * Projectile.spriteDirection;
            footPos.Y -= 4;
            return footPos;
        }

        Vector2 FindPillarPos(out bool foundPillar)
        {
            Player player = Main.player[Projectile.owner];
            MinionEnergyCounter minionCollection = player.GetModPlayer<ReworkMinion_Player>().GetMinionCollection(ItemID.HoundiusShootius);
            bool ret_foundPillar = false;
            Vector2 ret_vec2 = Vector2.Zero;
            float dist = -1;
            minionCollection.minions.ForEach(i =>
            {
                ReworkMinion_Projectile iFuncs = i.GetGlobalProjectile<ReworkMinion_Projectile>();
                MinionProjectileData iData = iFuncs.GetMinionProjData();
                if (iData.castingSpecialAbilityTime != -1 && DefaultMinionAI.HoundiusShootiusGetSpecialTimer(iData) == 0)
                {
                    bool foundTarget = false;
                    ModUtils.FindValidNPCsAndDoSomething(i, (NPC found) => {
                        foundTarget = true;
                        return true;
                    });
                    if (!foundTarget)
                        return;
                    Vector2 pillar_relative = i.Center;
                    float newDist = pillar_relative.DistanceSQ(Projectile.Center);
                    if (dist != -1 && newDist > dist)
                        return;
                    dist = newDist;
                    Entity ent = new DummyEntity(i.Center);
                    ret_foundPillar = Projectile.CanHitWithOwnBody(ent);
                    if (ret_foundPillar)
                        ret_vec2 = pillar_relative;

                }
            });
            foundPillar = ret_foundPillar;
            return ret_vec2;
        }
        bool PillarCollide()
        {
            Player player = Main.player[Projectile.owner];
            const float pillar_height_offset = -80;
            const float pillar_floor_offset = -16;
            const float pillar_halfwidth = 16;
            MinionEnergyCounter minionCollection = player.GetModPlayer<ReworkMinion_Player>().GetMinionCollection(ItemID.HoundiusShootius);
            bool foundPillar = false;
            minionCollection.minions.ForEach(i =>
            {
                if (foundPillar)
                    return;
                ReworkMinion_Projectile iFuncs = i.GetGlobalProjectile<ReworkMinion_Projectile>();
                MinionProjectileData iData = iFuncs.GetMinionProjData();
                if (iData.castingSpecialAbilityTime != -1)
                {
                    Vector2 pillar_relative = i.Bottom;
                    float projX = Projectile.position.X + Projectile.velocity.X;
                    float projY = Projectile.position.Y + Projectile.velocity.Y;
                    if (projY < pillar_relative.Y + pillar_floor_offset &&
                    projY + Projectile.height > pillar_relative.Y + pillar_height_offset &&
                    projX < pillar_relative.X + pillar_halfwidth &&
                    projX + Projectile.width > pillar_relative.X - pillar_halfwidth &&
                    (projX + Projectile.width / 2 - pillar_relative.X) * Projectile.spriteDirection <= 0)
                    {
                        foundPillar = true;
                        i.velocity += Projectile.velocity;
                        PillarSparks();
                        LeapSlam();
                        i.timeLeft = 7200;
                        if (DefaultMinionAI.HoundiusShootiusGetSpecialTimer(iData) == 0)
                            DefaultMinionAI.HoundiusShootiusSetSpecialTimer(iData, 1);
                    }
                }
            });
            return foundPillar;
        }

        void PillarSparks()
        {
            Vector2 hornPosition = Projectile.Center + new Vector2(16, -16) * Projectile.spriteDirection;
            for (int x = 0; x < 10; x++) {
                Dust dust = Dust.NewDustDirect(hornPosition, 0, 0, DustID.Smoke, newColor: Color.SlateGray);
                dust.velocity = new Vector2(Main.rand.NextFloat(3, 5), Main.rand.NextFloat(0.5f, 1.5f)) * -Projectile.spriteDirection;
            }
            for (int x = 0; x < 10; x++) {
                Dust dust = Dust.NewDustDirect(hornPosition, 0, 0, DustID.Pot, newColor: Color.Brown);
                dust.velocity = new Vector2(Main.rand.NextFloat(3, 5), Main.rand.NextFloat(0.5f, 1.5f)) * -Projectile.spriteDirection;
            }
        }
    }
}
