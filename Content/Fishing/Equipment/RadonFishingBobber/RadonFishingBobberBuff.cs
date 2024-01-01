namespace Aequus.Content.Fishing.Equipment.RadonFishingBobber;

public class RadonFishingBobberBuff : ModBuff {
    public override void SetStaticDefaults() {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        player.calmed = true;
        player.aggro -= 400;
    }
}