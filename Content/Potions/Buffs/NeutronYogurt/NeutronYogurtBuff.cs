namespace Aequus.Content.Potions.Buffs.NeutronYogurt;

public class NeutronYogurtBuff : ModBuff {
    public override void Update(NPC npc, ref System.Int32 buffIndex) {
        if (npc.velocity.Y > 0f) {
            npc.GravityMultiplier *= 2f;
        }
    }

    public override void Update(Player player, ref System.Int32 buffIndex) {
        player.GetModPlayer<AequusPlayer>().buffNeutronYogurt = true;
    }
}