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
        On_Player.GetRespawnTime += On_Player_GetRespawnTime;
        On_Player.DashMovement += On_Player_DashMovement;
        On_Player.PlaceThing_PaintScrapper_LongMoss += On_Player_PlaceThing_PaintScrapper_LongMoss;
    }

    public override void Unload() {
        _resetEffects = null;
    }

    public override void PreUpdate() {
        UpdateTimers();
    }

    public override void PostUpdateEquips() {
        UpdateWeightedHorseshoe();
        UpdateNeutronYogurt();
        UpdateTeamEffects();
    }

    public override void PostUpdate() {
        UpdateDangers();
    }

    public override void ModifyZoom(ref float zoom) {
        if (zoom < 0.5f) {
            if (Player.HeldItem.ModItem is SkyHunterCrossbow && (PlayerInput.UsingGamepad || Main.mouseRight)) {
                zoom = 0.5f;
            }
        }
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