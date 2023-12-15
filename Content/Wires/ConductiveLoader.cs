using Aequus.Common.Tiles;
using Aequus.Content.Wires.Conductive;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Wires;

public class ConductiveLoader : ModSystem {
    public override void Load() {
        var conductiveBlock = Add("Copper", ItemID.CopperBar, new(183, 88, 25), DustID.Copper);
        ModTypeLookup<ModTile>.RegisterLegacyNames(conductiveBlock, "ConductiveBlock", "ConductiveBlockTile");

        Add("Tin", ItemID.TinBar, new(187, 165, 124), DustID.Copper);

        ConductiveBlock Add(string name, int barItem, Color mapColor, int dustType) {
            var conductiveBlock = new ConductiveBlock(name, mapColor, dustType);
            Mod.AddContent(conductiveBlock);

            Mod.AddContent(new InstancedTileItem(conductiveBlock, value: Item.buyPrice(silver: 1)).WithRecipe((m) => {
                m.CreateRecipe()
                    .AddIngredient(barItem, 1)
                    .AddTile(TileID.Furnaces)
                    .Register();
            }));
            return conductiveBlock;
        }
    }
}