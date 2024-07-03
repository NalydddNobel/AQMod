namespace Aequu2.Old.Content.Items.Potions;

internal class PotionGlobalNPC : GlobalNPC {
    public override void DrawEffects(NPC npc, ref Color drawColor) {
        if (npc.rarity > 0 && Main.LocalPlayer.TryGetModPlayer(out PotionsPlayer potionPlayer) && potionPlayer.empoweredPotionId == BuffID.Spelunker) {
            drawColor = Color.Lerp(drawColor, Color.Yellow, 0.5f);

            if (Main.rand.NextBool(24)) {
                Dust d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.TreasureSparkle, Scale: 0.3f);
                d.fadeIn = 1f;
                d.velocity *= 0.1f;
            }
        }
    }

    public override void EditSpawnRate(Player player, ref int spawnChance, ref int maxSpawns) {
        if (player.TryGetModPlayer(out PotionsPlayer potionPlayer)) {
            if (potionPlayer.empoweredPotionId == BuffID.Battle) {
                spawnChance /= 2;
                maxSpawns *= 2;
            }
            else if (potionPlayer.empoweredPotionId == BuffID.Calm) {
                spawnChance *= 2;
                maxSpawns /= 2;
            }
        }
    }
}
