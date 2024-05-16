using Aequus.Content.Mounts.HotAirBalloon;
using Terraria.Audio;
using Terraria.Localization;

namespace Aequus.Core.ContentGeneration;

[Autoload(false)]
internal class InstancedMountItem : InstancedModItem {
    [CloneByReference]
    private readonly UnifiedModMount _parent;
    private readonly int _rarity;
    private readonly int _value;
    private readonly SoundStyle _soundStyle;

    public InstancedMountItem(UnifiedModMount parent, int itemRarity = ItemRarityID.Yellow, int value = Item.gold * 5, SoundStyle? SoundOverride = null)
        : base(parent.Name + "Item", parent.NamespaceFilePath() + $"/{parent.Name.Replace("Mount", "")}Item") {
        _parent = parent;
        _rarity = itemRarity;
        _value = value;
        _soundStyle = SoundOverride ?? SoundID.Item34;
    }

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