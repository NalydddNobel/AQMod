using Aequus.Items.Placeable.Graves;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Furniture
{
    [LegacyName("Tombstones")]
    public class AshTombstones : ModTile
    {
        public const int Style_AshTombstone = 0;
        public const int Style_AshGraveMarker = 1;
        public const int Style_AshCrossGraveMarker = 2;
        public const int Style_AshHeadstone = 3;
        public const int Style_AshGravestone = 4;
        public const int Style_AshObelisk = 5;

        public static int numAshTombstones;

        public override void SetStaticDefaults()
        {
            Main.tileSign[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 18 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(100, 20, 10, 255), CreateMapEntryName("AshTombstone"));
            DustType = 37;
            AdjTiles = new int[] { TileID.Tombstones };
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }

        public override bool RightClick(int i, int j)
        {
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            var source = new EntitySource_TileBreak(i, j);
            var r = new Rectangle(i * 16, j * 16, 32, 32);
            switch (frameX / 36)
            {
                case Style_AshTombstone:
                    Item.NewItem(source, r, ModContent.ItemType<AshTombstone>());
                    break;
                case Style_AshGraveMarker:
                    Item.NewItem(source, r, ModContent.ItemType<AshGraveMarker>());
                    break;
                case Style_AshCrossGraveMarker:
                    Item.NewItem(source, r, ModContent.ItemType<AshCrossGraveMarker>());
                    break;
                case Style_AshHeadstone:
                    Item.NewItem(source, r, ModContent.ItemType<AshHeadstone>());
                    break;
                case Style_AshGravestone:
                    Item.NewItem(source, r, ModContent.ItemType<AshGravestone>());
                    break;
                case Style_AshObelisk:
                    Item.NewItem(source, r, ModContent.ItemType<AshObelisk>());
                    break;
            }
            Sign.KillSign(i, j);
        }

        public override void MouseOverFar(int i, int j)
        {
            MouseOver(i, j);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.04f;
            g = 0.02f;
            b = 0.001f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var frame = new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 16, 16);
            if (Main.tile[i, j].TileFrameY >= 18)
            {
                frame.Y = 18;
            }
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow", AssetRequestMode.ImmediateLoad).Value, new Vector2(i * 16f - Main.screenPosition.X, j * 16f - Main.screenPosition.Y) + AequusHelpers.TileDrawOffset,
                frame, new Color(200, 100, 100, 0) * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 3f, 0.2f, 0.5f), 0f, new Vector2(0f, 0f), 1f, SpriteEffects.None, 0f);
        }
    }
}
