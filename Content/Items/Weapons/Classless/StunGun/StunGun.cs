using AequusRemake.Core;
using AequusRemake.Systems.Elements;
using AequusRemake.Core.Entities.DamageClasses;
using AequusRemake.Core.Entities.Items.Components;
using System;
using Terraria.Localization;

namespace AequusRemake.Content.Items.Weapons.Classless.StunGun;

public class StunGun : ModItem, ICooldownItem {
    public static float VisualTimer => Main.GlobalTimeWrappedHourly * 5f;
    public static int DebuffTime { get; set; } = 180;
    public static int CooldownTime { get; set; } = 480;

    int ICooldownItem.CooldownTime => CooldownTime;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ALanguage.Seconds(DebuffTime), ALanguage.Seconds(CooldownTime));

    public override void SetStaticDefaults() {
        Element.Flame.AddItem(Type);
    }

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.SetWeaponValues(20, 0.5f);
        Item.DamageType = ModContent.GetInstance<OmniDamageClassNoCrit>();
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useAnimation = 10;
        Item.useTime = 10;
        Item.mana = 60;
        Item.rare = Commons.Rare.NPCSkyMerchant;
        Item.value = Commons.Cost.NPCSkyMerchant;
        Item.shoot = ModContent.ProjectileType<StunGunProj>();
        Item.UseSound = SoundID.DD2_LightningBugZap;
        Item.shootSpeed = 12f;
        Item.noMelee = true;
        Item.shootsEveryUse = true;
    }

    public override bool CanUseItem(Player player) {
        return !this.HasCooldown(player);
    }

    public override bool? UseItem(Player player) {
        this.SetCooldown(player);
        return true;
    }

    public override Vector2? HoldoutOffset() {
        return new Vector2(6f, 0f);
    }

    #region Debuff Effect
    public static float GetVisualTime(float time, bool front) {
        return front ? time % MathHelper.Pi + MathHelper.Pi - MathHelper.PiOver2 : time % MathHelper.Pi - MathHelper.PiOver2;
    }

    public static Vector2 GetVisualOffset(int entityWidth, float time, int randomizer = 0) {
        return new Vector2(entityWidth * 1.1f * MathF.Sin(time), MathF.Sin(Main.GlobalTimeWrappedHourly * 10.8f + randomizer));
    }

    public static float GetVisualScale(float entitySize) {
        return MathF.Max(entitySize / 50f, 1f);
    }

    public static void DrawDebuffVisual(NPC npc, SpriteBatch spriteBatch, float waveTime) {
        var drawLocation = npc.Center + GetVisualOffset(npc.width, waveTime, npc.whoAmI);
        float scale = GetVisualScale(npc.Size.Length());
        spriteBatch.Draw(AequusTextures.StunEffect, drawLocation - Main.screenPosition, null, Color.White with { A = 0 }, 0f, AequusTextures.StunEffect.Size() / 2f, (0.9f + MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.1f) * scale, SpriteEffects.None, 0f);
    }
    #endregion
}