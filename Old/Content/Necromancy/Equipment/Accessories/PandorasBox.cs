using Aequu2.Core;
using Aequu2.Core.CodeGeneration;

namespace Aequu2.Old.Content.Necromancy.Equipment.Accessories;

[Gen.Aequu2Player_ResetField<float>("zombieDebuffMultiplier")]
[Gen.Aequu2Player_ResetField<int>("ghostProjExtraUpdates")]
public class PandorasBox : ModItem {
    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.accessory = true;
        Item.rare = Commons.Rare.BiomeDungeon;
        Item.value = Commons.Cost.BiomeDungeon;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        var Aequu2 = player.GetModPlayer<Aequu2Player>();
        Aequu2.zombieDebuffMultiplier++;
        Aequu2.ghostProjExtraUpdates += 1;
    }
}