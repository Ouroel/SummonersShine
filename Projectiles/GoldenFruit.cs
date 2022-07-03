using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.Projectiles
{
    public class GoldenFruit : ModProjectile
    {
        const int actualHalfWidth = 16;
        const int actualHalfHeight = 16;
        const int detectionHalfWidth = 80;
        const int detectionHalfHeight = 80;
        const int detectionWidth = detectionHalfWidth * 2;
        const int detectionHeight = detectionHalfHeight * 2;
        const int drawOffsetX = detectionHalfWidth - actualHalfWidth;
        const int drawOffsetY = detectionHalfHeight - actualHalfHeight;

        const int growTime = 60 * 120;
        const int t2FruitTime = -60 * 60;
        const int t3FruitTime = -60 * 30;
        public const int readyTime = -growTime;
        int attachedEntityID { get { return (int)Projectile.ai[0]; } set { Projectile.ai[0] = value; } }
        int collectingTime { get { return (int)Projectile.ai[1]; } set { Projectile.ai[1] = value; } }
        public override void SetStaticDefaults()
        {
            ProjectileModIDs.GoldenFruit = Projectile.type;
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.CanDistortWater[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.hide = true;
            Projectile.width = detectionWidth;
            Projectile.height = detectionHeight;
            DrawOffsetX = drawOffsetX;
            DrawOriginOffsetY = drawOffsetY;
            Projectile.scale = 1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.alpha = 50;
            Projectile.timeLeft = 2;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool? CanDamage()
        {
            return false;
        }
        public void TryEat(Player target)
        {
            if (collectingTime > 0)
                return;

            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, Projectile.type, 0, 0, Projectile.owner, Projectile.ai[0], readyTime);
            }
            attachedEntityID = target.whoAmI;
            collectingTime = 1;
            Projectile.hide = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = new Color(255, 255, 255, 205);
            return true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (!Projectile.hide)
                return;
            behindProjectiles.Add(index);
        }

        public override void Kill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            for (int x = 0; x < 10; x++)
                Dust.NewDust(Projectile.Center - new Vector2(actualHalfWidth, actualHalfHeight), actualHalfWidth * 2, actualHalfHeight * 2, DustID.GoldFlame);
        }
        public override void AI()
        {
            if (Projectile.timeLeft <= 2)
                Projectile.timeLeft = 2;

            if (collectingTime == 0)
            {
                Projectile.frame = 3;
                List<Player> validPlayers = ModUtils.DetectPlayersWithinCircle(Projectile.Center, 80);
                validPlayers.ForEach(p => TryEat(p));
                if (Main.rand.NextBool(5))
                    Dust.NewDust(Projectile.Center - new Vector2(actualHalfWidth, actualHalfHeight), actualHalfWidth * 2, actualHalfHeight * 2, DustID.GoldFlame);
            }
            else if (collectingTime < 0)
            {
                if (collectingTime < readyTime + 3)
                {
                    for (int x = 0; x < 5; x++)
                        Dust.NewDust(Projectile.Center - new Vector2(actualHalfWidth, actualHalfHeight), actualHalfWidth * 2, actualHalfHeight * 2, DustID.GoldFlame);
                }
                else if (collectingTime > t3FruitTime)
                {
                    Projectile.frame = 2;
                }
                else if (collectingTime > t2FruitTime)
                {
                    Projectile.frame = 1;
                }
                collectingTime++;
            }
            else
            {
                if (Main.rand.NextBool(2))
                    Dust.NewDust(Projectile.Center - new Vector2(actualHalfWidth, actualHalfHeight), actualHalfWidth * 2, actualHalfHeight * 2, DustID.GoldFlame);
                Player player = Main.player[attachedEntityID];
                if (!player.active)
                {
                    Projectile.Kill();
                    return;
                }

                Vector2 diff = player.Center - Projectile.Center;
                if (collectingTime == 60 || diff.LengthSquared() < 256)
                {
                    ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
                    playerFuncs.lifeSteal += Projectile.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionPower(Projectile, 0);
                    int heal = (int)playerFuncs.lifeSteal;
                    playerFuncs.lifeSteal -= heal;

                    player.HealEffect(heal);
                    player.statLife = Math.Min(player.statLife + heal, player.statLifeMax2);
                    Projectile.Center = Main.player[Projectile.owner].Center;
                    Projectile.Kill();
                    return;
                }

                collectingTime++;
                diff /= 60 - collectingTime;
                Projectile.position += diff;
                Projectile.rotation = diff.ToRotation() + MathF.PI / 2;
            }
        }
    }
}
