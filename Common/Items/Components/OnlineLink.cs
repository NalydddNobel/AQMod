using Aequus.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Common.Items.Components;

public interface IOnlineLink {
    string Link { get; }
}

internal sealed class OnlineLinkGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.ModItem is IOnlineLink;
    }

    public override bool InstancePerEntity => true;
    protected override bool CloneNewInstances => true;

    private uint _pressedTime;
    private float _hoverAnimation;

    private bool CanShowButton(int context) {
        return UISystem.ValidOnlineLinkedSlotContext.Contains(context);
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (item.ModItem is not IOnlineLink onlineLink || _hoverAnimation <= 0f) {
            return;
        }

        for (int i = 1; i < tooltips.Count; i++) {
            if (tooltips[i].Name == "ItemName" || tooltips[i].Mod != "Terraria") {
                continue;
            }
            tooltips.RemoveAt(i);
            i--;
        }

        tooltips.Insert(Math.Min(1, tooltips.Count), new(Mod, "OnlineLink", Language.GetTextValue("Mods.Aequus.Misc.ClickLink", onlineLink.Link)));
    }

    public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (item.ModItem is not IOnlineLink onlineLink || !CanShowButton(UISystem.CurrentItemSlot.Context)) {
            return;
        }

        var buttonPosition = position + new Vector2(14f, 14f) * Main.inventoryScale;
        if (Vector2.Distance(Main.MouseScreen, buttonPosition) > 200f) {
            if (_hoverAnimation > -2f) {
                _hoverAnimation -= 0.01f;
                if (_hoverAnimation < -1f) {
                    _hoverAnimation -= 0.05f;
                }
                if (_hoverAnimation < -2f) {
                    _hoverAnimation = -2f;
                }
            }
        }
        else if (_hoverAnimation < 0f) {
            _hoverAnimation = 0f;
        }
        var buttonTexture = AequusTextures.OnlineLink.Value;
        int context = Math.Abs(UISystem.CurrentItemSlot.Context);
        var buttonFrame = buttonTexture.Frame(verticalFrames: 2, frameY: 0);
        float hoverAnimation = Math.Max(_hoverAnimation, 0f);
        float buttonScale = 1f + 0.2f * hoverAnimation;
        float buttonRotation = MathF.Sin(hoverAnimation * MathHelper.TwoPi) * 0.2f;
        float buttonOpacity = 1f;
        if (_hoverAnimation < -1f) {
            buttonOpacity += (_hoverAnimation + 1f) * 0.5f;
        }
        spriteBatch.Draw(buttonTexture, buttonPosition, buttonFrame, Main.inventoryBack * buttonOpacity, buttonRotation, buttonFrame.Size() / 2f, buttonScale, SpriteEffects.None, 0f);

        if (!Utils.CenteredRectangle(buttonPosition, buttonTexture.Size() * buttonScale).Contains(Main.mouseX, Main.mouseY) || !UISystem.CanDoLeftClickItemActions || UISystem.linkClickDelay != 0) {
            if (_hoverAnimation > 0f) {
                _hoverAnimation *= 0.9f;
                _hoverAnimation -= 0.01f;
                if (_hoverAnimation < 0f) {
                    _hoverAnimation = 0f;
                }
            }
            return;
        }
        spriteBatch.Draw(buttonTexture, buttonPosition, buttonFrame.Frame(0, 1), Color.White, buttonRotation, buttonFrame.Size() / 2f, buttonScale, SpriteEffects.None, 0f);

        if (_hoverAnimation < 1f) {
            _hoverAnimation += 0.1f;
            if (_hoverAnimation > 1f) {
                _hoverAnimation = 1f;
            }
        }

        UISystem.DisableItemLeftClick = 2;

        if (Main.mouseLeft && Main.mouseLeftRelease) {
            if (_pressedTime < Main.GameUpdateCount) {
                _pressedTime = Main.GameUpdateCount + 30;
                return;
            }
            Main.mouseLeftRelease = false;
            Main.NewText(Language.GetTextValue("Mods.Aequus.Misc.OpenLink", onlineLink.Link), TextHelper.EventMessageColor);
            UISystem.linkClickDelay = 60;
            Utils.OpenToURL(onlineLink.Link);
        }
    }
}
