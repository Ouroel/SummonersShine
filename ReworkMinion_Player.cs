using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ModLoader;

using SummonersShine.SpecialData;
using Terraria.DataStructures;
using Terraria.Graphics.Capture;
using Terraria.ID;
using SummonersShine.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Creative;
using SummonersShine.SpecialAbilities.WhipSpecialAbility;
using SummonersShine.BakedConfigs;

namespace SummonersShine
{
    public class MinionEnergyCounter
    {
        public List<Projectile> minions = new();
        public int summonItemID;
        public ReworkMinion_Player _player;
        public List<float> minionFullPercentage = new();

        public MinionEnergyCounter(int summonItemID, ReworkMinion_Player player) {
            this.summonItemID = summonItemID;
            _player = player;
        }

        public void Update(Player player) {
            minionFullPercentage.Clear();
            int fullMinions = 0;
            int sendState = -1;
            minions.RemoveAll(i => Main.projectile[i.identity] != i);
            minions.ForEach(i => {
                ReworkMinion_Projectile projFuncs = i.GetGlobalProjectile<ReworkMinion_Projectile>();
                MinionProjectileData projData = projFuncs.GetMinionProjData();
                int SourceItem = projFuncs.SourceItem;
                if (SourceItem != -1 && ReworkMinion_Item.defaultSpecialAbilityUsed[SourceItem] == null && projData.maxEnergy > 0)
                {
                    bool wasNotFull = projData.energy < projData.maxEnergy;
                    projData.energy += projData.energyRegenRate * projData.energyRegenRateMult;
                    if (projData.energy >= projData.maxEnergy)
                    {
                        projData.energy = projData.maxEnergy;
                        if (wasNotFull)
                            sendState = 0;
                        fullMinions += 1;
                    }
                    float bracket = Math.Clamp(projData.energy / (float)projData.maxEnergy, 0, 1);
                    minionFullPercentage.Add(bracket);
                }
                else
                    fullMinions++;
            });
            minionFullPercentage.Sort();
            if (fullMinions == minions.Count && sendState == 0) {
                sendState = 1;
            }
            if (sendState != -1) {
                _player.AddNewThought(summonItemID, minions.Count > 1, sendState == 1);
            }
        }
    }

    public class minionWrapper
    {
        public Projectile projectile;
        public int minionCounter;
        public minionWrapper(Projectile projectile, int minionCounter)
        {
            this.projectile = projectile;
            this.minionCounter = minionCounter;
        }

        public static bool Compare(minionWrapper a, minionWrapper b) { return a.minionCounter < b.minionCounter; }
    }
    public delegate void OnMovementDel(Player player, Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, ref float gravMod, ref float velMod);

    public class PlayerMovementMod {
        public Projectile Projectile {
            get => projectile;
            set {
                projectile = value;
                projFuncs = value.GetGlobalProjectile<ReworkMinion_Projectile>();
                projData = projFuncs.GetMinionProjData();
            }
        }
        Projectile projectile;
        ReworkMinion_Projectile projFuncs;
        MinionProjectileData projData;
        public OnMovementDel OnMovement;

        public void MovementHook(Player player, ref float gravMod, ref float velMod)
        {
            OnMovement(player, projectile, projFuncs, projData, ref gravMod, ref velMod);
        }

        public PlayerMovementMod(Projectile Projectile, OnMovementDel OnMovement) {
            if (Projectile != null)
                this.Projectile = Projectile;
            this.OnMovement = OnMovement;
        }
    }
    public class ReworkMinion_PlayerDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HandOnAcc);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            ReworkMinion_Player player = drawInfo.drawPlayer.GetModPlayer<ReworkMinion_Player>();

            PlayerDrawSet rvDrawInfo = drawInfo;
            player.specialProjDrawInFrontEffects.ForEach(i =>
            {
                i.Item4(i.Item1, i.Item2, i.Item3, ref rvDrawInfo);
            });
            drawInfo = rvDrawInfo;
        }
    }

    public class ReworkMinion_Player : ModPlayer
    {
        int minionCounter = int.MaxValue - 1;
        public float minionPower = 1;
        public float energyRestoreRate = 1;
        public float minionAS = 1;
        public float minionASRetreating = 1;
        public float minionASIgnoreMainWeapon = 1;

        public WhipSpecialAbility lastWhipSpecial;

        public EnergyDisplay energyDisplay;

        //sync with player

        public Vector2 CursorPos { 
            get {
                if (Player.whoAmI == Main.myPlayer)
                {
                    return Main.MouseWorld;
                }
                return cursorPos;
            }
        }
        Vector2 cursorPos;

        public bool net_initialized = false;

        public int net_platformID = -1;
        public Platform platform = null;
        public Vector2 platformRelPos = Vector2.Zero;

        public bool UpdateFallStart = false;
        public bool IsStanding = false;
        public bool ShouldMakeIgnoreWater = false;

        public List<MinionEnergyCounter> minionCollections;
        List<SpecialDataBase> specialData;

        BinaryHeap<minionWrapper> minionHeap;

        public List<Projectile> projectilesToEvade;

        public List<PlayerMovementMod> movementMods;

        public float lifeSteal = 0;
        public bool slimeSlide = false;

        public short attackedThisFrame = 0;
        public NPC lastAttackedTarget = null;

        public List<Projectile> Minion_OnHitNPCWithProj;

        public bool playerSaved { get; private set; }
        Vector2 playerPos;
        Vector2 playerVel;
        int playerLastFallStart;
        bool playerJustJumped;
        int playerRocketDelay2;

        public float[] LeftoverProjectileImmunity;

        public List<Tuple<Projectile, ReworkMinion_Projectile, MinionProjectileData, specialProjDrawInFrontEffect>> specialProjDrawInFrontEffects;
        public delegate void specialProjDrawInFrontEffect(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, ref PlayerDrawSet drawInfo);

        //blood matzo

        public Items.BloodMatzo.BloodMatzo_Base currentBloodMatzo = null;

        public bool HasMagenChiunEquipped = false;
        public override ModPlayer NewInstance(Player entity)
        {
            ReworkMinion_Player rv = (ReworkMinion_Player)base.NewInstance(entity);
            rv.lastWhipSpecial = new WSA_Null();
            rv.minionCollections = new();
            rv.specialData = new();
            rv.minionHeap = new(minionWrapper.Compare);
            rv.projectilesToEvade = new();
            rv.movementMods = new();
            rv.Minion_OnHitNPCWithProj = new();
            rv.specialProjDrawInFrontEffects = new();
            rv.LeftoverProjectileImmunity = new float[ProjectileLoader.ProjectileCount];
            return rv;
        }

        public void PreSaveAndQuit() //temp garbage collection
        {
            lastWhipSpecial = null;
            minionCollections = null;
            specialData = null;
            minionHeap = null;
            projectilesToEvade = null;
            movementMods = null;
            Minion_OnHitNPCWithProj = null;
            specialProjDrawInFrontEffects = null;
            LeftoverProjectileImmunity = null;
        }
        public override void Initialize()
        {
            lastWhipSpecial = new WSA_Null();
            minionCollections = new();
            specialData = new();
            minionHeap = new(minionWrapper.Compare);
            projectilesToEvade = new();
            movementMods = new();
            Minion_OnHitNPCWithProj = new();
            specialProjDrawInFrontEffects = new();
            LeftoverProjectileImmunity = new float[ProjectileLoader.ProjectileCount];
        }

        public void SavePlayerData(Player player)
        {
            playerSaved = true;
            playerPos = player.position;
            playerVel = player.velocity;
            playerLastFallStart = player.fallStart;
            playerJustJumped = player.justJumped;
            playerRocketDelay2 = player.rocketDelay2;
        }

        public void LoadPlayerData(Player player)
        {
            if (!playerSaved)
                return;
            playerSaved = false;
            player.position = playerPos;
            player.velocity = playerVel;
            player.fallStart = playerLastFallStart;
            player.justJumped = playerJustJumped;
            player.rocketDelay2 = playerRocketDelay2;
        }
        public bool AddMovementMod(Projectile projectile, OnMovementDel OnMovement)
        {
            if (!movementMods.Exists(i => i.Projectile == projectile && i.OnMovement == OnMovement))
                movementMods.Add(new PlayerMovementMod(projectile, OnMovement));
            else
                return false;
            return true;
        }
        public void RemoveMovementMod(Projectile projectile, OnMovementDel OnMovement)
        {
            movementMods.RemoveAll(i => i.Projectile == projectile && i.OnMovement == OnMovement);
        }

        public T GetSpecialData<T>() where T : SpecialDataBase, new()
        {
            T rv = specialData.Find(i => i.GetType() == typeof(T)) as T;
            if (rv == null)
            {
                rv = new T();
                specialData.Add(rv);
            }
            return rv;
        }
        public class ReworkMinion_Player_Loader : ILoadable
        {
            public void Load(Mod mod)
            {
            }

            public void Unload()
            {
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (energyDisplay != null)
            {
                EnergyDisplay.AllDisplays.Remove(energyDisplay);
                energyDisplay = null;
            }

            //shifts all buffs to the left
            int shiftLeft = 0;
            for(int x = 0; x < Player.MaxBuffs; x++)
            {
                //shift left
                if (!Main.persistentBuff[Player.buffType[x]])
                {
                    shiftLeft++;
                    continue;
                }
                else if (shiftLeft > 0)
                {
                    Player.buffTime[x - shiftLeft] = Player.buffTime[x];
                    Player.buffType[x - shiftLeft] = Player.buffType[x];
                    Player.buffTime[x] = 0;
                    Player.buffType[x] = 0;
                }
            }
        }
        public override void PostUpdateEquips()
        {
            if (ShouldMakeIgnoreWater)
            {
                ShouldMakeIgnoreWater = false;
                Player.ignoreWater = true;
            }
            if (slimeSlide && Player.spikedBoots == 0)
                Player.spikedBoots++;
            slimeSlide = false;
        }

        public override void ModifyStartingInventory(IReadOnlyDictionary<string, List<Item>> itemsByMod, bool mediumCoreDeath)
        {
            if (Config.current.IsBloodtaintMode())
            {
                List<Item> items;
                bool found = itemsByMod.TryGetValue("Terraria", out items);
                if (found)
                {
                    items.RemoveAll(i => i.type == ItemID.CopperShortsword);
                    items.RemoveAll(i => i.type == ItemID.IronBroadsword);
                    items.Insert(0, new Item(ItemID.VampireFrogStaff));
                    items.Insert(0, new Item(ItemID.BlandWhip));
                    items.Add(new Item(ItemID.CrimsonTorch, 20));
                }
            }
        }
        public override void PostUpdateRunSpeeds()
        {
            float gravMult = 1;
            float velMult = 1;
            movementMods.ForEach(i => i.MovementHook(Player, ref gravMult, ref velMult));
            Player.gravity *= gravMult;
            Player.maxFallSpeed *= gravMult;
            Player.maxRunSpeed *= velMult;
            Player.moveSpeed *= velMult;
            Player.accRunSpeed *= velMult;
            Player.runAcceleration *= velMult;
            Player.runSlowdown *= velMult;
        }
        public override void clientClone(ModPlayer clientClone)
        {
            ReworkMinion_Player clone = clientClone as ReworkMinion_Player;
            clone.platform = platform;
            if (platform != null)
            {
                clone.platformRelPos = platformRelPos;
            }
            clone.lastWhipSpecial = lastWhipSpecial;
        }
        public override void SendClientChanges(ModPlayer clientPlayer)
        {

            ReworkMinion_Player clone = clientPlayer as ReworkMinion_Player;
            if (clone.platform != platform || clone.platformRelPos != platformRelPos) {
                PacketHandler.WritePacket_UpdatePlayerPosRelToPlatform(Player.whoAmI, platform, platformRelPos);
            }
            if (lastWhipSpecial.requireNetUpdate || lastWhipSpecial != clone.lastWhipSpecial)
            {
                PacketHandler.WritePacket_UpdatePlayerWhipSpecial(Player.whoAmI, lastWhipSpecial);
                lastWhipSpecial.requireNetUpdate = false;
            }
            if (Player.HeldItem.ModItem as Items.BloodMatzo.BloodMatzo_Base != null) {
                PacketHandler.WritePacket_SyncCursor(Player.whoAmI, Main.MouseWorld, Player.whoAmI);
            }
        }
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            if (Main.dedServ)
                return;
            PacketHandler.WritePacket_SyncPlayerRequest(Main.myPlayer);
        }

        public void SyncPlayerRequest(int playerDesired) {

            PacketHandler.WritePacket_UpdatePlayerPosRelToPlatform(Player.whoAmI, platform, platformRelPos);
            PacketHandler.WritePacket_UpdatePlayerWhipSpecial(Player.whoAmI, lastWhipSpecial);
            PacketHandler.WritePacket_SyncCursor(Player.whoAmI, Main.MouseWorld);
        }
        public override void PreUpdateMovement()
        {
            if (Player.IsMainPlayer())
            {
                Collision.TileCollision(Player.position, Player.velocity, Player.width, Player.height, Player.controlDown, Player.controlDown, (int)Player.gravDir);
                IsStanding = (Collision.down && Player.gravDir == 1) || (Collision.up && Player.gravDir == -1);
                //platform detection
                if (Player.CanStandOnPlatform())
                {
                    Player.velocity = Player.GetRealPlayerVelocity();
                    Tuple<bool, Vector2> result = PlatformCollection.TestPlatformCollision(Player);
                    if (result.Item1)
                    {
                        Player.velocity = result.Item2;
                        UpdateFallStart = true;
                        platformRelPos = Player.position - platform.pos + Player.velocity;
                    }
                    else
                        platformRelPos = Vector2.Zero;
                    Player.ConvertRealVelocityBack();
                }
                else
                {
                    PlatformCollection.UnattachAllPlatforms(Player);
                }
            }
            else
            {
                if (net_platformID != -1) {
                    //give the platform-detector a bit of wiggle room
                    platform = PlatformCollection.FindPlatform(net_platformID);
                    if (platform != null)
                    {
                        net_platformID = -1;
                        PlatformCollection.Net_ForcePlatform(Player, platform);
                    }
                }
                if (platform != null)
                {
                    Player.position = platform.pos + platformRelPos;
                    UpdateFallStart = true;
                    Player.velocity.Y = 0;
                }
            }
        }
        public override void PostUpdate()
        {
            if (attackedThisFrame > 0)
                attackedThisFrame--;
            List<Projectile> projectilesToEvade_ToRetain = new();
            projectilesToEvade.ForEach(i=> { if (i.active) projectilesToEvade_ToRetain.Add(i); });
            projectilesToEvade = projectilesToEvade_ToRetain;
            if (UpdateFallStart)
            {
                Player.fallStart = (int)(Player.position.Y / 16f);
                Player.releaseJump = true;
                Player.jump = 0;
                Player.wingTime = Player.wingTimeMax;
                Player.justJumped = false;
                Player.rocketDelay2 = 0;
                Player.mount.ResetFlightTime(Player.velocity.X);
                UpdateFallStart = false;
            }
            minionCollections.ForEach(i => i.Update(Player));
            if (energyDisplay != null && energyDisplay.Update())
                energyDisplay = null;

            lastWhipSpecial = lastWhipSpecial.Update();

            if (Player.IsMainPlayer()) {
                currentBloodMatzo = null;

                //constantly tell everyone to transfer data until something gets through. HandlePacket appears to not work on the same tick SyncPlayer is first called on joining player.
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    if (Main.PlayerList.Count <= 1)
                    {
                        net_initialized = true;
                    }
                    if (!net_initialized)
                        SyncPlayer(-1, -1, true);
                }
            }
        }
        public override void ResetEffects()
        {
            HasMagenChiunEquipped = false;
            if (Config.current.IsBloodtaintMode())
                Player.AddBuff(ModContent.BuffType<Buffs.Bloodtaint>(), 2);
            minionAS = 1;
            minionASRetreating = 1;
            minionASIgnoreMainWeapon = 1;
        }

        public override bool CanBeHitByProjectile(Projectile proj)
        {
            if (projectilesToEvade.Remove(proj))
                return false;
            return true;
        }
        public MinionEnergyCounter TryGetMinionCollection(int summonItemID)
        {
            if (minionCollections == null)
                return null;
            return GetMinionCollection(summonItemID);
        }
        public MinionEnergyCounter GetMinionCollection(int summonItemID)
        {
            MinionEnergyCounter rv = minionCollections.Find(i => i.summonItemID == summonItemID);
            if (rv == null) {
                rv = new MinionEnergyCounter(summonItemID, this);
                minionCollections.Add(rv);
            }
            return rv;
        }

        public void AddMinion(Projectile minion)
        {
            minionCounter++;
            if (minionCounter == int.MaxValue) {
                minionCounter = 0;
                List<minionWrapper> minions = new List<minionWrapper>();
                while (true) {
                    minionWrapper next = minionHeap.Pop();
                    if (next == null)
                        break;
                    minions.Add(next);
                }
                minions.ForEach(i => {
                    minionCounter++;
                    i.minionCounter = minionCounter;
                    minionHeap.Add(i);
                });
            }
            minionHeap.Add(new minionWrapper(minion, minionCounter));
        }

        public void PopOldestMinion() {

            minionHeap.Pop();
        }
        public List<Projectile> GetAllMinions() {
            List<Projectile> rv = new List<Projectile>();
            minionHeap.items.ForEach(i => rv.Add(i.projectile));
            return rv;
        }
        public void AddNewThought(int summonItemID, bool multiple, bool golden)
        {
            if (energyDisplay == null)
                energyDisplay = new EnergyDisplay(Player);
            energyDisplay.AddNewThought(summonItemID, multiple, golden);
        }

        public override void OnEnterWorld(Player player)
        {
            minionPower = 1;
            energyRestoreRate = 1;

            minionCollections = new List<MinionEnergyCounter>();
            specialData = new List<SpecialDataBase>();
            lastWhipSpecial = new WSA_Null();
        }

        public override void PlayerDisconnect(Player player)
        {
            if (energyDisplay != null)
            {
                EnergyDisplay.AllDisplays.Remove(energyDisplay);
                energyDisplay = null;
            }
        }
        public float GetMinionPower(int index, minionPower[] minionPowers, float prefixMinionPower)
        {
            if (index >= minionPowers.Length)
                return 0;
            minionPower mPower = minionPowers[index];
            float rv = mPower.power;
            if (mPower.DifficultyScale)
            {
                bool isJourneyMode = Main.GameModeInfo.IsJourneyMode;
                if (isJourneyMode)
                {
                    CreativePowers.DifficultySliderPower power = CreativePowerManager.Instance.GetPower<CreativePowers.DifficultySliderPower>();
                    bool isUnlocked = power.GetIsUnlocked();
                    if (isUnlocked)
                    {
                        rv *= power.StrengthMultiplierToGiveNPCs;
                    }
                }
                else
                    rv *= Main.GameModeInfo.EnemyDamageMultiplier;
            }
            switch (mPower.scalingType)
            {
                case mpScalingType.multiply:
                    rv *= prefixMinionPower * minionPower;
                    break;
                case mpScalingType.divide:
                    rv = rv * 1 / (prefixMinionPower * minionPower);
                    break;
                case mpScalingType.add:
                    rv += (prefixMinionPower + minionPower - 2) * 100;
                    break;
                case mpScalingType.subtract:
                    rv -= (prefixMinionPower + minionPower - 2) * 100;
                    break;
            }
            if (mPower.roundingType == mpRoundingType.integer)
                rv = MathF.Round(rv);
            else
                rv = MathF.Round(rv, 2);

            return rv;
        }

        public float GetWhipRange() {
            float rv = 1;
            rv *= lastWhipSpecial.GetWhipRange();
            return rv;
        }

        public float GetMinionAttackSpeed(int sourceItem, Entity minionTarget)
        {
            float rv = minionAS;
            if (minionTarget as Player != null)
                rv *= minionASRetreating;
            if (!BakedConfig.ItemCountAsWhip[sourceItem])
                rv *= minionASIgnoreMainWeapon;
            rv *= lastWhipSpecial.GetMinionAttackSpeed();
            return rv;
        }
        public float GetMinionArmorNegationPerc(NPC enemy)
        {
            float rv = 0;
            rv += lastWhipSpecial.GetMinionArmorNegationPerc(enemy);
            return rv;
        }

        public void OnWhipUsed(Item whip, ReworkMinion_Item whipFuncs) {
            lastWhipSpecial.OnWhipUsed(whip, whipFuncs);
        }
        public void OnWhippedEnemy(NPC enemy)
        {
            lastWhipSpecial.OnWhippedEnemy(enemy);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            Minion_OnHitNPCWithProj.RemoveAll(i => i == null || !i.active);
            Minion_OnHitNPCWithProj.ForEach(i =>
            {
                ReworkMinion_Projectile projFuncs = i.GetGlobalProjectile<ReworkMinion_Projectile>();
                if (projFuncs != null && projFuncs.IsMinion != ProjMinionRelation.notMinion)
                    projFuncs.minionOnPlayerHitNPCWithProj(i, proj, target, damage, knockback, crit);
            });
        }

        public void AddSpecialProjDrawInFrontEffects(Projectile projectile, ReworkMinion_Projectile projFuncs, MinionProjectileData projData, specialProjDrawInFrontEffect afterPlayerDraw)
        {
            specialProjDrawInFrontEffects.Add(new Tuple<Projectile, ReworkMinion_Projectile, MinionProjectileData, ReworkMinion_Player.specialProjDrawInFrontEffect>(projectile, projFuncs, projData, afterPlayerDraw));
        }

        public void RemoveSpecialProjDrawInFrontEffects(Projectile projectile)
        {
            specialProjDrawInFrontEffects.RemoveAll(i => i.Item1 == projectile);
        }
    }
}