using AQMod.Items.Placeable.Herbs;
using AQMod.Items.Potions.Concoctions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Tiles.Herbs
{
    public sealed class Moonflower : HerbTileBase
    {
        protected override int[] GrowableTiles => new int[]
        {
            TileID.Grass,
            TileID.HallowedGrass,
            TileID.Meteorite,
        };

        protected override Color MapColor => new Color(186, 122, 255, 255);
        public override Vector3 GlowColor => new Vector3(0.45f, 0.05f, 1f);
        protected override int DrawOffsetY => -14;

        public override bool IsBlooming(int i, int j)
        {
            return !Main.dayTime && Main.time > Main.nightLength / 2 - 3600 && Main.time < Main.nightLength / 2 + 3600;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            var clr = GlowColor;
            float multiplier = Math.Max(Main.tile[i, j].frameX / 56, 0.1f);
            r = clr.X * multiplier;
            g = clr.Y * multiplier;
            b = clr.Z * multiplier;

            //r = 0.1f * multiplier;
            //g = 0.325f * multiplier;
            //b = 0.1f * multiplier;
        }

        public override bool Drop(int i, int j)
        {
            if (Main.tile[i, j].frameX >= FrameShiftX)
            {
                Item.NewItem(new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<MoonflowerPollen>());
            }
            if (Main.tile[i, j].frameX == FrameShiftX * 2)
            {
                Item.NewItem(new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<MoonflowerSeeds>(), Main.rand.Next(3) + 1);
            }
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            int petals = 3;
            if (Main.tile[i, j].frameX == FrameShiftX)
            {
                petals = 6;
            }
            else if (Main.tile[i, j].frameX == FrameShiftX * 2)
            {
                petals = 12;
                var center = new Vector2(i * 16f + 8f, j * 16f + 4f);
                for (int k = 0; k < 12; k++)
                {
                    var d = Dust.NewDustDirect(new Vector2(i * 16f, j * 16f), 16, 16, 173);
                    var n = (MathHelper.TwoPi / 12f * k + Main.rand.NextFloat(-0.15f, 0.15f)).ToRotationVector2();
                    d.position = center + n * 4f;
                    d.velocity = n * 7.5f;
                }
            }
            for (int k = 0; k < petals; k++)
            {
                Dust.NewDust(new Vector2(i * 16f, j * 16f), 16, 16, 249);
            }
            return false;
        }

        public override bool KillSound(int i, int j)
        {
            if (Main.tile[i, j].frameX == FrameShiftX * 2)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/moonflower"), new Vector2(i * 16f, j * 16f));
                return false;
            }
            return base.KillSound(i, j);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var texture = Main.tileTexture[Type];
            var effects = SpriteEffects.None;
            SetSpriteEffects(i, j, ref effects);
            var frame = new Rectangle(Main.tile[i, j].frameX, Main.tile[i, j].frameY, FrameWidth, FrameHeight);
            var offset = (AQMod.Zero - Main.screenPosition).Floor();
            var groundPosition = new Vector2(i * 16f + 8f, j * 16f + 16f).Floor();
            spriteBatch.Draw(texture, groundPosition + offset, frame, Lighting.GetColor(i, j), 0f, new Vector2(FrameWidth / 2f, FrameHeight - 2f), 1f, effects, 0f);
            if (Main.tile[i, j].frameX == 56)
            {
                float wave = AQUtils.Wave(Main.GlobalTime * 0.4f, 0.9f, 1.25f);
                var bloom = AQMod.Texture("Assets/EffectTextures/Bloom");
                var ray = ModContent.GetTexture(this.GetPath("BloomRay"));
                var rayPosition = groundPosition + offset + new Vector2(0f, -22f);
                var rayColor = new Color(120, 100, 25, 5);
                var rayScale = new Vector2(0.85f, 0.65f);
                spriteBatch.Draw(bloom, rayPosition, null, rayColor * wave * 0.6f, 0f, bloom.Size() / 2f, rayScale * wave * 0.2f, SpriteEffects.None, 0f);
                spriteBatch.Draw(bloom, rayPosition, null, rayColor * wave * 0.3f, 0f, bloom.Size() / 2f, rayScale * wave * 0.6f, SpriteEffects.None, 0f);
                spriteBatch.Draw(ray, rayPosition, null, rayColor * wave, 0f, ray.Size() / 2f, rayScale * wave, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}