using System.Reflection.Metadata.Ecma335;
using Terraria;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;

namespace Aequus.Unused.Debug;

internal partial class TesterItem {
    private string GetItemWikiName(Item item) {
        if (item.type < ItemID.Count) {
            return item.Name;
        }
        return "#" + item.Name;
    }

    public void Wiki_GetRecipes(TestParameters parameters) {
        int tileID = Main.tile[parameters.TileCoordinates_Point].TileType;
        bool byHand = !Main.tile[parameters.TileCoordinates_Point].HasTile;
        using var file = Helper.CreateDebugFile("Wiki_Recipes_" + (byHand ? "ByHand" : TileID.Search.GetName(tileID)));
        var mod = Aequus.Instance;
        string data = "";
        string stationText = byHand ? "" : "\n| station=" + Lang._mapLegendCache.FromTile(Main.Map[parameters.X, parameters.Y], parameters.X, parameters.Y);
        for (int i = 0; i < Recipe.numRecipes; i++) {
            var r = Main.recipe[i];
            if (r == null || r.Disabled || r.Mod != mod || r.createItem == null) {
                continue;
            }
            if (byHand) {
                if (r.requiredTile.Count > 0){
                    continue;
                }
            }
            else if (!r.HasTile(tileID)) {
                continue;
            }

            data += "-->{{recipes/register" + stationText + "\n| result=" + GetItemWikiName(r.createItem) + " | amount=" + r.createItem.stack;
            string affixName = r.createItem.AffixName();
            if (Lang.GetItemNameValue(r.createItem.type) != affixName) {
                data += " | text=" + affixName;
            }
            for (int ingredients = 0; ingredients < r.requiredItem.Count; ingredients++) {
                if (r.requiredItem[ingredients] != null && !r.requiredItem[ingredients].IsAir) {
                    data += "\n| " + GetItemWikiName(r.requiredItem[ingredients]) + " | " + r.requiredItem[ingredients].stack;
                }
            }
            data += "\n}}<!--\n";
        }
        data = data.Substring(0, data.Length - 5);
        file.WriteText(data);
        Helper.OpenDebugFolder();
    }
}
