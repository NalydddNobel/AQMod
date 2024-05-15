using Aequus.Common;
using Aequus.Core.CodeGeneration;

namespace Aequus.Old.Content.Necromancy.Equipment.Accessories;

[Gen.AequusPlayer_ResetField<float>("zombieDebuffMultiplier")]
[Gen.AequusPlayer_ResetField<int>("ghostProjExtraUpdates")]
public class PandorasBox : ModItem {
    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.accessory = true;
        Item.rare = Commons.Rare.DungeonLoot;
        Item.value = Commons.Cost.DungeonLoot;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        var aequus = player.GetModPlayer<AequusPlayer>();
        aequus.zombieDebuffMultiplier++;
        aequus.ghostProjExtraUpdates += 1;
    }
}