using Aequus.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Moss
{
    public class RadonPlantTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18, };
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.AnchorValidTiles = new int[]
            {
                ModContent.TileType<RadonMossTile>(),
                ModContent.TileType<RadonMossBrickTile>(),
            };
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(50, 50, 66), CreateMapEntryName());
            HitSound = SoundID.Grass;
            DustType = DustID.Ambient_DarkBrown;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.LocalPlayer.active && !Main.LocalPlayer.dead)
            {
                Main.LocalPlayer.AddBuff(ModContent.BuffType<BrokenMapBuff>(), 30);
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i, j].TileFrameY >= 18)
            {
                spriteBatch.Draw(TextureAssets.Tile[Type].Value, new Vector2(i * 16f + 8f, j * 16f + 16f) + AequusHelpers.TileDrawOffset - Main.screenPosition, null, Lighting.GetColor(i, j), 0f, new Vector2(TextureAssets.Tile[Type].Value.Width / 2f, TextureAssets.Tile[Type].Value.Height - 4f), 1f, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}