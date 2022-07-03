using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.Prefixes
{
    public abstract class SummonerPrefix : ModPrefix
    {
        public static int Divine;
        public static int Remphanic;
        public static int Bloodbound;
        public static int Shining;
        public static int Righteous;
        public static int Devoted;
        public static int Fanatical;
        public static int Initiated;
        public static int Heretical;
        public static int Lunatic;
        public static int Unholy;
        public static int Benzona;



        public override string Name => _name;
        public string _name;
        readonly float _damageMult;
        readonly float _knockbackMult;
        readonly float _useTimeMult;
        readonly int _critBonus;
        readonly float _powerMult;
        readonly float _valueMult;
        public override void Apply(Item item)
        {
            ReworkMinion_Item data = item.GetGlobalItem<ReworkMinion_Item>();
            data.prefixMinionPower = _powerMult;
        }

        public SummonerPrefix(float damageMult, float knockbackMult, float useTimeMult, int critBonus, float powerMult, float valueMult)
        {
            this._damageMult = damageMult;
            this._knockbackMult = knockbackMult;
            this._useTimeMult = useTimeMult;
            this._critBonus = critBonus;
            this._powerMult = powerMult;
            this._valueMult = valueMult;
        }
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult *= _damageMult;
            knockbackMult *= _knockbackMult;
            useTimeMult /= _useTimeMult;
            critBonus += _critBonus;
        }
        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= _powerMult;
            valueMult *= _valueMult;
        }

        string GetRarityBonus(float valMult) {

            int tier = 0;
            if (valMult >= 1.2)
                tier = 2;
            else if (valMult >= 1.05)
                tier = 1;
            else if (valMult <= 0.8)
                tier = -2;
            else if (valMult <= 0.95)
                tier = -1;
            bool neg = valMult < 1;
            string rv = "";
            if (!neg)
                rv += "+";
            rv += tier;
            return rv;
        }

        public float GetRealValueMult() {
            float rv = 1f * _damageMult * (_useTimeMult) * _knockbackMult * (1f + _critBonus * 0.02f) * _powerMult * _valueMult;
            return rv;
        }

        static string mult_toValue(float mult, int roundDP = 0) {
            mult -= 1;
            mult *= 100;
            return crit_toValue(mult, roundDP);
        }
        static string crit_toValue(float mult, int roundDP = 0)
        {
            bool neg = mult < 0;
            string rv = "";
            if (!neg)
                rv += "+";
            rv += Math.Round(mult, roundDP);
            rv += "%";
            return rv;
        }

        public string PrintToTable() {
            string rv = "[tr]";
            rv += "[td]" + _name + "[/td]";
            rv += "[td]" + mult_toValue(_damageMult) + "[/td]";
            rv += "[td]" + mult_toValue(_useTimeMult) + "[/td]";
            rv += "[td]" + mult_toValue(_knockbackMult) + "[/td]";
            rv += "[td]" + crit_toValue(_critBonus) +"[/td]";
            rv += "[td]" + mult_toValue(_powerMult) + "[/td]";
            float valMult = GetRealValueMult();
            rv += "[td]" + GetRarityBonus(valMult) + "[/td]";
            rv += "[td]" + mult_toValue(valMult * valMult, 2) + "[/td]";
            return rv + "[/tr]";
        }

        public class SummonerPrefixLoader : ILoadable {

            public void Load(Mod mod)
            {
                mod.AddContent(new SummonerPrefix_Divine(1.15f, 1.15f, 1.1f, 5, 1.1f, 1));
                mod.AddContent(new SummonerPrefix_Remphanic(1f, 1, 1.1f, 0, 1.15f, 1));
                mod.AddContent(new SummonerPrefix_Bloodbound(1.10f, 1f, 1f, 0, 1.1f, 1));
                mod.AddContent(new SummonerPrefix_Shining(1.1f, 0.9f, 1.1f, 0, 1.1f, 1));
                mod.AddContent(new SummonerPrefix_Righteous(1.1f, 1f, 1.1f, 0, 0.9f, 1f));
                mod.AddContent(new SummonerPrefix_Devoted(1.1f, 1f, 0.9f, 0, 1.1f, 1f));
                mod.AddContent(new SummonerPrefix_Fanatical(1.15f, 1f, 1.1f, 0, 0.85f, 1f));
                mod.AddContent(new SummonerPrefix_Initiated(1f, 1f, 1f, 0, 1.15f, 1f));

                mod.AddContent(new SummonerPrefix_Heretical(1.1f, 0.85f, 1f, 0, 1f, 1f));
                mod.AddContent(new SummonerPrefix_Unholy(1f, 0.9f, 1f, 0, 0.9f, 1f));
                mod.AddContent(new SummonerPrefix_Lunatic(0.90f, 1f, 0.90f, 0, 1f, 1f));
                mod.AddContent(new SummonerPrefix_Benzona(0.85f, 0.85f, 1f, 0, 0.87f, 1f));
            }

            public void Unload() { }
        }

        public static void PostSetupContent()
        {
            Divine = ModContent.PrefixType<SummonerPrefix_Divine>();
            Remphanic = ModContent.PrefixType<SummonerPrefix_Remphanic>(); //registered as random prefix? wtf
            Bloodbound = ModContent.PrefixType<SummonerPrefix_Bloodbound>();
            Shining = ModContent.PrefixType<SummonerPrefix_Shining>();
            Righteous = ModContent.PrefixType<SummonerPrefix_Righteous>();
            Devoted = ModContent.PrefixType<SummonerPrefix_Devoted>();
            Fanatical = ModContent.PrefixType<SummonerPrefix_Fanatical>();
            Initiated = ModContent.PrefixType<SummonerPrefix_Initiated>();
            Heretical = ModContent.PrefixType<SummonerPrefix_Heretical>();
            Lunatic = ModContent.PrefixType<SummonerPrefix_Lunatic>();
            Unholy = ModContent.PrefixType<SummonerPrefix_Unholy>();
            Benzona = ModContent.PrefixType<SummonerPrefix_Benzona>();
        }
    }

    public class SummonerPrefix_Divine : SummonerPrefix
    {
        public SummonerPrefix_Divine(float damageMult, float knockbackMult, float useTimeMult, int critBonus, float powerMult, float valueMult) : base(damageMult, knockbackMult, useTimeMult, critBonus, powerMult, valueMult)
        {
            _name = "Divine";
        }
    }
    public class SummonerPrefix_Remphanic : SummonerPrefix
    {
        public SummonerPrefix_Remphanic(float damageMult, float knockbackMult, float useTimeMult, int critBonus, float powerMult, float valueMult) : base(damageMult, knockbackMult, useTimeMult, critBonus, powerMult, valueMult)
        {
            _name = "Remphanic";
        }
    }
    public class SummonerPrefix_Bloodbound : SummonerPrefix
    {
        public SummonerPrefix_Bloodbound(float damageMult, float knockbackMult, float useTimeMult, int critBonus, float powerMult, float valueMult) : base(damageMult, knockbackMult, useTimeMult, critBonus, powerMult, valueMult)
        {
            _name = "Bloodbound";
        }
    }
    public class SummonerPrefix_Shining : SummonerPrefix
    {
        public SummonerPrefix_Shining(float damageMult, float knockbackMult, float useTimeMult, int critBonus, float powerMult, float valueMult) : base(damageMult, knockbackMult, useTimeMult, critBonus, powerMult, valueMult)
        {
            _name = "Shining";
        }
    }
    public class SummonerPrefix_Righteous : SummonerPrefix
    {
        public SummonerPrefix_Righteous(float damageMult, float knockbackMult, float useTimeMult, int critBonus, float powerMult, float valueMult) : base(damageMult, knockbackMult, useTimeMult, critBonus, powerMult, valueMult)
        {
            _name = "Righteous";
        }
    }
    public class SummonerPrefix_Devoted : SummonerPrefix
    {
        public SummonerPrefix_Devoted(float damageMult, float knockbackMult, float useTimeMult, int critBonus, float powerMult, float valueMult) : base(damageMult, knockbackMult, useTimeMult, critBonus, powerMult, valueMult)
        {
            _name = "Devoted";
        }
    }
    public class SummonerPrefix_Fanatical : SummonerPrefix
    {
        public SummonerPrefix_Fanatical(float damageMult, float knockbackMult, float useTimeMult, int critBonus, float powerMult, float valueMult) : base(damageMult, knockbackMult, useTimeMult, critBonus, powerMult, valueMult)
        {
            _name = "Fanatical";
        }
    }
    public class SummonerPrefix_Initiated : SummonerPrefix
    {
        public SummonerPrefix_Initiated(float damageMult, float knockbackMult, float useTimeMult, int critBonus, float powerMult, float valueMult) : base(damageMult, knockbackMult, useTimeMult, critBonus, powerMult, valueMult)
        {
            _name = "Initiated";
        }
    }
    public class SummonerPrefix_Heretical : SummonerPrefix
    {
        public SummonerPrefix_Heretical(float damageMult, float knockbackMult, float useTimeMult, int critBonus, float powerMult, float valueMult) : base(damageMult, knockbackMult, useTimeMult, critBonus, powerMult, valueMult)
        {
            _name = "Heretical";
        }
    }
    public class SummonerPrefix_Unholy : SummonerPrefix
    {
        public SummonerPrefix_Unholy(float damageMult, float knockbackMult, float useTimeMult, int critBonus, float powerMult, float valueMult) : base(damageMult, knockbackMult, useTimeMult, critBonus, powerMult, valueMult)
        {
            _name = "Unholy";
        }
    }
    public class SummonerPrefix_Lunatic : SummonerPrefix
    {
        public SummonerPrefix_Lunatic(float damageMult, float knockbackMult, float useTimeMult, int critBonus, float powerMult, float valueMult) : base(damageMult, knockbackMult, useTimeMult, critBonus, powerMult, valueMult)
        {
            _name = "Lunatic";
        }
    }
    public class SummonerPrefix_Benzona : SummonerPrefix
    {
        public SummonerPrefix_Benzona(float damageMult, float knockbackMult, float useTimeMult, int critBonus, float powerMult, float valueMult) : base(damageMult, knockbackMult, useTimeMult, critBonus, powerMult, valueMult)
        {
            _name = "Benzona";
        }
    }
}
