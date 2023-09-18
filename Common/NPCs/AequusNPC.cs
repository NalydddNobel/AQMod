using Aequus.Core.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.NPCs;

public partial class AequusNPC : GlobalNPC {
    public override bool InstancePerEntity => true;
    protected override bool CloneNewInstances => true;

    public override void Load() {
        Load_AutomaticResetEffects();
        On_NPC.UpdateCollision += NPC_UpdateCollision;
        DetourHelper.AddHook(typeof(NPCLoader).GetMethod(nameof(NPCLoader.NPCAI)), typeof(AequusNPC).GetMethod(nameof(On_NPCLoader_NPCAI), BindingFlags.NonPublic | BindingFlags.Static));
    }

    private static void On_NPCLoader_NPCAI(Action<NPC> orig, NPC npc) {
        if (!npc.TryGetGlobalNPC<AequusNPC>(out var aequusNPC)) {
            orig(npc);
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

    public override void SetDefaults(NPC npc) {
        statSpeedX = 1f;
        statSpeedY = 1f;
    }

    public void DrawBehindNPC(int i, bool behindTiles, ref Vector2 drawOffset) {
        var npc = Main.npc[i];
        var sb = Main.spriteBatch;
        DrawBehindNPC_StunGun(npc, sb, ref drawOffset);
    }

    public void DrawAboveNPC(int i, bool behindTiles) {
        var npc = Main.npc[i];
        var sb = Main.spriteBatch;
        DrawAboveNPC_StunGun(npc, sb);
    }
}