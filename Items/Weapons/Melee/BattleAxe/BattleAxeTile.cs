using Aequus.Common.Tiles.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.BattleAxe {
    public class BattleAxeTile : ModTile {
        public override string Texture => AequusTextures.BattleAxe.Path;

        public override void SetStaticDefaults() {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            DustType = -1;
            TileID.Sets.DisableSmartCursor[Type] = true;
            AddMapEntry(new Color(136, 136, 148), TextHelper.GetItemName<BattleAxe>());
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
            if (!TileID.Sets.IsATreeTrunk[Framing.GetTileSafely(i - 1, j).TileType]
                && !TileID.Sets.IsATreeTrunk[Framing.GetTileSafely(i + 1, j).TileType]) {
                WorldGen.KillTile(i, j);
            }
            return false;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j) {
            return new[] { new Item(ModContent.ItemType<BattleAxe>()) };
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
            var texture = TextureAssets.Tile[Type].Value;
            var drawCoords = (this.GetDrawPosition(i, j) + Helper.TileDrawOffset + new Vector2(8f)).Floor();
            var effects = SpriteEffects.None;
            var frame = new Rectangle(0, 0, texture.Width / 2 + 10, texture.Height);
            if (TileID.Sets.IsATreeTrunk[Main.tile[i - 1, j].TileType]) {
                effects = SpriteEffects.FlipHorizontally;
                drawCoords.X += 14f;
            }
            spriteBatch.Draw(
                texture,
                drawCoords,
                frame,
                Lighting.GetColor(i, j),
                0f,
                new Vector2(texture.Width / 2f, texture.Height / 4f),
                1f,
                effects,
                0f
            );
            return false;
        }

        public static bool TrySpawnBattleAxe(in GlobalRandomTileUpdateParams info) {
            int battleAxeTile = ModContent.TileType<BattleAxeTile>();
            int x = info.X + (WorldGen.genRand.NextBool() ? -1 : 1);
            var tile = Framing.GetTileSafely(x, info.Y);
            var tree = Framing.GetTileSafely(info.X, info.Y);
            if (!TileID.Sets.IsATreeTrunk[info.TileTypeCache]
                || tile.HasTile
                || tree.TileFrameX > 22
                || tree.TileFrameY > 110
                || !Helper.InOuterX(info.X, info.Y, 3)) {
                return false;
            }
            //Helper.DebugDust(i, j);
            if (Helper.FindPlayerWithin(info.X, info.Y) != -1
                || TileHelper.ScanTiles(new(info.X - 200, info.Y - 50, 400, 100), TileHelper.HasTileAction(battleAxeTile))
                || TileHelper.ScanTiles(new(x, info.Y - 2, 1, 7), TileHelper.IsTree, TileHelper.IsSolid)
                ) {
                return false;
            }
            tile.Active(value: true);
            tile.TileType = (ushort)battleAxeTile;
            AequusWorld.battleAxeFrenzy = 0;
            //WorldGen.PlaceTile(x, j, battleAxeTile, mute: true);
            return tile.TileType == battleAxeTile;
        }
    }
}