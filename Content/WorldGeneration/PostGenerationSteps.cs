using Aequus.Content.Configuration;
using Aequus.Content.Items.Tools.NameTag;

namespace Aequus.Content.WorldGeneration;
public class PostGenerationSteps {
    public static int SlimeCrownSpawnrate { get; set; } = 7;

    public static void CheckSurfaceChest(Chest chest) {
        if (WorldGen.genRand.NextBool(SlimeCrownSpawnrate) && !chest.item.Any(i => i.type == ItemID.SlimeCrown)) {
            chest.AddItem(ItemID.SlimeCrown);
        }
    }

    public static void CheckUGGoldChest(Chest chest) {
        if (WorldGen.genRand.NextBool(NameTag.ChestSpawnrate)) {
            chest.AddItem(ModContent.ItemType<NameTag>());
        }
    }

    public static void CheckShadowChest(Chest chest) {
        if (VanillaChangesConfig.Instance.MoveTreasureMagnet) {
            chest.ReplaceFirst(ItemID.TreasureMagnet, ItemID.HellstoneBar, WorldGen.genRand.Next(10, 21));
        }
    }
}