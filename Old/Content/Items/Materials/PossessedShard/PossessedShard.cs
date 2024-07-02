using Aequus.Core;
using Aequus.Core.Entities.NPCs;

namespace Aequus.Old.Content.Items.Materials.PossessedShard;

public class PossessedShard : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 10;
        DropsGlobalNPC.AddNPCLoot(NPCID.PossessedArmor, new PossessedShardDropRule(2));
        DropsGlobalNPC.AddNPCLoot(NPCID.CursedHammer, new PossessedShardDropRule(1, min: 2, max: 3));
        DropsGlobalNPC.AddNPCLoot(NPCID.CrimsonAxe, new PossessedShardDropRule(1, min: 2, max: 3));
    }

    public override void SetDefaults() {
        Item.width = 14;
        Item.height = 14;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = Commons.Rare.MaterialStartHM;
        Item.value = Item.sellPrice(silver: 7);
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(Color.White, lightColor, Helper.Oscillate(Main.GlobalTimeWrappedHourly * 2.5f, 0.66f, 1f));
    }
}