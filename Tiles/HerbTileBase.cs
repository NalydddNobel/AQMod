using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles
{
    public abstract class HerbTileBase : ModTile
    {
        protected virtual int FrameWidth => 26;
        protected virtual int FrameHeight => 30;

        protected short FrameShiftX => (short)(FrameWidth + 2);

        protected virtual int[] GrowableTiles => new int[] { TileID.Grass, TileID.HallowedGrass, };
        protected virtual Color MapColor => Color.White;
        protected virtual string MapName => GetType().Name;
        protected virtual int DrawOffsetY => 0;

        public virtual Vector3 GlowColor => new Vector3(1f, 1f, 1f);
        public virtual bool IsBlooming(int i, int j)
        {
            return true;
        }

        public sealed override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileNoFail[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
            TileObjectData.newTile.CoordinateWidth = FrameWidth;
            TileObjectData.newTile.CoordinateHeights = new int[] { FrameHeight };
            TileObjectData.newTile.DrawYOffset = DrawOffsetY;
            TileObjectData.newTile.AnchorValidTiles = GrowableTiles;

            TileObjectData.newTile.AnchorAlternateTiles = new int[]
            {
                TileID.ClayPot,
                TileID.PlanterBox
            };

            TileObjectData.addTile(Type);

            DustType = 0;
            HitSound = SoundID.Grass;

            AddMapEntry(MapColor, CreateMapEntryName(MapName));
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (Main.tile[i, j].TileFrameX < 28)
            {
                Main.tile[i, j].TileFrameX += FrameShiftX;
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendTileSquare(-1, i, j, 1);
                return;
            }
            bool blooming = IsBlooming(i, j);
            switch (Main.tile[i, j].TileFrameY)
            {
                case 0:
                    {
                    }
                    break;

                case 32:
                    {
                        blooming = Main.windSpeedCurrent.Abs() > 0.3f;
                    }
                    break;
            }
            if (blooming)
            {
                if (Main.tile[i, j].TileFrameX < 56)
                {
                    Main.tile[i, j].TileFrameX += FrameShiftX;
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendTileSquare(-1, i, j, 1);
                }
            }
            else if (Main.tile[i, j].TileFrameX > 28)
            {
                Main.tile[i, j].TileFrameX -= FrameShiftX;
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendTileSquare(-1, i, j, 1);
            }
        }
    }
}