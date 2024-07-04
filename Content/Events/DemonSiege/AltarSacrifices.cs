using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AequusRemake.Content.Events.DemonSiege;

public class AltarSacrifices : ModSystem {
    public static readonly Dictionary<int, Conversion> OriginalToConversion = new();
    public static readonly Dictionary<int, List<int>> ConversionToOriginals = new();

    public static void Register(int from, int to, EventTier tier = EventTier.PreHardmode) {
        Register(new Conversion(from, to, tier));
    }

    public static void Register(Conversion sacrifice) {
        OriginalToConversion[sacrifice.OriginalItem] = sacrifice;
        (CollectionsMarshal.GetValueRefOrAddDefault(ConversionToOriginals, sacrifice.NewItem, out _) ??= new()).Add(sacrifice.OriginalItem);
    }

    public override void AddRecipes() {
        foreach (Conversion sacrifice in OriginalToConversion.Values) {
            if (sacrifice.Hide || sacrifice.OriginalItem == sacrifice.NewItem) {
                continue;
            }

            /*
            Recipe recipe = Recipe.Create(sacrifice.NewItem)
                .AddIngredient(sacrifice.OriginalItem)
                .AddTile<Old.Content.Events.DemonSiege.Tiles.OblivionAltarDummy>()
                .Register()
                .SortAfterFirstRecipesOf(sacrifice.OriginalItem);

            if (sacrifice.DisableDecraft) {
                recipe.DisableDecraft();
            }
            */
        }
    }

    public override void Unload() {
        OriginalToConversion.Clear();
        ConversionToOriginals.Clear();
    }
}
