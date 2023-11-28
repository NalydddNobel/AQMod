using Aequus.Content.Graphics.GameOverlays;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Aequus.Content.Equipment.Armor.Castaway;

public class CastawayPlayer : ModPlayer {
    public float kbResist;

    public int brokenDefenseMax;
    public int brokenDefense;
    public int defenseRegeneration;

    public int defenseDamageIndicator;
    public float defenseDamageAnimation;
    public float defenseDamageOpacity;

    public override void ResetEffects() {
        kbResist = 1f;
        brokenDefenseMax = 0;
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers) {
        modifiers.Knockback *= Math.Max(kbResist, 0f);
    }

    public override void OnHurt(Player.HurtInfo info) {
        if (brokenDefense < brokenDefenseMax) {
            int div = Math.Max((int)(10 * Player.DefenseEffectiveness.Value), 1);
            int oldBrokenDefense = brokenDefense;
            brokenDefense = Math.Min(brokenDefense + info.Damage / div, brokenDefenseMax);

            if (defenseDamageIndicator > 0) {
                defenseDamageIndicator = 0;
            }
            defenseDamageIndicator -= brokenDefense - oldBrokenDefense;
            defenseDamageAnimation = -1;
            defenseDamageOpacity = 1f;
        }
    }

    public override void PostUpdateEquips() {
        if (brokenDefense > 0) {
            Player.statDefense -= brokenDefense;

            if (Math.Abs(Player.velocity.X) <= 0.01f && Math.Abs(Player.velocity.Y) <= 0.01f) {
                defenseRegeneration++;
                if (defenseRegeneration >= CastawayArmor.DefenseRegenerationRate) {
                    defenseRegeneration = 0;
                    brokenDefense--;

                    if (defenseDamageIndicator < 0) {
                        defenseDamageIndicator = 0;
                    }
                    defenseDamageIndicator++;
                    defenseDamageAnimation = 1;
                    defenseDamageOpacity = 1f;
                }
            }
            else {
                defenseRegeneration = -CastawayArmor.DefenseRegenerationRate;
            }
        }
        defenseDamageAnimation += Math.Sign(defenseDamageAnimation);

        if (defenseDamageAnimation > 90f) {
            defenseDamageOpacity -= 0.05f;
        }
        else if (defenseDamageAnimation < -120f) {
            defenseDamageOpacity -= 0.033f;
        }
        else if (defenseDamageAnimation != 0f) {
            defenseDamageOpacity += 0.1f;
        }
        else {
            defenseDamageOpacity = 0f;
        }

        if (defenseDamageOpacity <= 0f) {
            defenseDamageAnimation = 0f;
            defenseDamageIndicator = 0;
        }
        defenseDamageOpacity = MathHelper.Clamp(defenseDamageOpacity, 0f, 1f);
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo) {
        if (drawInfo.headOnlyRender || drawInfo.shadow != 0f || defenseDamageAnimation == 0f) {
            return;
        }

        var texture = AequusTextures.DefenseDamageIndicator;
        var drawCoordinates = (new Vector2(drawInfo.Position.X + drawInfo.drawPlayer.width / 2f, drawInfo.Position.Y + drawInfo.drawPlayer.height + 22f) - Main.screenPosition).Floor();

        float scale = 1.1f * Main.UIScale;
        var font = FontAssets.MouseText.Value;
        string defenseText = defenseDamageIndicator.ToString();
        var defenseTextSize = Vector2.One * scale * 0.8f;
        var defenseTextMeasurement = ChatManager.GetStringSize(font, defenseText, Vector2.One);
        var textOrigin = defenseTextMeasurement / 2f;
        var textColor = Color.White;
        if (defenseDamageAnimation < 0f) {
            if (defenseDamageAnimation > -16f) {
                float anim = 1f - MathF.Pow(defenseDamageAnimation / -16f, 2f);
                textColor = Color.Lerp(textColor, Color.Red, anim);
                defenseTextSize *= 1f + anim * 0.33f;
                scale *= 1f - anim * 0.1f;
                drawCoordinates += Main.rand.NextVector2Square(-anim, anim) * 4f;
            }
        }
        else if (defenseDamageAnimation < 10f) {
            float anim = 1f - MathF.Pow(defenseDamageAnimation / 10f, 2f);
            defenseTextSize *= 1f + anim * 0.2f;
            scale *= 1f - anim * 0.2f;
            drawCoordinates.Y += anim * 4f;
            textColor = Color.Lerp(textColor, Color.LightGreen, anim);
        }


        var frame = texture.Frame();
        float defenseReductionPercent = brokenDefense / Math.Max(Math.Max(brokenDefenseMax, brokenDefense), 1f);
        int removeFrame = (int)(frame.Height * defenseReductionPercent);
        ModContent.GetInstance<DrawsOverTilesNPCs>().DrawCommands.Draw(texture, drawCoordinates, frame, Color.Lerp(Color.White, Color.Red * 0.75f, defenseReductionPercent) * 0.25f * defenseDamageOpacity, 0f, texture.Size() / 2f, scale, SpriteEffects.None, 0f);
        ModContent.GetInstance<DrawsOverTilesNPCs>().DrawCommands.Draw(texture, drawCoordinates + new Vector2(0f, removeFrame * scale), new Rectangle(frame.X, frame.Y + removeFrame, frame.Width, frame.Height - removeFrame), Color.White * 0.66f * defenseDamageOpacity, 0f, texture.Size() / 2f, scale, SpriteEffects.None, 0f);

        var textCoordinates = drawCoordinates + new Vector2(0f, 2f) * scale;
        ModContent.GetInstance<DrawsOverTilesNPCs>().DrawCommands.DrawColorCodedString(font, defenseText, textCoordinates + new Vector2(0f, defenseTextSize.Y * 2f), (textColor * 0.2f) with { A = 255 } * defenseDamageOpacity, 0f, textOrigin, defenseTextSize * 1.2f);
        ModContent.GetInstance<DrawsOverTilesNPCs>().DrawCommands.DrawColorCodedString(font, defenseText, textCoordinates, textColor * defenseDamageOpacity, 0f, textOrigin, defenseTextSize);
    }
}