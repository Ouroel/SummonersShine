using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SummonersShine.BakedConfigs;
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
        public const float spiderWebFormationTime = 15;
        public const float spiderDragInTime = 60;
        public static void SpiderOnSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity _target, int specialType, bool fromServer)
        {
            projData.energy = 0;
            projData.energyRegenRateMult = 0;
            projData.specialCastTarget = (NPC)_target;
            projData.castingSpecialAbilityTime = 0;
        }
        public static bool SpiderCustomAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            Player player = Main.player[projectile.owner];
            if(ModUtils.IsCastingSpecialAbility(projData, ItemID.SpiderStaff))
            {
                projData.actualMinionAttackTargetNPC = player.MinionAttackTargetNPC;
                player.MinionAttackTargetNPC = projData.specialCastTarget.whoAmI;
                NPC target = projData.specialCastTarget;
                if (!target.boss && projData.castingSpecialAbilityTime > spiderWebFormationTime)
                    target.AddBuff(BuffID.Slow, 2);
            }
            return true;
        }
        public static bool SpiderCustomDraw(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Color lightColor)
        {
            if (projData.castingSpecialAbilityTime == -1)
                return true;
            float webProgress = projData.castingSpecialAbilityTime / spiderWebFormationTime;
            float stringyWebProgress = MathHelper.Clamp(webProgress, 0, 1);

            Vector2 spiderMouth = getSpiderMouth(projectile);
            Vector2 direction = projData.specialCastTarget.Center - projectile.Center - spiderMouth;
            if (direction == Vector2.Zero)
                return true;

            direction *= stringyWebProgress;

            float webTautness = (spiderWebFormationTime - webProgress) / spiderWebFormationTime;

            DrawString(projectile, spiderMouth, direction, lightColor, webProgress < spiderWebFormationTime, webTautness);
            return true;
        }

        public static void DrawString(Projectile projectile, Vector2 spiderMouth, Vector2 direction, Color lightColor, bool formingWeb, float webTautness)
        {
            float distance = direction.Length();
            int steps = 0;
            if (formingWeb)
                steps = (int)(distance * 0.3f);
            steps += 1;

            Texture2D webTexture = TextureAssets.Extra[47].Value;
            Vector2 thisPos = projectile.Center + spiderMouth;
            for (int x = 0; x < steps; x++)
            {

                float thisStep = (x + 1) / (float)steps;
                Vector2 nextPos;
                if (x == steps - 1)
                    nextPos = projectile.Center + spiderMouth + direction;
                else
                {
                    float sineStep = MathF.Pow(thisStep, 15);
                    nextPos = new Vector2(sineStep * distance, MathF.Sin((1 - sineStep) * MathF.PI) * webTautness * 32 * MathF.Sin(thisStep * MathF.Ceiling(distance / 64) * 2 * MathF.PI));
                    nextPos = projectile.Center + spiderMouth + nextPos.RotatedBy(direction.ToRotation());
                }

                Vector2 webOrigin = thisPos;
                Vector2 nextDir = nextPos - thisPos;

                Vector2 scale = new Vector2(1f, nextDir.Length() / (float)webTexture.Height);

                Vector2 position = webOrigin - Main.screenPosition;

                Vector2 origin = new Vector2(webTexture.Width * 0.5f, 0);

                Main.EntitySpriteDraw(webTexture, position, null, lightColor * 0.5f, nextDir.ToRotation() - 1.5707964f, origin, scale, SpriteEffects.None, 0);
                thisPos = nextPos;
            }
        }

        static Vector2 getSpiderMouth(Projectile projectile) {
            bool spiderClimbing = projectile.frame > 3 && projectile.frame < 8;
            if (spiderClimbing)
                return new Vector2(12, 0).RotatedBy(projectile.rotation - 1.5707964f);
            return new Vector2(12 * projectile.direction, 12);
        }
    }
}
