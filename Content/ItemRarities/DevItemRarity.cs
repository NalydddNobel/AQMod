using Aequus.Common.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Aequus.Content.ItemRarities {
    public class DevItemRarity : ModRarity, IDrawRarity
    {
        public override Color RarityColor => Color.Lerp(Color.Violet, Color.MediumPurple, Helper.Wave(Main.GlobalTimeWrappedHourly * 10f, 0f, 1f));

        public void DrawTooltipLine(string text, int x, int y, float rotation, Vector2 origin, Vector2 baseScale, Color color)
        {
            if (string.IsNullOrWhiteSpace(text)) // since you can rename items.
            {
                return;
            }
            var font = FontAssets.MouseText.Value;
            var size = font.MeasureString(text);
            var center = size / 2f;
            var transparentColor = color * 0.4f;
            transparentColor.A = 0;
            var texture = ModContent.Request<Texture2D>(Aequus.AssetsPath + "UI/NarrizuulBloom", AssetRequestMode.ImmediateLoad).Value;
            var spotlightOrigin = texture.Size() / 2f;
            float spotlightRotation = rotation + MathHelper.PiOver2;
            var spotlightScale = new Vector2(1.2f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 4f) * 0.145f, center.Y * 0.15f);

            // black BG
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, Color.Black * 0.3f, rotation,
            spotlightOrigin, new Vector2(size.X / texture.Width * 2f, center.Y / texture.Height * 2.5f), SpriteEffects.None, 0f);
            ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, font, text, new Vector2(x, y), Color.Black,
                rotation, origin, baseScale);

            if (Aequus.HQ)
            {
                var rand = LegacyEffects.EffectRand;
                int reset = rand.SetRand(Main.LocalPlayer.name.GetHashCode());

                // particles
                var particleTexture = AequusTextures.Bloom0;
                var particleOrigin = particleTexture.Size() / 2f;
                int amt = (int)rand.Rand(size.X / 3, size.X);
                for (int i = 0; i < amt; i++)
                {
                    float lifeTime = (rand.Rand(20f) + Main.GlobalTimeWrappedHourly * 2f) % 20f;
                    int baseParticleX = (int)rand.Rand(4f, size.X - 4f);
                    int particleX = baseParticleX + (int)Helper.Wave(lifeTime + Main.GlobalTimeWrappedHourly * rand.Rand(2f, 5f), -rand.Rand(3f, 10f), rand.Rand(3f, 10f));
                    int particleY = (int)rand.Rand(10f);
                    float scale = rand.Rand(0.2f, 0.4f);
                    if (baseParticleX > 14 && baseParticleX < size.X - 14 && rand.RandChance(6))
                    {
                        scale *= 2f;
                    }
                    scale /= 4.5f;
                    var clr = color;
                    if (lifeTime < 0.3f)
                    {
                        clr *= lifeTime / 0.3f;
                    }
                    if (lifeTime < 5f)
                    {
                        if (lifeTime > MathHelper.PiOver2)
                        {
                            float timeMult = (lifeTime - MathHelper.PiOver2) / MathHelper.PiOver2;
                            scale -= timeMult * 0.4f;
                            if (scale < 0f)
                            {
                                continue;
                            }
                            int colorMinusAmount = (int)(timeMult * 255f);
                            clr.R = (byte)Math.Max(clr.R - colorMinusAmount, 0);
                            clr.G = (byte)Math.Max(clr.G - colorMinusAmount, 0);
                            clr.B = (byte)Math.Max(clr.B - colorMinusAmount, 0);
                            clr.A = (byte)Math.Max(clr.A - colorMinusAmount, 0);
                            if (clr.R == 0 && clr.G == 0 && clr.B == 0 && clr.A == 0)
                            {
                                continue;
                            }
                        }
                        if (scale > 0.4f)
                        {
                            Main.spriteBatch.Draw(particleTexture, new Vector2(x + particleX, y + particleY - lifeTime * 15f + 10), null, clr * 2f, 0f, particleOrigin, scale * 0.5f, SpriteEffects.None, 0f);
                        }
                        Main.spriteBatch.Draw(particleTexture, new Vector2(x + particleX, y + particleY - lifeTime * 15f + 10), null, clr, 0f, particleOrigin, scale, SpriteEffects.None, 0f);
                    }
                }

                rand.SetRand(reset);
            }

            // light effect
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, transparentColor * 0.3f, spotlightRotation,
               spotlightOrigin, spotlightScale * 1.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, transparentColor * 0.3f, spotlightRotation,
               spotlightOrigin, spotlightScale * 1.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, transparentColor * 0.35f, spotlightRotation,
               spotlightOrigin, spotlightScale * 2f, SpriteEffects.None, 0f);

            // colored text
            ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x, y), color,
                rotation, origin, baseScale);

            // glowy effect on text
            float wave = Helper.Wave(Main.GlobalTimeWrappedHourly * 10f, 0f, 1f);
            for (int i = 1; i <= 2; i++)
            {
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x + wave * 1f * i, y), transparentColor,
                    rotation, origin, baseScale);
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x - wave * 1f * i, y), transparentColor,
                    rotation, origin, baseScale);
            }
        }
    }
}