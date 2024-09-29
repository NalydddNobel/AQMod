namespace Aequus.Items.Potions.NoonPotion;
public class NoonBuff : ModBuff {
    public override void Update(Player player, ref int buffIndex) {
        Lighting.AddLight(player.Center, new Vector3(1.2f, 1f, 0.5f));
        player.nightVision = true;
        player.GetModPlayer<AequusPlayer>().forceDayState = 1;
    }
}