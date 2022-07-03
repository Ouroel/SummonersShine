using Microsoft.Xna.Framework;
using SummonersShine.SpecialAbilities;
using SummonersShine.SpecialData;
using System;
using Terraria;
using Terraria.ID;

namespace SummonersShine.Textures
{
    public class CustomPreDrawProjectile_Pygmy : CustomProjectileDrawLayer
    {
        public override void Update(Projectile projectile)
        {
            if (proj == null)
            {
                this.fadeTime--;
                if (this.fadeTime <= 0)
                    this.destroyThisNext = true;
            }
        }

        public override void Draw(Projectile projectile)
        {
            MinionProjectileData projData = null;
            if (proj != null)
                projData = projectile.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData();
            if (proj == null || ModUtils.IsCastingSpecialAbility(projData, ItemID.PygmyStaff))
            {

                Vector2 castPos;
                bool flip;
                int platformType;
                int alpha;
                if (proj == null)
                {
                    castPos = this.castPos;
                    flip = this.flip;
                    platformType = this.platformType;
                    alpha = 255 - this.fadeTime * 205 / 30;
                }
                else
                {
                    castPos = projData.specialCastPosition;
                    SpecialAbility.PygmyGetPlatformDrawData(projData.castingSpecialAbilityTime, out flip, out platformType, out alpha);
                    alpha++;
                    if (!Main.gamePaused && alpha < 30)
                        SpecialAbility.PygmySaveRuntimeData(projData, alpha);
                    alpha = 255 - alpha * 205 / 30;
                }

                ModTextures.JustDraw_Projectile(ModTextures.PygmyLivingTree, castPos + new Vector2(0, -SpecialAbility.pygmyTreeHalfHeight), 1, 0, 1, flip, Lighting.GetColor(new Point((int)castPos.X / 16, (int)castPos.Y / 16)), alpha);

                ModTextures.JustDraw_Projectile(ModTextures.PygmyPlatform, castPos + new Vector2(0, SpecialAbility.pygmyPlatformHalfHeight), 4, platformType, 1, flip, Lighting.GetColor(new Point((int)castPos.X / 16, (int)castPos.Y / 16)), alpha);
            }
        }
        public CustomPreDrawProjectile_Pygmy(Projectile proj, int width, int height) : base(proj, width, height) { }

        public Vector2 castPos;
        public bool flip;
        public int platformType;
        public int fadeTime;

        public Player player;
        public int shader;

        public void Terminate(Vector2 castPos, bool flip, int platformType, int runTime, Player player) { 
            this.castPos = castPos;
            this.flip = flip;
            this.platformType = platformType;
            this.fadeTime = runTime;
            this.proj = null;

            this.player = player;
        }

        public override int GetIntendedShader_NoProj()
        {
            if (player != null && player.active)
                shader = player.cMinion;
            else
                player = null;
            return shader;
        }
    }
}
