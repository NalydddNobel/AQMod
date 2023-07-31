using Aequus.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace Aequus.Content.UI.GrabBagReroll;

public class RerollSlot : UIElement {
    public List<Item> selection;
    public Item[] workingItems = new Item[3];
    public float rollSpeed;
    public float rollAnimation;
    public float giveUp;
    public bool playRollAnimation;
    public bool disable;

    public void BeginRoll(List<Item> selection) {
        rollSpeed = 0.12f;
        disable = false;
        playRollAnimation = true;
        this.selection = selection;
    }

    public override void Update(GameTime gameTime) {
        base.Update(gameTime);

        if (!playRollAnimation) {
            if (rollSpeed > 0f) {
                rollSpeed -= 0.001f;
                rollSpeed *= 0.95f;
                if (rollSpeed < 0f) {
                    rollSpeed = 0f;
                }
            }
            giveUp = 0f;
            if (rollSpeed == 0f) {
                rollAnimation %= 1f;
                rollAnimation = MathHelper.Lerp(rollAnimation, 0.5f, 0.1f);
            }
        }
        else {
            giveUp++;
            if (giveUp > 600) {
                giveUp = 0f;
                playRollAnimation = false;
            }
        }
        rollAnimation += rollSpeed;
        if (rollAnimation > 1f) {
            for (int i = workingItems.Length - 1; i > 0; i--) {
                workingItems[i] = workingItems[i - 1];
            }
            workingItems[0] = selection == null || selection.Count == 0 ?
                null : Main.rand.Next(selection);
            rollAnimation = 0f;
        }
    }

    public void EndRoll() {
        playRollAnimation = false;
        rollAnimation *= 0.98f;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch) {
        var dimensions = GetDimensions();

        if (workingItems[1] != null && new Rectangle((int)dimensions.X, (int)dimensions.Y + (int)dimensions.Height / 2 - 24, (int)dimensions.Width, 48).Contains(Main.mouseX, Main.mouseY)) {
            AequusUI.HoverItem(workingItems[1], -1);
        }

        Main.spriteBatch.End();
        Main.spriteBatch.Begin_UI(immediate: false, useScissorRectangle: true);
        Main.graphics.GraphicsDevice.ScissorRectangle = dimensions.ToRectangle();

        float heightAdd = dimensions.Height / workingItems.Length;
        float rollAnimationWrapped = rollAnimation % 1f;
        float animationOffset = 1f / workingItems.Length;
        Helper.DrawUIPanel(spriteBatch, AequusTextures.Panel_BountyBoard, dimensions.ToRectangle());
        float waveAmount = Math.Min(0.1f - rollSpeed, 0f);
        for (int i = 0; i < workingItems.Length; i++) {
            //float opacity = MathF.Sin((i + rollAnimation) / workingItems.Length * MathHelper.TwoPi);

            float progress = Math.Clamp(i / 2f + rollAnimation * 0.5f - 0.25f, 0f, 1f);

            if (progress < 0.5f) {
                progress = 1f - (float)Math.Pow(1f - progress / 0.5f, 2f) * 0.5f - 0.5f;
            }
            else {
                progress = (float)Math.Pow(1f - progress / 0.5f, 2f) * 0.5f + 0.5f;
            }

            float opacity = MathF.Sin(progress * MathHelper.Pi);

            Vector2 drawPosition = new(dimensions.X + dimensions.Width / 2f, dimensions.Y + dimensions.Height * progress);

            spriteBatch.Draw(
                AequusTextures.Bloom0,
                drawPosition,
                null,
                Color.Black * opacity * 0.5f,
                0f,
                AequusTextures.Bloom0.Size() / 2f,
                0.5f * opacity,
                SpriteEffects.None, 0f
            );

            if (workingItems[i] == null || workingItems[i].IsAir) {
                spriteBatch.Draw(
                    TextureAssets.Cd.Value,
                    drawPosition,
                    null,
                    Color.White * opacity,
                    0f,
                    TextureAssets.Cd.Value.Size() / 2f,
                    Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 1f - waveAmount, 1f + waveAmount),
                    SpriteEffects.None, 0f
                );
                continue;
            }

            Main.instance.LoadItem(workingItems[i].type);
            var texture = TextureAssets.Item[workingItems[i].type].Value;
            workingItems[i].GetItemDrawData(out var frame);
            float scale = 1f;
            int largestSide = Math.Max(frame.Width, frame.Height);
            if (largestSide > 40f) {
                scale = 40f / largestSide;
            }
            spriteBatch.Draw(
                texture,
                drawPosition,
                frame,
                Color.White * opacity,
                0f,
                frame.Size() / 2f,
                scale * opacity * Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 1f - waveAmount, 1f + waveAmount),
                SpriteEffects.None, 0f
            );
        }

        Main.spriteBatch.End();
        Main.spriteBatch.Begin_UI(immediate: false, useScissorRectangle: false);
    }
}