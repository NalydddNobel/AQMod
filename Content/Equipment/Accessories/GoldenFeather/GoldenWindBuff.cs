namespace Aequus.Content.Equipment.Accessories.GoldenFeather;

public class GoldenWindBuff : ModBuff {
    public override void SetStaticDefaults() {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
        Main.persistentBuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        player.lifeRegen += GoldenWind.LifeRegenerationAmount;
        player.GetModPlayer<AequusPlayer>().SetNonStackingRespawnTimeModifier(GoldenWind.RespawnTimeAmount);
    }

    public override bool RightClick(int buffIndex) {
        return false;
    }
}