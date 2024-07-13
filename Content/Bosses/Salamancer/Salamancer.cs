using AequusRemake.Core.ContentGeneration;

namespace AequusRemake.Content.Bosses.Salamancer;

public partial class Salamancer() : UnifiedBoss(new(ItemRarity: Commons.Rare.BossSalamancer)) {
    public Point Origin;

    public override void SetDefaults() {
        NPC.width = 60;
        NPC.height = 60;
        NPC.boss = true;
    }

    public override void FindFrame(int frameHeight) {
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        return base.PreDraw(spriteBatch, screenPos, drawColor);
    }

    public override void BossLoot(ref string name, ref int potionType) {
        potionType = ModContent.GetInstance<Items.Potions.RestorationPotions>().Lesser.Type;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        base.ModifyNPCLoot(npcLoot);
    }
}
