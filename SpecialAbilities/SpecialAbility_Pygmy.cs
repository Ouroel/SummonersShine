using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SummonersShine.BakedConfigs;
using SummonersShine.DataStructures;
using SummonersShine.Effects;
using SummonersShine.MinionAI;
using SummonersShine.ProjectileBuffs;
using SummonersShine.Projectiles;
using SummonersShine.SpecialData;
using SummonersShine.Textures;
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
        const float pygmyplatform_width = 16 * 5;
        const float pygmyplatform_halfwidth = pygmyplatform_width / 2;
        const float pygmyplatform_halfvalidwidth = pygmyplatform_width * 0.75f / 2;
        const float pygmyplatform_maxteleportrangesqr = 1400 * 1400;
        const float pygmyplatform_minteleportrangesqr = (16 * 4) * (16 * 4);

        public const float pygmyPlatformHalfHeight = 23;
        public const float pygmyTreeHalfHeight = 29;


        public static List<Projectile> PreAbility_Pygmy(Item summonItem, ReworkMinion_Player player)
        {
            return player.GetSpecialData<PygmyPlatformCollection>().GetPygmyFromValidList(PreAbility_FindAllMinions(summonItem, player));
        }
        public static void PygmySpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity _target, int specialType, bool fromServer)
        {
            projData.energy = 0;

            Projectile proj = PygmyTerminateOrReposition(projectile, projFuncs, projData, Main.player[projectile.owner], false);

            projData.specialCastPosition = _target.position;

            if (proj != null)
            {
                const int actualHalfWidth = 16;
                const int actualHalfHeight = 16;
                for (int x = 0; x < 10; x++)
                    Dust.NewDust(proj.Center - new Vector2(actualHalfWidth, actualHalfHeight), actualHalfWidth * 2, actualHalfHeight * 2, DustID.GoldFlame);

                proj.Center = PygmyGetFruitSpawnPosition(_target.position);
                for (int x = 0; x < 5; x++)
                    Dust.NewDust(proj.Center - new Vector2(actualHalfWidth, actualHalfHeight), actualHalfWidth * 2, actualHalfHeight * 2, DustID.GoldFlame);

            }

            PygmyInitPlatformData(projData, specialType);

            InitFreshlyLoadedPygmySpecial(projectile, projFuncs, projData);

            if (proj != null)
                PygmySaveAlrSpawnedFruit(projData);
        }

        static void InitFreshlyLoadedPygmySpecial(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            Vector2 newDraw_center = projData.specialCastPosition;
            newDraw_center.X -= pygmyplatform_halfwidth;
            projFuncs.hookedPlatform = new Platform_AttachedToProj(newDraw_center, pygmyplatform_width, projectile);

            newDraw_center.X += pygmyplatform_halfwidth;
            newDraw_center.Y += pygmyPlatformHalfHeight - pygmyTreeHalfHeight;
            CustomProjectileDrawLayer customDraw = new CustomPreDrawProjectile_Pygmy(projectile, 80, 112);
            customDraw.center = newDraw_center;
        }

        public static void PygmyPostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            projData.trackingState = (projectile.ai[0] == 1) ? MinionTracking_State.retreating : MinionTracking_State.noTracking;
            if (BakedConfig.CustomSpecialPowersEnabled(ItemID.PygmyStaff))
            {
                Player player = Main.player[projectile.owner];
                int specialTime = projData.castingSpecialAbilityTime;
                if (specialTime != -1)
                {
                    if (CustomProjectileDrawLayer.Find(projectile) == null)
                    {
                        InitFreshlyLoadedPygmySpecial(projectile, projFuncs, projData);
                    }
                    bool temp;
                    int temp2;
                    int runTime;
                    PygmyGetPlatformDrawData(specialTime, out temp, out temp2, out runTime);
                    if (runTime >= 29) {
                        if (!PygmyGetAlrSpawnedFruit(specialTime))
                            PygmySpawnFruit(projectile, projFuncs, projData, projData.specialCastPosition);
                    }
                }
                else
                    PygmyTerminateSpecialAbility(projectile, projFuncs, projData, player, null);


                if (Main.rand.Next(0, 60) == 0)
                {
                    Vector2 bestPlatformPoofLocation = Vector2.Zero;
                    if (Pygmy_FindBestPlatform(projectile, player, player.GetModPlayer<ReworkMinion_Player>(), ref bestPlatformPoofLocation)) { 
                        if((bestPlatformPoofLocation - projectile.position).LengthSquared() > pygmyplatform_minteleportrangesqr)
                        {
                            DefaultMinionAI.PygmySummonEffect(projectile, projFuncs, projData, player);
                            projectile.velocity = Vector2.Zero;
                            projectile.Bottom = bestPlatformPoofLocation + new Vector2(Main.rand.NextFloat(-pygmyplatform_halfvalidwidth, pygmyplatform_halfvalidwidth), -20);
                            DefaultMinionAI.PygmySummonEffect(projectile, projFuncs, projData, player);
                        }
                    }
                }

                if (projectile.tileCollide)
                {
                    Tuple<bool, Vector2> result = PlatformCollection.TestPlatformCollision(projectile, PygmyCanLandOnPlatform);
                    if (result.Item1)
                    {
                        projectile.velocity = result.Item2;
                        projData.trackingState = MinionTracking_State.noTracking;
                    }
                }
            }
        }

        public static bool PygmyCanLandOnPlatform(Platform platform) {
            Platform_AttachedToProj platformCasted = platform as Platform_AttachedToProj;
            if (platformCasted == null)
                return true;
            switch (platformCasted.parent.type)
            {
                case ProjectileID.Pygmy:
                case ProjectileID.Pygmy2:
                case ProjectileID.Pygmy3:
                case ProjectileID.Pygmy4:
                    return true;
            }
            return false;
        }
        public static void PygmyTerminateSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player, ReworkMinion_Player playerFuncs)
        {
            PygmyTerminateOrReposition(projectile, projFuncs, projData, player, true);
        }

        public static Projectile PygmyTerminateOrReposition(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player, bool terminate)
        {
            Projectile rv = null;
            PlatformCollection.UnattachAllPlatforms(projectile);
            Platform platform = projFuncs.hookedPlatform;
            if (platform != null)
            {
                platform.Destroy();

                for (int x = 0; x < Main.maxProjectiles; x++)
                {
                    Projectile proj = Main.projectile[x];
                    if (proj != null && proj.active && proj.type == ProjectileModIDs.GoldenFruit && proj.ai[0] == projectile.whoAmI && proj.ai[1] <= 0)
                    {
                        if (terminate)
                            proj.Kill();
                        else
                            rv = proj;

                    }
                }

                bool flip;
                Vector2 castPos = projData.specialCastPosition;
                int platformType;
                int runTime;
                PygmyGetPlatformDrawData(projData.castingSpecialAbilityTime, out flip, out platformType, out runTime);
                CustomProjectileDrawLayer drawy = CustomProjectileDrawLayer.Find(projectile);
                if (drawy != null)
                    (drawy as CustomPreDrawProjectile_Pygmy).Terminate(castPos, flip, platformType, runTime, player);
            }
            return rv;
        }

        static bool Pygmy_FindBestPlatform(Projectile pygmy, Player owner, ReworkMinion_Player ownerFuncs, ref Vector2 platformLoc) {

            MinionEnergyCounter minionCollection = ownerFuncs.GetMinionCollection(ItemID.PygmyStaff);

            float maxDist = 800 + 40 * pygmy.minionPos;
            if (owner.MinionAttackTargetNPC != -1)
            {
                NPC targetNPC = Main.npc[owner.MinionAttackTargetNPC];
                bool minionClosest;
                
                if (Pygmy_FindClosestValidPlatformToEnemy(pygmy, targetNPC, minionCollection, ref platformLoc, ref maxDist, out minionClosest))
                    return true;
                if (minionClosest)
                    return false;
            }

            bool rv = false;

            for (int x = 0; x < Main.npc.Length; x++)
            {
                NPC npc = Main.npc[x];
                bool minionClosest;
                bool closerFound = Pygmy_FindClosestValidPlatformToEnemy(pygmy, npc, minionCollection, ref platformLoc, ref maxDist, out minionClosest);
                if (minionClosest)
                    rv = false;
                else
                    rv = rv || closerFound;
            }

            return rv;
        }

        static bool Pygmy_FindClosestValidPlatformToEnemy(Projectile pygmy, NPC enemy, MinionEnergyCounter allPygmies, ref Vector2 platformPos, ref float maxDist, out bool minionClosest) {

            bool rv = false;
            Vector2 out_PlatformPos = Vector2.Zero;


            Vector2 origin = pygmy.Center;
            bool minionValid = PygmyMinionCheck(pygmy, enemy, origin, maxDist, ref maxDist);

            float maxDist_wrapped = maxDist;

            allPygmies.minions.ForEach(i => {
                MinionProjectileData iProjData = i.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData();
                if (ModUtils.IsCastingSpecialAbility(iProjData, ItemID.PygmyStaff))
                {
                    Vector2 origin = iProjData.specialCastPosition;
                    if ((pygmy.Center - origin).LengthSquared() < pygmyplatform_maxteleportrangesqr && PygmyMinionCheck(pygmy, enemy, origin, maxDist_wrapped, ref maxDist_wrapped))
                    {
                        rv = true;
                        out_PlatformPos = origin;
                    }
                }
            });

            maxDist = maxDist_wrapped;

            if (rv)
                platformPos = out_PlatformPos;

            minionClosest = minionValid && !rv;

            return rv;
        }
        public static bool PygmyMinionCheck(Projectile minion, NPC npc, Vector2 projectileOrigin, float len, ref float returnDist)
        {

            if (npc.CanBeChasedBy(minion, false))
            {
                Vector2 disp = npc.Center - projectileOrigin;
                float dist = Math.Abs(disp.X) + Math.Abs(disp.Y);
                bool canHit = dist < len && Collision.CanHitLine(projectileOrigin, 0, 0, npc.position, npc.width, npc.height);
                if (canHit)
                {
                    returnDist = dist;
                    return true;
                }
            }
            return false;
        }

        static Vector2 PygmyGetFruitSpawnPosition(Vector2 platformCenter)
        {
            return platformCenter + new Vector2(0, -28);
        }

        static void PygmyRepositionFruit(MinionProjectileData projData, Projectile fruit, Vector2 platformCenter) {
            fruit.Center = PygmyGetFruitSpawnPosition(platformCenter);
            PygmySaveAlrSpawnedFruit(projData);
        }
        static void PygmySpawnFruit(Projectile proj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Vector2 platformCenter) {

            if (Main.myPlayer == proj.owner)
            {
                Vector2 fruitSpawnPos = PygmyGetFruitSpawnPosition(platformCenter);
                Projectile.NewProjectileDirect(proj.GetSource_FromThis(), fruitSpawnPos, Vector2.Zero, ProjectileModIDs.GoldenFruit, 0, 0, proj.owner, proj.whoAmI, GoldenFruit.readyTime);
            }
            PygmySaveAlrSpawnedFruit(projData);
        }

        const int pygmy_drawdata_maxplatformdata = 8;
        const int pygmy_drawdata_maxruntimedata = 31;
        const int pygmy_drawdata_maxruntimedata_baked = pygmy_drawdata_maxruntimedata * pygmy_drawdata_maxplatformdata;
        const int pygmy_spawnedFruit = 2;
        const int pygmy_spawnedFruit_baked = pygmy_drawdata_maxruntimedata_baked * pygmy_spawnedFruit;
        public static void PygmyGetPlatformDrawData(int castingSpecialAbilityTime, out bool flip, out int platformType, out int runTime) {
            int flipNum = castingSpecialAbilityTime % 2;
            flip = flipNum == 0;
            castingSpecialAbilityTime -= flipNum;
            castingSpecialAbilityTime /= 2;
            platformType = castingSpecialAbilityTime % 4;
            castingSpecialAbilityTime -= platformType;
            castingSpecialAbilityTime /= 4;
            runTime = castingSpecialAbilityTime % pygmy_drawdata_maxruntimedata;
        }

        static void PygmyInitPlatformData(MinionProjectileData projData, int platformType)
        {
            projData.castingSpecialAbilityTime = platformType;
        }
        public static void PygmySaveRuntimeData(MinionProjectileData projData, int runTime)
        {
            int left = projData.castingSpecialAbilityTime % pygmy_drawdata_maxplatformdata;
            projData.castingSpecialAbilityTime -= projData.castingSpecialAbilityTime % pygmy_drawdata_maxruntimedata_baked;
            projData.castingSpecialAbilityTime += runTime * pygmy_drawdata_maxplatformdata + left;
        }

        static bool PygmyGetAlrSpawnedFruit(int specialTime)
        {
            int left = specialTime % pygmy_drawdata_maxruntimedata_baked;
            left = (specialTime - left) / pygmy_drawdata_maxruntimedata_baked;
            return left % pygmy_spawnedFruit_baked == 1;
        }
        static void PygmySaveAlrSpawnedFruit(MinionProjectileData projData)
        {
            int left = projData.castingSpecialAbilityTime % pygmy_drawdata_maxruntimedata_baked;
            projData.castingSpecialAbilityTime -= projData.castingSpecialAbilityTime % pygmy_spawnedFruit_baked;
            projData.castingSpecialAbilityTime += 1 * pygmy_drawdata_maxruntimedata_baked + left;
        }
    }
}
