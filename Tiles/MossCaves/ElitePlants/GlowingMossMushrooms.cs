using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.MossCaves.ElitePlants {
    [Obsolete("Kept for loading the old mushroom tiles and properly converting them into the new ones.")]
    public class GlowingMossMushrooms : ModTile {
        public override string Texture => Aequus.BlankTexture;

        public override void SetStaticDefaults() {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18, };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.addTile(Type);
            AddMapEntry(Color.White);
            HitSound = SoundID.Item10.WithPitchOffset(0.9f);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
            var tile = Main.tile[i, j];
            tile.TileType = (ushort)ModContent.TileType<EliteBuffPlantsHostile>();
            tile.TileFrameX /= 54;
            tile.TileFrameX *= ElitePlantTile.FullFrameSize;
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, i, j);
            return false;
        }
    }
}