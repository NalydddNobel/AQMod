using AequusRemake.Core.CodeGeneration;

namespace AequusRemake.Core.Entities.NPCs;

public partial class AequusRemakeNPC {
    [ResetEffects(1f)]
    public float statSpeedX = 1f;
    [ResetEffects(1f)]
    public float statSpeedY = 1f;

    private static void NPC_UpdateCollision(On_NPC.orig_UpdateCollision orig, NPC npc) {
        if (!npc.TryGetGlobalNPC<AequusRemakeNPC>(out var AequusRemakeNPC)) {
            orig(npc);
            return;
        }

        var velocityBoost = new Vector2(AequusRemakeNPC.statSpeedX, AequusRemakeNPC.statSpeedY);
        npc.velocity.X *= velocityBoost.X;
        npc.velocity.Y *= velocityBoost.Y;
        orig(npc);
        npc.velocity.X /= velocityBoost.X;
        npc.velocity.Y /= velocityBoost.Y;
    }

    public override void PostAI(NPC npc) {
        if (npc.noTileCollide) {
            npc.position.X += npc.velocity.X * (statSpeedX - 1f);
            npc.position.Y += npc.velocity.Y * (statSpeedY - 1f);
        }
    }
}