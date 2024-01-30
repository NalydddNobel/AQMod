using System;

namespace Aequus.Content.Equipment.Accessories.GoldenFeather;

public class GoldenFeatherBuff : ModBuff {
    public override void SetStaticDefaults() {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
        Main.persistentBuff[Type] = true;
    }

    public override void Update(Player player, ref Int32 buffIndex) {
        if (!player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
            return;
        }
        player.lifeRegen += GoldenFeather.LifeRegenerationAmount;
        aequusPlayer.SetAccRespawnTimeModifier(GoldenFeather.RespawnTimeAmount);
    }

    public override Boolean RightClick(Int32 buffIndex) {
        return false;
    }
}