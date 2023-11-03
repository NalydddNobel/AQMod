using Aequus.Common.Items;
using Aequus.Content.Bosses;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Bosses;

[Autoload(false)]
public class BossTrophy : ManualLoadItem {
    protected readonly int TileStyle;

    private readonly ModNPC _bossInstance;

    public BossTrophy(ModNPC boss, int tileStyle) {
        _bossInstance = boss;
        _internalName = $"{boss.Name}Trophy";
        _texturePath = $"Aequus/Content/Bosses/{boss.Name}/Items/{_internalName}";
    }

    public override LocalizedText DisplayName => this.GetLocalization("DisplayName", () => $"{_bossInstance.DisplayName.Value} Trophy");
    public override LocalizedText Tooltip => LocalizedText.Empty;

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<BossTrophiesTile>(), TileStyle);
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(gold: 1);
    }
}