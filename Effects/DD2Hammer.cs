using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace SummonersShine.Effects
{
    public class DD2Hammer : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLightEmittence = false;
            dust.frame = new Rectangle(0, 0, 24, 24);
            dust.customData = 0;
            dust.noLight = false;
            dust.alpha = 50;
        }
        public override bool Update(Dust dust)
        {
            int projSpace = Main.maxProjectiles + 1;

            int orig = (int)dust.customData;
            int temp = orig;
            int proj = temp % projSpace;
            temp = (temp - proj) / projSpace;
            int posType = temp % 12;
            int timer = (temp - proj) / 12;
            timer++;
            dust.customData = orig + projSpace * 12;
            int frameTimer = timer % 30;
            if (frameTimer > 25)
                frameTimer = 3;
            else if (frameTimer > 15)
                frameTimer = 2;
            else if (frameTimer > 10)
                frameTimer = 1;
            else
                frameTimer = 0;
            dust.frame.Y = frameTimer * 24;
            Projectile projectile = Main.projectile[proj];
            if (projectile == null || !projectile.active || timer == 300)
                dust.active = false;
            dust.position = projectile.Top + GetRelPos(posType);
            dust.velocity = Vector2.Zero;
            dust.scale = 1;
            return true;
        }
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color * (1 - dust.alpha * 0.003922f);
        }

        public static Vector2 GetRelPos(int posType)
        {
            switch (posType)
            {
                case 0:
                    return new Vector2(-16, -6);
                case 3:
                    return new Vector2(-16, -24);
                case 6:
                    return new Vector2(-16, -24);
                case 9:
                    return new Vector2(-16, -16);

            }
            return Vector2.Zero;
        }
    }
}