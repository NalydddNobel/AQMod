using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items.Tools.Building {
    public class OmniPaint : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.RedPaint);
            Item.paint = 0;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(silver: 2);
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
        }

        public override void UpdateInfoAccessory(Player player) {
            if (Main.myPlayer != player.whoAmI) {
                return;
            }

            var ui = ModContent.GetInstance<OmniPaintUI>();
            if (!Main.playerInventory) {
                ui.Enabled = true;
            }
            Item.paint = Math.Max(ui.SelectedPaint, (byte)1);
            Item.paintCoating = ui.SelectedCoating;
        }
    }

    public class OmniPaintUI : AequusUserInterface {
        public record struct PaintOrCoatingInfo(byte PaintID, int ItemID);

        public static readonly List<PaintOrCoatingInfo> NormalPaints = new();
        public static readonly List<PaintOrCoatingInfo> MiscPaints = new();
        public static readonly List<PaintOrCoatingInfo> DeepPaints = new();
        public static readonly List<PaintOrCoatingInfo> Coatings = new();
        public static readonly HashSet<int> Paintbrush = new();

        public bool Enabled;
        public byte SelectedPaint;
        public byte SelectedCoating;
        public Vector2 location;
        public float activateAnimation;

        public bool Visible => location.X > 0f;

        public override string Layer => AequusUI.InterfaceLayers.WireSelection_11;
        public override InterfaceScaleType ScaleType => InterfaceScaleType.UI;

        public override void Load() {
            NormalPaints.Add(new(PaintID.RedPaint, ItemID.RedPaint));
            NormalPaints.Add(new(PaintID.OrangePaint, ItemID.OrangePaint));
            NormalPaints.Add(new(PaintID.YellowPaint, ItemID.YellowPaint));
            NormalPaints.Add(new(PaintID.LimePaint, ItemID.LimePaint));
            NormalPaints.Add(new(PaintID.GreenPaint, ItemID.GreenPaint));
            NormalPaints.Add(new(PaintID.TealPaint, ItemID.TealPaint));
            NormalPaints.Add(new(PaintID.CyanPaint, ItemID.CyanPaint));
            NormalPaints.Add(new(PaintID.SkyBluePaint, ItemID.SkyBluePaint));
            NormalPaints.Add(new(PaintID.BluePaint, ItemID.BluePaint));
            NormalPaints.Add(new(PaintID.PurplePaint, ItemID.PurplePaint));
            NormalPaints.Add(new(PaintID.VioletPaint, ItemID.VioletPaint));
            NormalPaints.Add(new(PaintID.PinkPaint, ItemID.PinkPaint));

            MiscPaints.Add(new(PaintID.WhitePaint, ItemID.WhitePaint));
            MiscPaints.Add(new(PaintID.GrayPaint, ItemID.GrayPaint));
            MiscPaints.Add(new(PaintID.BlackPaint, ItemID.BlackPaint));
            MiscPaints.Add(new(PaintID.BrownPaint, ItemID.BrownPaint));
            MiscPaints.Add(new(PaintID.ShadowPaint, ItemID.ShadowPaint));
            MiscPaints.Add(new(PaintID.NegativePaint, ItemID.NegativePaint));

            DeepPaints.Add(new(PaintID.DeepRedPaint, ItemID.DeepRedPaint));
            DeepPaints.Add(new(PaintID.DeepOrangePaint, ItemID.DeepOrangePaint));
            DeepPaints.Add(new(PaintID.DeepYellowPaint, ItemID.DeepYellowPaint));
            DeepPaints.Add(new(PaintID.DeepLimePaint, ItemID.DeepLimePaint));
            DeepPaints.Add(new(PaintID.DeepGreenPaint, ItemID.DeepGreenPaint));
            DeepPaints.Add(new(PaintID.DeepTealPaint, ItemID.DeepTealPaint));
            DeepPaints.Add(new(PaintID.DeepCyanPaint, ItemID.DeepCyanPaint));
            DeepPaints.Add(new(PaintID.DeepSkyBluePaint, ItemID.DeepSkyBluePaint));
            DeepPaints.Add(new(PaintID.DeepBluePaint, ItemID.DeepBluePaint));
            DeepPaints.Add(new(PaintID.DeepPurplePaint, ItemID.DeepPurplePaint));
            DeepPaints.Add(new(PaintID.DeepVioletPaint, ItemID.DeepVioletPaint));
            DeepPaints.Add(new(PaintID.DeepPinkPaint, ItemID.DeepPinkPaint));

            Coatings.Add(new(PaintCoatingID.Glow, ItemID.GlowPaint));
            Coatings.Add(new(PaintCoatingID.Echo, ItemID.EchoCoating));

            Paintbrush.Add(ItemID.Paintbrush);
            Paintbrush.Add(ItemID.SpectrePaintbrush);
            Paintbrush.Add(ItemID.PaintRoller);
            Paintbrush.Add(ItemID.SpectrePaintRoller);
        }

        private void OnRightClick() {
            if (!Visible) {
                location = Main.MouseScreen;
                activateAnimation = 0f;
                return;
            }
            if (Vector2.Distance(Main.MouseScreen, location) > 100f) {
                location = Vector2.Zero;
                return;
            }
        }

        public override void OnPreUpdatePlayers() {
            Enabled = false;
            if (Main.mouseRight && Main.mouseRightRelease) {
                OnRightClick();
            }
        }

        public override void OnUIUpdate(GameTime gameTime) {
            if (!Enabled) {
                location.X = 0f;
                activateAnimation = 0f;
                return;
            }

            if (activateAnimation < 1f) {
                activateAnimation += 0.05f;
                if (activateAnimation > 1f) {
                    activateAnimation = 1f;
                }
            }
        }

        private void DrawPaint(SpriteBatch spriteBatch, PaintOrCoatingInfo paint, Vector2 spinningPoint, int i, int count, ref byte selection, float drawScale = 1f) {
            Main.instance.LoadItem(paint.ItemID);
            Helper.GetItemDrawData(paint.ItemID, out var frame);

            var texture = TextureAssets.Item[paint.ItemID].Value;
            float rotation = MathHelper.TwoPi / count * i;
            // rotation += (1f - MathF.Pow(activateAnimation, 3f)) * MathHelper.PiOver2;
            var drawLocation = location + spinningPoint.RotatedBy(rotation);
            bool hovering = Utils.CenteredRectangle(drawLocation, frame.Size() * 1.1f).Contains(Main.mouseX, Main.mouseY);
            bool selected = selection == paint.PaintID;
            var clr = (Color.White * ((selected || hovering) ? 1f : 0.8f)) with { A = 255, } * activateAnimation;
            float scale = 1f * drawScale;
            var outlineColor = hovering ? Color.White : selected ? Main.OurFavoriteColor : Color.Transparent;
            var paintOrigin = frame.Size() / 2f;
            float paintScale = scale * (hovering ? 1.2f : 1f);

            spriteBatch.Draw(
                AequusTextures.Bloom0,
                drawLocation,
                null,
                WorldGen.paintColor(paint.PaintID) * 0.6f * activateAnimation,
                0f,
                AequusTextures.Bloom0.Size() / 2f,
                scale * 0.4f,
                SpriteEffects.None,
                0f
            );


            if (outlineColor != Color.Transparent) {
                spriteBatch.End();
                spriteBatch.Begin_UI(immediate: true);
                var colorOnlyShader = Helper.ShaderColorOnly;
                colorOnlyShader.Apply(null, null);

                for (int j = 0; j < 4; j++) {
                    spriteBatch.Draw(
                        texture,
                        drawLocation + (MathHelper.PiOver2 * j).ToRotationVector2() * 2f * paintScale,
                        frame,
                        outlineColor,
                        0f,
                        paintOrigin,
                        paintScale,
                        SpriteEffects.None,
                        0f
                    );
                }

                spriteBatch.End();
                spriteBatch.Begin_UI(immediate: false);
            }
            spriteBatch.Draw(
                texture,
                drawLocation,
                frame,
                clr,
                0f,
                paintOrigin,
                paintScale,
                SpriteEffects.None,
                0f
            );

            if (hovering) {
                if (Main.mouseLeft && Main.mouseLeftRelease) {
                    if (selection == paint.PaintID) {
                        selection = 0;
                    }
                    else {
                        selection = paint.PaintID;
                    }
                }
                var player = Main.LocalPlayer;
                player.mouseInterface = true;
                Main.instance.MouseText(Lang.GetItemName(paint.ItemID).Value, 1);
            }
            // Helper.DebugTextDraw(paint.PaintID, drawLocation + new Vector2(frame.Width, frame.Height) * 0.4f, 0.75f);
        }

        public override bool Draw(SpriteBatch spriteBatch) {
            if (location.X <= 0f) {
                return true;
            }

            spriteBatch.Draw(
                AequusTextures.Bloom0,
                location,
                null,
                Color.Black * 0.75f,
                0f,
                AequusTextures.Bloom0.Size() / 2f,
                1f,
                SpriteEffects.None,
                0f
            );

            var spinningPoint = new Vector2(0f, -60f - MathF.Pow(activateAnimation, 2f) * 30f);
            for (int i = 0; i < NormalPaints.Count; i++) {
                DrawPaint(spriteBatch, NormalPaints[i], spinningPoint, i, NormalPaints.Count, ref SelectedPaint);
            }
            for (int i = 0; i < DeepPaints.Count; i++) {
                DrawPaint(spriteBatch, DeepPaints[i], spinningPoint * 0.66f, i, DeepPaints.Count, ref SelectedPaint);
            }
            for (int i = 0; i < MiscPaints.Count; i++) {
                DrawPaint(spriteBatch, MiscPaints[i], spinningPoint * 0.33f, i, MiscPaints.Count, ref SelectedPaint);
            }
            for (int i = 0; i < Coatings.Count; i++) {
                DrawPaint(spriteBatch, Coatings[i], spinningPoint * 1.3f, i, Coatings.Count, ref SelectedCoating);
            }
            return true;
        }
    }
}