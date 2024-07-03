using System;
using System.Reflection;

namespace Aequu2.Core.Entities.NPCs;

public partial class Aequu2NPC : GlobalNPC {
    public override bool InstancePerEntity => true;
    protected override bool CloneNewInstances => true;

    public byte immuneToDamageTime;

    public override void Load() {
        Load_AutomaticResetEffects();
        On_NPC.UpdateCollision += NPC_UpdateCollision;
        HookManager.ApplyAndCacheHook(typeof(NPCLoader).GetMethod(nameof(NPCLoader.NPCAI)), typeof(Aequu2NPC).GetMethod(nameof(On_NPCLoader_NPCAI), BindingFlags.NonPublic | BindingFlags.Static));
    }

    private static void On_NPCLoader_NPCAI(Action<NPC> orig, NPC npc) {
        if (!npc.TryGetGlobalNPC<Aequu2NPC>(out var Aequu2NPC)) {
            orig(npc);
            return;
        }
        if (Aequu2NPC.AI_StunGun(npc)) {
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

    public override bool CanHitNPC(NPC npc, NPC target) {
        if (target.TryGetGlobalNPC(out Aequu2NPC Aequu2)) {
            return Aequu2.immuneToDamageTime == 0;
        }

        return true;
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