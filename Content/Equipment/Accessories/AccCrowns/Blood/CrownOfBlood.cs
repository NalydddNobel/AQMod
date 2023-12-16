using Aequus.Common.Items;
using Aequus.Common.Items.Components;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Equipment.Accessories.AccCrowns.Blood;

[LegacyName("CrownOfBloodItem")]
public class CrownOfBlood : ModItem, IHaveDownsideTip {
    public const int SlotId = Player.SupportedSlotsArmor;

    public const string CategoryKey = "Mods.Aequus.Items.CrownOfBlood";

    public override void SetDefaults() {
        Item.DefaultToAccessory(14, 20);
        Item.rare = ItemDefaults.Rarity.AccessoryCrown;
        Item.value = ItemDefaults.Price.AccessoryCrown;
        Item.hasVanityEffects = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().accBloodCrown = true;
    }
}