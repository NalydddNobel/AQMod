using Aequu2.Content.Tiles.CraftingStations.TrashCompactor;
using Terraria.GameContent.Creative;

namespace Aequu2.Content.VanillaChanges;

public class BoneBlockChanges : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.type == ItemID.BoneBlock;
    }

    public override void SetStaticDefaults() {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[ItemID.BoneBlock] = 100;
        TrashCompactorRecipe.AddCustomRecipe(ItemID.Bone, (ItemID.BoneBlock, 1));
        TrashCompactorRecipe.AddCustomRecipe(ItemID.BoneBlock, (ItemID.Bone, 1));
    }

    public override void SetDefaults(Item entity) {
        entity.StatsModifiedBy.Add(Mod);
    }
}