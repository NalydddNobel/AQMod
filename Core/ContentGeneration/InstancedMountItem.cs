using AequusRemake.Content.Mounts.HotAirBalloon;
using Terraria.Audio;
using Terraria.Localization;

namespace AequusRemake.Core.ContentGeneration;

[Autoload(false)]
internal class InstancedMountItem(UnifiedModMount parent, int itemRarity = ItemRarityID.Yellow, int value = Item.gold * 5, SoundStyle? SoundOverride = null)
    : InstancedModItem(parent.Name + "Item", parent.NamespaceFilePath() + $"/{parent.Name.Replace("Mount", "")}Item") {
    [CloneByReference]
    private readonly UnifiedModMount _parent = parent;
    private readonly int _rarity = itemRarity;
    private readonly int _value = value;
    private readonly SoundStyle _soundStyle = SoundOverride ?? SoundID.Item34;

    public override string LocalizationCategory => "Pets";

    public override LocalizedText DisplayName => Language.GetOrRegister(_parent.GetLocalizationKey("ItemDisplayName"));
    public override LocalizedText Tooltip => Language.GetOrRegister(_parent.GetLocalizationKey("ItemTooltip"));

    public override void SetDefaults() {
        Item.DefaultToMount(ModContent.MountType<HotAirBalloonMount>());
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTime = 15;
        Item.useAnimation = 15;
        Item.UseSound = _soundStyle;
        Item.rare = _rarity;
        Item.value = _value;
    }
}