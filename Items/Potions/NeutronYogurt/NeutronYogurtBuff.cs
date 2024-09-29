namespace Aequus.Items.Potions.NeutronYogurt;

public class NeutronYogurtBuff : ModBuff {
    public override void Update(Player player, ref int buffIndex) {
        player.gravity *= 1.35f;
    }
}