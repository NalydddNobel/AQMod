using Aequus.Common.Utilities;
using Terraria.Audio;
using Terraria.Localization;

namespace Aequus.Content.Items.Armor.Conehead;

public class ConeHelmet : BrittleArmor {
    public static readonly float BonusEndurance = 0.25f;

    internal override LocalizedText GetTooltip(BrittleArmorItem armor) {
        return base.GetTooltip(armor).WithFormatArgs(ALanguage.Percent(BonusEndurance / (armor.Tier + 1)));
    }

    internal override void SetItemStaticDefaults(Item Item, BrittleArmorItem armor) {
        ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
    }

    internal override void SetItemDefaults(Item Item, BrittleArmorItem armor) {
        Item.rare = ItemRarityID.Blue - armor.Tier;
        Item.defense = 15 / (armor.Tier + 1);
    }

    internal override void UpdateEquip(Player player, Item Item, BrittleArmorItem armor) {
        player.SafelyAddDamageReduction(BonusEndurance / (armor.Tier + 1));
    }

    internal override void OnBreak(Player player, BrittleArmorItem armor) {
        if (armor.HitsLeft == 0) {
            SoundEngine.PlaySound(AequusSounds.ItemBreak, player.Center);
        }

        base.OnBreak(player, armor);
    }

    public ConeHelmet() : base(numTiers: 3, maxHits: 30, EquipType.Head) { }
}
