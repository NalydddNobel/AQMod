namespace Aequus.Items.Equipment.Vanity;
[AutoloadEquip(EquipType.Back, EquipType.Front)]
public class PumpkingCloak : ModItem {
    public static int FrontID { get; private set; }
    public static int BackID { get; private set; }

    public override void SetStaticDefaults() {
        FrontID = Item.frontSlot;
        BackID = Item.backSlot;
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory(20, 20);
        Item.vanity = true;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.buyPrice(gold: 1);
    }
}
