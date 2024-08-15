using Aequus;
using Aequus.Common.GUI;
using Aequus.Common.Items;
using Aequus.Common.UI;
using Aequus.Common.Utilities;
using System;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.Localization;

namespace Aequus.Systems;

public interface ICooldownItem {
    int CooldownTime { get; }

    string TimerId => GetType().Name;
    bool ShowCooldownTip => true;
    bool HasCooldownShakeEffect => true;

    void CustomPreDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        spriteBatch.Draw(TextureAssets.Item[item.type].Value, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
    }
}

public sealed class CooldownGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.ModItem is ICooldownItem;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (item.ModItem is not ICooldownItem cooldownItem || !cooldownItem.ShowCooldownTip) {
            return;
        }

        tooltips.AddTooltip(new(Mod, "CooldownTip", Language.GetTextValue("Mods.Aequus.Items.CommonTooltips.Cooldown", ALanguage.Seconds(cooldownItem.GetCooldownTime(item.prefix)))));
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
        if (Main.playerInventory) {
            slotColor *= 0.4f;
        }
        slotColor = slotColor.MultiplyRGBA(Main.inventoryBack);

        var highlightFrame = new Rectangle(cooldownFrame.X, cooldownFrame.Y + frameRemoveY, cooldownFrame.Width, 3);
        spriteBatch.Draw(texture, position - new Vector2(0f, frame.Height / 2f - frameRemoveY) * Main.inventoryScale, highlightFrame, slotColor, 0f, highlightFrame.Size() / 2f, Main.inventoryScale * 1.1f, SpriteEffects.None, 0f);

        cooldownFrame.Height -= frameRemoveY;
        cooldownFrame.Y += frameRemoveY;
        position.Y += frameRemoveY * Main.inventoryScale / 2f;

        spriteBatch.Draw(texture, position, cooldownFrame, slotColor.MultiplyRGBA(Main.inventoryBack), 0f, cooldownFrame.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
    }

    public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (item.ModItem is not ICooldownItem cooldownItem || !InventoryUI.ContextsInv.Contains(AequusUI.CurrentItemSlot.Context) || !cooldownItem.TryGetCooldown(Main.LocalPlayer, out TimerData? timer) || !timer!.Active()) {
            return true;
        }

        if (Main.playerInventory) {
            int timerFrameIndex = (int)(Main.GlobalTimeWrappedHourly * 8f) % 8;
            Texture2D timerTexture = AequusTextures.ItemCooldown;
            Rectangle timerFrame = timerTexture.Frame(verticalFrames: 8, frameY: timerFrameIndex);
            DrawBackground(timer.TimeElapsed, timer.Duration, spriteBatch, position);
            spriteBatch.Draw(timerTexture, position + new Vector2(20f, 20f) * Main.inventoryScale, timerFrame, Color.White, 0f, timerFrame.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
        }
        else {
            spriteBatch.Draw(AequusTextures.Bloom, position, null, Color.Black * Math.Min(timer.TimeElapsed / 5f, 1f) * (1f - timer.TimeElapsed / timer.Duration), 0f, AequusTextures.Bloom.Size() / 2f, Main.inventoryScale * 0.8f, SpriteEffects.None, 0f);
            DrawBackground(timer.TimeElapsed, timer.Duration, spriteBatch, position);

            // Shake effect
            if (cooldownItem.HasCooldownShakeEffect) {
                float shakeIntensity = 1f - Math.Min(timer.TimeElapsed / 15f, 1f);
                position += Main.rand.NextVector2Square(-shakeIntensity * 7f, shakeIntensity * 7f) * Main.inventoryScale;
                float shakeAnim = timer.TimeElapsed % 120f;
                if (shakeAnim > 100f) {
                    float shakeAmount = (shakeAnim - 100f) * 0.15f * Main.inventoryScale;
                    position += Main.rand.NextVector2Square(-shakeAmount, shakeAmount) * Main.inventoryScale;
                }
            }
        }

        cooldownItem.CustomPreDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        return false;
    }
}

public static class CooldownExtensions {
    public static int GetCooldownTime<T>(this T modItem, bool ignorePrefixes = false) where T : ModItem, ICooldownItem {
        return modItem.GetCooldownTime(ignorePrefixes ? 0 : modItem.Item.prefix);
    }
    public static int GetCooldownTime(this ICooldownItem cd, int prefix = 0) {
        double cooldown = cd.CooldownTime;
        /*
        if (PrefixLoader.GetPrefix(prefix) is CooldownPrefix cooldownPrefix) {
            cooldown *= cooldownPrefix.cooldownMultiplier;
        }
        */
        return (int)cooldown;
    }

    public static bool TryGetCooldown<T>(this T modItem, Player player, out TimerData? timer) where T : ModItem, ICooldownItem {
        return ((ICooldownItem)modItem).TryGetCooldown(player, out timer);
    }
    public static bool TryGetCooldown(this ICooldownItem cd, Player player, out TimerData? timer) {
        return player.GetModPlayer<TimerPlayer>().TryGetTimer(cd.TimerId, out timer);
    }

    public static bool HasCooldown<T>(this T modItem, Player player) where T : ModItem, ICooldownItem {
        return ((ICooldownItem)modItem).HasCooldown(player);
    }
    public static bool HasCooldown(this ICooldownItem cd, Player player) {
        return player.GetModPlayer<TimerPlayer>().IsTimerActive(cd.TimerId);
    }

    public static void SetCooldown<T>(this T modItem, Player player) where T : ModItem, ICooldownItem {
        ((ICooldownItem)modItem).SetCooldown(player);
    }
    public static void SetCooldown(this ICooldownItem cd, Player player) {
        player.GetModPlayer<TimerPlayer>().SetTimer(cd.TimerId, cd.CooldownTime);
    }
}