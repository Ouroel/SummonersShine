using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SummonersShine.BakedConfigs;
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
using Terraria.ID;

namespace SummonersShine.SpecialAbilities
{
    public static partial class SpecialAbility
    {
        public static bool BabyBirdAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (BakedConfig.CustomSpecialPowersEnabled(ItemID.BabyBirdStaff))
            {
                Player player = Main.player[projectile.owner];
                if (projData.castingSpecialAbilityTime != -1)
                {
                    bool dead = player.dead || !player.HasBuff(BuffID.BabyBird);
                    if (dead)
                    {
                        player.babyBird = false;
                    }
                    bool babyBird = player.babyBird;
                    if (babyBird)
                    {
                        projectile.timeLeft = 2;
                    }

                    BabyBird_NormalUpdateWithMovement(projectile, BabyBird_FindMinionPos(projectile, projFuncs, player), player.velocity, projData.lastRelativeVelocity);
                    return false;
                }
            }
            return true;
        }

        public static void BabyBirdOnMovement(Player player, Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, ref float gravMod, ref float velMod) {

            if (!player.controlDown)
                gravMod *= 0.5f;
            velMod += projFuncs.GetMinionPower(projectile, 0) * 0.01f;
            player.fallStart = (int)(player.position.Y / 16);
        }

        public static void BabyBirdTerminateSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player, ReworkMinion_Player playerFuncs)
        {
            playerFuncs.RemoveMovementMod(projectile, BabyBirdOnMovement);

            switch (projectile.ai[0])
            {
                case 4:
                case 6:
                    projectile.ai[0]++;
                    projectile.Center = BabyBird_FindMinionPos(projectile, projFuncs, player);
                    break;
            }
            projectile.hide = true;
            projFuncs.drawBehindType = DrawBehindType.normal;
            projectile.ai[0] = 0;
        }
        public static void BabyBirdPostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (BakedConfig.CustomSpecialPowersEnabled(ItemID.BabyBirdStaff))
            {
                Player player = Main.player[projectile.owner];
                ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
                if (projData.castingSpecialAbilityTime != -1)
                {
                    projectile.frame %= (Main.projFrames[projectile.type] - 1);

                    if (projectile.ai[0] == 4 || projectile.ai[0] == 6)
                    {
                        playerFuncs.AddMovementMod(projectile, BabyBirdOnMovement);
                        projData.trackingState = MinionTracking_State.noTracking;

                        projectile.Center = BabyBird_FindMinionPos(projectile, projFuncs, player);
                        projectile.velocity = Vector2.Zero;
                        projectile.spriteDirection = projectile.direction = player.direction;
                        projectile.rotation = 0;

                    }
                    else
                    {
                        projData.trackingState = MinionTracking_State.retreating;
                    }
                }
            }
        }
        public static void BabyBirdSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity _target, int specialType, bool fromServer)
        {
            Player player = Main.player[projectile.owner];
            ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
            projData.energy = 0;
            if (projData.castingSpecialAbilityTime == -1)
            {
                projData.castingSpecialAbilityTime = 0;
                ReworkMinion_Projectile.SetMoveTarget_FromID(projectile, -1);

                MinionEnergyCounter collection = playerFuncs.GetMinionCollection(ItemID.BabyBirdStaff);
                int weight = 0;
                if (projectile.ai[0] < 4)
                {
                    collection.minions.ForEach(i =>
                    {
                        switch (i.ai[0])
                        {
                            case 4:
                            case 5:
                                weight++;
                                break;
                            case 6:
                            case 7:
                                weight--;
                                break;
                        }
                    });
                    if (weight < 0)
                        projectile.ai[0] = 5;
                    else
                        projectile.ai[0] = 7;
                }
                return;
            }
            projData.castingSpecialAbilityTime = -1;
            BabyBirdTerminateSpecialAbility(projectile, projFuncs, projData, player, playerFuncs);
        }
        static void BabyBird_NormalUpdateWithMovement(Projectile babyBird, Vector2 target, Vector2 vel, Vector2 lastRelativeVel) {
            float maxSpeed = 6;

            float dist = (babyBird.position + lastRelativeVel).Distance(target);
            Vector2 dir = babyBird.DirectionTo(target);

            babyBird.tileCollide = false;

            switch (babyBird.ai[0])
            {
                case 5:
                case 7:
                    {
                        if (dist > 40f)
                        {

                            float superMaxSpeed = maxSpeed + dist * 0.006f;
                            dir *= MathHelper.Lerp(1f, 5f, Utils.GetLerpValue(40f, 800f, maxSpeed, true));
                            babyBird.velocity = Vector2.Lerp(babyBird.velocity, dir * superMaxSpeed, 0.025f);
                            if (babyBird.velocity.Length() > superMaxSpeed)
                            {
                                babyBird.velocity *= superMaxSpeed / babyBird.velocity.Length();
                            }
                            babyBird.rotation = babyBird.velocity.X * 0.1f;
                            babyBird.spriteDirection = babyBird.direction = ((babyBird.velocity.X > 0f) ? 1 : -1);
                        }
                        else if (dist > 8f + vel.Length())
                        {
                            babyBird.velocity += dir * 0.05f;
                            if (babyBird.velocity.Length() > maxSpeed)
                            {
                                babyBird.velocity *= maxSpeed / babyBird.velocity.Length();
                            }
                            babyBird.rotation = babyBird.velocity.X * 0.1f;
                            babyBird.spriteDirection = babyBird.direction = ((babyBird.velocity.X > 0f) ? 1 : -1);
                        }
                        else
                        {
                            babyBird.ai[0]--;
                            babyBird.netUpdate = true;
                        }

                        float spaceout = 0.025f;
                        float maxspace = (float)(babyBird.width * 3);
                        for (int i = 0; i < 1000; i++)
                        {
                            if (i != babyBird.whoAmI && Main.projectile[i].active && Main.projectile[i].owner == babyBird.owner && Main.projectile[i].type == babyBird.type && Math.Abs(babyBird.position.X - Main.projectile[i].position.X) + Math.Abs(babyBird.position.Y - Main.projectile[i].position.Y) < maxspace)
                            {
                                if (babyBird.position.X < Main.projectile[i].position.X)
                                {
                                    babyBird.velocity.X = babyBird.velocity.X - spaceout;
                                }
                                else
                                {
                                    babyBird.velocity.X = babyBird.velocity.X + spaceout;
                                }
                                if (babyBird.position.Y < Main.projectile[i].position.Y)
                                {
                                    babyBird.velocity.Y = babyBird.velocity.Y - spaceout;
                                }
                                else
                                {
                                    babyBird.velocity.Y = babyBird.velocity.Y + spaceout;
                                }
                            }
                        }
                    }
                    break;
            }
            babyBird.frameCounter++;
            if (babyBird.frameCounter >= 6)
            {
                babyBird.frameCounter = 0;
                babyBird.frame = (babyBird.frame + 1) % (Main.projFrames[babyBird.type] - 1);
            }
        }

        static Vector2 BabyBird_FindMinionPos(Projectile projectile, ReworkMinion_Projectile projFuncs, Player player)
        {
            int frontPos = 0;
            int backPos = 0;
            MinionEnergyCounter collection = player.GetModPlayer<ReworkMinion_Player>().GetMinionCollection(ItemID.BabyBirdStaff);

            bool climbToTop = projectile.ai[0] == 5 || projectile.ai[0] == 7;
            bool odd = (projectile.ai[0] == 6 || projectile.ai[0] == 7);
            int sideDisp = odd ? 2 : 0;

            collection.minions.ForEach(i =>
            {
                if ((climbToTop || projectile.identity > i.identity) && i.ai[0] == 4 + sideDisp) {
                    if (odd)
                        backPos++;
                    else
                        frontPos++;
                }
            });
            //bool odd = index % 2 == 0;
            if (odd)
            {
                projectile.hide = true;
                projFuncs.drawBehindType = DrawBehindType.normal;
            }
            else
            {
                projectile.hide = false;
                projFuncs.drawBehindType = DrawBehindType.none;
            }
            int stack = odd ? backPos : frontPos;
            Vector2 attachmentPoint;
            if (odd)
                attachmentPoint = player.Top + new Vector2(-8 * player.direction, 20) + new Vector2(-6 * player.direction, -12) * stack;
            else
                attachmentPoint = player.Top + new Vector2(12 * player.direction, 20) + new Vector2(5 * player.direction, -12) * stack;
            return attachmentPoint;
        }
    }
}
