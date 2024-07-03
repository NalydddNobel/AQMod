namespace Aequu2.Content.Items.Accessories.GoldenFeather;

public class GoldenFeatherBuff : ModBuff {
    public override void SetStaticDefaults() {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
        Main.persistentBuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        if (!player.TryGetModPlayer<AequusPlayer>(out var Aequu2Player)) {
            return;
        }
        player.lifeRegen += GoldenFeather.LifeRegenerationAmount;
        Aequu2Player.SetNonStackingRespawnTimeModifier(GoldenFeather.RespawnTimeAmount);
    }

    public override bool RightClick(int buffIndex) {
        return false;
    }
}