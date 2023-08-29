using Terraria;
using Terraria.ModLoader;

namespace Aequus;

public partial class AequusPlayer : ModPlayer {
    public override void Load() {
        On_Player.DashMovement += On_Player_DashMovement;
    }

    public override void ResetEffects() {
        if (Player.dashDelay == 0) {
            DashData = null;
        }
        ResetEffects_InformationalAccessories();
    }
}