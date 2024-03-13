namespace Aequus.Content.Enemies.PollutedOcean.BreadOfCthulhu.Items;

[AutoloadEquip(EquipType.Head)]
public class BreadOfCthulhuMask : ModItem {
    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.rare = ItemRarityID.White;
        Item.value = Item.sellPrice(silver: 10);
        Item.vanity = true;
    }
}