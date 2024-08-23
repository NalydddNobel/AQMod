using Aequus.Common.DataSets;
using Aequus.Common.NPCs.Global;
using System;

namespace Aequus.Content.Items.Weapons.Classless.StunGun;

public class StunGunNPC : GlobalNPC {
    private bool stunned;
    private bool stunnedVisual;
    private bool stunnedOld;

    private bool oldNoTileCollide;
    private bool oldNoGravity;

    public override bool InstancePerEntity => true;

    public override void ResetEffects(NPC npc) {
        stunned = false;
        stunnedVisual = false;
    }

    public override void Load() {
        On_NPC.VanillaAI_Inner += On_NPC_VanillaAI_Inner;
        On_Main.DrawNPC += On_Main_DrawNPC;
    }

    private static void On_NPC_VanillaAI_Inner(On_NPC.orig_VanillaAI_Inner orig, NPC npc) {
        try {
            if (npc.active && npc.EntityGlobals.Length > 0 && npc.TryGetGlobalNPC(out StunGunNPC stunNPC)) {
                bool updateFields = stunNPC.stunnedOld != stunNPC.stunned;
                if (stunNPC.stunned) {
                    if (updateFields) {
                        stunNPC.oldNoTileCollide = npc.noTileCollide;
                        stunNPC.oldNoGravity = npc.noGravity;
                        npc.noTileCollide = false;
                        npc.noGravity = false;
                        stunNPC.stunnedOld = true;
                    }
                    stunNPC.stunned = false;
                    npc.velocity.X *= 0.8f;
                    if (npc.velocity.Y < 0f) {
                        npc.velocity.Y *= 0.8f;
                    }
                    return;
                }

                if (updateFields) {
                    npc.noTileCollide = stunNPC.oldNoTileCollide;
                    npc.noGravity = stunNPC.oldNoGravity;
                    stunNPC.stunnedOld = false;
                }
            }
        }
        catch (Exception ex) {
            Aequus.Instance.Logger.Error(ex);
        }

        orig(npc);
    }

    private static void On_Main_DrawNPC(On_Main.orig_DrawNPC orig, Main self, int iNPCIndex, bool behindTiles) {
        NPC npc = Main.npc[iNPCIndex];
        npc.TryGetGlobalNPC(out StunGunNPC stunNPC);

        Vector2 drawOffset = Vector2.Zero;
        if (stunNPC?.stunnedVisual == true) {
            drawOffset += Main.rand.NextVector2Square(-2f, 2f);
            StunGun.DrawDebuffVisual(npc, Main.spriteBatch, StunGun.GetVisualTime(StunGun.VisualTimer, front: false));
        }

        npc.position += drawOffset;
        orig(self, iNPCIndex, behindTiles);
        npc.position -= drawOffset;

        if (stunNPC?.stunnedVisual == true) {
            StunGun.DrawDebuffVisual(npc, Main.spriteBatch, StunGun.GetVisualTime(StunGun.VisualTimer, front: true));
        }
    }

    public void SetStunState(NPC npc) {
        if (IsStunnable(npc)) {
            stunned = true;
        }
        else if (npc.TryGetGlobalNPC(out StatSpeedGlobalNPC speed)) {
            speed.statSpeed *= 0.5f;
        }
        stunnedVisual = true;
    }

    public static bool IsStunnable(NPC npc) {
        //if (npc.ModNPC?.Mod?.Name == "CalamityMod") {
        //    return true;
        //}
        return !NPCID.Sets.BelongsToInvasionOldOnesArmy[npc.type] && !npc.buffImmune[BuffID.Confused] && !NPCSets.StatSpeedBlacklist.Contains(npc.type)/* && NPCDataSet.StunnableByAI.Contains(npc.aiStyle)*/;
    }
}