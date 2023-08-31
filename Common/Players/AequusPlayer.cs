using Aequus.Items.Weapons.Ranged.Bows.SkyHunterCrossbow;
using Terraria;
using Terraria.GameInput;
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

    public override void ModifyZoom(ref float zoom) {
        if (zoom < 0.5f) {
            if (Player.HeldItem.ModItem is SkyHunterCrossbow && (PlayerInput.UsingGamepad || Main.mouseRight)) {
                zoom = 0.5f;
            }
        }
    }
}