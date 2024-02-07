namespace Aequus.Old.Content.Potions;

internal class PotionGlobalNPC : GlobalNPC {
    public override void DrawEffects(NPC npc, ref Color drawColor) {
        if (npc.rarity > 0 && Main.LocalPlayer.TryGetModPlayer(out PotionsPlayer potionPlayer) && potionPlayer.empoweredPotionId == BuffID.Spelunker) {
            Color.Lerp(drawColor, Color.Yellow, 0.5f);

            if (Main.rand.NextBool(24)) {
                Dust d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.TreasureSparkle);
                d.fadeIn = d.scale + 0.5f;
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
