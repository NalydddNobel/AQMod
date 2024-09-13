#if CUSTOM_RESOURCE_UI
using System;

namespace Aequus.Content.UI.PlayerResourceUI;

public class MinionSlotResource : PlayerResource {
    public float slotsFilled;

    public override void DrawFancy(SpriteBatch sb, ResourceDrawInfo drawInfo) {
        Texture2D texture = AequusTextures.ResourceBars.Value;
        int minionCount = Main.LocalPlayer.numMinions;

        if (slotsFilled < minionCount) {
            slotsFilled += drawInfo.ElapsedTime * 5f;

            if (slotsFilled > minionCount) {
                slotsFilled = minionCount;
            }
        }
        else if (slotsFilled > minionCount) {
            slotsFilled -= drawInfo.ElapsedTime * 20f;

            if (slotsFilled < minionCount) {
                slotsFilled = minionCount;
            }
        }

        int maxMinions = Main.LocalPlayer.maxMinions;
        Vector2 drawLocation = drawInfo.Position + new Vector2(16f, 16f);

        Color drawColor = Color.White * drawInfo.Opacity;
        if (maxMinions == 1) {
            Rectangle frame = new Rectangle(136, 36, 26, 34);
            sb.DrawAlign(texture, drawLocation, frame, drawColor, 0f, 1f, SpriteEffects.None);
            DrawCrystal(sb, texture, drawLocation + new Vector2(0f, 3f), 0, frame with { Y = 0 }, drawColor);
            return;
        }

        for (int i = 0; i < maxMinions; i++) {
            int dir = i % 2 == 0 ? -1 : 1;
            Rectangle frame = new Rectangle(136, dir == 1 ? 116 : 156, 26, 34);
            if (i == 0) {
                frame.Y = 76;
            }
            if (i == maxMinions - 1) {
                frame.Y = dir == -1 ? 236 : 196;
            }

            Vector2 drawCoords = drawLocation + new Vector2(dir == -1 ? -6f : 4f, 16f * i);
            sb.DrawAlign(texture, drawCoords, frame, drawColor, 0f, 1f, SpriteEffects.None);

            DrawCrystal(sb, texture, drawCoords + new Vector2(0f, 3f), i, frame with { Y = 0 }, drawColor);
        }
    }

    void DrawCrystal(SpriteBatch sb, Texture2D texture, Vector2 drawCoords, int i, Rectangle frame, Color drawColor) {
        float amount = Math.Min(slotsFilled - i, 1f);
        if (amount > 0) {
            //if (amount == 1f && (int)slotsFilled == i + 1) {
            //    amount += Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0f, 0.1f);
            //}
            sb.DrawAlign(texture, drawCoords, frame, drawColor * amount, 0f, amount, SpriteEffects.None);
        }
    }

    public override void Load() {
        Instance<CustomResourceUI>().Minion = this;
        ResourceColor = new Color(90, 220, 230);
    }

    protected override bool Active(Player player, ResourcesPlayer resources, Item heldItem) {
        return player.maxMinions > 0 && heldItem.shoot > ProjectileID.None && heldItem.shoot < ProjectileLoader.ProjectileCount && ContentSamples.ProjectilesByType[heldItem.shoot].minion;
    }
}
#endif