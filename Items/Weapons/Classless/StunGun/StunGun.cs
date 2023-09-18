using Aequus.Common.Items;
using Aequus.Common.Items.Components;
using Aequus.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Classless.StunGun;

public class StunGun : ClasslessWeapon, ICooldownItem {
    public static float VisualTimer => Main.GlobalTimeWrappedHourly * 5f;
    public static int DebuffTime = 180;
    public static int CooldownTime = 480;

    public const string TimerId = "StunGun";

    int ICooldownItem.CooldownTime => CooldownTime;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.Seconds(DebuffTime), TextHelper.Seconds(CooldownTime));

    public override void SetDefaults() {
        Item.SetWeaponValues(20, 0.5f);
        Item.DamageType = ModContent.GetInstance<GenericDamageClassNoCrit>();
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useAnimation = 10;
        Item.useTime = 10;
        Item.mana = 60;
        Item.rare = ItemCommons.Rarity.SkyMerchantShopItem;
        Item.value = ItemCommons.Price.SkyMerchantShopItem;
        Item.shoot = ModContent.ProjectileType<StunGunProj>();
        Item.UseSound = SoundID.DD2_LightningBugZap;
        Item.shootSpeed = 12f;
        Item.noMelee = true;
        Item.shootsEveryUse = true;
    }

    public override bool CanUseItem(Player player) {
        return !player.GetModPlayer<AequusPlayer>().TimerActive(TimerId);
    }

    public override bool? UseItem(Player player) {
        player.GetModPlayer<AequusPlayer>().SetTimer(TimerId, CooldownTime);
        return true;
    }

    public override Vector2? HoldoutOffset() {
        return new Vector2(-2f, 0f);
    }

    public static float GetVisualTime(float time, bool front) {
        return front ? time % MathHelper.Pi + MathHelper.Pi - MathHelper.PiOver2 : time % MathHelper.Pi - MathHelper.PiOver2;
    }

    public static Vector2 GetVisualOffset(NPC npc, float time) {
        return new Vector2(npc.width * 1.1f * MathF.Sin(time), MathF.Sin(Main.GlobalTimeWrappedHourly * 10.8f + npc.whoAmI));
    }

    public static float GetVisualScale(NPC npc) {
        return MathF.Max(npc.Size.Length() / 50f, 1f);
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (UIHelper.CurrentlyDrawingHotbarSlot) {
            if (AequusPlayer.LocalTimers.TryGetValue(TimerId, out var timer) && timer.Active) {

                UIHelper.InventoryDrawCentered(spriteBatch, AequusTextures.Bloom, position, null,
                    Color.Black * Math.Min(timer.TimePassed / 5f, 1f) * (1f - timer.TimePassed / timer.MaxTime), 0f, AequusTextures.Bloom.Size() / 2f, Main.inventoryScale * 0.8f);

                float shakeIntensity = 1f - Math.Min(timer.TimePassed / 15f, 1f);
                position += Main.rand.NextVector2Square(-shakeIntensity * 7f, shakeIntensity * 7f) * Main.inventoryScale;
                float shakeAnim = timer.TimePassed % 120f;
                if (shakeAnim > 100f) {
                    float shakeAmount = (shakeAnim - 100f) * 0.15f * Main.inventoryScale;
                    position += Main.rand.NextVector2Square(-shakeAmount, shakeAmount) * Main.inventoryScale;
                }
            }
        }

        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
        return false;
    }

    public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (UIHelper.CurrentlyDrawingHotbarSlot) {
            if (AequusPlayer.LocalTimers.TryGetValue(TimerId, out var timer) && timer.Active) {
                float intensity = Math.Min(timer.TimePassed / 15f, 1f);
                UIHelper.InventoryDrawCentered(spriteBatch, AequusTextures.Bloom, position, null, Color.White * 0.1f, 0f, AequusTextures.Bloom.Size() / 2f, Main.inventoryScale * 0.8f);
                UIHelper.InventoryDrawCentered(spriteBatch, TextureAssets.Cd.Value, position, null, Color.White * (1f - timer.TimePassed / timer.MaxTime) * 0.8f, 0f, TextureAssets.Cd.Value.Size() / 2f, Main.inventoryScale);
                //UIHelper.InventoryDrawCentered(spriteBatch, TextureAssets.SettingsPanel.Value, position, null, Color.White with { A = 0 }, timer.TimePassed / timer.MaxTime * MathHelper.TwoPi, new Vector2(TextureAssets.SettingsPanel.Value.Width / 2f, TextureAssets.SettingsPanel.Value.Height - 4f), Main.inventoryScale);
            }
        }
    }
}