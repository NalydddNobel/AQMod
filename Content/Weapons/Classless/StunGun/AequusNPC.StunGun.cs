using Aequus.Content.Weapons.Classless.StunGun;
using Aequus.Core.Generator;

namespace Aequus.Common.NPCs;

public partial class AequusNPC {
    [ResetEffects]
    public System.Boolean stunGun;
    [ResetEffects]
    public System.Boolean stunGunVisual;
    public System.Boolean stunGunOld;
    public System.Boolean stunned_NoTileCollide;
    public System.Boolean stunned_NoGravity;

    private void DrawBehindNPC_StunGun(NPC npc, SpriteBatch spriteBatch, ref Vector2 drawOffset) {
        if (stunGunVisual) {
            drawOffset += Main.rand.NextVector2Square(-2f, 2f);
            StunGun.DrawDebuffVisual(npc, spriteBatch, StunGun.GetVisualTime(StunGun.VisualTimer, front: false));
        }
    }

    private void DrawAboveNPC_StunGun(NPC npc, SpriteBatch spriteBatch) {
        if (stunGunVisual) {
            StunGun.DrawDebuffVisual(npc, spriteBatch, StunGun.GetVisualTime(StunGun.VisualTimer, front: true));
        }
    }

    private System.Boolean AI_StunGun(NPC npc) {
        System.Boolean updateFields = stunGunOld != stunGun;
        stunGunOld = stunGun;
        if (stunGun) {
            if (updateFields) {
                stunned_NoTileCollide = npc.noTileCollide;
                stunned_NoGravity = npc.noGravity;
                npc.noTileCollide = false;
                npc.noGravity = false;
            }
            npc.velocity.X *= 0.8f;
            if (npc.velocity.Y < 0f) {
                npc.velocity.Y *= 0.8f;
            }
            return true;
        }

        if (updateFields) {
            npc.noTileCollide = stunned_NoTileCollide;
            npc.noGravity = stunned_NoGravity;
        }
        return false;
    }
}