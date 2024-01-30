using Aequus.Common.Tiles;
using Aequus.Content.Materials;
using Aequus.Core.Initialization;

namespace Aequus.Content.Fishing.CrabPots;
public class CrabPot : BaseCrabPot {
    public const System.Int32 CopperPot = 0;
    public const System.Int32 TinPot = 1;
    public const System.Int32 StyleCount = 2;

    public override void Load() {
        AddItem(CopperPot, ItemID.CopperBar, "Copper");
        AddItem(TinPot, ItemID.TinBar, "Tin");

        void AddItem(System.Int32 style, System.Int32 barItem, System.String name) {
            ModItem item = new InstancedTileItem(this, style: style, nameSuffix: name, rarity: ItemRarityID.Blue, value: Item.sellPrice(silver: 20));
            Mod.AddContent(item);
            LoadingSteps.EnqueueAddRecipes(() => {
                item.CreateRecipe()
                    .AddIngredient(barItem, 10)
                    .AddIngredient(ItemID.Chain, 3)
                    .AddIngredient<CompressedTrash>()
                    .AddTile(TileID.Anvils)
                    .Register();
            });
        }
    }

    protected override void SetupCrabPotContent() {
        base.SetupCrabPotContent();
        DustType = DustID.Iron;
        AddMapEntry(new(105, 186, 181), this.GetLocalization("MapEntryCopper"));
        AddMapEntry(new(152, 186, 188), this.GetLocalization("MapEntryTin"));
    }
}