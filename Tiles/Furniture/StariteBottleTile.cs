using Aequus.Items.Placeable.Furniture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Furniture
{
    public class StariteBottleTile : ModTile
    {
        public const int BlessingDelayMax = 90;

        public static int blessedPlayerDelay;
        public static Point BlesserTile;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.StyleWrapLimit = 111;
            TileObjectData.addTile(Type);
            DustType = -1;
            TileID.Sets.DisableSmartCursor[Type] = true;
            AddMapEntry(new Color(20, 166, 200), CreateMapEntryName());
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.05f;
            g = 0.35f;
            b = 0.75f;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 32, ModContent.ItemType<StariteBottle>());
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer && Main.tile[i, j].TileFrameY == 0 && blessedPlayerDelay <= 0 && Main.rand.NextBool(100))
            {
                int mana = Math.Min(Main.LocalPlayer.statManaMax2 - Main.LocalPlayer.statMana, 20);
                if (mana > 0)
                {
                    SoundEngine.PlaySound(SoundID.Item30.WithPitch(Main.rand.NextFloat(0.25f, 0.75f)).WithVolume(0.2f), new Vector2(i * 16f, j * 16f));
                    Main.LocalPlayer.ManaEffect(mana);
                    Main.LocalPlayer.statMana += mana;
                    BlesserTile = new Point(i, j);
                    blessedPlayerDelay = BlessingDelayMax;
                }
            }
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
        }

        public int GetFrame(int i, int y)
        {
            if (BlesserTile.X == i && BlesserTile.Y == y && blessedPlayerDelay > (BlessingDelayMax - 24))
            {
                return 3 + (BlessingDelayMax - blessedPlayerDelay) / 6;
            }
            return 1 + (int)(Main.GameUpdateCount / 8 + i * i / y + y * y) % 2;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int y = j;
            if (Main.tile[i, j].TileFrameY == 18)
            {
                y--;
            }
            int frame = GetFrame(i, y);
            var drawCoords = new Vector2(i * 16f, j * 16f) - Main.screenPosition + AequusHelpers.TileDrawOffset;
            var spriteFrame = new Rectangle(18 * frame, Main.tile[i, j].TileFrameY, 16, 16);
            Main.spriteBatch.Draw(TextureAssets.Tile[Type].Value, drawCoords, spriteFrame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>($"{Texture}_Glow").Value, drawCoords, spriteFrame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}