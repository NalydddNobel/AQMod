using Aequus.Core.CodeGeneration;

namespace Aequus.Content.Equipment.Informational.Monocle;

[PlayerGen.InfoField("accInfoMoneyMonocle")]
public class RichMansMonocle : ModItem {
    public override void SetStaticDefaults() {
        ItemSets.WorksInVoidBag[Type] = true;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.LifeformAnalyzer);
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().accInfoMoneyMonocle = true;
    }
}