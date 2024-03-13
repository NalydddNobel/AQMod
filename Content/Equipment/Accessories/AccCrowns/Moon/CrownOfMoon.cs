using Aequus.Common.Items;
using Aequus.Common.Items.Components;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Equipment.Accessories.AccCrowns.Moon;

[LegacyName("CrownOfDarkness")]
public class CrownOfMoon : ModItem, IHaveDownsideTip {
    public override void SetDefaults() {
        Item.DefaultToAccessory(14, 20);
        Item.rare = ItemCommons.Rarity.OccultistCrown;
        Item.value = ItemCommons.Price.OccultistCrown;
        Item.hasVanityEffects = true;
    }
}