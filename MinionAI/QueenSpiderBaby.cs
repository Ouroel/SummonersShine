using Microsoft.Xna.Framework;
using SummonersShine.BakedConfigs;
using SummonersShine.SpecialAbilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.MinionAI
{
    public class QueenSpiderBaby : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.type == ProjectileID.BabySpider;
        }

        public override void AI(Projectile projectile)
        {
            bool justDrag = false;
            if (projectile.timeLeft > spiderTotalTime + 31)
            {
                int x = (int)projectile.position.X / 16;
                int y = (int)projectile.position.Y / 16;
                int y2 = y + 32;
                Tuple<int, int> result;
                Collision.TupleHitLine(x, y, x, y2, 0, 0, new(), out result);
                if (result.Item2 == y2)
                    projectile.timeLeft = Main.rand.Next(30) + spiderTotalTime;
            }
            else if (projectile.timeLeft == spiderTotalTime + 31)
            {
                projectile.timeLeft = Main.rand.Next(30) + spiderTotalTime;
            }
            if ((justDrag || projectile.timeLeft <= spiderTotalTime) && projectile.originalDamage == 0)
            {
                int originalDamage = -1;
                projectile.Minion_FindTargetInRange(1400, ref originalDamage, true);
                projectile.originalDamage = originalDamage + 1;
                projectile.timeLeft = spiderTotalTime;
                if (originalDamage != -1)
                {
                    for (int x = 0; x < 5; x++)
                        Dust.NewDust(projectile.Center, 1, 1, DustID.Web);
                }
            }
            else if (projectile.originalDamage != 0)
            {
                NPC npc = Main.npc[projectile.originalDamage - 1];
                if (npc == null || !npc.active)
                {
                    projectile.tileCollide = true;
                    projectile.originalDamage = 0;
                    return;
                }
                Vector2 diff = npc.Center - projectile.Center;
                projectile.spriteDirection = diff.X < 0 ? 1 : -1;

                if (projectile.timeLeft == spiderDragInTime)
                {
                    for (int x = 0; x < 5; x++)
                        Dust.NewDust(npc.Center, 1, 1, DustID.Web);
                }
                if (projectile.timeLeft < spiderDragInTime)
                {
                    projectile.tileCollide = false;
                    float remaining = spiderDragInTime - projectile.timeLeft;
                    projectile.velocity = diff * MathHelper.Clamp(remaining / spiderDragInTime, 0, 1);
                }
            }
        }

        const int spiderTotalTime = spiderWebFormationTime + spiderDragInTime;
        const int spiderWebFormationTime = 15;
        const int spiderDragInTime = 45;
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (projectile.originalDamage != 0)
            {
                NPC npc = Main.npc[projectile.originalDamage - 1];
                float webProgress = spiderTotalTime - projectile.timeLeft;
                if(webProgress > spiderWebFormationTime)
                    webProgress = spiderWebFormationTime;
                webProgress /= spiderWebFormationTime;
                float webTautness = webProgress;

                Vector2 direction = npc.Center - projectile.Center;
                if (direction == Vector2.Zero)
                    return true;
                float dirLen = direction.Length();
                int bound = Math.Min(npc.width, npc.height);
                direction *= (dirLen - bound / 3) / dirLen;
                float stringyWebProgress = MathHelper.Clamp(webProgress, 0, 1);
                direction *= stringyWebProgress;

                SpecialAbility.DrawString(projectile, new Vector2(4, 2) * projectile.spriteDirection, direction, lightColor, true, webTautness);
            }
            return true;
        }
    }
}
