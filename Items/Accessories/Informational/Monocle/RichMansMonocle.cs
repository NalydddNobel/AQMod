using Aequus.Common.Items.EquipmentBooster;

namespace Aequus.Items.Accessories.Informational.Monocle;

[Gen.AequusPlayer_InfoField("accInfoMoneyMonocle")]
public class RichMansMonocle : ModItem {
    public override void SetStaticDefaults() {
        ItemID.Sets.WorksInVoidBag[Type] = true;
        EquipBoostDatabase.Instance.SetNoEffect(Type);
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.LifeformAnalyzer);
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().accInfoMoneyMonocle = true;
    }
}