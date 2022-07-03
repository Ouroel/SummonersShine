using Microsoft.Xna.Framework;
using SummonersShine.BakedConfigs;
using SummonersShine.Effects;
using SummonersShine.Projectiles;
using SummonersShine.SpecialData;
using SummonersShine.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.MinionAI
{
    public static partial class DefaultMinionAI
    {
        public static void AbigailCounterSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            projectile.hide = false;
            projectile.alpha = 255;
        }

        public static void AbigailCounterDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            ReworkMinion_Player ownerData = Main.player[projectile.owner].GetModPlayer<ReworkMinion_Player>();
            ReworkMinion_Projectile counterData = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            AbigailCounterStat stats = counterData.GetSpecialData<AbigailCounterStat>();
            AbigailStatCollection collection = ownerData.GetSpecialData<AbigailStatCollection>();
            collection.Remove(stats);
        }

        public static void AbigailCounterOnSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Entity _target, int specialType, bool fromServer)
        {
            projData.energy = 0;
            projData.castingSpecialAbilityTime = 0;
            projData.energyRegenRateMult = 0;
            projectile.ai[0] = Main.rand.Next(0, 25);
        }

        public static bool AbigailCounterCustomAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            Player player = Main.player[projectile.owner];
            if (player.dead)
            {
                player.abigailMinion = false;
            }
            if (player.abigailMinion)
            {
                projectile.timeLeft = 2;
            }
            projectile.frameCounter++;
            int frame = projectile.frameCounter / 6;
            if (frame == 7) {
                projectile.frameCounter = 0;
                frame = 0;
            }
            if (frame > 3) {
                frame = 6 - frame;
            }
            projectile.frame = frame;

            Lighting.AddLight(projectile.Center, TorchID.Bone);
            if (ModUtils.IsCastingSpecialAbility(projData, ItemID.AbigailsFlower) && Main.myPlayer == projectile.owner)
            {
                if (projectile.ai[0] <= 0)
                {
                    ReworkMinion_Player ownerData = player.GetModPlayer<ReworkMinion_Player>();
                    AbigailStatCollection collection = ownerData.GetSpecialData<AbigailStatCollection>();
                    Projectile MegaMinion = collection.megaMinionBody;
                    float range = projFuncs.GetMinionPower(projectile, 0) * 16;
                    int attackTarget = SpecialAbilities.SpecialAbility.RandomMinionTarget(projectile, range: range);
                    if (attackTarget != -1)
                    {
                        int damage = MegaMinion.damage;
                        int count = Math.Max(0, Main.player[MegaMinion.owner].ownedProjectileCounts[970] - 1);
                        float mult = 0.55f;
                        if (Main.hardMode)
                        {
                            mult = 1.3f;
                        }
                        damage = (int)((float)damage * (1f + (float)count * mult));
                        Entity ent = Main.npc[attackTarget];
                        Projectile bolt = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), projectile.Center, new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, -6)), ProjectileModIDs.MourningGloryBolt, damage, projectile.knockBack, projectile.owner, ent.ConvertToGlobalEntityID());
                        bolt.originalDamage = (int)range;
                        bolt.netUpdate = true;
                    }
                    projectile.ai[0] = 60 + Main.rand.NextFloat(-10, 10);
                }
                projectile.ai[0]--;
                ModUtils.IncrementSpecialAbilityTimer(projectile, projFuncs, projData, 300);
            }

            if (Main.rand.Next(150) == 0)
                for (int x = 0; x <= Main.rand.Next(3); x++)
                    Dust.NewDustDirect(projectile.Center + Main.rand.NextVector2Circular(projectile.width, projectile.height), 0, 0, DustID.SteampunkSteam, newColor: Color.GhostWhite, Alpha: 50).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);

            return false;
        }

        public static void AbigailCounterPostDraw(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Color color)
        {
            ModTextures.JustDraw_Projectile(ModTextures.MourningGlory, projectile.Center, 4, projectile.frame, 2, false, color, 100);
        }

        public static void AbigailSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            for (int i = 0; i < 10; i++)
                Gore.NewGoreDirect(projectile.GetSource_FromThis(), projectile.Top + Main.rand.NextVector2Circular(16, 16), Main.rand.NextVector2Circular(2, 2), GoreID.TreeLeaf_Crimson);
        }
        public static void AbigailDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            for (int i = 0; i < 30; i++)
                Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.SteampunkSteam, newColor: Color.GhostWhite, Alpha: 50).shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            //gore
            ReworkMinion_Player ownerData = Main.player[projectile.owner].GetModPlayer<ReworkMinion_Player>();
            AbigailStatCollection collection = ownerData.GetSpecialData<AbigailStatCollection>();
            collection.megaMinion = null;
            collection.megaMinionBody = null;
            collection.TransferEnergyInCaseOfDeath(projData.energy, projData.energyRegenRate, projData.maxEnergy);
        }

        const int minFlowersPerRow_c = 1;
        const int addFlowersPerRow_k = 8;
        const float addFlowersPerRow_k_x2 = 2f / addFlowersPerRow_k;
        const float twoc_k_div_k = (2 * minFlowersPerRow_c - addFlowersPerRow_k) / (2f * addFlowersPerRow_k);
        const float twoc_k_div_k_sqr = twoc_k_div_k * twoc_k_div_k;
        const float extraWidthPerStack = 16f;

        public static void AbigailPostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData) {
            Abigail_PositionFlowers(projectile);
        }

        static void Abigail_PositionFlowers(Projectile projectile) {

            int maxIndex = 0;
            List<Projectile> allFlowers = new();
            for (int i = 0; i < 1000; i++)
            {
                Projectile flower = Main.projectile[i];
                if (flower.type == ProjectileID.AbigailCounter && flower.active && flower.owner == projectile.owner)
                {
                    allFlowers.Add(flower);
                    maxIndex++;
                }
            }

            int maxHeight = (int)Math.Floor(Math.Sqrt(twoc_k_div_k_sqr + (maxIndex - 0.5f) * addFlowersPerRow_k_x2) - twoc_k_div_k);
            int width = addFlowersPerRow_k * maxHeight + minFlowersPerRow_c;
            int flowerCapacity = (maxHeight + 1) * (width + minFlowersPerRow_c) / 2;

            int diff = flowerCapacity - maxIndex;
            int heightToDecreAdd = 0;
            int extraAdd = 0;
            if(diff > 0 && maxHeight > 1)
            {
                int lastFlowerCapacity = flowerCapacity - (maxHeight) * addFlowersPerRow_k - minFlowersPerRow_c;
                int extras = maxIndex - lastFlowerCapacity;
                heightToDecreAdd = extras % maxHeight;
                extraAdd = (extras - heightToDecreAdd) / maxHeight + 1;
                if (heightToDecreAdd == 0)
                    extraAdd -= 1;
                width -= addFlowersPerRow_k;
            }
            maxHeight -= 1;

            int remainder = maxIndex;
            int index = 0;
            int height = 0;
            int storedRemaining = remainder;
            int dir = -projectile.spriteDirection;
            int basewidth = 32;
            if(maxIndex < 8)
            {
                basewidth = 8 + 3 * maxIndex;
            }
            allFlowers.ForEach(i =>
            {
                int workingWidth = width + extraAdd;
                int sentwidth;
                if (height == maxHeight && workingWidth > storedRemaining)
                    sentwidth = storedRemaining;
                else
                    sentwidth = workingWidth;
                Abigail_TeleportFlower(projectile, i, index, sentwidth, height, dir, basewidth);
                index++;
                remainder--;
                if (index >= workingWidth)
                {
                    height++;
                    width -= addFlowersPerRow_k;
                    index = 0;
                    if (diff > 0 && height == heightToDecreAdd)
                        extraAdd--;
                    storedRemaining = remainder;
                }
            });
        }

        static void Abigail_TeleportFlower(Projectile abi, Projectile flower, int pos, int width, int height, int dir, int baseWidth)
        {
            Vector2 initialDisp = new Vector2(0, -6);
            Vector2 layerDisp = new Vector2(0, -12);
            Vector2 abiHead = abi.Top + initialDisp;
            Vector2 disp = abiHead + layerDisp * height;

            float posinrow;
            if (width > 1)
            {
                posinrow = (float)(pos) / (width - 1);
                if (width < 8)
                {
                    float diff = width / 8f;
                    posinrow *= diff;
                    posinrow += (8 - width) / 16f;
                }
            }
            else
                posinrow = 0.5f;

            float rad = extraWidthPerStack * (height) + baseWidth;
            Vector2 circle = new Vector2(rad, rad).RotatedBy((posinrow) * -Math.PI * 1.5f);
            circle.X *= dir;
            disp += circle;

            flower.Center = disp;
        }
    }
}
