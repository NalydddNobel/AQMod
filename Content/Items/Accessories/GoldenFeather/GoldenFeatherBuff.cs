namespace Aequus.Content.Items.Accessories.GoldenFeather;

public class GoldenFeatherBuff : ModBuff {
    public override void SetStaticDefaults() {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
        Main.persistentBuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        if (!player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
            return;
        }
        player.lifeRegen += GoldenFeather.LifeRegenerationAmount;
        aequusPlayer.SetNonStackingRespawnTimeModifier(GoldenFeather.RespawnTimeAmount);
    }

    public override bool RightClick(int buffIndex) {
        return false;
    }
}