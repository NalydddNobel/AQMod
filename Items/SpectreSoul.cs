using AQMod.Assets;
using AQMod.Common;
using AQMod.Common.Utilities;
using AQMod.Effects.Batchers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AQMod.Items
{
    public class SpectreSoul : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.rare = ItemRarityID.Blue;
            item.buffType = ModContent.BuffType<Buffs.SpectreHealing>();
            item.buffTime = 150;
        }

        public override void PostUpdate()
        {
            if (Main.rand.NextBool(4))
            {
                int d = Dust.NewDust(item.position, item.width, item.height, 180);
                Main.dust[d].scale = 1f + item.velocity.Length() * 0.2f;
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity.X *= 0.035f;
                Main.dust[d].velocity.Y = Main.rand.NextFloat(-1.5f, -0.2f);
            }
        }

        public override bool OnPickup(Player player)
        {
            Main.PlaySound(SoundID.DD2_DarkMageCastHeal, player.Center);
            player.AddBuff(item.buffType, item.buffTime);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0);
        }

        private float _existence;

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            rotation += (float)Math.Sin(_existence * 2f) * 0.2f;
            _existence += 0.0157f;
            return true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var texture = this.GetTexture();
            var spriteOrig = frame.Size() / 2f;
            var center = position + origin + spriteOrig;
            Main.spriteBatch.Draw(texture, center, frame, drawColor, _existence, spriteOrig, scale, SpriteEffects.None, 0f);
            _existence += 0.0157f;
            return false;
        }

        private static bool _superSecret = false;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            _superSecret = AQMod.DebugKeysPressed;
            if (_superSecret)
            {
                tooltips.Add(new TooltipLine(mod, "bad0", "ooo secret text on the item which you can't normally have in your inventory"));
                tooltips.Add(new TooltipLine(mod, "bad1", "ssssssss"));
                tooltips.Add(new TooltipLine(mod, "bad2", "aaaaaaasldkvnsjfvnasjkdv"));
                tooltips.Add(new TooltipLine(mod, "bad3", "very bad"));
                tooltips.Add(new TooltipLine(mod, "bad4", "ur very bad"));
                tooltips.Add(new TooltipLine(mod, "bad5", "u kind of smell tbh"));
                tooltips.Add(new TooltipLine(mod, "bad6", "臭的"));
                tooltips.Add(new TooltipLine(mod, "bad7", "臭的"));
                tooltips.Add(new TooltipLine(mod, "bad8", "臭的臭的臭的臭的臭的臭的臭的臭的"));
                tooltips.Add(new TooltipLine(mod, "bad9", "臭的臭的臭的"));
            }
        }

        public static int lastShader = 0;
        public static int shaderTime = 0;

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (!_superSecret)
            {
                return false;
            }
            float l = line.text.Length;
            string text = line.text;
            var textColor = line.overrideColor.GetValueOrDefault(line.color);
            var trueTextColor = Colors.AlphaDarken(textColor);
            if (line.Name == "bad2" || line.Name == "bad3")
            {
                yOffset = 40;
            }
            else
            {
                yOffset = 0;
            }
            var textPosition = new Vector2(line.X + (int)((float)Math.Max(Math.Sin(Main.GlobalTime + line.Y / 16f) + 0.6f, 0.4f) * 8f), line.Y);
            var ghostlyColor = new Color(100, 120, 255, 0);
            ghostlyColor = Colors.AlphaDarken(ghostlyColor);
            if (line.Name == "bad3")
            {
                Vector3[] positions = new Vector3[8];
                float[] scales = new float[8];
                float time = Main.GlobalTime * 60f;
                for (int i = 0; i < 8; i++)
                {
                    positions[i] = Vector3.Transform(new Vector3(l * 10f, 0f, 0f), Matrix.CreateFromYawPitchRoll(time * 0.01f, time * 0.0157f, time * 0.0314f + MathHelper.TwoPi / 8f * i));
                    scales[i] = ThreeDimensionsEffect.GetParralaxScale((line.baseScale.X + line.baseScale.Y) / 2f, positions[i].Z * 0.157f);
                    if (positions[i].Z > 0f)
                    {
                        var drawPosition = ThreeDimensionsEffect.GetParralaxPosition(new Vector2(textPosition.X + (int)positions[i].X, textPosition.Y + (int)positions[i].Y) + Main.screenPosition, positions[i].Z * 0.0314f) - Main.screenPosition;
                        ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, text, drawPosition, ghostlyColor, line.rotation, line.origin, new Vector2(scales[i], scales[i]), line.maxWidth, false);
                    }
                }
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, text, textPosition, trueTextColor, line.rotation, line.origin, line.baseScale, line.maxWidth, line.spread);
                for (int i = 0; i < 8; i++)
                {
                    if (positions[i].Z <= 0f)
                    {
                        var drawPosition = ThreeDimensionsEffect.GetParralaxPosition(new Vector2(textPosition.X + (int)positions[i].X, textPosition.Y + (int)positions[i].Y) + Main.screenPosition, positions[i].Z * 0.0314f) - Main.screenPosition;
                        ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, text, drawPosition, ghostlyColor, line.rotation, line.origin, new Vector2(scales[i], scales[i]), line.maxWidth, false);
                    }
                }
            }
            else if (line.Name == "bad5")
            {
                ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, Main.fontMouseText, text, textPosition, new Color(0, 0, 0, 255), line.rotation, line.origin, line.baseScale, line.maxWidth, line.spread);
                Main.spriteBatch.End();
                BatcherTypes.StartShaderBatch_UI(Main.spriteBatch);
                shaderTime--;
                if (shaderTime <= 0)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        try
                        {
                            lastShader = GameShaders.Armor.GetShaderIdFromItemId(Main.rand.Next(ItemLoader.ItemCount));
                            if (lastShader != 0)
                            {
                                shaderTime = 60;
                                GameShaders.Armor.Apply(lastShader, item, null);
                                break;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    GameShaders.Armor.Apply(lastShader, item, null);
                }
                ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, text, textPosition, trueTextColor, line.rotation, line.origin, line.baseScale, line.maxWidth, false);
                Main.spriteBatch.End();
                BatcherTypes.StartBatch_UI(Main.spriteBatch);
            }
            else
            {
                float time = Main.GlobalTime + line.Y * 0.01f;
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, text, textPosition, trueTextColor, line.rotation, line.origin, line.baseScale, line.maxWidth, line.spread);
                float a = (float)Math.Sin(time) * (l / 5);
                int reps = 4 + (int)l / 10;
                var rot = MathHelper.TwoPi / reps;
                for (int i = 0; i < reps; i++)
                {
                    var off = new Vector2(a, 0f).RotatedBy(rot * i);
                    ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, text, new Vector2(textPosition.X + (int)off.X, textPosition.Y + (int)off.Y), ghostlyColor, line.rotation, line.origin, line.baseScale, line.maxWidth, false);
                }
            }
            return false;
        }
    }
}