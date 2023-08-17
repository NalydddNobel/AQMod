using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Aequus.Common.Items.DropRules;

public class AequusDropRules {
    public const int DroprateMask = 7;
    public const int DroprateTrophy = 10;
    public const int DroprateMasterPet = 4;

    public static IItemDropRule Trophy<T>() where T : ModItem {
        return new GuaranteedFlawlesslyRule(ModContent.ItemType<T>(), 10);
    }
}