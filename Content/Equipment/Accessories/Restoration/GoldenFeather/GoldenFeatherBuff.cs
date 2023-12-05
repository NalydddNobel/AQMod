using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Equipment.Accessories.Restoration.GoldenFeather;

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
        if (aequusPlayer.respawnTimeModifier > GoldenFeather.RespawnTimeAmount) {
            aequusPlayer.respawnTimeModifier = Math.Max(aequusPlayer.respawnTimeModifier + GoldenFeather.RespawnTimeAmount, GoldenFeather.RespawnTimeAmount);
        }
    }

    public override bool RightClick(int buffIndex) {
        return false;
    }
}