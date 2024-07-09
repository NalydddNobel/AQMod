using AequusRemake.Content.Items.Materials;
using AequusRemake.Core.ContentGeneration;

namespace AequusRemake.Systems.Fishing.CrabPots;
public class CrabPot : UnifiedCrabPot {
    public const int CopperPot = 0;
    public const int TinPot = 1;
    public const int StyleCount = 2;

    public override void Load() {
        AddItem(CopperPot, ItemID.CopperBar, "Copper");
        AddItem(TinPot, ItemID.TinBar, "Tin");

        void AddItem(int style, int barItem, string name) {
            ModItem item = new InstancedTileItem(this, style: style, nameSuffix: name, rarity: ItemRarityID.Blue, value: Item.sellPrice(silver: 20));
            Mod.AddContent(item);
            AequusRemake.OnAddRecipes += AddRecipes;

            void AddRecipes() {
                item.CreateRecipe()
                    .AddIngredient(barItem, 10)
                    .AddIngredient(ItemID.Chain, 3)
                    .AddIngredient<CompressedTrash>(5)
                    .AddTile(TileID.Anvils)
                    .Register();
            }
        }
    }

    protected override void SetupCrabPotContent() {
        base.SetupCrabPotContent();
        DustType = DustID.Iron;
        AddMapEntry(new Color(105, 186, 181), this.GetLocalization("MapEntryCopper"));
        AddMapEntry(new Color(152, 186, 188), this.GetLocalization("MapEntryTin"));
    }
}