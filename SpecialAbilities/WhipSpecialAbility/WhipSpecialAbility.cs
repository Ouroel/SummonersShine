using SummonersShine.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.SpecialAbilities.WhipSpecialAbility
{
    public abstract class WhipSpecialAbility
    {

        public int duration;
        public virtual int maxDuration => 300;
        public virtual int id => 0;
        public bool requireNetUpdate = true;
        public float minionPower0;
        public float minionPower1;

        public static WhipSpecialAbility GetWhipSpecialAbility(int id, float mp1, float mp2) {
            WhipSpecialAbility rv;
            switch (id)
            {
                case 1:
                    rv = new WSA_Kvetch(mp1);
                    break;
                case 2:
                    rv = new WSA_Reach(mp1);
                    break;
                case 3:
                    rv = new WSA_Lacerate(mp1);
                    break;
                default:
                    rv = new WSA_Null();
                    break;
            }
            rv.minionPower0 = mp1;
            rv.minionPower1 = mp2;
            return rv;
        }

        public virtual void LoadNetData_extra(BinaryReader reader) { }

        public virtual void SaveNetData_extra(ModPacket writer) { }

        public void SaveNetData(ModPacket writer)
        {
            writer.Write(minionPower0);
            writer.Write(minionPower1);
            SaveNetData_extra(writer);
        }
        public virtual WhipSpecialAbility Update()
        {
            if (duration <= 0)
                return new WSA_Null();
            duration--;
            return this;
        }

        public virtual float GetMinionAttackSpeed() { return 1; }

        public virtual float GetWhipRange() { return 1; }

        public virtual float GetMinionArmorNegationPerc(NPC enemy) { return 0; }

        public virtual void OnWhippedEnemy(NPC enemy) { }
        public virtual void OnWhipUsed(Item whip, ReworkMinion_Item whipFuncs) { }

        public virtual void Kill() { }

    }

    public class WSA_Null : WhipSpecialAbility
    {
        public override WhipSpecialAbility Update()
        {
            return this;
        }
    }

    public class WSA_Kvetch : WhipSpecialAbility
    {
        public override int id => 1;

        public WSA_Kvetch(float mp1)
        {
            duration = maxDuration;
        }

        public override float GetMinionAttackSpeed()
        {
            return 100 / (100 + minionPower0);
        }
    }

    public class WSA_Reach : WhipSpecialAbility
    {
        public override int id => 2;

        double pRNG = 0;
        bool longWhip = false;
        public WSA_Reach(float mp1)
        {
            duration = maxDuration;
        }

        public override float GetWhipRange()
        {
            if (longWhip)
            {
                return 2;
            }
            return 1;
        }

        public override void OnWhipUsed(Item whip, ReworkMinion_Item whipFuncs)
        {
            pRNG += PseudoRandom.GetPseudoRandomRNG((int)minionPower0);
            bool oldLongWhip = longWhip;
            longWhip = Main.rand.NextDouble() <= pRNG;
            if (longWhip)
                pRNG = 0;
            if(longWhip != oldLongWhip)
                requireNetUpdate = true;
        }
        public override void LoadNetData_extra(BinaryReader reader)
        {
            longWhip = reader.ReadBoolean();
        }

        public override void SaveNetData_extra(ModPacket writer)
        {
            writer.Write(longWhip);
        }
    }
    public class WSA_Lacerate : WhipSpecialAbility
    {
        class taggedEnemyCollection
        {
            public int netID;
            public int duration;
            public taggedEnemyCollection(int netID, int duration) { this.netID = netID; this.duration = duration; }
        }

        public override int id => 3;

        List<taggedEnemyCollection> taggedEnemies = new();
        public WSA_Lacerate(float mp1)
        {
            duration = maxDuration;
        }

        public override void LoadNetData_extra(BinaryReader reader)
        {
            int count = reader.Read7BitEncodedInt();
            taggedEnemies.Clear();
            for (int x = 0; x < count; x++) {
                taggedEnemies.Add(new taggedEnemyCollection(reader.Read7BitEncodedInt(), reader.Read7BitEncodedInt()));
            }
        }

        public override void SaveNetData_extra(ModPacket writer)
        {
            writer.Write7BitEncodedInt(taggedEnemies.Count);
            taggedEnemies.ForEach(i => {
                writer.Write7BitEncodedInt(i.netID);
                writer.Write7BitEncodedInt(i.duration);
            });
        }

        public override float GetMinionArmorNegationPerc(NPC enemy)
        {
            if(taggedEnemies.Find(i => i.netID == enemy.netID) != null)
                return minionPower0 / 100;
            return 0;
        }

        public override void OnWhippedEnemy(NPC enemy)
        {
            duration = maxDuration;
            taggedEnemyCollection col = taggedEnemies.Find(i => i.netID == enemy.netID);
            if (col == null)
                taggedEnemies.Add(new taggedEnemyCollection(enemy.netID, maxDuration));
            else
                col.duration = maxDuration;
            requireNetUpdate = true;
        }

        public override WhipSpecialAbility Update()
        {
            taggedEnemies.ForEach(i => i.duration--);
            taggedEnemies.RemoveAll(i => i.duration <= 0);
            return base.Update();
        }
    }
}
