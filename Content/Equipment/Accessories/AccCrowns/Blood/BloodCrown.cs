using Aequus.Common.Items;
using Aequus.Common.Items.Components;
using Aequus.Common.Players.Stats;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Equipment.Accessories.AccCrowns.Blood;

[LegacyName("CrownOfBloodItem", "CrownOfBlood")]
public class BloodCrown : ModItem, IHaveDownsideTip {
    public const int SlotId = Player.SupportedSlotsArmor;

    public const string CategoryKey = "Mods.Aequus.Items.BloodCrown";

    public static int TooltipUUID { get; set; }

    public static StatComparer StatComparer { get; private set; }

    public static int StatTickUpdateRate { get; set; } = 60;
    public static int statTickUpdate { get; internal set; }

    public override void Load() {
        StatComparer = new();
    }

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