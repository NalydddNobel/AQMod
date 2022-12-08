using Aequus.Graphics;
using Aequus.Items.GlobalItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Aequus.Items.Misc.Fish.Legendary
{
    public class Fi : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            TooltipsGlobal.Dedicated[Type] = new TooltipsGlobal.ItemDedication(() => (Main.DiscoColor * 0.66f).UseA(255));
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Batfish);
            Item.uniqueStack = false;
            Item.questItem = false;
        }

        public static void DrawDedicatedTooltip(string text, int x, int y, float rotation, Vector2 origin, Vector2 baseScale, Color color)
        {
            var font = FontAssets.MouseText.Value;
            var rainbow = Colors.AlphaDarken(AequusHelpers.GetRainbowColor(Main.myPlayer, Main.GlobalTimeWrappedHourly * 0.3f).UseA(0));

            var size = font.MeasureString(text);
            var center = size / 2f;
            var texture = ModContent.Request<Texture2D>(Aequus.AssetsPath + "UI/NarrizuulBloom", AssetRequestMode.ImmediateLoad).Value;
            var spotlightOrigin = texture.Size() / 2f;
            float spotlightRotation = rotation + MathHelper.PiOver2;
            var spotlightScale = new Vector2(1.2f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 4f) * 0.145f, center.Y * 0.15f);

            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, new Vector2(x, y), Color.Black, rotation, origin, baseScale);

            for (int k = 0; k < 4; k++)
            {
                float wave = AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 2.5f + k / 4f * MathHelper.TwoPi, 0f, 1f);
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x, y) + new Vector2(wave * 1.5f).RotatedBy(k * MathHelper.PiOver2) * baseScale,
                    rainbow * 1.5f * 0.66f * wave, rotation, origin, baseScale);
            }
            ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x, y), Color.White, rotation, origin, baseScale);

            // light effect
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, rainbow.UseA(0) * 0.1f, spotlightRotation,
               spotlightOrigin, spotlightScale * 1.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, rainbow.UseA(0) * 0.2f, spotlightRotation,
               spotlightOrigin, spotlightScale * 1.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, rainbow.UseA(0) * 0.15f, spotlightRotation,
               spotlightOrigin, spotlightScale * 2f, SpriteEffects.None, 0f);


            if (Aequus.HQ)
            {
                var rand = EffectsSystem.EffectRand;
                int reset = rand.SetRand(Main.LocalPlayer.name.GetHashCode());

                // particles
                var particleTexture = TextureCache.Bloom[0].Value;
                var particleOrigin = particleTexture.Size() / 2f;
                int amt = (int)rand.Rand(size.X / 2, size.X * 1);
                for (int i = 0; i < amt; i++)
                {
                    float lifeTime = (Main.GlobalTimeWrappedHourly * 3f + i) % 20f;
                    int baseParticleX = (int)rand.Rand(4f, size.X - 4f);
                    float particleX = baseParticleX + lifeTime * rand.Rand(-6f, 6f);
                    float particleY = (int)rand.Rand(-15f, 5f);
                    float yDir = rand.Rand(0.33f, 1.1f);
                    float sparkleRotation = rand.Rand(MathHelper.TwoPi) + Main.GlobalTimeWrappedHourly * rand.Rand(2f, 4f);
                    float scale = rand.Rand(0.2f, 0.4f);
                    if (baseParticleX > 14 && baseParticleX < size.X - 14 && rand.RandChance(6))
                    {
                        scale *= 1.5f;
                    }
                    scale /= 4f;
                    var clr = rainbow;
                    if (lifeTime < 0.3f)
                    {
                        clr *= lifeTime / 0.3f;
                    }
                    if (lifeTime < 6f)
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
                        if (lifeTime < 1f)
                        {
                            scale *= lifeTime / 1f;
                        }
                        if (scale > 0.4f)
                        {
                            Main.spriteBatch.Draw(particleTexture, new Vector2(x + particleX, y + particleY - lifeTime * 15f * yDir + 10), null, clr * 1f, 0f, particleOrigin, new Vector2(scale * 0.6f, scale) * 0.5f, SpriteEffects.None, 0f);
                        }
                        Main.spriteBatch.Draw(particleTexture, new Vector2(x + particleX, y + particleY - lifeTime * 15f * yDir + 10), null, clr * 0.55f,
                            0f, particleOrigin, new Vector2(scale, scale), SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(particleTexture, new Vector2(x + particleX, y + particleY - lifeTime * 15f * yDir + 10), null, Color.White,
                            sparkleRotation, particleOrigin, new Vector2(scale * 0.25f, scale), SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(particleTexture, new Vector2(x + particleX, y + particleY - lifeTime * 15f * yDir + 10), null, Color.White,
                            sparkleRotation, particleOrigin, new Vector2(scale, scale * 0.3f), SpriteEffects.None, 0f);
                    }
                }

                rand.SetRand(reset);
            }
        }
    }
}