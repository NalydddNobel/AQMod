using AequusRemake.Core.UI;
using System;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace AequusRemake.Content.Items.Armor.Castaway;

public class DefenseDamageUI : UILayer {
    public DefenseDamageUI() : base("DefenseDamage", InterfaceLayerNames.EntityHealthBars_16, InterfaceScaleType.Game, 0) { }

    private float _anim;
    private float _opacity;

    private float _regenAnimX;
    private float _regenAnim;

    public void OnPlayerHurt(Player player, CastawayPlayer castaway) {
        _anim = -1f;
        _opacity = 1f;
        this.Activate();
    }

    public void OnRegenTick(Player player, CastawayPlayer castaway) {
        _anim = 1;
        _opacity = 1f;
        _regenAnim = 1f;
        _regenAnimX = Main.rand.NextFloat(-1f, 1f);
        this.Activate();
    }

    public override bool OnUIUpdate(GameTime gameTime) {
        _anim += Math.Sign(_anim);

        if (_anim > 90f) {
            _opacity -= 0.05f;
        }
        else if (_anim < -120f) {
            _opacity -= 0.033f;
        }
        else if (_anim != 0f) {
            _opacity += 0.1f;
        }
        else {
            _opacity = 0f;
        }

        if (_regenAnim > 0f) {
            _regenAnim -= 1f / CastawayArmor.DefenseRegenerationRate;
        }

        return (_opacity = MathHelper.Clamp(_opacity, 0f, 1f)) > 0f;
    }

    protected override bool DrawSelf() {
        Player player = Main.LocalPlayer;
        CastawayPlayer castaway = player.GetModPlayer<CastawayPlayer>();

        Texture2D texture = AequusTextures.DefenseDamageIndicator;
        Vector2 drawCoordinates = (new Vector2(player.Center.X, player.Bottom.Y + 22f + player.gfxOffY) - Main.screenPosition).Floor();

        float scale = 1.1f * Main.UIScale;
        var font = FontAssets.MouseText.Value;
        string defenseText = (-castaway.brokenDefense).ToString();
        Vector2 defenseTextSize = Vector2.One * scale * 0.8f;
        Vector2 defenseTextMeasurement = ChatManager.GetStringSize(font, defenseText, Vector2.One);
        Vector2 textOrigin = defenseTextMeasurement / 2f;
        Color textColor = Color.White;
        if (_anim < 0f) {
            if (_anim > -16f) {
                float anim = 1f - MathF.Pow(_anim / -16f, 2f);
                textColor = Color.Lerp(textColor, Color.Red, anim);
                defenseTextSize *= 1f + anim * 0.33f;
                scale *= 1f - anim * 0.1f;
                drawCoordinates += Main.rand.NextVector2Square(-anim, anim) * 4f;
            }
        }
        else if (_anim < 10f) {
            float anim = 1f - MathF.Pow(_anim / 10f, 2f);
            defenseTextSize *= 1f + anim * 0.2f;
            scale *= 1f - anim * 0.2f;
            drawCoordinates.Y += anim * 4f;
            textColor = Color.Lerp(textColor, Color.LightGreen, anim);
        }

        var frame = texture.Frame();
        float defenseReductionPercent = castaway.brokenDefense / Math.Max(Math.Max(castaway.brokenDefenseMax, castaway.brokenDefense), 1f);
        int removeFrame = (int)(frame.Height * defenseReductionPercent);
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Draw(texture, drawCoordinates, frame, Color.Lerp(Color.White, Color.Red * 0.75f, defenseReductionPercent) * 0.25f * _opacity, 0f, texture.Size() / 2f, scale, SpriteEffects.None, 0f);
        spriteBatch.Draw(texture, drawCoordinates + new Vector2(0f, removeFrame * scale), new Rectangle(frame.X, frame.Y + removeFrame, frame.Width, frame.Height - removeFrame), Color.White * 0.66f * _opacity, 0f, texture.Size() / 2f, scale, SpriteEffects.None, 0f);

        var textCoordinates = drawCoordinates + new Vector2(0f, 2f) * scale;
        ChatManager.DrawColorCodedString(spriteBatch, font, defenseText, textCoordinates + new Vector2(0f, defenseTextSize.Y * 2f), (textColor * 0.2f) with { A = 255 } * _opacity, 0f, textOrigin, defenseTextSize * 1.2f);
        ChatManager.DrawColorCodedString(spriteBatch, font, defenseText, textCoordinates, textColor * _opacity, 0f, textOrigin, defenseTextSize);

        if (_regenAnim > 0f) {
            Vector2 regenPosition = textCoordinates + new Vector2(12f * _regenAnimX, _regenAnim * 40f - 20f);
            float textOpacity = MathF.Sin(_regenAnim * MathHelper.Pi);
            DrawHelper.DrawCenteredText(spriteBatch, font, "+1", regenPosition, CombatText.HealLife * textOpacity, scale: new Vector2(0.66f, 0.66f) * Math.Max(textOpacity, 0.5f));
        }

        return true;
    }
}
