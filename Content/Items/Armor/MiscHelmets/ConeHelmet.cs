using AequusRemake.Core;
using Terraria.Audio;
using Terraria.Localization;

namespace AequusRemake.Content.Items.Armor.MiscHelmets;

public class ConeHelmet : BrittleArmor {
    public static float BonusEndurance { get; set; } = 0.25f;

    internal override LocalizedText GetTooltip(BrittleArmorItem armor) {
        return base.GetTooltip(armor).WithFormatArgs(ALanguage.Percent(BonusEndurance / (armor.Tier + 1)));
    }

    internal override void SetItemStaticDefaults(Item Item, BrittleArmorItem armor) {
        HelmetEquipSets.DrawHatHair[Item.headSlot] = true;
    }

    internal override void SetItemDefaults(Item Item, BrittleArmorItem armor) {
        Item.rare = Commons.Rare.BiomeOcean - armor.Tier;
        Item.defense = 15 / (armor.Tier + 1);
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
