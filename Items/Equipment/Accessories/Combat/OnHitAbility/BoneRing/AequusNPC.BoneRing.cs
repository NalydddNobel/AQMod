namespace Aequus;

public partial class AequusNPC {
    public bool debuffBoneRing;

    private void DrawEffects_BoneRingDebuff(NPC npc, ref Color drawColor) {
        if (debuffBoneRing) {
            drawColor = (drawColor * 0.8f) with { A = drawColor.A };
        }
    }

    private void DebuffEffects_BoneRing(NPC npc) {
        if (!debuffBoneRing) {
            return;
        }

        if (syncedTimer % 10 == 0) {
            var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Slush, Alpha: 160, Scale: Main.rand.NextFloat(0.6f, 1f));
            d.velocity = npc.velocity.SafeNormalize(Vector2.Zero);
            d.noGravity = true;
            d.fadeIn = d.scale + 0.5f;
        }
        if (npc.life >= 0 && Main.GameUpdateCount % 30 == 0) {
            npc.HitEffect(0, 10);
        }
        if (!npc.noTileCollide) {
            int x = (int)((npc.position.X + npc.width / 2f) / 16f);
            int checkTilesTop = (int)((npc.position.Y + npc.height) / 16f);
            int checkTilesBottom = (int)(npc.position.Y / 16f);
            for (int j = checkTilesBottom; j <= checkTilesTop; j++) {
                if (Main.tile[x, j].IsFullySolid()) {
                    var d = Dust.NewDustPerfect(new Vector2(npc.position.X + Main.rand.NextFloat(npc.width), npc.position.Y + npc.height), DustID.Slush, Vector2.Zero, 160, Scale: Main.rand.NextFloat(0.6f, 1f));
                    d.noGravity = true;
                    d.fadeIn = d.scale + 0.5f;
                    break;
                }
            }
        }
    }
}
