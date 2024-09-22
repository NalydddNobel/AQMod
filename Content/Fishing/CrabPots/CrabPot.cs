using Aequus.Common.ContentTemplates.Generic;
using System.Collections.Generic;

namespace Aequus.Content.Fishing.CrabPots;
public class CrabPot : UnifiedCrabPot, IAddRecipes {
    public const int CopperPot = 0;
    public const int TinPot = 1;
    public const int StyleCount = 2;

    List<(ModItem Item, int BarId)> Items = [];

    public override void Load() {
        AddItem(CopperPot, ItemID.CopperBar, "Copper");
        AddItem(TinPot, ItemID.TinBar, "Tin");

        void AddItem(int style, int barItem, string name) {
            ModItem item = new InstancedTileItem(this, style: style, nameSuffix: name, Settings: new() {
                Rare = ItemRarityID.White,
                Value = Item.sellPrice(silver: 20)
            });
            Mod.AddContent(item);
            Items.Add((item, barItem));
        }
    }

    void IAddRecipes.AddRecipes() {
        foreach (var pair in Items) {
            pair.Item.CreateRecipe()
                .AddIngredient(pair.BarId, 10)
                .AddIngredient(ItemID.Chain, 3)
#if POLLUTED_OCEAN
                .AddIngredient<Items.Materials.CompressedTrash.CompressedTrash>(5)
#else
                .AddIngredient(ItemID.Coral, 5)
#endif
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    protected override void SetupCrabPotContent() {
        base.SetupCrabPotContent();
        DustType = DustID.Iron;
        AddMapEntry(new Color(105, 186, 181), this.GetLocalization("MapEntryCopper"));
        AddMapEntry(new Color(152, 186, 188), this.GetLocalization("MapEntryTin"));
    }
}