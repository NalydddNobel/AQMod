using Aequus.Content.Tiles.CraftingStations.TrashCompactor;
using Terraria.GameContent.Creative;

namespace Aequus.Common.Items;

public class ItemUnUnuser : GlobalItem {
    public override void SetStaticDefaults() {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[ItemID.BoneBlock] = 100;
        TrashCompactorRecipe.AddCustomRecipe(ItemID.Bone, (ItemID.BoneBlock, 1));
        TrashCompactorRecipe.AddCustomRecipe(ItemID.BoneBlock, (ItemID.Bone, 1));
    }
}