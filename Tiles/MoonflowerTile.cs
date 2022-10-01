using Aequus.Items.Misc;
using Aequus.Items.Placeable.Seeds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles
{
    public class MoonflowerTile : HerbTileBase
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
            float multiplier = Math.Max(Main.tile[i, j].TileFrameX / 56, 0.1f);
            r = clr.X * multiplier;
            g = clr.Y * multiplier;
            b = clr.Z * multiplier;
        }

        public override bool Drop(int i, int j)
        {
            if (Main.tile[i, j].TileFrameX >= FrameShiftX)
            {
                Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<MoonflowerPollen>(), Main.rand.Next(3) + 1);
            }
            if (Main.tile[i, j].TileFrameX == FrameShiftX * 2)
            {
                Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 16, 16), ModContent.ItemType<MoonflowerSeeds>(), Main.rand.Next(3) + 1);
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
            if (Main.tile[i, j].TileFrameX == FrameShiftX)
            {
                petals = 6;
            }
            else if (Main.tile[i, j].TileFrameX == FrameShiftX * 2)
            {
                petals = 12;
                var center = new Vector2(i * 16f + 8f, j * 16f + 4f);
                for (int k = 0; k < 12; k++)
                {
                    var d = Dust.NewDustDirect(new Vector2(i * 16f, j * 16f), 16, 16, DustID.ShadowbeamStaff);
                    var n = (MathHelper.TwoPi / 12f * k + Main.rand.NextFloat(-0.15f, 0.15f)).ToRotationVector2();
                    d.position = center + n * 4f;
                    d.velocity = n * 7.5f;
                }
            }
            for (int k = 0; k < petals; k++)
            {
                Dust.NewDust(new Vector2(i * 16f, j * 16f), 16, 16, DustID.Grubby);
            }
            return false;
        }

        public override bool KillSound(int i, int j, bool fail)
        {
            if (Main.tile[i, j].TileFrameX == FrameShiftX * 2)
            {
                SoundEngine.PlaySound(Aequus.GetSound("moonflower", variance: 0.1f), new Vector2(i * 16f, j * 16f));
                return false;
            }
            return base.KillSound(i, j, fail);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var texture = TextureAssets.Tile[Type].Value;
            var effects = SpriteEffects.None;
            SetSpriteEffects(i, j, ref effects);
            var frame = new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, FrameWidth, FrameHeight);
            var offset = (AequusHelpers.TileDrawOffset - Main.screenPosition).Floor();
            var groundPosition = new Vector2(i * 16f + 8f, j * 16f + 16f).Floor();
            spriteBatch.Draw(texture, groundPosition + offset, frame, Lighting.GetColor(i, j), 0f, new Vector2(FrameWidth / 2f, FrameHeight - 2f), 1f, effects, 0f);
            if (Main.tile[i, j].TileFrameX == 56)
            {
                float wave = AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 0.4f, 0.9f, 1.25f);
                var bloom = TextureCache.Bloom[0].Value;
                var ray = ModContent.Request<Texture2D>(Texture + "Effect").Value;
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