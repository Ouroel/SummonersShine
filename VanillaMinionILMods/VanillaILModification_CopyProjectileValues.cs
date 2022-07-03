using Mono.Cecil.Cil;
using MonoMod.Cil;
using SummonersShine.Projectiles;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {
        static void SendData_MinionDataValues(On.Terraria.NetMessage.orig_SendData orig, int msgType, int remoteClient = -1, int ignoreClient = -1, NetworkText text = null, int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0, int number6 = 0, int number7 = 0)
        {
            orig(msgType, remoteClient, ignoreClient, text, number, number2, number3, number4, number5, number6, number7);
            if (msgType == 27)
            {
                Projectile projectile = Main.projectile[number];
                int myPlayer = Main.myPlayer;
                ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
                PacketHandler.WritePacket_SyncProjFuncs(projectile, projFuncs);
            }
        }
        
        static void GetData_EmitMinionDataValues(ILContext il)
        {
            var c = new ILCursor(il);

            if (!c.TryGotoNext(i => i.MatchCallvirt<Projectile>("ProjectileFixDesperation")))
            {
                SummonersShine.logger.Error("[GetData_EmitMinionDataValues] Cannot find i.MatchCall<Projectile>(\"ProjectileFixDesperation\")");
                return;
            }
            c.Index--;
            c.Emit(OpCodes.Ldarg_0);
            c.Index--;
            ILLabel start = c.DefineLabel();
            c.MarkLabel(start);
            c.Index++;
            c.Emit(OpCodes.Ldloc, 179);
            c.EmitDelegate(ReadMinionData);

            ILLabel value = null;
            if (!c.TryGotoPrev(i => i.MatchBrfalse(out value)))
            {
                SummonersShine.logger.Error("[GetData_EmitMinionDataValues] Cannot find i.MatchBrfalse(out value)");
                return;
            }
            c.Next.Operand = start.Target;
        }
        static void SendData_EmitMinionDataValues(ILContext il)
        {
            var c = new ILCursor(il);

            if (!c.TryGotoNext(i => i.MatchCall("Terraria.ModLoader.ProjectileLoader", "SendExtraAI")))
            {
                SummonersShine.logger.Error("[SendData_EmitMinionDataValues] Cannot find i.MatchCall(\"Terraria.ModLoader.ProjectileLoader\", \"SendExtraAI\")");
                return;
            }
            ILLabel value = null;

            if (!c.TryGotoNext(i => i.MatchBr(out value)))
            {
                SummonersShine.logger.Error("[SendData_EmitMinionDataValues] Cannot find i.MatchBr(out value)");    
                return;
            }

            ILLabel start = c.DefineLabel();

            c.Emit(OpCodes.Ldloc, 3);
            c.Index--;
            c.MarkLabel(start);
            c.Index++;
            if (ModLoader.versionedName == "tModLoader v2022.4.62.6")
            {
                c.Emit(OpCodes.Ldloc, 58); //stable
            }
            else c.Emit(OpCodes.Ldloc, 59); //preview
            c.EmitDelegate(WriteMinionData);
            if (!c.TryGotoPrev(i => i.MatchBrfalse(out value)))
            {
                SummonersShine.logger.Error("[SendData_EmitMinionDataValues] Cannot find i.MatchBrfalse(out value)");
                return;
            }
            c.Next.Operand = start.Target;
        }

        static void WriteMinionData(BinaryWriter writer, Projectile projectile)
        {
            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            projFuncs.WriteMinionProjectileData(writer);
        }

        static void ReadMinionData(MessageBuffer buffer, Projectile projectile)
        {
            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();

            projFuncs.ReadMinionProjectileData(buffer.reader);
            projFuncs.OnCreation(projectile);
        }
    }
}
