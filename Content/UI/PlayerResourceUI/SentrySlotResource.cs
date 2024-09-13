#if CUSTOM_RESOURCE_UI
using Aequus.Common.UI;
using System;
using Terraria.Localization;

namespace Aequus.Content.UI.PlayerResourceUI;

public class SentrySlotResource : PlayerResource {
    public float slotsFilled;
    public int maxMinions => Main.LocalPlayer.maxTurrets;
    public int minionCount => Main.LocalPlayer.GetModPlayer<AequusPlayer>().turretSlotCount;

    public LocalizedText? ResourceText { get; private set; }

    public override void DrawFancy(SpriteBatch sb, ResourceDrawInfo drawInfo) {
        Texture2D texture = AequusTextures.ResourceBars.Value;

        Vector2 drawLocation = drawInfo.Position + new Vector2(16f, 16f);

        Color drawColor = Color.White * drawInfo.Opacity;
        if (maxMinions == 1) {
            Rectangle frame = new Rectangle(170, 36, 26, 36);
            sb.DrawAlign(texture, drawLocation, frame, drawColor, 0f, 1f, SpriteEffects.None);
            DrawFancyCrystal(sb, texture, drawLocation + new Vector2(0f, 3f), 0, frame with { Y = 0 }, drawColor);
            return;
        }

        for (int i = 0; i < maxMinions; i++) {
            int dir = i % 2 == 0 ? -1 : 1;
            Rectangle frame = new Rectangle(170, dir == 1 ? 116 : 156, 26, 36);
            if (i == 0) {
                frame.Y = 76;
            }
            if (i == maxMinions - 1) {
                frame.Y = dir == -1 ? 236 : 196;
            }

            Vector2 drawCoords = drawLocation + new Vector2(dir == -1 ? -4f : 4f, 20f * i);
            sb.DrawAlign(texture, drawCoords, frame, drawColor, 0f, 1f, SpriteEffects.None);

            DrawFancyCrystal(sb, texture, drawCoords + new Vector2(0f, 3f), i, frame with { Y = 0 }, drawColor);
        }
    }

    void DrawFancyCrystal(SpriteBatch sb, Texture2D texture, Vector2 drawCoords, int i, Rectangle frame, Color drawColor) {
        float amount = Math.Min(slotsFilled - i, 1f);
        if (amount > 0) {
            //if (amount == 1f && (int)slotsFilled == i + 1) {
            //    amount += Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0f, 0.1f);
            //}
            sb.DrawAlign(texture, drawCoords, frame, drawColor * amount, 0f, amount, SpriteEffects.None);
        }
    }

    public override void DrawClassic(SpriteBatch sb, ResourceDrawInfo drawInfo) {
        Texture2D texture = AequusTextures.ResourceBars.Value;

        Vector2 drawLocation = drawInfo.Position + new Vector2(16f, 16f);

        Color drawColor = Color.White * drawInfo.Opacity;

        Rectangle frame = new Rectangle(84, 102, 14, 24);
        float finalPulse = 0.66f + Main.cursorAlpha * 0.66f;
        if (maxMinions == 1) {
            sb.DrawAlign(texture, drawLocation, frame, drawColor, 0f, finalPulse, SpriteEffects.None);
            return;
        }

        for (int i = 0; i < maxMinions; i++) {
            int dir = i % 2 == 0 ? -1 : 1;
            Vector2 drawCoords = drawLocation + new Vector2(dir == -1 ? -6f : 4f, 16f * i);

            float amount = 0.7f + Math.Clamp(slotsFilled - i, 0f, 1f) * 0.3f;
            float scale = i == minionCount - 1 ? finalPulse : 1f;
            sb.DrawAlign(texture, drawCoords, frame, drawColor * amount, 0f, scale * amount, SpriteEffects.None);
        }
    }

    public override void DrawBar(SpriteBatch sb, ResourceDrawInfo drawInfo) {
        Texture2D texture = AequusTextures.ResourceBars.Value;

        Vector2 drawLocation = new Vector2(Main.screenWidth - 86, 36f);
        Color drawColor = Color.White * drawInfo.Opacity;
        Rectangle leftFrame = new Rectangle(60, 52, 44, 40);

        Rectangle barFrame = new Rectangle(38, 52, 12, 30);
        for (int i = 0; i < maxMinions; i++) {
            int dir = i % 2 == 0 ? -1 : 1;
            Rectangle frame = new Rectangle(136, dir == 1 ? 116 : 156, 26, 34);
            if (i == 0) {
                frame.Y = 76;
            }
            if (i == maxMinions - 1) {
                frame.Y = dir == -1 ? 236 : 196;
            }

            Vector2 drawCoords = (drawLocation + new Vector2(-barFrame.Width * i + 6f, 0f)).Floor();
            sb.Draw(texture, drawCoords, barFrame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            float amount = Math.Min(minionCount - i, 1f);
            if (amount > 0) {
                //int fill = (int)(barFrame.Width / 2f * amount) * 2;

                //sb.Draw(texture, drawCoords + new Vector2(barFrame.Width - fill, 0), new Rectangle(18 + barFrame.Width - fill, barFrame.Y, fill, barFrame.Height), drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                sb.Draw(texture, drawCoords, barFrame with { X = 18 }, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

        sb.Draw(texture, drawLocation, leftFrame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        sb.Draw(texture, drawLocation + new Vector2(-barFrame.Width * (maxMinions - 1), 0f), barFrame with { X = 2, }, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    }

    public override void Update(ResourceDrawInfo drawInfo) {
        if (slotsFilled < minionCount) {
            slotsFilled += AequusUI.Elapsed * 5f;

            if (slotsFilled > minionCount) {
                slotsFilled = minionCount;
            }
        }
        else if (slotsFilled > minionCount) {
            slotsFilled -= AequusUI.Elapsed * 20f;

            if (slotsFilled < minionCount) {
                slotsFilled = minionCount;
            }
        }
    }

    public override void Load() {
        Instance<CustomResourceUI>().Sentry = this;
        ResourceText = this.GetLocalization("DisplayName");
        ResourceColor = new Color(120, 255, 90);
    }

    protected override bool Active(Player player, ResourcesPlayer resources, Item heldItem) {
        return heldItem.shoot > ProjectileID.None && heldItem.shoot < ProjectileLoader.ProjectileCount && ContentSamples.ProjectilesByType[heldItem.shoot].sentry;
    }
}
#endif