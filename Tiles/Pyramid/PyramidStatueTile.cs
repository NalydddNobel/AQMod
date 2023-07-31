using Aequus.Common;
using Aequus.Common.DataSets;
using Aequus.Content.UI.PyramidOffering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Pyramid {
    public abstract class PyramidStatueTileBase : ModTile {
        public override void SetStaticDefaults() {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;
            TileSets.PreventsSlopesBelow.Add(Type);
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 7;
            TileObjectData.newTile.Origin = new(2, 6);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16 };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            DustType = DustID.Stone;
            MinPick = 210;
            AddMapEntry(new Color(140, 140, 140), TextHelper.GetItemName<PyramidStatue>());
        }
    }

    public class PyramidStatueTile : PyramidStatueTileBase {
        public int SelectionLoopTime = 60;

        private void DrawMoon(int i, int j, SpriteBatch spriteBatch) {
            var drawPosition = this.GetDrawPosition(i, j) + Helper.TileDrawOffset + new Vector2(-16f, -126f + Helper.Wave(Main.GlobalTimeWrappedHourly, -2f, 2f));

            var moonTexture = TextureAssets.Moon[Main.moonType].Value;
            var moonFrame = moonTexture.Frame(verticalFrames: 8, frameY: Main.moonPhase);
            var moonOrigin = moonFrame.Size() / 2f;
            float moonScale = 0.66f;

            var bloomTexture = AequusTextures.Bloom0;
            var bloomOrigin = bloomTexture.Size() / 2f;

            spriteBatch.Draw(bloomTexture, drawPosition, null, Color.White * 0.1f, 0f,
                bloomOrigin, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(bloomTexture, drawPosition, null, Color.Black, 0f,
                bloomOrigin, 0.7f, SpriteEffects.None, 0f);
            spriteBatch.Draw(bloomTexture, drawPosition, null, Color.Black, 0f,
                bloomOrigin, 0.55f, SpriteEffects.None, 0f);
            spriteBatch.Draw(
                moonTexture, drawPosition,
                moonFrame with { Y = 0 },
                Color.Black * 0.5f, 0f, moonOrigin, moonScale, SpriteEffects.None, 0f
            );
            spriteBatch.Draw(
                moonTexture, drawPosition,
                moonFrame,
                Color.White, 0f, moonOrigin, moonScale, SpriteEffects.None, 0f
            );

            spriteBatch.Draw(bloomTexture, drawPosition, null, Color.White * 0.1f, 0f,
                bloomOrigin, 1f, SpriteEffects.None, 0f);
        }

        private void ManualDraw(int i, int j, SpriteBatch spriteBatch, int frameOffset) {
            var tileTexture = TextureAssets.Tile[Type].Value;
            Rectangle frame = new(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY + frameOffset * 126, 16, 16);
            spriteBatch.Draw(tileTexture, this.GetDrawPosition(i, j) + Helper.TileDrawOffset, frame, Lighting.GetColor(i, j), 0f,
                Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
            //var tile = Main.tile[i, j];
            //if (tile.TileFrameX == 54 && tile.TileFrameY == 108) {
            //    DrawMoon(i, j, spriteBatch);
            //}

            var dualism = Helper.GetMoonDualism();
            switch (dualism) {
                case Duality.Light:
                    ManualDraw(i, j, spriteBatch, 1);
                    return false;

                case Duality.Dark:
                    ManualDraw(i, j, spriteBatch, 2);
                    return false;
            }
            return true;
        }

        public override void MouseOver(int i, int j) {
            var player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<PyramidStatue>();
        }

        public override bool RightClick(int i, int j) {
            if (Aequus.UserInterface.CurrentState != null) {
                Aequus.UserInterface.SetState(null);
                return false;
            }

            var tile = Main.tile[i, j];
            int left = i - tile.TileFrameX / 18;
            int top = j - tile.TileFrameY / 18;
            Aequus.UserInterface.SetState(new PyramidOfferingUI(left, top, PyramidOfferingUI.Items[Helper.GetMoonDualism()]));
            return true;
        }
    }
}