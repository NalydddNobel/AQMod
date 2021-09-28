using AQMod.Assets.Textures;
using AQMod.Common.Utilities;
using AQMod.Content.Skies;
using AQMod.Content.WorldEvents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles
{
    public class GlimmeringStatue : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 18 };
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(75, 139, 166));
            dustType = DustID.Stone;
            animationFrameHeight = 56;
            disableSmartCursor = true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 32, 48, ModContent.ItemType<Items.Placeable.WorldInteractors.GlimmeringStatue>());
        }

        public override bool NewRightClick(int i, int j)
        {
            Main.PlaySound(SoundID.Mech, i * 16, j * 16, 0);
            HitWire(i, j);
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.showItemIcon = true;
            player.showItemIcon2 = ModContent.ItemType<Items.Placeable.WorldInteractors.GlimmeringStatue>();
        }

        public static bool GlimmerInStatue(int x, int y)
        {
            int glimmerX = GlimmerEvent.X;
            int glimmerY = GlimmerEvent.Y;
            if (!GlimmerEvent.IsActive)
            {
                glimmerX = -1;
                glimmerY = -1;
            }
            return glimmerX == x + 1 && glimmerY == y + 3;
        }

        public override void HitWire(int i, int j)
        {
            int x = i - Main.tile[i, j].frameX / 18 % 2;
            int y = j - Main.tile[i, j].frameY / 18 % 3;
            bool glimmerInStatue = GlimmerInStatue(x, y);
            if (GlimmerEvent.IsActive && glimmerInStatue)
            {
                GlimmerEvent.FakeActive = false;
            }
            else
            {
                if (!GlimmerEvent.IsActive)
                {
                    GlimmerEventSky._glimmerLight = 1f;
                }
                GlimmerEvent.Activate((ushort)(x + 1), (ushort)(y + 3), genuine: false);
            }
            if (Wiring.running)
            {
                Wiring.SkipWire(x, y);
                Wiring.SkipWire(x, y + 1);
                Wiring.SkipWire(x, y + 2);
                Wiring.SkipWire(x + 1, y);
                Wiring.SkipWire(x + 1, y + 1);
                Wiring.SkipWire(x + 1, y + 2);
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int x = i - Main.tile[i, j].frameX / 18 % 2;
            int y = j - Main.tile[i, j].frameY / 18 % 3;
            float glowmaskIntensity = 0f;
            if (GlimmerEvent.FakeActive && GlimmerInStatue(x, y))
            {
                glowmaskIntensity = ((float)Math.Sin(Main.GlobalTime * 2f) + 1f) * 0.4f + 0.2f;
            }
            else if (GlimmerEvent.ActuallyActive)
            {
                glowmaskIntensity += 0.1f;
            }
            if (glowmaskIntensity > 0f)
            {
                var texture = DrawUtils.Textures.Glows[GlowID.GlimmeringStatue];
                Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
                if (Main.drawToScreen)
                {
                    zero = Vector2.Zero;
                }
                Main.spriteBatch.Draw(texture, new Vector2(i * 16f, j * 16f) + zero - Main.screenPosition, new Rectangle(Main.tile[i, j].frameX, Main.tile[i, j].frameY, 16, 18), new Color(250, 250, 250, 0) * glowmaskIntensity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}