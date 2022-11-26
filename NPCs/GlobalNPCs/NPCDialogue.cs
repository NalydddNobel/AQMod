﻿using Aequus.Biomes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.GlobalNPCs
{
    public class NPCDialogue : GlobalNPC
    {
        public override void GetChat(NPC npc, ref string chat)
        {
            if (Main.LocalPlayer.Aequus().ZoneGlimmer && Main.rand.NextBool(3) && AequusText.TryGetText($"Mods.Aequus.Chat.{AequusText.NPCKeyName(npc.type)}.Glimmer", out var text))
            {
                chat = text;
            }
            if (npc.type == NPCID.SkeletonMerchant && GlimmerBiome.EventActive && Main.rand.NextBool())
            {
                chat = AequusText.GetText("Chat.SkeletonMerchant.Glimmer");
            }
        }
    }
}