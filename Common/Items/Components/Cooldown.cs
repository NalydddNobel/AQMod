using Aequus.Common.ItemPrefixes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;

namespace Aequus.Common.Items.Components;

public interface ICooldownItem {
    int CooldownTime { get; }
    string TimerId => GetType().Name;

    void CustomPreDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        spriteBatch.Draw(TextureAssets.Item[item.type].Value, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
    }
}

public sealed class CooldownGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.ModItem is ICooldownItem;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (item.ModItem is not ICooldownItem cooldownItem) {
            return;
        }

        tooltips.AddTooltip(new(Mod, "CooldownTip", Language.GetTextValue("Mods.Aequus.Items.CommonTooltips.Cooldown", TextHelper.Seconds(cooldownItem.GetCooldownTime(item.prefix)))));
    }

    private static void DrawBackground(float time, float timeMax, SpriteBatch spriteBatch, Vector2 position) {
        float progress = Math.Clamp(time / timeMax, 0f, 1f);
        var texture = TextureAssets.InventoryBack16.Value;
        var frame = texture.Frame();
        var cooldownFrame = frame;
        int frameRemoveY = cooldownFrame.Height - (int)(cooldownFrame.Height * (1f - progress));

        var slotColor = new Color(150, 150, 150, 0);
        if (time < 30f) {
            slotColor *= 1f + (1f - time / 30f);
        }
        slotColor = Utils.MultiplyRGBA(slotColor, Main.inventoryBack);

        var highlightFrame = new Rectangle(cooldownFrame.X, cooldownFrame.Y + frameRemoveY, cooldownFrame.Width, 3);
        spriteBatch.Draw(texture, position - new Vector2(0f, frame.Height / 2f - frameRemoveY) * Main.inventoryScale, highlightFrame, slotColor, 0f, highlightFrame.Size() / 2f, Main.inventoryScale * 1.1f, SpriteEffects.None, 0f);

        cooldownFrame.Height -= frameRemoveY;
        cooldownFrame.Y += frameRemoveY;
        position.Y += frameRemoveY * Main.inventoryScale / 2f;

        spriteBatch.Draw(texture, position, cooldownFrame, Utils.MultiplyRGBA(slotColor, Main.inventoryBack), 0f, cooldownFrame.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
    }

    public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (item.ModItem is not ICooldownItem cooldownItem) {
            return true;
        }
        
        if (!UIHelper.CurrentlyDrawingHotbarSlot) {
            cooldownItem.CustomPreDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            return false;
        }

        if (cooldownItem.TryGetCooldown(Main.LocalPlayer, out var timer) && timer.Active) {
            spriteBatch.Draw(AequusTextures.Bloom, position, null, Color.Black * Math.Min(timer.TimePassed / 5f, 1f) * (1f - timer.TimePassed / timer.MaxTime), 0f, AequusTextures.Bloom.Size() / 2f, Main.inventoryScale * 0.8f, SpriteEffects.None, 0f);
            DrawBackground(timer.TimePassed, timer.MaxTime, spriteBatch, position);

            float shakeIntensity = 1f - Math.Min(timer.TimePassed / 15f, 1f);
            position += Main.rand.NextVector2Square(-shakeIntensity * 7f, shakeIntensity * 7f) * Main.inventoryScale;
            float shakeAnim = timer.TimePassed % 120f;
            if (shakeAnim > 100f) {
                float shakeAmount = (shakeAnim - 100f) * 0.15f * Main.inventoryScale;
                position += Main.rand.NextVector2Square(-shakeAmount, shakeAmount) * Main.inventoryScale;
            }
        }

        cooldownItem.CustomPreDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        return false;
    }
}

public static class CooldownExtensions {
    public static int GetCooldownTime<T>(this T modItem, bool ignorePrefixes = false) where T : ModItem, ICooldownItem {
        return GetCooldownTime((ICooldownItem)modItem, ignorePrefixes ? 0 : modItem.Item.prefix);
    }
    public static int GetCooldownTime(this ICooldownItem cd, int prefix = 0) {
        double cooldown = cd.CooldownTime;
        if (PrefixLoader.GetPrefix(prefix) is CooldownPrefix cooldownPrefix) {
            cooldown *= cooldownPrefix.cooldownMultiplier;
        }
        return (int)cooldown;
    }

    public static bool TryGetCooldown<T>(this T modItem, Player player, out AequusPlayer.TimerData timer) where T : ModItem, ICooldownItem {
        return TryGetCooldown((ICooldownItem)modItem, player, out timer);
    }
    public static bool TryGetCooldown(this ICooldownItem cd, Player player, out AequusPlayer.TimerData timer) {
        return AequusPlayer.LocalTimers.TryGetValue(cd.TimerId, out timer);
    }

    public static bool HasCooldown<T>(this T modItem, Player player) where T : ModItem, ICooldownItem {
        return HasCooldown((ICooldownItem)modItem, player);
    }
    public static bool HasCooldown(this ICooldownItem cd, Player player) {
        return player.GetModPlayer<AequusPlayer>().TimerActive(cd.TimerId);
    }

    public static void SetCooldown<T>(this T modItem, Player player) where T : ModItem, ICooldownItem {
        SetCooldown((ICooldownItem)modItem, player);
    }
    public static void SetCooldown(this ICooldownItem cd, Player player) {
        player.GetModPlayer<AequusPlayer>().SetTimer(cd.TimerId, cd.CooldownTime);
    }
}