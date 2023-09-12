using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.NPCs;

public partial class AequusNPC : GlobalNPC {
    public override bool InstancePerEntity => true;

    public override void Load() {
        Load_AutomaticResetEffects();
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