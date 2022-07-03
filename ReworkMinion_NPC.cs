using Microsoft.Xna.Framework;
using SummonersShine.SpecialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SummonersShine
{
    public class ReworkMinion_NPC : GlobalNPC
    {
        public float slowAmount = 1;
        float lastSlowAmount = 1;

        public bool abducted = false;
        public bool spawnedFromPlayer = false;

        public bool teleported = false;
        public Vector2 lastPos;

        public int ChatCounter = 0;
        public int ChatType = 0;

        static BitsByte[] npcChatDone = new BitsByte[4];

        public override bool InstancePerEntity => true;

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (source as EntitySource_Parent != null)
                spawnedFromPlayer = true;
        }
        public override bool PreAI(NPC npc)
        {
            teleported = false;
            lastPos = npc.position;
            if (npc.noGravity)
                npc.velocity *= lastSlowAmount;
            else
                npc.velocity.X *= lastSlowAmount;

            return true;
        }

        public override void PostAI(NPC npc)
        {
            if (npc.position != lastPos)
                teleported = true;
            if (npc.noGravity)
                npc.velocity *= slowAmount;
            else
                npc.velocity.X *= slowAmount;
            lastSlowAmount = 1 / slowAmount;
        }

        public override void ResetEffects(NPC npc)
        {
            slowAmount = 1;
        }

        readonly int[] WitchDoctorSummonItemIDs = new int[]
        {
            ItemID.AbigailsFlower,
            ItemID.BabyBirdStaff,
            ItemID.SlimeStaff,
            ItemID.FlinxStaff,
            ItemID.VampireFrogStaff,
            ItemID.HornetStaff,
            ItemID.ImpStaff,
            ItemID.SpiderStaff,
            ItemID.Smolstar,
            ItemID.SanguineStaff,
            ItemID.DeadlySphereStaff,
            ItemID.PygmyStaff,
            ItemID.RavenStaff,
            ItemID.XenoStaff,
            ItemID.TempestStaff,
            ItemID.HoundiusShootius,
            ItemID.QueenSpiderStaff,
        };

        readonly string[] WitchDoctorSummonTextStrings = new string[]
         {
            "WitchDoctor_AbigailsFlower.",
            "WitchDoctor_BabyBirdStaff.",
            "WitchDoctor_SlimeStaff.",
            "WitchDoctor_FlinxStaff.",
            "WitchDoctor_VampireFrogStaff.",
            "WitchDoctor_HornetStaff.",
            "WitchDoctor_ImpStaff.",
            "WitchDoctor_SpiderStaff.",
            "WitchDoctor_Smolstar.",
            "WitchDoctor_SanguineStaff.",
            "WitchDoctor_DeadlySphereStaff.",
            "WitchDoctor_PygmyStaff.",
            "WitchDoctor_RavenStaff.",
            "WitchDoctor_XenoStaff.",
            "WitchDoctor_TempestStaff.",
            "WitchDoctor_HoundiusShootius.",
            "WitchDoctor_QueenSpiderStaff.",
         };
        public override void GetChat(NPC npc, ref string chat)
        {
            LocalizedText[] lang = Array.Empty<LocalizedText>();
            object subObj = Lang.CreateDialogSubstitutionObject(npc);
            Player player = Main.player[Main.myPlayer];
            ReworkMinion_Player playerFuncs = player.GetModPlayer<ReworkMinion_Player>();

            int npcType = npc.type;
            BitsByte ChatDone = GetChatDone(npcType);
            switch (npcType)
            {
                case NPCID.Pirate:
                    bool PirateQueenActive = playerFuncs.GetSpecialData<PirateStatCollection>().megaMinion != null;
                    int chance = PirateQueenActive ? 3 : 10;
                    if (ChatType == 0 && Main.rand.Next(0, chance) == 0)
                        ChatType = 1;
                    if (ChatType > 0)
                    {
                        if (ChatType == 1 && PirateQueenActive)
                        {
                            if (player.Male)
                                ChatType = 2;
                            else
                                ChatType = 3;
                            ChatCounter = 0;
                        }
                        if ((ChatType == 2 || ChatType == 3) && !PirateQueenActive)
                        {
                            ChatType = 4;
                            ChatCounter = 0;
                        }
                        switch (ChatType)
                        {
                            case 1:
                                lang = Language.FindAll(Lang.CreateDialogFilter(SummonersShine.NPCDialogPath + "Pirate_Queenie.", subObj));
                                break;
                            case 2:
                                lang = Language.FindAll(Lang.CreateDialogFilter(SummonersShine.NPCDialogPath + "Pirate_Queenie_Found_Male.", subObj));
                                break;
                            case 3:
                                lang = Language.FindAll(Lang.CreateDialogFilter(SummonersShine.NPCDialogPath + "Pirate_Queenie_Found_Female.", subObj));
                                break;
                            case 4:
                                lang = Language.FindAll(Lang.CreateDialogFilter(SummonersShine.NPCDialogPath + "Pirate_Queenie_Gone.", subObj));
                                break;
                        }
                    }
                    break;
                case NPCID.WitchDoctor:
                    if (ChatType == 0)
                    {
                        if (Main.rand.Next(0, 10) == 0)
                        {
                            int Count = 1;
                            for (int x = 0; x < WitchDoctorSummonItemIDs.Length; x++)
                            {
                                if (playerFuncs.GetMinionCollection(WitchDoctorSummonItemIDs[x]).minions.Count != 0 && Main.rand.Next(0, Count) == 0)
                                {
                                    ChatType = x + 1;
                                    Count++;
                                }
                            }
                        }
                    }
                    if (ChatType > 0)
                        lang = Language.FindAll(Lang.CreateDialogFilter(SummonersShine.NPCDialogPath + WitchDoctorSummonTextStrings[ChatType - 1], subObj));
                    break;

                case NPCID.Steampunker:
                    mechanic_steampunker_getchat(player, playerFuncs, npcType);

                    switch (ChatType)
                    {
                        case 1:
                            lang = Language.FindAll(Lang.CreateDialogFilter(SummonersShine.NPCDialogPath + "Steampunker_OpticStaff.", subObj));
                            break;
                        case 2:
                            lang = Language.FindAll(Lang.CreateDialogFilter(SummonersShine.NPCDialogPath + "Steampunker_TwinsDefeated.", subObj));
                            break;
                        case 3:
                            lang = Language.FindAll(Lang.CreateDialogFilter(SummonersShine.NPCDialogPath + "Steampunker_OpticStaffFound.", subObj));
                            break;
                    }
                    break;
                case NPCID.Mechanic:
                    mechanic_steampunker_getchat(player, playerFuncs, npcType);

                    switch (ChatType)
                    {
                        case 1:
                            lang = Language.FindAll(Lang.CreateDialogFilter(SummonersShine.NPCDialogPath + "Mechanic_OpticStaff.", subObj));
                            break;
                        case 2:
                            lang = Language.FindAll(Lang.CreateDialogFilter(SummonersShine.NPCDialogPath + "Mechanic_TwinsDefeated.", subObj));
                            break;
                        case 3:
                            lang = Language.FindAll(Lang.CreateDialogFilter(SummonersShine.NPCDialogPath + "Mechanic_OpticStaffFound.", subObj));
                            break;
                    }
                    break;
                case NPCID.Princess:

                    if (ChatType == 0)
                    {
                        if (Main.rand.Next(0, 10) == 0)
                        {
                            if (playerFuncs.GetMinionCollection(ItemID.Smolstar).minions.Count != 0)
                            {
                                ChatType = 1;
                            }
                        }
                    }
                    if(ChatType == 1)
                        lang = Language.FindAll(Lang.CreateDialogFilter(SummonersShine.NPCDialogPath + "Princess_BladeStaff.", subObj));
                    break;
            }
            if (lang.Length > 0)
            {
                chat = lang[ChatCounter].Value;
                ChatCounter++;
                if (ChatCounter == lang.Length)
                {
                    ChatType = 0;
                    ChatCounter = 0;
                }
            }
        }

        void mechanic_steampunker_getchat(Player player, ReworkMinion_Player playerFuncs, int npcID) {
            bool OpticStaffActive = playerFuncs.GetMinionCollection(ItemID.OpticStaff).minions.Count != 0;

            BitsByte ChatDone = GetChatDone(npcID);

            if (OpticStaffActive)
            {
                if (!ChatDone[2] || ChatType != 0 && Main.rand.Next(0, 10) == 0)
                {
                    if(ChatType != 3)
                        ChatCounter = 0;
                    ChatType = 3;
                    SaveChatDone(npcID, 2, true);
                }
            }
            else if (player.HasItem(ItemID.SoulofSight))
            {
                if (/*!ChatDone[1] ||*/ ChatType == 1 || Main.rand.Next(0, 10) == 0)
                {
                    if (ChatType != 2)
                        ChatCounter = 0;
                    ChatType = 2;
                    SaveChatDone(npcID, 1, true);
                }
            }
            else if(Main.hardMode && !NPC.downedMechBoss2) {
                if (!ChatDone[0] || Main.rand.Next(0, 10) == 0) {

                    if (ChatType != 1)
                        ChatCounter = 0;
                    ChatType = 1;
                    SaveChatDone(npcID, 0, true);
                }
            }
        }

        BitsByte GetChatDone(int id)
        {
            switch (id)
            {
                case NPCID.Pirate:
                    return npcChatDone[0];
                case NPCID.WitchDoctor:
                    return npcChatDone[1];
                case NPCID.Mechanic:
                    return npcChatDone[2];
                case NPCID.Steampunker:
                    return npcChatDone[3];
            }
            return new();
        }
        void SaveChatDone(int id, int index, bool val)
        {
            switch (id)
            {
                case NPCID.Pirate:
                    npcChatDone[0][index] = val;
                    return;
                case NPCID.WitchDoctor:
                    npcChatDone[1][index] = val;
                    return;
                case NPCID.Mechanic:
                    npcChatDone[2][index] = val;
                    return;
                case NPCID.Steampunker:
                    npcChatDone[3][index] = val;
                    return;
            }
        }
    }
}
