using Aequus.Common.Items.EquipmentBooster;

namespace Aequus.Items.Accessories.Informational.AnglerBroadcaster;

public class AnglerBroadcaster : ModItem {
    public override void SetStaticDefaults() {
        ItemID.Sets.WorksInVoidBag[Type] = true;
        EquipBoostDatabase.Instance.SetNoEffect(Type);
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.LifeformAnalyzer);
    }

    public override void UpdateInfoAccessory(Player player) {
        player.Aequus().accShowQuestFish = true;
    }
}
