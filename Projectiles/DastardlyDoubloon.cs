using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SummonersShine.DataStructures;
using SummonersShine.MinionAI;
using SummonersShine.SpecialData;
using SummonersShine.Textures;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;

using SummonersShine.Projectiles.MiniPirateData;
using Terraria.GameContent;
using System.IO;
using System.Collections;

namespace SummonersShine.Projectiles
{
    public class DastardlyDoubloon : CustomSpecialAbilitySourceMinion
    {
        const float blocksOffset = (90 * 16);
        const float blocksOffsetSqr = blocksOffset * blocksOffset;
        const float turny = 1 - 0.7f / 60;

        const float acceleration = 0.1f;
        const float drag = 0.99f;
        const float vroomTime = 1 / 2f / 60f;
        const float halfPi = MathF.PI / 2;

        const int detectionRad = 160;
        const int refractionRad = 80;
        const float refractionRadSqr = refractionRad * refractionRad;

        public override float minionSlots => 0;
        public Platform platform;
        public int numberStanding {
            get
            {
                return ((int)Projectile.ai[0]) >> 1;
            }
            set
            {
                Projectile.ai[0] = (value << 1) + Projectile.ai[0] % 2;
            }
        }
        int grapplies = 0;

        bool riding
        {
            get {
                return Projectile.ai[0] % 2 == 1;
            }
            set
            {
                int sign = (int)(Projectile.ai[0] % 2);
                Projectile.ai[0] += (value ? 1 : 0) - sign;
            }
        }

        List<MiniPirate> minis = new();

        //breakthrough
        RefractionLattice thisLattice;
        List<Player> playersInRadius = new();
        List<ReworkMinion_Projectile> refractedProjectiles = new();
        int health { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; } //share this in MP
        int timeLeft { get => ProjData.castingSpecialAbilityTime; set => ProjData.castingSpecialAbilityTime = value; }

        PirateStatCollection collection;

        public override MinionSpeedModifier minionSpeedModType => MinionSpeedModifier.stepped;
        public override MinionTracking_State trackingState => MinionTracking_State.noTracking;
        public override float minionTrackingAcceleration => 0;
        public override float minionFlickeringThreshold => 0f;

        public override bool ComparativelySlowPlayer => false;

        ProjectileOnHit onProjectileCollideHook;

        NPC hitscanTarget = null;
        public override bool? CanHitNPC(NPC target)
        {
            return target == hitscanTarget;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (hitscanTarget == null)
                return;
            hitbox.X = (int)hitscanTarget.position.X - 100000;
            hitbox.Y = (int)hitscanTarget.position.Y - 100000;
            hitbox.Width = hitscanTarget.width + 200000;
            hitbox.Height = hitscanTarget.height + 200000;
        }

        //Instantly hurts an NPC

        public int GetNpcHitCD(int hitCD) {
            return (int)(hitCD * Projectile.localNPCHitCooldown / (float)100);
        }
        public void HitscanNPC(NPC npc, int npcCd, float damageMod) {
            int oldnpcCD = Projectile.localNPCHitCooldown;
            npcCd = (int)(npcCd * oldnpcCD / (float)100);
            Projectile.localNPCHitCooldown = npcCd;
            int oldDamage = Projectile.damage;
            Projectile.damage = (int)(oldDamage * damageMod);
            ProjFuncs.ArmorIgnoredPerc = (1 - damageMod);
            hitscanTarget = npc;
            Projectile.Damage();
            hitscanTarget = null;
            Projectile.localNPCHitCooldown = oldnpcCD;
            Projectile.damage = oldDamage;
            ProjFuncs.ArmorIgnoredPerc = 0;
        }

        public override void onPostCreation(Projectile projectile, Player player)
        {

            platform = new Platform_AttachedToProj(projectile.position + new Vector2(2, 44), 116, Projectile);
            platform.OnEntityAdded = OnEntityAdded;
            platform.OnEntityRemoved = OnEntityRemoved;
            platform.OnNetUpdate = OnNetUpdate;

            MiniPirate queen = new(this);
            minis.Add(queen);

            ReworkMinion_Player ownerData = Main.player[projectile.owner].GetModPlayer<ReworkMinion_Player>();
            collection = ownerData.GetSpecialData<PirateStatCollection>();
            collection.kickPirateWrapper = new MiniPirate.MiniPirateEventWrapper_KickPirate(queen);

            Rectangle rect = new(-detectionRad, -detectionRad, detectionRad * 2, detectionRad * 2);
            onProjectileCollideHook = new(Projectile, rect, OnProjectileCollide);
        }

        public void OnProjectileCollide(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Projectile collider)
        {
            if (timeLeft == -1)
                return;
            refractedProjectiles.RemoveAll(i => i.killed);
            playersInRadius.ForEach(i => i.GetModPlayer<ReworkMinion_Player>().projectilesToEvade.Add(collider));
            ReworkMinion_Projectile colliderFuncs = collider.GetGlobalProjectile<ReworkMinion_Projectile>();
            if (!refractedProjectiles.Contains(colliderFuncs) && (projectile.Center - collider.Center).LengthSquared() < refractionRadSqr)
            {
                Player player = Main.player[projectile.owner];
                ArmorShaderData shaderData = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                refractedProjectiles.Add(colliderFuncs);
                Vector2 diff = collider.Center - projectile.Center;
                Vector2 pos = projectile.Center + diff * 0.75f;
                for (int x = 0; x < 30; x++)
                {
                    Dust dust = Dust.NewDustDirect(pos, 2, 2, DustID.GoldFlame);
                    dust.velocity = Main.rand.NextVector2Circular(2, 2) + diff * 0.01f;
                    dust.shader = shaderData;
                    dust.scale = Main.rand.NextFloat(1, 1.5f);
                }

                int damage = Projectile.GetDamageTaken(collider.damage);
                CombatText.NewText(new Rectangle((int)pos.X, (int)pos.Y, 0, 0), CombatText.OthersDamagedHostile, damage, false, false);
                health -= damage;
                if (health <= 0)
                {
                    DestroyShield(player);
                }
            }
        }

        public override void minionSummonEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            //projFuncs.GetMinionProjData().alphaOverride = 50;
        }

        public override void minionDespawnEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            ReworkMinion_Player ownerData = Main.player[projectile.owner].GetModPlayer<ReworkMinion_Player>();
            PirateStatCollection collection = ownerData.GetSpecialData<PirateStatCollection>();
            collection.KillMegaMinion();
            if (platform != null)
                platform.Destroy();
            if (onProjectileCollideHook != null)
                onProjectileCollideHook.Unhook();
        }

        public void Deactivate() {
            if (ProjFuncs.killedTicks == 0)
            {
                for (int x = 0; x < 30; x++)
                {
                    Vector2 position = Projectile.position + new Vector2(Main.rand.NextFloat(Projectile.width), Main.rand.NextFloat(Projectile.height));
                    Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), position, Vector2.Zero, GoreID.FogMachineCloud1 + Main.rand.Next(0, 3));
                    gore.position = position;
                    gore.velocity /= 10;
                }
                Projectile.timeLeft = Projectile.GetFadeTime(ProjData);
                if (ProjData.alphaOverride == 0)
                    ProjData.alphaOverride = 1;
                platform.Destroy();
                ProjFuncs.killedTicks = 1;
                onProjectileCollideHook.Unhook();
            }
        }

        public bool Reactivate()
        {
            if (ProjFuncs.killedTicks > 0)
            {
                onProjectileCollideHook.Rehook();
                platform = new Platform_AttachedToProj(Projectile.position + new Vector2(2, 44), 116, Projectile); //increase height later on
                platform.OnEntityAdded = OnEntityAdded;
                platform.OnEntityRemoved = OnEntityRemoved;
                ProjFuncs.killedTicks = 0;
                return true;
            }
            return false;
        }

        public override void minionPostAI(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {

            float simRate = projFuncs.GetInternalSimRate(Projectile);
            //update minis (placed here because of killedTicks)
            bool killed = projFuncs.killedTicks > 0;
            updateMinis(killed);

            DefaultMinionAI.HandleFadeInOut(projectile, projFuncs, projData, false, true);
        }

        void updateMinis(bool killed) {

            float simRate = ProjFuncs.GetInternalSimRate(Projectile);
            minis.ForEach(i =>
            {
                i.Update(simRate, killed);
            });
        }

        public override void SetDefaults()
        {
            minis = new();
            playersInRadius = new();
            refractedProjectiles = new();

            Projectile.width = 120;
            Projectile.height = 64;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 100;
            base.SetDefaults();
        }

        public override void minionOnCreation(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player)
        {
            ReworkMinion_Player ownerData = Main.player[Projectile.owner].GetModPlayer<ReworkMinion_Player>();
            PirateStatCollection collection = ownerData.GetSpecialData<PirateStatCollection>();
            collection.ForceInsertMegaMinion(Projectile);
        }

        public override void SetStaticDefaults()
        {
            ProjectileModIDs.DastardlyDoubloon = Projectile.type;
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.CanDistortWater[Projectile.type] = true;
        }

        /*int RaytraceBlocks(int dir, int count, Vector2 pos) {

            for (int i = 0; i < count; i++)
            {
                float nextTileOffset = (float)(16 * i);
                Point point = (pos + new Vector2(0, nextTileOffset * dir)).ToTileCoordinates();
                if (point.X < 0 || point.Y < 0 || point.X >= Main.maxTilesX || point.Y >= Main.maxTilesY)
                    return i - 1;
                bool flag4 = WorldGen.SolidOrSlopedTile(point.X, point.Y);
                if (flag4)
                {
                    return i;
                }
            }
            return -1;
        }*/

        public void RechargeShield(int health) {
            this.health += health;
            timeLeft = 300;
            thisLattice = new(Projectile, Main.player[Projectile.owner]);
        }

        void DestroyShield(Player player) {
            thisLattice.PopLattice(Projectile, player);
            thisLattice = null;
            timeLeft = -1;
            health = 0;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            float simRate = ProjFuncs.GetInternalSimRate(Projectile);

            Player player = Main.player[Projectile.owner];
            Projectile.timeLeft = 2;

            collection.CanFindEnemyTarget = minis[0].AnyTargets() != -1;

            //update refraction lattice
            if (timeLeft > 0)
            {
                if (thisLattice == null)
                    thisLattice = new(Projectile, Main.player[Projectile.owner]);
                thisLattice.Update(simRate);
            }
            //movement independent of speed mods
            if (Projectile.IsOnRealTick(ProjData))
            {
                //count down special
                if (timeLeft > 0)
                {
                    timeLeft -= 1;
                    if (timeLeft <= 0)
                    {
                        DestroyShield(player);
                    }
                }

                Vector2 target = player.Bottom;
                float offset = 1 - Math.Clamp(-player.GetRealPlayerVelocity().Y / 20, 0, 1);

                //hover 16 blocks above ground if possible

                int xPos = (int)(target.X / 16);
                int yPos = (int)(target.Y / 16) - 1;
                List<Tuple<int, int>> def = new();
                Collision.TupleHitLine(xPos, yPos, xPos, yPos - 32, 0, 0, def, out Tuple<int, int> topResult);
                Collision.TupleHitLine(xPos, yPos, xPos, yPos + 32, 0, 0, def, out Tuple<int, int> bottomResult);
                target.Y = (topResult.Item2 + bottomResult.Item2) * 8 + 16;

                /*int distToGround = RaytraceBlocks(-1, 16, target);
                if (distToGround != -1)
                    target += new Vector2(0, 16 * (distToGround - 1));

                int distToCeil = RaytraceBlocks(-1, 16, target);
                if (distToCeil != -1)
                {
                    distToCeil /= 2;
                    target += new Vector2(0, -16 * (distToCeil));
                }
                else if (distToGround != -1)
                    target += new Vector2(0, -256);
                else
                    target += new Vector2(0, -196 * offset);*/


                MoveTowards(target);
                if (grapplies + numberStanding > 0) {
                    Projectile.velocity.Y += 0.06f;
                }
                Projectile.netUpdate = true;

                Vector2 diff = Projectile.Center - player.Center;
                float diffLenSqr = diff.LengthSquared();
                if (diffLenSqr > 2560000) // 100 blocks
                {
                    Projectile.Center = player.Top;
                    platform.RemoveAttachedEntity(player);
                }

                //grapple draggies

                Vector2 grappleVel = Vector2.Zero;
                grapplies = 0;
                platform.grapples.ForEach(i =>
                {
                    Vector2 dist = (i.position - Main.player[i.owner].position);
                    grappleVel += dist;

                    if (Main.rand.Next(5) == 0)
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            Dust dust = Dust.NewDustDirect(i.position + Main.rand.NextVector2Circular(5, 5), 0, 0, DustID.MartianSaucerSpark);
                            dust.velocity.Y /= 8;
                            dust.velocity += Projectile.velocity + new Vector2(0, -0.5f);
                        }
                        for (int x = 0; x < 1; x++)
                        {
                            Dust dust = Dust.NewDustDirect(i.position + Main.rand.NextVector2Circular(5, 5), 0, 0, DustID.Smoke, newColor: Color.LightGray);
                            dust.velocity.Y /= 8;
                            dust.velocity += Projectile.velocity + new Vector2(0, -0.5f);
                            dust.noGravity = true;
                            dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                        }
                    }
                    if (dist.LengthSquared() < 1024) {
                        grapplies++;
                    }
                });
                if (grappleVel != Vector2.Zero)
                {
                    float grappleSpeed = grappleVel.Length();
                    Projectile.velocity += grappleVel * (Math.Min(grappleSpeed, 0.01f) / grappleSpeed);
                }
            }

            //animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 10 == 0)
            {
                int frame = Projectile.frameCounter / 10;
                if (frame > 3)
                {
                    frame = 0;
                    Projectile.frameCounter = 0;
                }
                if (frame > 2)
                    frame = 1;
                Projectile.frame = frame;
            }

            //effects
            float randRot = Main.rand.NextFloat(-0.5f, 1.57079f);
            if(Main.rand.Next(0, 5) == 0)
            {
                Vector2 dustPos = Projectile.Center + new Vector2(-10 * Projectile.spriteDirection, 20);
                Dust dust = Dust.NewDustDirect(dustPos, 0, 0, DustID.GoldFlame, 0f, 0f, 0, default(Color), 1f);
                Vector2 offset = new Vector2(40f * Projectile.spriteDirection, 30f);
                Vector2 rotationSin = Vector2.UnitY.RotatedBy((double)(randRot), default(Vector2));
                rotationSin *= 0.8f;
                dust.position = dustPos + rotationSin * offset;
                dust.velocity = rotationSin + Projectile.velocity;
                if (Main.rand.Next(2) == 0)
                {
                    dust.velocity *= 0.5f;
                }
                dust.noGravity = true;
                dust.scale = 1.5f + Main.rand.NextFloat() * 0.8f;
                dust.fadeIn = 0f;
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
        }

        public override bool ShouldUpdatePosition()
        {
            return ProjData.currentTick == 1 && Projectile.numUpdates < 0;
        }

        public override void minionTerminateSpecialAbility(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, Player player, ReworkMinion_Player playerFuncs)
        {
            Deactivate();
        }
        public override void minionOnMovement(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData)
        {
            //movement independent of speed mods
            if (projData.currentTick == 1 && Projectile.numUpdates < 0)
            {
                platform.Move(Projectile.position + new Vector2(2, 44) - platform.pos);

                riding = riding || ((grapplies + numberStanding) != 0 && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height));
                Projectile.tileCollide = riding;
            }

            playersInRadius.Clear();
            for (int i = 0; i < Main.maxPlayers; i++) {
                if ((Main.player[i].Center - Projectile.Center).LengthSquared() <= refractionRadSqr)
                    playersInRadius.Add(Main.player[i]);
            }
        }

        void MoveTowards(Vector2 pos) {
            bool useRelativeVel = platform.grapples.Count == 0;

            Player player = Main.player[Projectile.owner];
            Vector2 dir = pos - Projectile.Center;
            Vector2 playerVel = player.GetRealPlayerVelocity();// * projFuncs.GetSpeed(Projectile);
            Vector2 rel = playerVel;
            if (!useRelativeVel)
                rel = Vector2.Zero;

            Vector2 relativeVel = Projectile.velocity - rel;
            //make shape oval
            relativeVel.Y *= 2;
            Vector2 relativeDir = dir - rel;
            float dirMagnitude = relativeDir.Length();


            if (dirMagnitude > 16)
            {
                float newAccel = 0.1f;
                float dot = Vector2.Dot(relativeDir, relativeVel);
                if (dot <= 0) {
                    float dist = Math.Clamp((Projectile.Center - player.Center).LengthSquared() / blocksOffsetSqr, 0f, 1f);
                    Vector2 usefulVel = relativeVel * dot / relativeDir.LengthSquared();
                    relativeVel -= usefulVel;
                    relativeVel *= (1 - dist) * turny;
                    relativeVel += usefulVel;
                }
                relativeDir *= newAccel / dirMagnitude;
                relativeVel += relativeDir;
            }

            relativeVel *= 0.99f;
            //make shape oval
            relativeVel.Y *= 0.5f;
            relativeVel += rel;
            Projectile.velocity = relativeVel;

            int desiredSpriteDirection = (dir.X + ProjData.lastRelativeVelocity.X) < 0 ? -1 : 1;
            if(Projectile.velocity.X * desiredSpriteDirection > 0.5)
                Projectile.spriteDirection = desiredSpriteDirection;
        }

        public void OnEntityAdded(Entity entity, bool fromServer = false)
        {
            Player player = Main.player[Projectile.owner];
            numberStanding++;
            if(!fromServer)
                Projectile.velocity.Y = 2.5f;
            for (int x = 0; x < 20; x++)
            {
                Dust dust = Dust.NewDustDirect(entity.Bottom, 0, 0, DustID.WoodFurniture);
                dust.velocity /= 4;
                dust.velocity += Projectile.velocity + new Vector2(0, -1.5f);
                dust.position.X += Main.rand.NextFloat(-16, 16);
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            }
            for (int x = 0; x < 20; x++)
            {
                Dust dust = Dust.NewDustDirect(entity.Bottom, 0, 0, DustID.Smoke, 0, 0, DustID.Smoke, newColor: Color.LightGray);
                dust.velocity /= 4;
                dust.velocity += Projectile.velocity + new Vector2(0, Main.rand.NextFloat(-0.5f, 0));
                dust.position.X += Main.rand.NextFloat(-16, 16);
                dust.noGravity = true;
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            }
            if (entity != player)
                return;
        }
        public void OnEntityRemoved(Entity entity, bool fromServer = false)
        {
            Projectile.netUpdate = true;
            Player player = Main.player[Projectile.owner];
            numberStanding--;
            if (numberStanding == 0)
                riding = false;
            if (!fromServer)
            {
                float velMult = entity as Projectile != null ? 0.25f : 1f;
                if (entity.velocity.Y < 0)
                    Projectile.velocity.Y = MathF.Max(Projectile.velocity.Y, 5f * velMult);
                Projectile.velocity.X -= Math.Sign(entity.velocity.X) * Math.Min(10, MathF.Abs(entity.velocity.X) * 2) * velMult;
            }
            float posX = Projectile.position.X;
            float posY = Projectile.position.Y + 44;
            for (int x = 0; x < 20; x++)
            {
                Dust dust = Dust.NewDustDirect(new Vector2(Math.Clamp(entity.position.X, posX + 18, posX + 112), posY), 0, 0, DustID.WoodFurniture);
                dust.velocity /= 4;
                dust.velocity += Projectile.velocity + new Vector2(0, -1.5f);
                dust.position.X += Main.rand.NextFloat(-16, 16);
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            }
            for (int x = 0; x < 20; x++)
            {
                Dust dust = Dust.NewDustDirect(new Vector2(Math.Clamp(entity.position.X, posX + 18, posX + 112), posY), 0, 0, DustID.Smoke, newColor: Color.LightGray);
                dust.velocity /= 4;
                dust.velocity += Projectile.velocity + new Vector2(0, Main.rand.NextFloat(-1.5f, 0));
                dust.position.X += Main.rand.NextFloat(-16, 16);
                dust.noGravity = true;
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
            }
            if (entity != player)
                return;
        }

        public void OnNetUpdate()
        {
            Projectile.netUpdate = true;
        }
        public override void PostDraw(Color lightColor)
        {
            minis.ForEach(i =>
            {
                i.Draw(lightColor);
            });
            if(thisLattice != null)
                thisLattice.DrawLattice(lightColor, Projectile);
        }

        class RefractionLattice
        {
            List<RefractionLatticePoint> list = new();

            public RefractionLattice(Projectile proj, Player player)
            {
                for (int x = 0; x < 90; x++)
                    list.Add(new(1f, 0.05f, 0.99f));
                for (int x = 0; x < 5; x++)
                    list.Add(new(2f, 0.2f));
                PopLattice(proj, player);
            }
            public void Update(float simRate)
            {
                list.ForEach(i => i.Update(simRate));
            }
            public void DrawLattice(Color lightColor, Projectile proj)
            {
                Color rvColor = Color.White;
                rvColor *= (255 - proj.alpha) * 0.00392157f;
                Texture2D sprites = TextureAssets.Dust.Value;
                Vector2 origin = new Vector2(4, 4);
                SpriteEffects effect = SpriteEffects.None;

                Rectangle rectangle = new Rectangle();
                rectangle.Width = 8;
                rectangle.Height = 8;

                list.ForEach(i => {
                    Color renderColor = rvColor * i.alpha * i.color;
                    Vector2 pos = proj.Center + i.GetRelativePos();

                    rectangle.X = 10 * DustID.GoldFlame - 2000;
                    rectangle.Y = 60 + 10 * Main.rand.Next(3);

                    Vector2 dustPos = pos - Main.screenPosition;

                    Main.EntitySpriteDraw(sprites, dustPos, new Rectangle?(rectangle), renderColor, i.rot, origin, i.scale, effect, 0);
                });
            }
            public void PopLattice(Projectile proj, Player player) {
                list.ForEach(i => {
                    Vector2 diff = i.GetRelativePos();
                    Vector2 pos = proj.Center + diff;
                    Dust dust = Dust.NewDustDirect(pos, 2, 2, DustID.GoldFlame);
                    dust.velocity = Main.rand.NextVector2Circular(2, 2) + diff * 0.01f;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                    dust.scale = i.scale;
                    dust.rotation = i.rot;
                });
            }
        }

        class RefractionLatticePoint {
            const float halfPi = MathF.PI / 2;

            float x;
            float y;
            float yMax;

            public float scale = 1;
            public float rot = 1;

            float speed;
            float moveSpeed;
            float maxScale;

            float time = 0;
            public float alpha;

            public float color = 1;

            Vector2 vel = Vector2.Zero;
            float dir = 0;

            public Vector2 GetRelativePos() {
                return new Vector2(MathF.Sin(y / yMax) * refractionRad, 0).RotatedBy(x);
            }

            void Move(Vector2 dir) {
                y += dir.Y;
                if (y > yMax)
                    y = yMax;
                if (y > 0)
                    x += dir.X * yMax / y * halfPi;
                else
                    y = 0;
                x %= MathF.PI * 2;
            }

            public void Update(float simRate) {
                time += simRate;
                scale = (0.5f * MathF.Sin(time * 0.1f * speed) + 0.5f) * maxScale;
                rot = MathF.Sin(time * 0.1f * speed);
                color = 1 + MathF.Sin(time * 0.1f * speed);
                vel += Vector2.UnitX.RotatedBy(dir) * 0.01f * moveSpeed * simRate;
                dir += Main.rand.NextFloat(-0.1f, 0.1f);
                vel *= 0.9f;
                Move(vel);
            }

            public RefractionLatticePoint(float scale, float moveSpeed, float alpha = 1)
            {
                yMax = halfPi * refractionRad;
                y = yMax;
                x = Main.rand.NextFloat(MathF.PI * 2);
                dir = Main.rand.NextFloat(0, MathF.PI * 2);
                speed = Main.rand.NextFloat(0.5f, 1.5f);
                this.moveSpeed = Main.rand.NextFloat(0.5f, 1.5f) * moveSpeed;
                maxScale = Main.rand.NextFloat(0.75f, 1.25f) * scale;
                time = Main.rand.NextFloat(MathF.PI * 200);
                this.alpha = alpha;
            }
        }
    }
}
