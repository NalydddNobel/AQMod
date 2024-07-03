using Aequu2.Old.Content.Items.Weapons.Melee.SickBeat;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Aequu2.Old.Content.VanillaChanges;

public class ToxicSludgeItems : GlobalNPC {
    public int containsItem;

    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
        return entity.type == NPCID.ToxicSludge;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NeverTrue(), ModContent.ItemType<SickBeat>(), SickBeat.DropRate));
    }

    public override void OnSpawn(NPC npc, IEntitySource source) {
        if (!Main.rand.NextBool(SickBeat.DropRate)) {
            return;
        }

        containsItem = ModContent.ItemType<SickBeat>();
    }

    public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (containsItem > 0) {
            npc.Opacity = 0.5f;
            Main.GetItemDrawFrame(containsItem, out Texture2D texture, out Rectangle frame);

            float scale = 1f;
            if (texture.Width > npc.frame.Width / 2) {
                scale = npc.frame.Width / 2 / (float)texture.Width;
            }

            spriteBatch.Draw(texture, npc.Center - screenPos, frame, drawColor, npc.rotation, frame.Size() / 2f, scale * npc.scale, SpriteEffects.None, 0f);
        }
        return true;
    }
}
