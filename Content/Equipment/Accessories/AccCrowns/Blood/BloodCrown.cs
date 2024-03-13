using Aequus.Common.Items;
using Aequus.Common.Items.Components;
using Aequus.Common.Players.Stats;
using System;
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
        Item.rare = ItemCommons.Rarity.OccultistCrown;
        Item.value = ItemCommons.Price.OccultistCrown;
        Item.hasVanityEffects = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        var modPlayer = player.GetModPlayer<BloodCrownPlayer>();
        if (!modPlayer.accBloodCrownOld) {
            modPlayer.bloodHeartRegen = -300;
            modPlayer.BloodHearts = (int)Math.Max(modPlayer.BloodHearts, player.HeartCount() - player.statLife / player.HealthPerHeart());
        }
        modPlayer.accBloodCrown = true;
    }
}