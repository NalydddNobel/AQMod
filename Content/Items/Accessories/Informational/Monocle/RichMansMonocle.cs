using AequusRemake.Core.CodeGeneration;
using AequusRemake.Systems.Synergy;

namespace AequusRemake.Content.Items.Accessories.Informational.Monocle;

[Gen.AequusPlayer_InfoField("accInfoMoneyMonocle")]
[Replacement]
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