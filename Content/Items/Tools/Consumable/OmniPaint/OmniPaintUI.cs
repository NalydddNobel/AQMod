using Aequus.Common.UI;
using Aequus.Common.Utilities;
using System;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.UI;

namespace Aequus.Content.Items.Tools.Consumable.OmniPaint;

public class OmniPaintUI : UILayer {
    public static OmniPaintUI Instance => ModContent.GetInstance<OmniPaintUI>();

    readonly record struct PaintOrCoatingInfo(byte PaintId, int ItemId, Color GlowColor, Condition? Condition = null) {
        public PaintOrCoatingInfo(byte PaintID, int ItemID, Condition? Condition = null) : this(PaintID, ItemID, WorldGen.paintColor(PaintID), Condition) { }

        public static implicit operator PaintOrCoatingInfo(ValueTuple<byte, short> tuple) {
            return new PaintOrCoatingInfo(tuple.Item1, tuple.Item2);
        }
        public static implicit operator PaintOrCoatingInfo(ValueTuple<byte, short, Color> tuple) {
            return new PaintOrCoatingInfo(tuple.Item1, tuple.Item2, tuple.Item3);
        }
        public static implicit operator PaintOrCoatingInfo(ValueTuple<byte, short, Condition> tuple) {
            return new PaintOrCoatingInfo(tuple.Item1, tuple.Item2, tuple.Item3);
        }
        public static implicit operator PaintOrCoatingInfo(ValueTuple<byte, short, Color, Condition> tuple) {
            return new PaintOrCoatingInfo(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
        }

        public bool IsUnlocked() {
            return Condition?.IsMet() ?? true;
        }
    }

    readonly record struct PaintCollection(float RadiusMultiplier, RefFunc<byte> GetValueReference, params PaintOrCoatingInfo[] Paints);

    static ref byte SelectedPaintValue() {
        return ref Instance.SelectedPaint;
    }
    static ref byte SelectedCoatingValue() {
        return ref Instance.SelectedCoating;
    }

    readonly List<PaintCollection> _paints = [
        new PaintCollection(1f, SelectedPaintValue, [
            (PaintID.RedPaint, ItemID.RedPaint),
            (PaintID.OrangePaint, ItemID.OrangePaint),
            (PaintID.YellowPaint, ItemID.YellowPaint),
            (PaintID.LimePaint, ItemID.LimePaint),
            (PaintID.GreenPaint, ItemID.GreenPaint),
            (PaintID.TealPaint, ItemID.TealPaint),
            (PaintID.CyanPaint, ItemID.CyanPaint),
            (PaintID.SkyBluePaint, ItemID.SkyBluePaint),
            (PaintID.BluePaint, ItemID.BluePaint),
            (PaintID.PurplePaint, ItemID.PurplePaint),
            (PaintID.VioletPaint, ItemID.VioletPaint),
            (PaintID.PinkPaint, ItemID.PinkPaint),
        ]),
        new PaintCollection(0.66f, SelectedPaintValue, [
            (PaintID.DeepRedPaint, ItemID.DeepRedPaint),
            (PaintID.DeepOrangePaint, ItemID.DeepOrangePaint),
            (PaintID.DeepYellowPaint, ItemID.DeepYellowPaint),
            (PaintID.DeepLimePaint, ItemID.DeepLimePaint),
            (PaintID.DeepGreenPaint, ItemID.DeepGreenPaint),
            (PaintID.DeepTealPaint, ItemID.DeepTealPaint),
            (PaintID.DeepCyanPaint, ItemID.DeepCyanPaint),
            (PaintID.DeepSkyBluePaint, ItemID.DeepSkyBluePaint),
            (PaintID.DeepBluePaint, ItemID.DeepBluePaint),
            (PaintID.DeepPurplePaint, ItemID.DeepPurplePaint),
            (PaintID.DeepVioletPaint, ItemID.DeepVioletPaint),
            (PaintID.DeepPinkPaint, ItemID.DeepPinkPaint),
        ]),
        new PaintCollection(0.33f, SelectedPaintValue, [
            (PaintID.WhitePaint, ItemID.WhitePaint),
            (PaintID.GrayPaint, ItemID.GrayPaint),
            (PaintID.BlackPaint, ItemID.BlackPaint),
            (PaintID.BrownPaint, ItemID.BrownPaint),
            (PaintID.ShadowPaint, ItemID.ShadowPaint, Condition.Hardmode),
            (PaintID.NegativePaint, ItemID.NegativePaint, Condition.Hardmode),
        ]),
        new PaintCollection(1.3f, SelectedCoatingValue, [
            (PaintCoatingID.Glow, ItemID.GlowPaint, Color.HotPink),
            // TODO -- Remove in 1.4.5.
            (PaintCoatingID.Echo, ItemID.EchoCoating, Color.Cyan, Condition.DownedPlantera),
        ]),
    ];

    public bool Enabled;
    public byte SelectedPaint;
    public byte SelectedCoating;
    public Vector2 location;
    private Vector2 scaledLocation;
    public float activateAnimation;

    public bool Visible => location.X > 0f;

    public override string Layer => AequusUI.InterfaceLayers.WireSelection_11;
    public override InterfaceScaleType ScaleType => InterfaceScaleType.UI;

    private void OnRightClick() {
        activateAnimation = 0f;
        if (!Visible) {
            location = Main.MouseScreen;
            return;
        }
        location = Vector2.Zero;
    }

    public override void OnPreUpdatePlayers() {
        if (Enabled && Main.mouseRight && Main.mouseRightRelease) {
            OnRightClick();
        }
        Enabled = false;
    }

    public override void OnUIUpdate(GameTime gameTime) {
        if (activateAnimation < 1f) {
            activateAnimation += 0.05f;
            if (activateAnimation > 1f) {
                activateAnimation = 1f;
            }
        }
    }

    private void DrawPaint(SpriteBatch spriteBatch, PaintOrCoatingInfo paint, Vector2 spinningPoint, int i, int count, ref byte selection, float drawScale = 1f) {
        Main.GetItemDrawFrame(paint.ItemId, out Texture2D texture, out Rectangle frame);

        float rotation = MathHelper.TwoPi / count * i;
        // rotation += (1f - MathF.Pow(activateAnimation, 3f)) * MathHelper.PiOver2;
        var drawLocation = scaledLocation + spinningPoint.RotatedBy(rotation);
        bool hovering = Utils.CenteredRectangle(drawLocation, frame.Size() * 1.1f).Contains(Main.mouseX, Main.mouseY);
        bool selected = selection == paint.PaintId;
        var clr = (Color.White * (selected || hovering ? 1f : 0.8f)) with { A = 255, } * activateAnimation;
        float scale = 1f * drawScale;
        var outlineColor = (selected ? Main.OurFavoriteColor : hovering ? Color.White : Color.Transparent) * MathF.Pow(activateAnimation, 2f);
        float paintScale = scale * (hovering ? 1.2f : 1f);

        if (!paint.IsUnlocked()) {
            spriteBatch.DrawAlign(texture, drawLocation, frame, Color.Black, 0f, paintScale, SpriteEffects.None);

            if (hovering) {
                Main.LocalPlayer.mouseInterface = true;
                string text = Lang.GetItemName(paint.ItemId).Value;
                if (paint.Condition?.Description?.IsValid() ?? false) {
                    text += $"\n{ALanguage.GetText("UnlockCondition").Format(paint.Condition.Description.Value)}";
                }
                Main.instance.MouseText(text, 4);
            }

            return;
        }

        Color glowColor = paint.GlowColor * 0.6f * activateAnimation;
        spriteBatch.DrawAlign(AequusTextures.Bloom, drawLocation, null, glowColor, 0f, scale * 0.4f, SpriteEffects.None);

        if (outlineColor != Color.Transparent) {
            spriteBatch.End();
            spriteBatch.Begin_UI(immediate: true);
            var colorOnlyShader = Helper.ShaderColorOnly;
            colorOnlyShader.Apply(null, null);

            for (int j = 0; j < 4; j++) {
                Vector2 offsetLocation = drawLocation + (MathHelper.PiOver2 * j).ToRotationVector2() * 2f * paintScale;
                spriteBatch.DrawAlign(texture, offsetLocation, frame, outlineColor, 0f, paintScale, SpriteEffects.None);
            }

            spriteBatch.End();
            spriteBatch.Begin_UI(immediate: false);
        }

        spriteBatch.DrawAlign(texture, drawLocation, frame, clr, 0f, paintScale, SpriteEffects.None);

        if (hovering) {
            if (Main.mouseLeft && Main.mouseLeftRelease) {
                if (selection == paint.PaintId) {
                    selection = 0;
                }
                else {
                    selection = paint.PaintId;
                }
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            var player = Main.LocalPlayer;
            player.mouseInterface = true;
            Main.instance.MouseText(Lang.GetItemName(paint.ItemId).Value, 1);
        }
        // Helper.DebugTextDraw(paint.PaintID, drawLocation + new Vector2(frame.Width, frame.Height) * 0.4f, 0.75f);
    }

    public override bool Draw(SpriteBatch spriteBatch) {
        if (location.X <= 0f || !Enabled) {
            return true;
        }

        scaledLocation = location / Main.UIScale;
        spriteBatch.DrawAlign(AequusTextures.Bloom, scaledLocation, null, Color.Black * 0.75f * activateAnimation, 0f, 1f, SpriteEffects.None);

        Vector2 spinningPoint = new Vector2(0f, -60f - MathF.Pow(activateAnimation, 2f) * 30f);
        foreach (PaintCollection collection in _paints) {
            int count = collection.Paints.Length;
            Vector2 offsetSpinningPoint = spinningPoint * collection.RadiusMultiplier;
            ref byte selectedPaintOrCoating = ref collection.GetValueReference();

            for (int i = 0; i < collection.Paints.Length; i++) {
                PaintOrCoatingInfo info = collection.Paints[i];
                DrawPaint(spriteBatch, info, offsetSpinningPoint, i, count, ref selectedPaintOrCoating);
            }
        }
        return true;
    }
}