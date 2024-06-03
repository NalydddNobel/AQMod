namespace Aequus.Old.Content.Items.Potions.NeutronYogurt;

public class NeutronYogurtBuff : ModBuff {
    public override void Update(NPC npc, ref int buffIndex) {
        if (npc.velocity.Y > 0f) {
            npc.GravityMultiplier *= 2f;
        }
    }

    public override void Update(Player player, ref int buffIndex) {
        player.GetModPlayer<AequusPlayer>().buffNeutronYogurt += 1f;
    }
}