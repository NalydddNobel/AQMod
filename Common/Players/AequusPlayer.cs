using Aequus.Content.Weapons.Ranged.Bows.SkyHunterCrossbow;
using Aequus.Core.CodeGeneration;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.UI;

namespace Aequus;

public partial class AequusPlayer : ModPlayer {
    public Vector2 transitionVelocity;

    public int timeSinceRespawn;

    [ResetEffects]
    public StatModifier wingTime;

    public override void Load() {
        _resetEffects = new();
        _resetEffects.Generate();
        On_Player.UpdateVisibleAccessories += On_Player_UpdateVisibleAccessories;
        On_PlayerDrawLayers.DrawPlayer_RenderAllLayers += PlayerDrawLayers_DrawPlayer_RenderAllLayers;
        On_ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick;
        On_ChestUI.QuickStack += On_ChestUI_QuickStack;
        On_Player.QuickStackAllChests += On_Player_QuickStackAllChests;
        On_Player.ConsumeItem += On_Player_ConsumeItem;
        On_Player.QuickMount_GetItemToUse += On_Player_QuickMount_GetItemToUse;
        On_Player.QuickHeal_GetItemToUse += On_Player_QuickHeal_GetItemToUse;
        On_Player.QuickMana_GetItemToUse += On_Player_QuickMana_GetItemToUse;
        On_Player.HasUnityPotion += Player_HasUnityPotion;
        On_Player.TakeUnityPotion += Player_TakeUnityPotion;
        On_Player.GetRespawnTime += On_Player_GetRespawnTime;
        On_Player.DashMovement += On_Player_DashMovement;
        On_Player.PlaceThing_PaintScrapper_LongMoss += On_Player_PlaceThing_PaintScrapper_LongMoss;
    }

    public override void Unload() {
        _resetEffects = null;
    }

    public override void Initialize() {
        Timers = new();
    }

    public override void OnRespawn() {
        timeSinceRespawn = 0;
    }

    public override void OnEnterWorld() {
        timeSinceRespawn = 0;
    }

    public override void PreUpdate() {
        UpdateTimers();
        UpdateItemFields();
    }

    public override void PostUpdateEquips() {
        UpdateCosmicChest();
        UpdateWeightedHorseshoe();
        UpdateNeutronYogurt();
        UpdateTeamEffects();
        Player.wingTimeMax = (int)wingTime.ApplyTo(Player.wingTimeMax);
#if !DEBUG
        UpdateLegacyNecromancyAccs();
#endif
    }

    public override void PostUpdateMiscEffects() {
        HandleTileEffects();
        UpdateScrapBlockState();
        if ((transitionVelocity - Player.velocity).Length() < 0.01f) {
            transitionVelocity = Player.velocity;
        }
        transitionVelocity = Vector2.Lerp(transitionVelocity, Player.velocity, 0.25f);
    }

    public override void PostUpdate() {
        UpdateDangers();
        timeSinceRespawn++;
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