using Aequus.Common.Items;
using Terraria.Audio;
using Terraria.Localization;

namespace Aequus.Content.Equipment.Armor.MiscHelmets;

public class ConeHelmet : BrittleArmor {
    public static float BonusEndurance { get; set; } = 0.25f;

    internal override LocalizedText GetTooltip(BrittleArmorItem armor) => base.GetTooltip(armor).WithFormatArgs(ExtendLanguage.Percent(BonusEndurance / (armor.Tier + 1)));

    internal override void SetItemStaticDefaults(Item Item, BrittleArmorItem armor) {
        HelmetEquipSets.DrawHatHair[Item.headSlot] = true;
    }

    internal override void SetItemDefaults(Item Item, BrittleArmorItem armor) {
        Item.rare = ItemCommons.Rarity.PollutedOceanLoot - armor.Tier;
        Item.defense = 15 / (armor.Tier+1);
    }

    internal override void UpdateEquip(Player player, Item Item, BrittleArmorItem armor) {
        player.SafelyAddDamageReduction(BonusEndurance);
    }

    internal override void OnBreak(Player player, BrittleArmorItem armor) {
        if (armor.HitsLeft == 0) {
            SoundEngine.PlaySound(AequusSounds.ItemBreak, player.Center);
        }

        base.OnBreak(player, armor);
    }

    public ConeHelmet() : base(numTiers: 3, maxHits: 30, EquipType.Head) { }
}
