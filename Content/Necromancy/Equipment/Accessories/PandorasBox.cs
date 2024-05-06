using Aequus.Common.Items;

namespace Aequus.Content.Necromancy.Equipment.Accessories;

public class PandorasBox : ModItem {
    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.accessory = true;
        Item.rare = ItemDefaults.RarityDungeon;
        Item.value = ItemDefaults.ValueDungeon;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        var aequus = player.GetModPlayer<AequusPlayer>();
        aequus.zombieDebuffMultiplier++;
        aequus.ghostProjExtraUpdates += 1;
    }
}