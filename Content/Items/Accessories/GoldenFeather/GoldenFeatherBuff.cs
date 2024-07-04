namespace AequusRemake.Content.Items.Accessories.GoldenFeather;

public class GoldenFeatherBuff : ModBuff {
    public override void SetStaticDefaults() {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
        Main.persistentBuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        if (!player.TryGetModPlayer<AequusPlayer>(out var AequusRemakePlayer)) {
            return;
        }
        player.lifeRegen += GoldenFeather.LifeRegenerationAmount;
        AequusRemakePlayer.SetNonStackingRespawnTimeModifier(GoldenFeather.RespawnTimeAmount);
    }

    public override bool RightClick(int buffIndex) {
        return false;
    }
}