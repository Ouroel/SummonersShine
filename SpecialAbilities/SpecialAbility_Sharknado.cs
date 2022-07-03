using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SummonersShine.BakedConfigs;
using SummonersShine.Effects;
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
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace SummonersShine.SpecialAbilities
{
    public static partial class SpecialAbility
    {
        public static void SharknadoOnSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity target, int specialType, bool fromServer)
        {
            projData.energy = 0;
            projData.castingSpecialAbilityTime = 0;
            Player player = Main.player[projectile.owner];
            projData.specialCastPosition = (target.position - player.position);
        }

        const float carryOverVelMod_X = 1.8f;
        const float oneOverCarryOverVelModY = 1 / (carryOverVelMod_X * carryOverVelMod_X / (2 * carryOverVelMod_X - 1));
        public static void SharknadoPreAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            if (ModUtils.IsCastingSpecialAbility(projData, ItemID.TempestStaff))
            {
                float mod = projFuncs.GetMinionPower(projectile, 0);
                float time = mod * 60;
                Player player = Main.player[projectile.owner];
                if (time - projData.castingSpecialAbilityTime > 1)
                {
                    float total = time * carryOverVelMod_X;
                    float perc = projData.castingSpecialAbilityTime / time;
                    float ddx = 2 * carryOverVelMod_X * oneOverCarryOverVelModY * (1 - perc * oneOverCarryOverVelModY);
                    float step = ddx / total;
                    Vector2 diffVel = projData.specialCastPosition * step;
                    player.velocity = diffVel;
                    if (diffVel.X * player.direction < 0)
                        player.direction *= -1;

                    for (int x = 0; x < 3; x++)
                    {
                        Dust dust = Dust.NewDustDirect(player.Bottom + new Vector2(0, -10) + Main.rand.NextVector2Circular(8, 8), 0, 0, DustID.FishronWings, 0, 0, 50, Scale: 2);
                        dust.velocity *= Main.rand.NextFloat(0.5f);
                        dust.noGravity = true;
                        dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                    }
                }
                ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
                playerFuncs.ShouldMakeIgnoreWater = true;

                SoundEngine.PlaySound(SoundID.Item21, player.Center);

                projectile.IncrementSpecialAbilityTimer(projFuncs, projData, (int)time + 1);
            }
        }

        public static void SharknadoOnPlayerDraw(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, ref PlayerDrawSet drawInfo)
        {
            if (ModUtils.IsCastingSpecialAbility(projData, ItemID.TempestStaff))
            {
                Player player = Main.player[projectile.owner];
                //animation
                int frame = projData.castingSpecialAbilityTime % 9;

                Texture2D value = ModTextures.TempestWind;

                Entity entity = player;

                Vector2 vector = entity.Center;
                ModTextures.JustDraw_Player(value, player, vector, 9, frame, 2, ref drawInfo, Color.White, player.cMinion);
            }
        }
    }
}
