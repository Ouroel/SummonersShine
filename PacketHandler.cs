using Microsoft.Xna.Framework;
using SummonersShine.SpecialAbilities.WhipSpecialAbility;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SummonersShine
{
    public enum PacketHandlerType
    {
        SyncProjFuncs,
        SyncPlatforms,
        ClearPlatforms,
        SendRightClick,
        SendSpecialAbility,
        FakeHitEffect,
        FakeHitEffectCrit,
        FakeHitEffectPlayer,
        FakeHitEffectCritPlayer,
        SyncWhipSpecial,
        SyncCursor,
        SyncPlayerRequest, //temp
        SyncApplyVelocityToNPC,
        SyncApplyPositionToNPC,
    }
    public static partial class PacketHandler
    {
        public static void WritePacket_SyncProjFuncs(Projectile projectile, ReworkMinion_Projectile projFuncs, int ignore = -1, int toWho = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = SummonersShine.modInstance.GetPacket();
            packet.Write((byte)PacketHandlerType.SyncProjFuncs);
            packet.Write7BitEncodedInt(projectile.owner);
            packet.Write7BitEncodedInt(projectile.identity);
            projFuncs.WriteMinionProjectileData(packet);
            if(ignore != projectile.owner)
                ignore = projectile.owner;
            packet.Send(toWho, ignore);
        }
        public static void WritePacket_SyncApplyVelocityToNPC(int npcID, Vector2 vel, int ignore = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = SummonersShine.modInstance.GetPacket();
            packet.Write((byte)PacketHandlerType.SyncApplyVelocityToNPC);
            packet.Write7BitEncodedInt(npcID);
            packet.WriteVector2(vel);
            packet.Send(-1, ignore);
        }
        public static void WritePacket_SyncApplyPositionToNPC(int npcID, Vector2 pos, int ignore = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = SummonersShine.modInstance.GetPacket();
            packet.Write((byte)PacketHandlerType.SyncApplyPositionToNPC);
            packet.Write7BitEncodedInt(npcID);
            packet.WriteVector2(pos);
            packet.Send(-1, ignore);
        }
        public static void WritePacket_SyncPlayerRequest(int playerID, int ignore = -1, int toWho = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = SummonersShine.modInstance.GetPacket();
            packet.Write((byte)PacketHandlerType.SyncPlayerRequest);
            packet.Write7BitEncodedInt(playerID);
            packet.Send(toWho, ignore);
        }
        public static void WritePacket_SyncCursor(int playerID, Vector2 pos, int ignore = -1, int toWho = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = SummonersShine.modInstance.GetPacket();
            packet.Write((byte)PacketHandlerType.SyncCursor);
            packet.Write7BitEncodedInt(playerID);
            packet.WriteVector2(pos);
            packet.Send(toWho, ignore);
        }
        public static void WritePacket_UpdatePlayerPosRelToPlatform(int playerID, Platform platform, Vector2 pos, int ignore = -1, int toWho = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            if (platform == null)
            {
                ModPacket packet = SummonersShine.modInstance.GetPacket();
                packet.Write((byte)PacketHandlerType.ClearPlatforms);
                packet.Write7BitEncodedInt(playerID);
                packet.Send(toWho, ignore);
            }
            else
                WritePacket_UpdatePlayerPosRelToPlatform(playerID, platform.Net_GetKey(), pos, ignore, toWho);
        }
        public static void WritePacket_UpdatePlayerPosRelToPlatform(int playerID, int platformID, Vector2 pos, int ignore = -1, int toWho = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = SummonersShine.modInstance.GetPacket();
            packet.Write((byte)PacketHandlerType.SyncPlatforms);
            packet.Write7BitEncodedInt(playerID);
            packet.Write7BitEncodedInt(platformID);
            packet.WriteVector2(pos);
            packet.Send(toWho, ignore);
        }
        public static void WritePacket_UpdatePlayerWhipSpecial(int playerID, WhipSpecialAbility special, int ignore = -1, int toWho = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = SummonersShine.modInstance.GetPacket();
            packet.Write((byte)PacketHandlerType.SyncWhipSpecial);
            packet.Write7BitEncodedInt(special.id);
            packet.Write7BitEncodedInt(playerID);
            special.SaveNetData(packet);
            packet.Send(toWho, ignore);
        }

        public static void WritePacket_SendRightClick(int ignore = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = SummonersShine.modInstance.GetPacket();
            packet.Write((byte)PacketHandlerType.SendRightClick);
            if(ignore != -1)
                packet.Write7BitEncodedInt(ignore);

            packet.Send(-1, ignore);
        }

        public static void WritePacket_SendSpecialAbility(Vector2 mouseWorld, int itemID, Projectile[] viableMinions, int ignore = -1)
        {
            int[] viableMinionsIds = new int[viableMinions.Length];
            int[] castingSpecialTypes = new int[viableMinions.Length];
            for (int x = 0; x < viableMinions.Length; x++)
            {
                viableMinionsIds[x] = viableMinions[x].identity;
                castingSpecialTypes[x] = viableMinions[x].GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData().castingSpecialAbilityType;
            }
            WritePacket_SendSpecialAbility(mouseWorld, itemID, viableMinionsIds, castingSpecialTypes, ignore);
        }
        public static void WritePacket_SendSpecialAbility(Vector2 mouseWorld, int itemID, int[] viableMinions, int[] castingSpecialTypes, int ignore = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = SummonersShine.modInstance.GetPacket();
            packet.Write((byte)PacketHandlerType.SendSpecialAbility);
            if (ignore != -1)
                packet.Write7BitEncodedInt(ignore);
            packet.WriteVector2(mouseWorld);
            packet.Write7BitEncodedInt(itemID);
            packet.Write7BitEncodedInt(viableMinions.Length);
            for (int x = 0; x < viableMinions.Length; x++)
            {
                packet.Write7BitEncodedInt(viableMinions[x]);
                packet.Write7BitEncodedInt(castingSpecialTypes[x]);
            }

            packet.Send(-1, ignore);
        }
        public static void WritePacket_FakeHitEffect(Projectile projectile, Entity target, int damage, float knockback, bool crit, int hitDirection, int ignore = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = SummonersShine.modInstance.GetPacket();
            if (target as NPC != null)
            {
                if (crit)
                    packet.Write((byte)PacketHandlerType.FakeHitEffectCrit);
                else
                    packet.Write((byte)PacketHandlerType.FakeHitEffect);

            }
            else
            {
                if (crit)
                    packet.Write((byte)PacketHandlerType.FakeHitEffectCritPlayer);
                else
                    packet.Write((byte)PacketHandlerType.FakeHitEffectPlayer);
            }
            packet.Write7BitEncodedInt(projectile.owner);
            packet.Write7BitEncodedInt(projectile.identity);
            packet.Write7BitEncodedInt(target.whoAmI);
            packet.Write7BitEncodedInt(damage);
            packet.Write(knockback);
            packet.Write7BitEncodedInt(hitDirection);

            packet.Send(-1, ignore);
        }
        public static void HandlePacket(BinaryReader reader, int whoAmI)
        {
            byte msgType = reader.ReadByte();
            switch (msgType)
            {
                //Syncs platforms
                case (byte)PacketHandlerType.SyncPlatforms:
                case (byte)PacketHandlerType.ClearPlatforms:
                    int playerID = reader.Read7BitEncodedInt();
                    Platform platform;
                    Vector2 relPos;
                    Player player = Main.player[playerID];
                    ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
                    if (msgType == (byte)PacketHandlerType.SyncPlatforms)
                    {
                        int platformID = reader.Read7BitEncodedInt();
                        relPos = reader.ReadVector2();
                        if (Main.dedServ)
                        {
                            WritePacket_UpdatePlayerPosRelToPlatform(playerID, platformID, relPos, whoAmI);
                            return;
                        }
                        platform = PlatformCollection.FindPlatform(platformID);
                        if (platform == null)
                        {
                            //if platform cannot br found (i.e. player just joined, not everything is loaded properly)
                            playerFuncs.net_platformID = platformID;
                        }
                        else
                            playerFuncs.net_platformID = -1;
                    }
                    else
                    {
                        platform = null;
                        relPos = Vector2.Zero;
                        if (Main.dedServ)
                        {
                            WritePacket_UpdatePlayerPosRelToPlatform(playerID, null, relPos, whoAmI);
                            return;
                        }
                    }

                    playerFuncs.platform = platform;
                    playerFuncs.platformRelPos = relPos;
                    PlatformCollection.Net_ForcePlatform(player, platform);

                    break;
                case (byte)PacketHandlerType.SendRightClick:
                    if (Main.dedServ)
                    {
                        WritePacket_SendRightClick(whoAmI);
                        return;
                    }

                    playerID = reader.Read7BitEncodedInt();

                    player = Main.player[playerID];
                    int altFeatureUse = player.altFunctionUse;
                    player.altFunctionUse = 2;
                    player.ApplyItemAnimation(player.inventory[player.selectedItem]);
                    player.altFunctionUse = altFeatureUse;
                    break;
                case (byte)PacketHandlerType.SendSpecialAbility:
                    if (!Main.dedServ)
                        playerID = reader.Read7BitEncodedInt();
                    else
                        playerID = whoAmI;
                    Vector2 mouseWorld = reader.ReadVector2();
                    int itemID = reader.Read7BitEncodedInt();
                    int arrayCount = reader.Read7BitEncodedInt();

                    int[] targetIDs = new int[arrayCount];
                    int[] castingSpecialTypes = new int[arrayCount];
                    for (int x = 0; x < arrayCount; x++) {
                        targetIDs[x] = reader.Read7BitEncodedInt();
                        castingSpecialTypes[x] = reader.Read7BitEncodedInt();
                    }

                    if (Main.dedServ)
                    {
                        WritePacket_SendSpecialAbility(mouseWorld, itemID, targetIDs, castingSpecialTypes, whoAmI);
                    }

                    Projectile[] targets = new Projectile[arrayCount];

                    for (int x = 0; x < arrayCount; x++)
                    {
                        targets[x] = ModUtils.FindProjWithIdentity(playerID, targetIDs[x]);
                        targets[x].GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData().castingSpecialAbilityType = castingSpecialTypes[x];
                    }

                    player = Main.player[playerID];
                    ReworkMinion_Item.UseMinionAbilities(player, mouseWorld, itemID, targets, true);
                    break;
                case (byte)PacketHandlerType.FakeHitEffect:
                case (byte)PacketHandlerType.FakeHitEffectCrit:
                case (byte)PacketHandlerType.FakeHitEffectPlayer:
                case (byte)PacketHandlerType.FakeHitEffectCritPlayer:
                    int owner = reader.Read7BitEncodedInt();
                    int identity = reader.Read7BitEncodedInt();
                    int from = reader.Read7BitEncodedInt();
                    int damage = reader.Read7BitEncodedInt();
                    float knockBack = reader.ReadSingle();
                    int hitDirection = reader.Read7BitEncodedInt();
                    bool crit = msgType == (byte)PacketHandlerType.FakeHitEffectCrit || msgType == (byte)PacketHandlerType.FakeHitEffectCritPlayer;
                    bool isPlayer = msgType == (byte)PacketHandlerType.FakeHitEffectPlayer || msgType == (byte)PacketHandlerType.FakeHitEffectCritPlayer;

                    Projectile proj = ModUtils.FindProjWithIdentity(owner, identity);
                    Entity entity;
                    if (isPlayer)
                        entity = Main.player[from];
                    else
                        entity = Main.npc[from];

                    if (Main.dedServ)
                    {
                        WritePacket_FakeHitEffect(proj, entity, damage, knockBack, crit, hitDirection, whoAmI);
                        return;
                    }

                    ReworkMinion_Projectile projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
                    projFuncs.ModifyHitNPC_Fake(proj, entity, damage, knockBack, crit, hitDirection);
                    break;
                case (byte)PacketHandlerType.SyncWhipSpecial:
                    int specialID = reader.Read7BitEncodedInt();
                    playerID = reader.Read7BitEncodedInt();
                    player = Main.player[playerID];
                    playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
                    WhipSpecialAbility special = playerFuncs.lastWhipSpecial;

                    float mp0 = reader.ReadSingle();
                    float mp1 = reader.ReadSingle();
                    if (special.id != specialID || special.minionPower0 != mp0 || special.minionPower1 != mp1)
                    {
                        special = WhipSpecialAbility.GetWhipSpecialAbility(specialID, mp0, mp1);
                        playerFuncs.lastWhipSpecial = special;
                    }
                    special.LoadNetData_extra(reader);
                    if (Main.dedServ)
                    {
                        WritePacket_UpdatePlayerWhipSpecial(playerID, special, whoAmI);
                        return;
                    }
                    break;
                case (byte)PacketHandlerType.SyncCursor:
                    playerID = reader.Read7BitEncodedInt();
                    player = Main.player[playerID];
                    playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
                    Vector2 pos = reader.ReadVector2();
                    if (Main.dedServ)
                    {
                        WritePacket_SyncCursor(playerID, pos, whoAmI);
                        return;
                    }
                    break;
                case (byte)PacketHandlerType.SyncPlayerRequest:
                    playerID = reader.Read7BitEncodedInt();
                    if (Main.dedServ)
                    {
                        WritePacket_SyncPlayerRequest(playerID);
                        return;
                    }
                    player = Main.player[Main.myPlayer];
                    playerFuncs = player.GetModPlayer<ReworkMinion_Player>();
                    if (playerID == Main.myPlayer)
                        playerFuncs.net_initialized = true;
                    else
                        playerFuncs.SyncPlayerRequest(playerID);
                    break;
                case (byte)PacketHandlerType.SyncApplyVelocityToNPC:
                    int npcID = reader.Read7BitEncodedInt();
                    Vector2 vel = reader.ReadVector2();
                    if (Main.dedServ)
                    {
                        WritePacket_SyncApplyVelocityToNPC(npcID, vel, whoAmI);
                    }
                    NPC npc = Main.npc[npcID];
                    if (npc != null && npc.active)
                        npc.velocity += vel;
                    break;
                case (byte)PacketHandlerType.SyncApplyPositionToNPC:
                    npcID = reader.Read7BitEncodedInt();
                    pos = reader.ReadVector2();
                    if (Main.dedServ)
                    {
                        WritePacket_SyncApplyPositionToNPC(npcID, pos, whoAmI);
                    }
                    npc = Main.npc[npcID];
                    if (npc != null && npc.active)
                        npc.position = pos;
                    break;
                case (byte)PacketHandlerType.SyncProjFuncs:
                    owner = reader.Read7BitEncodedInt();
                    identity = reader.Read7BitEncodedInt();
                    proj = ModUtils.FindProjWithIdentity(owner, identity);
                    if (proj != null)
                    {
                        projFuncs = proj.GetGlobalProjectile<ReworkMinion_Projectile>();
                        if (proj.owner == Main.myPlayer)
                            return;
                        projFuncs.ReadMinionProjectileData(reader);
                        if (Main.dedServ)
                        {
                            WritePacket_SyncProjFuncs(proj, projFuncs, whoAmI);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
