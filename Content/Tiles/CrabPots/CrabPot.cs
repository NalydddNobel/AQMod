using Aequus.Common.Tiles;
using Aequus.Content.Items.Material;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.CrabPots;
public class CrabPot : BaseCrabPot {
    public const int CopperPot = 0;
    public const int TinPot = 1;
    public const int StyleCount = 2;

    public override void Load() {
        AddItem(CopperPot, ItemID.CopperBar, "Copper");
        AddItem(TinPot, ItemID.TinBar, "Tin");

        void AddItem(int style, int barItem, string name) {
            Mod.AddContent(new InstancedTileItem(this, style: style, nameSuffix: name, rarity: ItemRarityID.Blue, value: Item.sellPrice(silver: 20)).WithRecipe((m) => {
                m.CreateRecipe()
                    .AddIngredient(barItem, 10)
                    .AddIngredient(ItemID.Chain, 3)
                    .AddIngredient<CompressedTrash>()
                    .AddTile(TileID.Anvils)
                    .Register();
            }));
        }
    }

    protected override void SetupCrabPotContent() {
        base.SetupCrabPotContent();
        DustType = DustID.Iron;
        AddMapEntry(new(105, 186, 181), this.GetLocalization("MapEntryCopper"));
        AddMapEntry(new(152, 186, 188), this.GetLocalization("MapEntryTin"));
    }
}