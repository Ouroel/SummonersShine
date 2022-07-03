using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SummonersShine.BakedConfigs;
using SummonersShine.DataStructures;
using SummonersShine.SpecialData;
using SummonersShine.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine.Projectiles
{
    class MartianShieldEntityData {
        public bool Disabled = false;
        public int Dependency;
        public Entity Entity;
        public Entity SourceEntity;
        public MartianShieldEntityData(Entity entity, Entity sourceEntity, int dependency)
        {
            Dependency = dependency;
            SourceEntity = sourceEntity;
            Entity = entity;
        }
    }
    class MartianShieldLightning : TimerData
    {
        uint endTime;
        List<MartianShieldEntityData> data;

        public MartianShieldLightning(List<MartianShieldEntityData> data) {
            this.data = data;
            endTime = Main.GameUpdateCount + 30;
        }
        public uint GetEndTime()
        {
            return endTime;
        }
        public void Update()
        {
            data.ForEach(i => {
                if (!i.Disabled && (!i.Entity.active || (i.Dependency != -1 && data[i.Dependency].Disabled)))
                    i.Disabled = true;
            });
        }

        public void Draw(Projectile proj)
        {
            data.ForEach(i => {
                if (!i.Disabled)
                    DrawLightning(i.SourceEntity.Center, i.Entity.Center);
            });
        }
        public static void DrawLightning(Vector2 start, Vector2 end)
        {
            Texture2D tex = MartianShieldGenerator.MartianTurretBolt;
            Vector2 diff = end - start;
            int iters = (int)(diff.Length() / (tex.Width - 4)) + 1;
            Vector2 normal = new(diff.Y, -diff.X);
            if (normal != Vector2.Zero)
                normal.Normalize();
            Vector2 prevPosEnd = start;
            for (int x = 0; x < iters; x++) {
                Vector2 nextPos = start + diff * (x + 1) / iters;
                if (x != iters - 1)
                    nextPos += normal * Main.rand.NextFloat(-8, 8);
                Vector2 posDiff = nextPos - prevPosEnd;
                Vector2 pos = prevPosEnd + posDiff * 0.5f - Main.screenPosition;
                prevPosEnd = nextPos;
                Rectangle frame = tex.Frame(1, 4, 0, Main.rand.Next(4));
                frame.X += 2;
                frame.Width -= 4;
                Main.EntitySpriteDraw(tex, pos, frame, Color.White * (0.4f + 0.6f * (x + 1) / iters), posDiff.ToRotation(), new Vector2(frame.Width / 2, frame.Height / 2), new Vector2(posDiff.Length() / (frame.Width - 4), 1), SpriteEffects.None, 0);
            }
        }
    }
    public class MartianShieldGenerator : ModProjectile
    {

        public static Texture2D MartianTurretBolt;

        CustomDrawOverPlayerProjectile_MartianShieldGenerator drawLayer;
        Timer<MartianShieldLightning> LightningData;

        ProjectileOnHit onProjectileCollideHook;

        List<Projectile> collidedProjectiles = new();
        List<Entity> filledList = new();

        Vector2[] ShieldGeneratorLightningPos = new Vector2[3];
        Vector2[] ShieldGeneratorLightningVel = new Vector2[3];

        public class MartianShieldGenerator_Loader : ILoadable
        {
            public void Load(Mod mod)
            {
            }

            public void Unload()
            {
                MartianTurretBolt = null;
            }
        }
        public override void SetStaticDefaults()
        {
            ProjectileModIDs.MartianShieldGenerator = Projectile.type;
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.CanDistortWater[Projectile.type] = true;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            MartianTurretBolt = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.MartianTurretBolt, AssetRequestMode.ImmediateLoad).Value;
        }

        public int Duration => (int)(1200 - Projectile.ai[1]);
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.timeLeft = 1200;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.idStaticNPCHitCooldown = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Summon;

            LightningData = new();
            for (int x = 0; x < 3; x++) {
                ShieldGeneratorLightningPos[x] = Main.rand.NextVector2Circular(len, len);
                if (ShieldGeneratorLightningPos[x].Y > 0)
                    ShieldGeneratorLightningPos[x].Y = -ShieldGeneratorLightningPos[x].Y;
                ShieldGeneratorLightningVel[x] = Vector2.Zero;
            }
            Projectile.netImportant = true;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        const int len = 16 * 4;

        Entity TargetedEnt = null;
        public override bool? CanHitNPC(NPC target)
        {
            return target == TargetedEnt;
        }
        public void OnProjectileCollide(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Projectile collider)
        {
            if (projectile.ai[0] > 0 || collidedProjectiles.Contains(collider))
                return;
            Vector2 Normal = (Projectile.rotation - MathF.PI / 2).ToRotationVector2();
            Vector2 diff = collider.Center - Projectile.Center;
            if (Vector2.Dot(collider.velocity, Normal) > 0 ||
                Vector2.Dot(diff, Normal) < 0)
                return;
            int lenSqr = len + Math.Min(collider.width, collider.height);
            lenSqr = lenSqr * lenSqr;
            if (collider.DistanceSQ(Projectile.Center) > lenSqr)
                return;
            collidedProjectiles.Add(collider);

            //particles
            if (projectile.localAI[1] == 0)
            {
                CollisionParticles(Projectile.Center, diff, collider);
                GenerateLightning();
            }
            Zap();
            projectile.ai[0] += 10 * projFuncs.GetSpeed(projectile);
        }

        public void CollisionParticles(Vector2 pos, Vector2 diff, Entity collider)
        {
            for (int x = 0; x < 10; x++)
            {
                Dust dust = Dust.NewDustDirect(pos + diff * 0.8f, 0, 0, DustID.Electric);
                dust.velocity = diff / 16;
                dust.velocity += Main.rand.NextVector2Circular(2, 2);
            }
        }
        const float damageMod = 0.4f;
        public override void AI()
        {
            Projectile.ai[1]++;
            Projectile.timeLeft = Duration;

            if (!BakedConfig.CustomSpecialPowersEnabled(ItemID.XenoStaff) && Projectile.timeLeft > 30)
            {
                Projectile.timeLeft = 30;
                if (onProjectileCollideHook != null)
                {
                    onProjectileCollideHook.Unhook();
                }
            }
            if (Projectile.localAI[1] > 0)
                Projectile.localAI[1]--;
            if (Projectile.ai[0] > 0)
                Projectile.ai[0]--;
            collidedProjectiles.RemoveAll(i =>
            {
                return !i.active || i.type != Main.projectile[i.whoAmI].type;
            });
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;

                Rectangle rect = new(-len, -len, len * 2, len * 2);
                onProjectileCollideHook = new(Projectile, rect, OnProjectileCollide);
                Projectile.GetGlobalProjectile<ReworkMinion_Projectile>().ArmorIgnoredPerc = 1 - damageMod;
            }
            if (Projectile.timeLeft < 30)
                Projectile.alpha = 255 - 255 * Projectile.timeLeft / 30;
            else if (Projectile.alpha > 0)
                Projectile.alpha -= (int)(1 / 30f * 255);

            if (Projectile.frameCounter++ == 15)
            {
                Projectile.frame += 1;
                Projectile.frame %= 2;
                Projectile.frameCounter = 0;
            }

            if (drawLayer == null)
                drawLayer = new(Projectile);
            Player player = Main.player[Projectile.owner];
            Vector2 diff = Projectile.Center - player.Center;
            Projectile.rotation = diff.ToRotation() + MathF.PI / 2;
            LightningData.Tick();
            LightningData.items.ForEach(i =>
            {
                i.Update();
            });
            if (Main.rand.NextBool(60))
            {

                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric);
                diff.Normalize();
                dust.position -= diff * 16;
                dust.velocity = -diff * 2;
                dust.velocity += Main.rand.NextVector2Circular(1, 1);
            }

            //lightningEffects
            for (int x = 0; x < 3; x++)
            {
                ShieldGeneratorLightningVel[x] += Main.rand.NextVector2Circular(2f, 2f);
                ShieldGeneratorLightningVel[x] *= 0.9f;
                ShieldGeneratorLightningPos[x] += ShieldGeneratorLightningVel[x];
                if (ShieldGeneratorLightningPos[x] == Vector2.Zero)
                    return;

                Vector2 testY = ShieldGeneratorLightningPos[x];
                if (ShieldGeneratorLightningPos[x].Y > 0)
                {
                    testY.Y *= 5;
                }
                float ratio = len / testY.Length();
                if (ratio < 1)
                {
                    ShieldGeneratorLightningPos[x] *= ratio;
                    ShieldGeneratorLightningVel[x] = Vector2.Zero;
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * damageMod);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            LightningData.items.ForEach(i =>
            {
                i.Draw(Projectile);
            });
            lightColor = Color.White;
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            for (int x = 0; x < 3; x++)
            {
                MartianShieldLightning.DrawLightning(Projectile.Center, Projectile.Center + ShieldGeneratorLightningPos[x].RotatedBy(Projectile.rotation));
            }
        }

        bool FillLightningData(Entity newEnt, Tuple<Vector2, int> parentPoint, List<MartianShieldEntityData> entityDataList, List<Tuple<Vector2, int>> nextPoints, List<Entity> filledList, bool isBoss = false)
        {
            int last = entityDataList.Count;
            Entity ent;
            if (parentPoint.Item2 == -1)
                ent = Projectile;
            else
                ent = entityDataList[parentPoint.Item2].Entity;
            entityDataList.Add(new(newEnt, ent, parentPoint.Item2));
            nextPoints.Add(new(newEnt.Center, last));
            filledList.Add(newEnt);
            return true;
        }
        void GenerateLightning()
        {
            Player player = Main.player[Projectile.owner];
            ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
            filledList.Clear();

            List<MartianShieldEntityData> entityDataList = new();
            List<Tuple<Vector2, int>> points = new();
            points.Add(new(Projectile.Center, -1));

            bool dontTerminate = true;
            float chainDist = 16 * 20 * Projectile.GetGlobalProjectile<ReworkMinion_Projectile>().PrefixMinionPower;
            chainDist *= chainDist;
            List<Projectile> martianSaucers = playerFuncs.GetMinionCollection(ItemID.XenoStaff).minions.ToList();
            martianSaucers.RemoveAll(i => i.type != ProjectileID.UFOMinion);
            while (dontTerminate)
            {
                dontTerminate = false;
                List<Tuple<Vector2, int>> nextPoints = new();
                for (int x = 0; x < Main.npc.Length; x++) {
                    NPC test = Main.npc[x];
                    if (!test.CanBeChasedBy(Projectile))
                        continue;

                    points.TrueForAll(i =>
                    {
                        if (filledList.Contains(test) || test.Center.DistanceSQ(i.Item1) > chainDist)
                            return true;

                        dontTerminate = FillLightningData(test, i, entityDataList, nextPoints, filledList, test.boss);
                        return false;
                    });
                }
                martianSaucers.RemoveAll(i =>
                {
                    return !points.TrueForAll(j =>
                    {
                        if (i.Center.DistanceSQ(j.Item1) > chainDist)
                            return true;

                        dontTerminate = FillLightningData(i, j, entityDataList, nextPoints, filledList);
                        return false;
                    });
                });
                points = nextPoints;
            }


            if (Projectile.localAI[1] == 0)
            {
                LightningData.Add(new(entityDataList));
                SoundEngine.PlaySound(SoundID.NPCHit53, Projectile.Center);
                Projectile.localAI[1] = 20;
                drawLayer.ShieldOpacityProgress = 20;
            }
        }

        void Zap()
        {
            bool PlayerIsOwner = Main.myPlayer == Projectile.owner;
            Vector2 originalPos = Projectile.position;
            filledList.RemoveAll(i => !i.active);
            for (int x = 0; x < filledList.Count; x++)
            {
                NPC npc = filledList[x] as NPC;
                if (PlayerIsOwner && npc != null)
                {
                    Projectile.Center = npc.Center;
                    TargetedEnt = npc;
                    Projectile.Damage();
                    continue;
                }
                Projectile saucer = filledList[x] as Projectile;
                if (saucer != null)
                {
                    const int time = 2 * 60;
                    MinionProjectileData saucerFuncs = saucer.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData();
                    saucerFuncs.castingSpecialAbilityTime = time;
                }
            }
            Projectile.position = originalPos;
            TargetedEnt = null;
        }

        public override void Kill(int timeLeft)
        {
            drawLayer.proj = null;
            if (onProjectileCollideHook != null)
                onProjectileCollideHook.Unhook();
        }
    }
}
