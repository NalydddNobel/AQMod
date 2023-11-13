using Aequus.Content.Configuration;
using Aequus.Content.Items.Tools.NameTag;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.WorldGeneration;
public class PostGenerationSteps {
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