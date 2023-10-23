using Aequus.Common.Items.Components;
using Aequus.Common.UI;
using Aequus.Content.Items.Weapons.Ranged.Bows.SkyHunterCrossbow;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Aequus;

public partial class AequusPlayer : ModPlayer {
    public Vector2 transitionVelocity;

    public override void Load() {
        _resetEffects = new();
        _resetEffects.Generate();
        LoadVisuals();
        On_Player.HasUnityPotion += Player_HasUnityPotion;
        On_Player.TakeUnityPotion += Player_TakeUnityPotion;
        On_Player.GetRespawnTime += On_Player_GetRespawnTime;
        On_Player.DashMovement += On_Player_DashMovement;
        On_Player.PlaceThing_PaintScrapper_LongMoss += On_Player_PlaceThing_PaintScrapper_LongMoss;
    }

    public override void Unload() {
        _resetEffects = null;
    }

    public override void PreUpdate() {
        EquipmentModifierUpdate = false;
        CheckExtraInventoryMax();
        UpdateTimers();
    }

    public override void UpdateEquips() {
        EquipmentModifierUpdate = true;
    }

    public override void PostUpdateEquips() {
        UpdateWeightedHorseshoe();
        UpdateNeutronYogurt();
        UpdateTeamEffects();
    }

    public override void PostUpdateMiscEffects() {
        HandleTileEffects();
        if ((transitionVelocity - Player.velocity).Length() < 0.01f) {
            transitionVelocity = Player.velocity;
        }
        transitionVelocity = Vector2.Lerp(transitionVelocity, Player.velocity, 0.25f);
    }

    public override void PostUpdate() {
        UpdateDangers();
        EquipmentModifierUpdate = false;
    }

    public override void ModifyZoom(ref float zoom) {
        if (zoom < 0.5f) {
            if (Player.HeldItem.ModItem is SkyHunterCrossbow && (PlayerInput.UsingGamepad || Main.mouseRight)) {
                zoom = 0.5f;
            }
        }
    }

    public override bool HoverSlot(Item[] inventory, int context, int slot) {
        bool returnValue = false;
        if (inventory[slot].ModItem is IHoverSlot hoverSlot) {
            returnValue |= hoverSlot.HoverSlot(inventory, context, slot);
        }
        if (UISystem.TalkInterface?.CurrentState is AequusUIState aequusUI) {
            returnValue |= aequusUI.HoverSlot(inventory, context, slot);
        }
        return returnValue;
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