using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Restoration.GoldenFeather;

public class GoldenWindBuff : ModBuff {
    public override void SetStaticDefaults() {
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        if (!player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
            return;
        }
        player.lifeRegen += GoldenWind.LifeRegenerationAmount;
        if (aequusPlayer.respawnTimeModifier > GoldenWind.RespawnTimeAmount) {
            aequusPlayer.respawnTimeModifier = Math.Max(aequusPlayer.respawnTimeModifier - GoldenWind.RespawnTimeAmount, GoldenWind.RespawnTimeAmount);
        }
    }

    public override bool RightClick(int buffIndex) {
        return false;
    }
}