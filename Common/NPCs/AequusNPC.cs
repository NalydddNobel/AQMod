using Aequus.Core.Utilities;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.NPCs;

public partial class AequusNPC : GlobalNPC {
    public override bool InstancePerEntity => true;

    public override void Load() {
        Load_AutomaticResetEffects();
        DetourHelper.AddHook(typeof(NPCLoader).GetMethod(nameof(NPCLoader.NPCAI)), typeof(AequusNPC).GetMethod(nameof(On_NPCLoader_NPCAI), BindingFlags.NonPublic | BindingFlags.Static));
    }

    private static void On_NPCLoader_NPCAI(Action<NPC> orig, NPC npc) {
        if (!npc.TryGetGlobalNPC<AequusNPC>(out var aequusNPC)) {
            return;
        }
        if (aequusNPC.AI_StunGun(npc)) {
            return;
        }
        orig(npc);
    }

    public override void Unload() {
        _resetEffects = null;
    }

    public void DrawBehindNPC(int i, bool behindTiles) {
        var npc = Main.npc[i];
        var sb = Main.spriteBatch;
        DrawBehindNPC_StunGun(npc, sb);
    }

    public void DrawAboveNPC(int i, bool behindTiles) {
        var npc = Main.npc[i];
        var sb = Main.spriteBatch;
        DrawAboveNPC_StunGun(npc, sb);
    }
}