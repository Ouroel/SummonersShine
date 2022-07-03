using Microsoft.Xna.Framework;
using SummonersShine.ModSupport;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace SummonersShine.SpecialAbilities.DefaultSpecialAbility
{
    class SA_Multishot : DefaultSpecialAbility
    {
        bool creatingProjectile = false;
        public override string ToString()
        {
            return "Multishot";
        }

        public override void GetRandomMinionPower(minionPower[] minionPowers, Random rand)
        {
            GetMinionPower(minionPowers, rand.Next(4, 7) * 5, 0);
        }

        public override void GetMinionPower(minionPower[] minionPowers, float val1, float val2) {
            minionPowers[0] = minionPower.NewMP(val1, mpScalingType.add);
        }

        public override bool ValidForItem(Item item, Projectile projectile)
        {
            return IsModdedSafe(item, projectile) && IsNotWhip(projectile) && IsRanged(projectile);
        }
        public override void OnProjectileCreated(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            projFuncs.minionOnShootProjectile += MinionOnShootProjectile;
        }

        public override void UnhookMinionPower(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            projFuncs.minionOnShootProjectile -= MinionOnShootProjectile;
        }
        public void MinionOnShootProjectile(Projectile projectile, Projectile source, ReworkMinion_Projectile sourceFuncs, MinionProjectileData sourceData)
        {
            if (creatingProjectile || source.owner != Main.myPlayer)
                return;
            sourceData.specialCastPosition.Y += (float)PseudoRandom.GetPseudoRandomRNG((int)sourceFuncs.GetMinionPower(projectile, 0));
            if (Main.rand.NextFloat() > sourceData.specialCastPosition.Y)
                return;
            sourceData.specialCastPosition.Y = 0;
            creatingProjectile = true;
            NPC originalFollowTarget = sourceData.moveTarget as NPC;
            Vector2 center;
            if (originalFollowTarget == null)
                center = projectile.Center;
            else
                center = originalFollowTarget.Center;

            List<NPC> targets = ModUtils.FindValidNPCsOrderedByDist(source, center);
            targets.Remove(originalFollowTarget);
            int count = 0;

            targets.ForEach(x =>
            {
                count++;
                if (count > 2)
                    return;
                Vector2 newVel = x.Center - projectile.Center;
                Vector2 newPos = projectile.Center;
                float velLength = projectile.velocity.Length();
                newVel *= velLength / newVel.Length();
                float ai0 = projectile.ai[0];
                float ai1 = projectile.ai[1];
                Multishot_GetProjectileAI01Modifiers(projectile.type, x, ref ai0, ref ai1);
                ModSupportDefaultSpecialAbility.HandleMultishotPreGenerate(projectile, source, x, ref newPos, ref newVel, ref ai0, ref ai1);
                Projectile newProj = Projectile.NewProjectileDirect(source.GetSource_FromThis(), newPos, newVel, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, ai0, ai1);
                ModSupportDefaultSpecialAbility.HandleMultishotPostGenerate(newProj, source, x);

                //fix for homing projectiles
                if (source.type == ProjectileID.StardustCellMinion)
                    newProj.ai[1] = x.whoAmI;


                if (newProj.GetGlobalProjectile<ReworkMinion_Projectile>().ShouldUpdatePosition(projectile)) {
                    sourceData.moveTarget = x;
                    newProj.velocity = ReworkMinion_Projectile.GetTotalProjectileVelocity(newProj, source, x);
                }
            });

            sourceData.moveTarget = originalFollowTarget;
            creatingProjectile = false;
        }

        void Multishot_GetProjectileAI01Modifiers(int type, NPC target, ref float ai0, ref float ai1)
        {
            switch (type)
            {
                case ProjectileID.StardustCellMinionShot:
                    ai1 = target.whoAmI;
                    break;
            }
        }
    }
}
