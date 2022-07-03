using Microsoft.Xna.Framework;
using SummonersShine.SpecialAbilities.DefaultSpecialAbility;
using SummonersShine.SpecialAbilities.WhipSpecialAbility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.ModSupport
{
    public class WhipSpecialAbility_ModdedCustom : WhipSpecialAbility
    {
        object ArbitraryData;
        public DefaultWhipSpecialAbility_ModdedCustom whipSpecialAbility;
        public override int id => -1;
        public override int maxDuration => whipSpecialAbility.maxDuration;

        public WhipSpecialAbility_ModdedCustom(DefaultWhipSpecialAbility_ModdedCustom source, float mp0, float mp1, object arbitraryData, int duration) {
            whipSpecialAbility = source;
            this.ArbitraryData = arbitraryData;
            this.minionPower0 = mp0;
            this.minionPower1 = mp1;
            this.duration = duration;
        }
        public override float GetMinionArmorNegationPerc(NPC enemy)
        {
            if (whipSpecialAbility.GetMinionArmorNegationPerc != null)
                return whipSpecialAbility.GetMinionArmorNegationPerc(enemy, minionPower0, minionPower1, ArbitraryData);
            return 0;
        }

        public override float GetMinionAttackSpeed()
        {
            if (whipSpecialAbility.GetMinionAttackSpeed != null)
                return whipSpecialAbility.GetMinionAttackSpeed(minionPower0, minionPower1, ArbitraryData);
            return 1;
        }

        public override float GetWhipRange()
        {
            if (whipSpecialAbility.GetWhipRange != null)
                return whipSpecialAbility.GetWhipRange(minionPower0, minionPower1, ArbitraryData);
            return 1;
        }

        public override void OnWhippedEnemy(NPC enemy)
        {
            if (whipSpecialAbility.OnWhippedEnemy != null)
                whipSpecialAbility.OnWhippedEnemy(enemy, minionPower0, minionPower1, ArbitraryData);
        }

        public override void OnWhipUsed(Item whip, ReworkMinion_Item whipFuncs)
        {
            if (whipSpecialAbility.OnWhipUsedFunc != null)
                whipSpecialAbility.OnWhipUsedFunc(whip, minionPower0, minionPower1, ArbitraryData);
        }

        public override void LoadNetData_extra(BinaryReader reader)
        {
            if (whipSpecialAbility.LoadNetData_Extra != null)
                whipSpecialAbility.LoadNetData_Extra(reader, minionPower0, minionPower1, ArbitraryData);
        }

        public override void SaveNetData_extra(ModPacket writer)
        {
            if (whipSpecialAbility.SaveNetData_extra != null)
                whipSpecialAbility.SaveNetData_extra(writer, minionPower0, minionPower1, ArbitraryData);
        }

        public override WhipSpecialAbility Update()
        {
            if (whipSpecialAbility.Update != null)
            {
                bool? rv = whipSpecialAbility.Update(minionPower0, minionPower1, ArbitraryData);
                if (rv == false)
                    return new WSA_Null();
                else if (rv == true)
                    return this;
                    
            }
            return base.Update();
        }
        public void ModifyDuration(int duration)
        {
            this.duration = duration;
        }

        public void ForceNetUpdate()
        {
            requireNetUpdate = true;
        }

        public override void Kill()
        {
            if (whipSpecialAbility.Kill != null)
            {
                whipSpecialAbility.Kill(minionPower0, minionPower1, ArbitraryData);
            }
        }
    }
    public class DefaultWhipSpecialAbility_ModdedCustom : DefaultSpecialAbility_Whip
    {
        public static List<DefaultWhipSpecialAbility_ModdedCustom> modAbils;

        public Mod mod;
        public string name;

        public DefaultWhipSpecialAbility_ModdedCustom(Mod mod, string name)
        { 
            this.mod = mod;
            this.name = name;
        }

        public int maxDuration;

        Func<float, float, Tuple<float, int, int, bool>[]> getMinionPower;
        Func<Random, Tuple<float, int, int, bool>[]> getRandomMinionPower;
        Func<Item, Projectile, bool> validForItem;
        Func<float, float, object> GetArbitraryData;
        Action<float, float, object, Action<int>, Action> ReceiveCallbacks;

        public Func<NPC, float, float, object, float> GetMinionArmorNegationPerc;
        public Func<float, float, object, float> GetMinionAttackSpeed;
        public Func<float, float, object, float> GetWhipRange;


        public Action<NPC, float, float, object> OnWhippedEnemy;
        public Action<Item, float, float, object> OnWhipUsedFunc;

        public Action<BinaryReader, float, float, object> LoadNetData_Extra;
        public Action<ModPacket, float, float, object> SaveNetData_extra;

        public Func<float, float, object, bool?> Update;

        public Action<float, float, object> Kill;

        public override bool ValidForItem(Item item, Projectile projectile)
        {
            if (validForItem != null)
                return validForItem(item, projectile);
            return base.ValidForItem(item, projectile);
        }

        public class DefaultWhipSpecialAbility_ModdedCustom_Loader : ILoadable
        {
            public void Load(Mod mod)
            {
                modAbils = new();
            }

            public void Unload()
            {
                modAbils = null;
            }
        }
        public override string GetFullLocalizationPath(string midPath)
        {
            return "Mods." + mod.Name + "." + midPath + name;
        }

        public static void AssignHook(Mod mod, string name, int hookNum, object hook)
        {
            DefaultWhipSpecialAbility_ModdedCustom special = modAbils.Find(i=>i.mod == mod && i.name == name);
            if (special == null)
            {
                special = new(mod, name);
                modAbils.Insert(0, special);
                if(Loaded)
                    megaBakedAbilities.Add(special);
            }
            special.AssignHook(hookNum, hook);
        }

        public override string ToString()
        {
            return name;
        }

        public override int id => -1;
        public override void GetRandomMinionPower(minionPower[] minionPowers, Random rand)
        {
            Tuple<float, int, int, bool>[] randomMinionPowerList = getRandomMinionPower(rand);
            for (int x = 0; x < randomMinionPowerList.Length; x++)
            {
                Tuple<float, int, int, bool> minionPowerTuple = randomMinionPowerList[x];
                minionPowers[x] = minionPower.NewMP(minionPowerTuple.Item1, (mpScalingType)minionPowerTuple.Item2, (mpRoundingType)minionPowerTuple.Item3, minionPowerTuple.Item4);
            }
        }

        public override void GetMinionPower(minionPower[] minionPowers, float val1, float val2)
        {
            Tuple<float, int, int, bool>[] randomMinionPowerList = getMinionPower(val1, val2);
            for (int x = 0; x < randomMinionPowerList.Length; x++)
            {
                Tuple<float, int, int, bool> minionPowerTuple = randomMinionPowerList[x];
                minionPowers[x] = minionPower.NewMP(minionPowerTuple.Item1, (mpScalingType)minionPowerTuple.Item2, (mpRoundingType)minionPowerTuple.Item3, minionPowerTuple.Item4);
            }
        }

        public override bool OnWhipUsed(Player player, ReworkMinion_Player playerFuncs, Item whip, ReworkMinion_Item whipFuncs)
        {
            float mp0 = whipFuncs.GetMinionPower(player, whip.type, 0);
            float mp1 = whipFuncs.GetMinionPower(player, whip.type, 1);
            WhipSpecialAbility lastWhipSpecial = playerFuncs.lastWhipSpecial;

            if (lastWhipSpecial.id != id || lastWhipSpecial.minionPower0 != mp0 || lastWhipSpecial.minionPower1 != mp1)
            {
                GenerateNewWhipSpecial(playerFuncs, mp0, mp1);
            }
            else
            {
                WhipSpecialAbility_ModdedCustom moddedCustom = lastWhipSpecial as WhipSpecialAbility_ModdedCustom;
                if (moddedCustom != null && moddedCustom.whipSpecialAbility != this)
                {
                    GenerateNewWhipSpecial(playerFuncs, mp0, mp1);
                }
                else
                    playerFuncs.lastWhipSpecial.duration = playerFuncs.lastWhipSpecial.maxDuration;
            }

            return true;
        }

        void GenerateNewWhipSpecial(ReworkMinion_Player playerFuncs, float mp0, float mp1)
        {
            playerFuncs.lastWhipSpecial.Kill();
            object arbitraryData = GetArbitraryData(mp0, mp1);
            WhipSpecialAbility_ModdedCustom custom = new WhipSpecialAbility_ModdedCustom(this, mp0, mp1, arbitraryData, maxDuration);
            playerFuncs.lastWhipSpecial = custom;
            ReceiveCallbacks(mp0, mp1, arbitraryData, custom.ModifyDuration, custom.ForceNetUpdate);
        }

        public void AssignHook(int hookNum, object hook)
        {
            switch (hookNum)
            {
                case 0:
                    getMinionPower = (Func<float, float, Tuple<float, int, int, bool>[]>)hook;
                    return;
                case 1:
                    getRandomMinionPower = (Func<Random, Tuple<float, int, int, bool>[]>)hook;
                    return;
                case 2:
                    validForItem = (Func<Item, Projectile, bool>)hook;
                    return;
                case 3:
                    GetArbitraryData = (Func<float, float, object>)hook;
                    return;
                case 4:
                    maxDuration = (int)hook;
                    return;
                case 5:
                    ReceiveCallbacks = (Action<float, float, object, Action<int>, Action>)hook;
                    break;
                case 6:
                    Update = (Func<float, float, object, bool?>)hook;
                    return;
                case 7:
                    Kill = (Action<float, float, object>)hook;
                    return;
                case 8:
                    GetMinionArmorNegationPerc = (Func<NPC, float, float, object, float>)hook;
                    return;
                case 9:
                    GetMinionAttackSpeed = (Func<float, float, object, float>)hook;
                    return;
                case 10:
                    GetWhipRange = (Func<float, float, object, float>)hook;
                    return;
                case 11:
                    OnWhippedEnemy = (Action<NPC, float, float, object>)hook;
                    return;
                case 12:
                    OnWhipUsedFunc = (Action<Item, float, float, object>)hook;
                    return;
                case 13:
                    LoadNetData_Extra = (Action<BinaryReader, float, float, object>)hook;
                    return;
                case 14:
                    SaveNetData_extra = (Action<ModPacket, float, float, object>)hook;
                    return;
            }
        }
    }

    public class DefaultSpecialAbility_ModdedCustom : DefaultSpecialAbility
    {
        public static List<DefaultSpecialAbility_ModdedCustom> modAbils;

        public Mod mod;
        public string name;

        Func<float, float, Tuple<float, int, int, bool>[]> getMinionPower;
        Func<Random, Tuple<float, int, int, bool>[]> getRandomMinionPower;
        Func<Item, Projectile, bool> validForItem;
        Func<List<Projectile>, Tuple<bool, float, float>> getArbitraryBuffGaugeData;

        Action<Projectile, float, float> onProjectileCreated;
        Action<Projectile, float, float> unhookMinionPower;

        Func<Player, Item, bool> onWhipUsed;
        public override string ToString()
        {
            return name;
        }
        public DefaultSpecialAbility_ModdedCustom(Mod mod, string name)
        {
            this.mod = mod;
            this.name = name;
        }
        public override string GetFullLocalizationPath(string midPath)
        {
            return "Mods." + mod.Name + "." + midPath + name;
        }

        public class DefaultSpecialAbility_ModdedCustom_Loader : ILoadable
        {
            public void Load(Mod mod)
            {
                modAbils = new();
            }

            public void Unload()
            {
                modAbils = null;
            }
        }


        public static void AssignHook(Mod mod, string name, int hookNum, object hook)
        {
            DefaultSpecialAbility_ModdedCustom special = modAbils.Find(i => i.mod == mod && i.name == name);
            if (special == null)
            {
                special = new(mod, name);
                modAbils.Insert(0, special);
                if (Loaded)
                    megaBakedAbilities.Add(special);
            }
            special.AssignHook(hookNum, hook);
        }
        public override void GetRandomMinionPower(minionPower[] minionPowers, Random rand)
        {
            Tuple<float, int, int, bool>[] randomMinionPowerList = getRandomMinionPower(rand);
            for (int x = 0; x < randomMinionPowerList.Length; x++)
            {
                Tuple<float, int, int, bool> minionPowerTuple = randomMinionPowerList[x];
                minionPowers[x] = minionPower.NewMP(minionPowerTuple.Item1, (mpScalingType)minionPowerTuple.Item2, (mpRoundingType)minionPowerTuple.Item3, minionPowerTuple.Item4);
            }
        }

        public override void GetMinionPower(minionPower[] minionPowers, float val1, float val2)
        {
            Tuple<float, int, int, bool>[] randomMinionPowerList = getMinionPower(val1, val2);
            for (int x = 0; x < randomMinionPowerList.Length; x++)
            {
                Tuple<float, int, int, bool> minionPowerTuple = randomMinionPowerList[x];
                minionPowers[x] = minionPower.NewMP(minionPowerTuple.Item1, (mpScalingType)minionPowerTuple.Item2, (mpRoundingType)minionPowerTuple.Item3, minionPowerTuple.Item4);
            }
        }

        public override bool ValidForItem(Item item, Projectile projectile)
        {
            return validForItem(item, projectile);
        }

        public override bool GetArbitraryBuffGaugeData(List<Projectile> projList, out float topGauge, out float bottomGauge)
        {
            if (getArbitraryBuffGaugeData == null)
                return base.GetArbitraryBuffGaugeData(projList, out topGauge, out bottomGauge);
            Tuple<bool, float, float> rv = getArbitraryBuffGaugeData(projList);
            topGauge = rv.Item2;
            bottomGauge = rv.Item3;
            return rv.Item1;
        }

        public override void OnProjectileCreated(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            if (onProjectileCreated != null)
            {
                minionPower[] mp = ReworkMinion_Item.minionPowers[projFuncs.SourceItem];
                onProjectileCreated(projectile, mp[0].power, mp[1].power);
            }
        }

        public override void UnhookMinionPower(Projectile projectile, ReworkMinion_Projectile projFuncs)
        {
            if (unhookMinionPower != null)
            {
                minionPower[] mp = ReworkMinion_Item.minionPowers[projFuncs.SourceItem];
                unhookMinionPower(projectile, mp[0].power, mp[1].power);
            }
        }

        //returns IsWhip
        public override bool OnWhipUsed(Player player, ReworkMinion_Player playerFuncs, Item whip, ReworkMinion_Item whipFuncs)
        {
            if (onWhipUsed != null)
                return onWhipUsed(player, whip);
            return false;
        }
        public void AssignHook(int hookNum, object hook)
        {
            switch (hookNum)
            {
                case 0:
                    getMinionPower = (Func<float, float, Tuple<float, int, int, bool>[]>)hook;
                    return;
                case 1:
                    getRandomMinionPower = (Func<Random, Tuple<float, int, int, bool>[]>)hook;
                    return;
                case 2:
                    validForItem = (Func<Item, Projectile, bool>)hook;
                    return;
                case 3:
                    getArbitraryBuffGaugeData = (Func<List<Projectile>, Tuple<bool, float, float>>)hook;
                    return;
                case 4:
                    onProjectileCreated = (Action<Projectile, float, float>)hook;
                    return;
                case 5:
                    unhookMinionPower = (Action<Projectile, float, float>)hook;
                    return;
                case 6:
                    onWhipUsed = (Func<Player, Item, bool>)hook;
                    return;
            }
        }
    }
    public class ModSupportDefaultSpecialAbility
    {
        public static List<ModSupportDefaultSpecialAbility> handlers;

        public class ModSupportDefaultSpecialAbility_Loader : ILoadable
        {
            public void Load(Mod mod)
            {
                handlers = new();

            }

            public void Unload()
            {
                handlers = null;
            }
        }
        public Mod mod;

        public ModSupportDefaultSpecialAbility(Mod mod) {
            this.mod = mod;
        }
        public static ModSupportDefaultSpecialAbility Generate(Mod mod)
        {
            ModSupportDefaultSpecialAbility rv = handlers.Find(i => i.mod == mod);
            if (rv == null)
            {
                rv = new(mod);
                handlers.Add(rv);
            }
            return rv;
        }

        public Func<Vector2, Vector2, Projectile, Projectile, NPC, Tuple<Vector2, Vector2, float, float>> MultishotPreGenerateHandler;
        public Action<Projectile, Projectile, NPC> MultishotPostGenerateHandler;

        public Action<Projectile, NPC> InstastrikePostMeleeHandler;
        public Func<Projectile, bool?> IsRangedOverride;

        public void AssignHook(int hookNum, object hook)
        {
            switch (hookNum)
            {
                case 0:
                    MultishotPreGenerateHandler = (Func<Vector2, Vector2, Projectile, Projectile, NPC, Tuple<Vector2, Vector2, float, float>>)hook;
                    return;
                case 1:
                    MultishotPostGenerateHandler = (Action<Projectile, Projectile, NPC>)hook;
                    return;
                case 2:
                    InstastrikePostMeleeHandler = (Action<Projectile, NPC>)hook;
                    return;
                case 3:
                    IsRangedOverride = (Func<Projectile, bool?>)hook;
                    return;
            }
        }

        public static void HandleMultishotPreGenerate(Projectile template, Projectile source, NPC npc, ref Vector2 position, ref Vector2 velocity, ref float ai0, ref float ai1)
        {
            Vector2 pos = position;
            Vector2 vel = velocity;
            float ai0in = ai0;
            float ai1in = ai1;
            handlers.ForEach(i => {
                if(i.MultishotPreGenerateHandler != null)
                {
                    Tuple<Vector2, Vector2, float, float> rv = i.MultishotPreGenerateHandler(pos, vel, template, source, npc);
                    pos = rv.Item1;
                    vel = rv.Item2;
                    ai0in = rv.Item3;
                    ai1in = rv.Item4;
                }
            });
            position = pos;
            velocity = vel;
            ai0 = ai0in;
            ai1 = ai1in;
        }

        public static void HandleMultishotPostGenerate(Projectile newShot, Projectile source, NPC target)
        {
            handlers.ForEach(i =>
            {
                if (i.MultishotPostGenerateHandler != null)
                {
                    i.MultishotPostGenerateHandler(newShot, source, target);
                }
            });
        }

        public static void HandleInstastrikePostMelee(Projectile minion, NPC target)
        {
            handlers.ForEach(i =>
            {
                if (i.InstastrikePostMeleeHandler != null)
                {
                    i.InstastrikePostMeleeHandler(minion, target);
                }
            });
        }

        public static bool? HandleIsRangedOverride(Projectile projectile)
        {
            bool? rv = null;
            handlers.ForEach(i =>
            {
                if (i.IsRangedOverride != null)
                {
                    if (rv == null)
                        rv = i.IsRangedOverride(projectile);
                }
            });
            return rv;
        }
    }
}
