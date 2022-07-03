using Microsoft.Xna.Framework;
using SummonersShine.ModSupport;
using SummonersShine.SpecialData;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.SpecialAbilities.DefaultSpecialAbility
{
    class SA_Instastrike : DefaultSpecialAbility
    {
        public override string ToString()
        {
            return "Instastrike";
        }
        public override void GetRandomMinionPower(minionPower[] minionPowers, Random rand)
        {
            GetMinionPower(minionPowers, rand.Next(4, 7) * 5, 0);
        }

        public override void GetMinionPower(minionPower[] minionPowers, float val1, float val2)
        {
            minionPowers[0] = minionPower.NewMP(val1, mpScalingType.add);
        }

        public override bool ValidForItem(Item item, Projectile projectile)
        {
            return IsModdedSafe(item, projectile) && IsNotWhip(projectile);
        }
        public override void OnProjectileCreated(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            projFuncs.minionPostAI += PostAI;
            projFuncs.minionOnShootProjectile += MinionOnShootProjectile;
        }

        public override void UnhookMinionPower(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            projFuncs.minionPostAI -= PostAI;
            projFuncs.minionOnShootProjectile -= MinionOnShootProjectile;
        }

        bool terminate = false;
        bool spammingAI = false;

        public void PostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (spammingAI)
                return;
            if (projData.castingSpecialAbilityTime != -1)
                projData.castingSpecialAbilityTime--;

            Player player = Main.player[projectile.owner];
            if (player.GetModPlayer<ReworkMinion_Player>().attackedThisFrame != 1)
                return;
            projData.specialCastPosition.Y += (float)PseudoRandom.GetPseudoRandomRNG((int)projFuncs.GetMinionPower(projectile, 0));
            if (Main.rand.NextFloat() > projData.specialCastPosition.Y)
                return;
            projData.specialCastPosition.Y = 0;

            if (IsRanged(projectile))
                PostAI_Ranged(projectile, projFuncs, projData);
            else
                PostAI_Melee(projectile, projFuncs, projData);
        }

        public void PostAI_Ranged(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            int timeLeft = projectile.timeLeft;

            spammingAI = true;

            Gore.NewGore(projectile.GetSource_FromThis(), projectile.position, default, GoreID.Smoke1 + Main.rand.Next(0, 3));

            MinionTracking_State trackingState = projData.trackingState;
            MinionSpeedModifier minionSpeedModType = projData.minionSpeedModType;

            float ai0 = projectile.ai[0];
            float ai1 = projectile.ai[1];

            int iters;
            if (projectile.owner != Main.myPlayer)
                iters = 1;
            else if (projData.castingSpecialAbilityTime != -1)
                iters = 60 - projData.castingSpecialAbilityTime;
            else
                iters = 200;
            terminate = false;

            int numUpdates = projectile.numUpdates;

            for (int x = 0; x < iters; x++)
            {
                projData.trackingState = MinionTracking_State.noTracking;
                projData.minionSpeedModType = MinionSpeedModifier.none;
                projectile.AI();
                if (ProjectileLoader.ShouldUpdatePosition(projectile))
                {
                    if (projectile.tileCollide)
                        projectile.velocity = Collision.TileCollision(projectile.position, projectile.velocity, projectile.width, projectile.height, projectile.shouldFallThrough, projectile.shouldFallThrough);
                    if (ModUtils.NextVelOutOfBounds(projectile))
                    {
                        projectile.velocity = Vector2.Zero;
                        projectile.position = Main.player[projectile.owner].Center;
                    }
                    else
                        projectile.position += projectile.velocity;
                }

                if (terminate)
                {
                    break;
                }
            }
            projData.trackingState = trackingState;
            projData.minionSpeedModType = minionSpeedModType;

            projectile.numUpdates = numUpdates;

            projectile.timeLeft = timeLeft;
            if (!terminate)
                projData.castingSpecialAbilityTime = 60;
            else
            {
                projectile.ai[0] = ai0;
                projectile.ai[1] = ai1;
            }

            Gore.NewGore(projectile.GetSource_FromThis(), projectile.position, default, GoreID.Smoke1 + Main.rand.Next(0, 3));
            spammingAI = false;
        }
        const float INSTASTRIKE_DAMAGE_MOD = 0.5f;
        public void MinionOnShootProjectile(Projectile projectile, Projectile source, ReworkMinion_Projectile sourceFuncs, MinionProjectileData sourceData)
        {
            if (spammingAI)
            {
                terminate = true;
                sourceData.castingSpecialAbilityTime = -1;
                ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
                projFuncs.ArmorIgnoredPerc = (1 - (1 - projFuncs.ArmorIgnoredPerc) * INSTASTRIKE_DAMAGE_MOD);
                projectile.damage /= 2;

                if (!projectile.usesLocalNPCImmunity)
                {
                    int immunity;
                    if (projectile.usesIDStaticNPCImmunity)
                    {
                        projectile.usesIDStaticNPCImmunity = false;
                        immunity = projectile.idStaticNPCHitCooldown;
                    }
                    else
                        immunity = 10;
                    projectile.usesLocalNPCImmunity = true;
                    projectile.localNPCHitCooldown = immunity;
                }
            }
        }
        public void PostAI_Melee(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            Player player = Main.player[projectile.owner];
            ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
            NPC npc = playerFuncs.lastAttackedTarget;
            if (npc == null || !npc.active || !npc.CanBeChasedBy(projectile))
                return;
            Gore.NewGore(projectile.GetSource_FromThis(), projectile.position, default, GoreID.Smoke1 + Main.rand.Next(0, 3));
            projectile.Center = npc.Center;
            Gore.NewGore(projectile.GetSource_FromThis(), projectile.position, default, GoreID.Smoke1 + Main.rand.Next(0, 3));

            int immunity = 0;
            uint idStaticImmunity = Main.GameUpdateCount;

            if (projectile.usesLocalNPCImmunity)
            {
                immunity = projectile.localNPCImmunity[npc.whoAmI];
                projectile.localNPCImmunity[npc.whoAmI] = 0;
            }
            else if (projectile.usesIDStaticNPCImmunity)
            {
                idStaticImmunity = Projectile.perIDStaticNPCImmunity[projectile.type][npc.whoAmI];
                Projectile.perIDStaticNPCImmunity[projectile.type][npc.whoAmI] = Main.GameUpdateCount;
            }
            else
            {
                immunity = npc.immune[player.whoAmI];
                npc.immune[player.whoAmI] = 0;
            }

            Fix_Terraprisma_BatofLight_positions(projectile, projFuncs, npc.whoAmI);

            int damage = projectile.damage;
            float armorIgnoredPerc = projFuncs.ArmorIgnoredPerc;
            bool friendly = projectile.friendly;

            projFuncs.ArmorIgnoredPerc = (1 - (1 - projFuncs.ArmorIgnoredPerc) * INSTASTRIKE_DAMAGE_MOD);
            projectile.damage /= 2;
            projectile.friendly = true;

            projectile.Damage();

            projectile.damage = damage;
            projFuncs.ArmorIgnoredPerc = armorIgnoredPerc;
            projectile.friendly = friendly;

            if (projectile.usesLocalNPCImmunity)
            {
                projectile.localNPCImmunity[npc.whoAmI] = immunity;
            }
            else if (projectile.usesIDStaticNPCImmunity)
            {
                Projectile.perIDStaticNPCImmunity[projectile.type][npc.whoAmI] = idStaticImmunity;
            }
            else
            {
                npc.immune[player.whoAmI] = immunity;
            }

            ModSupportDefaultSpecialAbility.HandleInstastrikePostMelee(projectile, npc);
        }

        void Fix_Terraprisma_BatofLight_positions(Projectile projectile, ReworkMinion_Projectile projFuncs, int enemyID)
        {
            switch (projectile.type)
            {
                case ProjectileID.BatOfLight:

                    float simRate = projFuncs.GetInternalSimRate(projectile);
                    projectile.ai[0] = (int)(60 / simRate);
                    projectile.ai[1] = enemyID;
                    projectile.localAI[0] = projectile.Center.X;
                    projectile.localAI[1] = projectile.Center.Y;
                    break;
                case ProjectileID.EmpressBlade:
                    simRate = projFuncs.GetInternalSimRate(projectile);
                    projectile.ai[0] = (int)(40 / simRate);
                    projectile.ai[1] = enemyID;
                    projectile.localAI[0] = projectile.Center.X;
                    projectile.localAI[1] = projectile.Center.Y;
                    break;
            }
        }
    }
    class SA_Instastrike_Melee : DefaultSpecialAbility
    {
        public override string ToString()
        {
            return "Instastrike";
        }
        public override void GetRandomMinionPower(minionPower[] minionPowers, Random rand)
        {
            GetMinionPower(minionPowers, rand.Next(3, 7) * 5, 0);
        }

        public override void GetMinionPower(minionPower[] minionPowers, float val1, float val2)
        {
            minionPowers[0] = minionPower.NewMP(val1, scalingType: mpScalingType.add);
        }

        public override bool ValidForItem(Item item, Projectile projectile)
        {
            return false;
            //return IsModdedSafe(item, projectile) && IsNotWhip(projectile) && !IsRanged(projectile);
        }
        public override void OnProjectileCreated(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            projFuncs.minionPostAI += PostAI;
        }
        public override void UnhookMinionPower(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            projFuncs.minionPostAI -= PostAI;
        }

        public void PostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            Player player = Main.player[projectile.owner];
            ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
            if (player.GetModPlayer<ReworkMinion_Player>().attackedThisFrame != 1)
                return;
            projData.specialCastPosition.Y += (float)PseudoRandom.GetPseudoRandomRNG((int)projFuncs.GetMinionPower(projectile, 0));
            if (Main.rand.NextFloat() > projData.specialCastPosition.Y)
                return;
            projData.specialCastPosition.Y = 0;
            Gore.NewGore(projectile.GetSource_FromThis(), projectile.position, default, GoreID.Smoke1 + Main.rand.Next(0, 3));
            projectile.Center = playerFuncs.lastAttackedTarget.Center;
            Gore.NewGore(projectile.GetSource_FromThis(), projectile.position, default, GoreID.Smoke1 + Main.rand.Next(0, 3));
            if (projectile.usesIDStaticNPCImmunity)
                Projectile.perIDStaticNPCImmunity[projectile.type][playerFuncs.lastAttackedTarget.whoAmI] = Main.GameUpdateCount;
            else if (projectile.usesLocalNPCImmunity)
                projectile.localNPCImmunity[playerFuncs.lastAttackedTarget.whoAmI] = 0;
            else
                playerFuncs.lastAttackedTarget.immune[player.whoAmI] = 0;

            Fix_Terraprisma_BatofLight_positions(projectile, projFuncs, playerFuncs.lastAttackedTarget.whoAmI);
            ModSupportDefaultSpecialAbility.HandleInstastrikePostMelee(projectile, playerFuncs.lastAttackedTarget);
        }

        void Fix_Terraprisma_BatofLight_positions(Projectile projectile, ReworkMinion_Projectile projFuncs, int enemyID)
        {
            switch (projectile.type)
            {
                case ProjectileID.BatOfLight:

                    float simRate = projFuncs.GetInternalSimRate(projectile);
                    projectile.ai[0] = 40 / simRate;
                    projectile.ai[1] = enemyID;
                    projectile.localAI[0] = projectile.Center.X;
                    projectile.localAI[1] = projectile.Center.Y;
                    break;
                case ProjectileID.EmpressBlade:
                    simRate = projFuncs.GetInternalSimRate(projectile);
                    projectile.ai[0] = 60 / simRate;
                    projectile.ai[1] = enemyID;
                    projectile.localAI[0] = projectile.Center.X;
                    projectile.localAI[1] = projectile.Center.Y;
                    break;
            }
        }
    }
}
