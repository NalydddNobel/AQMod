using Aequus.Common.Items;
using Aequus.Content.Bosses;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Bosses;

public class BossRelic : ManualLoadItem {
    protected readonly int TileStyle;

    private readonly ModNPC _bossInstance;

    public BossRelic(ModNPC boss, int tileStyle) {
        _bossInstance = boss;
        _internalName = $"{boss.Name}Relic";
        _texturePath = $"{this.NamespaceFilePath()}/{boss.Name}/Items/{_internalName}";
    }

    public override LocalizedText DisplayName => this.GetLocalization("DisplayName", () => $"{_bossInstance.DisplayName.Value} Relic");
    public override LocalizedText Tooltip => LocalizedText.Empty;

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<BossRelicsTile>(), BossRelicsTile.Crabson);
        Item.width = 30;
        Item.height = 40;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.Master;
        Item.master = true;
        Item.value = Item.buyPrice(gold: 5);
    }
}