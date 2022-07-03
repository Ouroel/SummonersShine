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
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace SummonersShine.SpecialAbilities
{
    public static partial class SpecialAbility
    {
        const int DeadlySphere_Immunity = 10;
        const int DeadlySphere_Immunity_DataSize = DeadlySphere_Immunity + 1;
        static Rectangle DeadlySphereRect = new (-24, -24, 24 * 2, 24 * 2);
        const int DeadlySphere_CloseRangeSpray = 16 * 16;

        const int DeadlySphereTimer = 60 * 10;
        public static void DeadlySphereSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity _target, int specialType, bool fromServer)
        {
            projData.energy = 0;
            projData.energyRegenRate = 0;
            SetDeadlySphereTimer(projData, DeadlySphereTimer);
        }
        public static void DeadlySpherePostCreation(Projectile projectile, Player player)
        {
            new ProjectileOnHit(projectile, DeadlySphereRect, DeadlySphereOnProjectileCollide);
        }

        public static void DeadlySphereOnProjectileCollide(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Projectile collidingProj)
        {
            if (BakedConfig.CustomSpecialPowersEnabled(ItemID.DeadlySphereStaff) && Main.myPlayer == proj.owner)
            {
                if (GetDeadlySphereImmunity(projData) == 0)
                {
                    DeadlySphereSpray(proj, projFuncs, projData, null, 100, 50);
                    SetDeadlySphereImmunity(projData, DeadlySphere_Immunity);
                }
            }
        }
        public static void DeadlySphereOnHitNPC(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (BakedConfig.CustomSpecialPowersEnabled(ItemID.DeadlySphereStaff) && Main.myPlayer == projectile.owner)
            {
                DeadlySphereSpray(projectile, projFuncs, projData, target);
            }
        }

        static void DeadlySphereSpray(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity target, int chanceMain = 50, int chanceOff = 50)
        {
            NPC targeted = null;
            int maxHP = -1;
            int projDamage = (int)(proj.damage * projFuncs.GetMinionPower(proj, 0) * 0.01f);
            ModUtils.FindValidNPCsAndDoSomething(proj, i =>
            {
                if (targeted == null || targeted == target || (target != i && i.lifeMax > maxHP)) {
                    targeted = i;
                    maxHP = i.lifeMax;
                }
                return false;
            }, 1400, true, Main.player[proj.owner].Center);

            bool shot = false;
            if (targeted != null)
            {
                if (Main.rand.Next(1, 101) <= chanceMain)
                {
                    Projectile.NewProjectile(proj.GetSource_FromThis(), proj.Center, proj.velocity, ProjectileModIDs.DeadlyInstrument, projDamage, proj.knockBack, proj.owner, ModUtils.ConvertToGlobalEntityID(targeted));
                    shot = true;
                }
            }

            if (GetDeadlySphereTimer(projData) > 0) {
                int numTargets = 0;
                ModUtils.FindValidNPCsAndDoSomething(proj, i =>
                {
                    if (i != targeted && i != target)
                    {
                        if (numTargets < 5 && Main.rand.Next(1, 101) <= chanceOff)
                        {
                            Projectile.NewProjectile(proj.GetSource_FromThis(), proj.Center, proj.velocity, ProjectileModIDs.DeadlyInstrument, projDamage, proj.knockBack, proj.owner, ModUtils.ConvertToGlobalEntityID(i));
                            numTargets++;
                            shot = true;
                        }
                    }
                    return false;
                }, DeadlySphere_CloseRangeSpray);
            }

            if (shot)
                SoundEngine.PlaySound(SoundID.Item72, proj.Center);
        }

        static void SetDeadlySphereImmunity(MinionProjectileData projData, int number)
        {
            int deadlySphereImmunity = (projData.castingSpecialAbilityTime + 1) % (DeadlySphere_Immunity_DataSize);
            projData.castingSpecialAbilityTime += number - deadlySphereImmunity;
        }

        static int GetDeadlySphereImmunity(MinionProjectileData projData)
        {
            return (projData.castingSpecialAbilityTime + 1) % DeadlySphere_Immunity_DataSize;
        }

        public static void CountDownDeadlySphereTimer(MinionProjectileData projData)
        {
            int specialTime = (projData.castingSpecialAbilityTime + 1);

            //invul timer
            int deadlySphereImmunity = specialTime % DeadlySphere_Immunity_DataSize;
            if (deadlySphereImmunity > 0)
                projData.castingSpecialAbilityTime -= 1;
            //special timer
            int timer = (specialTime - deadlySphereImmunity) / DeadlySphere_Immunity_DataSize;
            if (timer > 0)
                projData.castingSpecialAbilityTime -= DeadlySphere_Immunity_DataSize;
            else
                projData.energyRegenRate = 1;
        }

        static int GetDeadlySphereTimer(MinionProjectileData projData)
        {
            int specialTime = (projData.castingSpecialAbilityTime + 1);
            return (specialTime - specialTime % DeadlySphere_Immunity_DataSize) / DeadlySphere_Immunity_DataSize;
        }

        static void SetDeadlySphereTimer(MinionProjectileData projData, int number)
        {
            int deadlySphereImmunity = (projData.castingSpecialAbilityTime + 1) % DeadlySphere_Immunity_DataSize;
            projData.castingSpecialAbilityTime = number * DeadlySphere_Immunity_DataSize + deadlySphereImmunity - 1;
        }
    }
}
