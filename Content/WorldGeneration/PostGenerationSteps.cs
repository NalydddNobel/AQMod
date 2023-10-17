using Aequus.Content.Configuration;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.WorldGeneration;
public class PostGenerationSteps {
    public static void CheckShadowChest(Chest chest) {
        if (VanillaChangesConfig.Instance.MoveTreasureMagnet) {
            chest.ReplaceFirst(ItemID.TreasureMagnet, ItemID.HellstoneBar, WorldGen.genRand.Next(10, 21));
        }
    }
}