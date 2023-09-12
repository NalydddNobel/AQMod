using Aequus.Items.Weapons.Ranged.Bows.SkyHunterCrossbow;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Aequus;

public partial class AequusPlayer : ModPlayer {
    public override void Load() {
        Load_AutomaticResetEffects();
        Load_Visuals();
        On_Player.DashMovement += On_Player_DashMovement;
    }

    public override void Unload() {
        _resetEffects = null;
    }

    public override void PreUpdate() {
        PreUpdate_UpdateTimers();
    }

    public override void ModifyZoom(ref float zoom) {
        if (zoom < 0.5f) {
            if (Player.HeldItem.ModItem is SkyHunterCrossbow && (PlayerInput.UsingGamepad || Main.mouseRight)) {
                zoom = 0.5f;
            }
        }
    }

    public override void PostUpdateEquips() {
        PostUpdateEquips_WeightedHorseshoe();
    }

    #region Misc
    private struct MiscDamageHit {
        public DamageClass DamageClass;
        public Rectangle DamagingHitbox;
        public double Damage;
        public float Knockback;
    }
    #endregion
}