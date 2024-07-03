namespace Aequu2.Old.Content.Events.Glimmer.CosmicMonolith;

public class CosmicMonolith : ModItem {
    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.BloodMoonMonolith);
        Item.accessory = true;
        Item.vanity = true;
        Item.hasVanityEffects = true;
        Item.createTile = ModContent.TileType<CosmicMonolithTile>();
        Item.placeStyle = 0;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<Aequu2Player>().cosmicMonolithShader = true;
    }

    public override void UpdateVanity(Player player) {
        player.GetModPlayer<Aequu2Player>().cosmicMonolithShader = true;
    }
}