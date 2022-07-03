using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using SummonersShine.SpecialData;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Graphics.Shaders;

namespace SummonersShine.MinionAI
{
    public static partial class DefaultMinionAI
    {

        public static void DragonSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            if (projectile.type == ProjectileID.StardustDragon1)
                projectile.velocity += projectile.rotation.ToRotationVector2();

            for (int x = 0; x < 8; x++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position + Main.rand.NextVector2Circular(8, 8), projectile.width, projectile.height, DustID.IceTorch);
                dust.noGravity = true;
                dust.fadeIn = 2f;
                dust.velocity = (dust.position - projectile.position + new Vector2(0, 16).RotatedBy(projectile.rotation)) * 0.2f;
                Point point = dust.position.ToTileCoordinates();
                if (WorldGen.InWorld(point.X, point.Y, 5) && WorldGen.SolidTile(point.X, point.Y, false))
                {
                    dust.noLight = true;
                }
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            }
        }
        public static void DragonDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            DragonSummonEffect(projectile, projFuncs, projData, player);

            ReworkMinion_Player ownerData = Main.player[projectile.owner].GetModPlayer<ReworkMinion_Player>();
            ReworkMinion_Projectile dragonData = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            StardustDragonStat stats = dragonData.GetSpecialData<StardustDragonStat>();
            StardustDragonStatCollection collection = ownerData.GetSpecialData<StardustDragonStatCollection>();
            collection.Remove(stats);
        }
        /*public static void DragonEndOfAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
        }*/
        public static void DragonPostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (projData.moveTarget as NPC != null)
                projData.trackingState = MinionTracking_State.normal;
            else
                projData.trackingState = MinionTracking_State.retreating;
        }

        public static void DragonBodyPostAI(Projectile thisProj, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            Player player = Main.player[thisProj.owner];
            if ((int)Main.timeForVisualEffects % 120 == 0)
            {
                thisProj.netUpdate = true;
            }
            if (!player.active)
            {
                thisProj.active = false;
                return;
            }
            if (player.dead)
            {
                player.stardustDragon = false;
            }
            if (player.stardustDragon)
            {
                thisProj.timeLeft = 2;
            }

            bool hasParent = false;
            if (thisProj.ai[1] == 1f)
            {
                thisProj.ai[1] = 0f;
                thisProj.netUpdate = true;
            }
            int byUUID = Projectile.GetByUUID(thisProj.owner, (int)thisProj.ai[0]);
            if (Main.projectile.IndexInRange(byUUID))
            {
                Projectile parentProj = Main.projectile[byUUID];
                if (parentProj.active && (parentProj.type == 625 || parentProj.type == 626 || parentProj.type == 627))
                {
                    hasParent = true;
                    parentProj.localAI[0] = thisProj.localAI[0] + 1f;
                    if (parentProj.type != 625)
                    {
                        parentProj.localAI[1] = (float)thisProj.whoAmI;
                    }
                }
            }
            if (!hasParent)
            {
                for (int k = 0; k < 1000; k++)
                {
                    Projectile projectile2 = Main.projectile[k];
                    if (projectile2.active && projectile2.owner == thisProj.owner && ProjectileID.Sets.StardustDragon[projectile2.type] && projectile2.localAI[1] == thisProj.ai[0])
                    {
                        thisProj.ai[0] = (float)projectile2.projUUID;
                        projectile2.localAI[1] = (float)thisProj.whoAmI;
                        thisProj.netUpdate = true;
                    }
                }
                return;
            }
        }

        public static void Dragon_PostAIPostRelativeVelocity(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            Vector2 totalVelocity = projectile.velocity + projData.lastRelativeVelocity;

            projectile.rotation = totalVelocity.ToRotation() + 1.5707964f;

            int direction = projectile.direction;

            if (totalVelocity.X < 0.1f && totalVelocity.X > -0.1f)
                totalVelocity.X = 0;

            projectile.direction = (projectile.spriteDirection = ((totalVelocity.X > 0f) ? 1 : -1));

            if (direction != projectile.direction)
            {
                projectile.netUpdate = true;
            }

            /*
            if (projData.nextTicks < 0)
                return;

            List<Projectile> allUpdated = ModUtils.GetWholeStardustDragon_Sorted(projectile.owner);
            allUpdated.Remove(projectile);
            allUpdated.ForEach(i =>
            {
                i.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData().currentTick = projData.currentTick;
                float ai0 = i.ai[0];
                float ai1 = i.ai[1];
                float localai0 = i.localAI[0];
                float localai1 = i.localAI[1];
                i.VanillaAI();
                i.ai[0] = ai0;
                i.ai[1] = ai1;
                i.localAI[0] = localai0;
                i.localAI[1] = localai1;
            });*/
        }
    }
}
