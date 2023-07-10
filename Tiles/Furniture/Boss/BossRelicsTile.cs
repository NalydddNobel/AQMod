using Aequus;
using Aequus.Common.Rendering.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Furniture.Boss {
    public class BossRelicsTile : ModTile, ISpecialTileRenderer {
        private const int FrameWidth = 18 * 3;
        private const int FrameHeight = 18 * 4;

        public const int OmegaStarite = 0;
        public const int Crabson = 1;
        public const int RedSprite = 2;
        public const int SpaceSquid = 3;
        public const int DustDevil = 4;
        public const int UltraStarite = 5;
        public const int FrameCount = 6;

        public override string Texture => $"{Aequus.VanillaTexture}Tiles_{TileID.MasterTrophyBase}";

        public override void SetStaticDefaults() {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.StyleWrapLimitVisualOverride = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.styleLineSkipVisualOverride = 0;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);

            AdjTiles = new int[] { TileID.MasterTrophyBase, };

            AddMapEntry(new Color(233, 207, 94, 255), Language.GetText("MapObject.Relic"));
        }

        public override bool CreateDust(int i, int j, ref int type) {
            return false;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
            tileFrameX %= FrameWidth;
            tileFrameY %= FrameHeight * 2;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
            if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0) {
                SpecialTileRenderer.Add(i, j, TileRenderLayer.PostDrawMasterRelics);
            }
        }

        private void DrawGlowySprite(SpriteBatch spriteBatch, Texture2D texture, Vector2 drawPos, Rectangle frame, Color color, Vector2 origin, SpriteEffects effects, float offset) {
            drawPos /= 4f;
            drawPos.Floor();
            drawPos *= 4f;
            spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);

            float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 2f) * 0.3f + 0.7f;
            Color effectColor = color;
            effectColor.A = 0;
            effectColor = effectColor * 0.1f * scale;
            for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI)) {
                spriteBatch.Draw(texture, drawPos + (MathHelper.TwoPi * num5).ToRotationVector2() * (6f + offset * 2f), frame, effectColor, 0f, origin, 1f, effects, 0f);
            }
        }

        private void Render_OmegaStarite(Texture2D tileTexture, Vector2 drawPos, Rectangle frame, Color color, Vector2 origin, SpriteEffects effects, float offset) {
            var orbTexture = AequusTextures.BossRelicsOrbs;
            var orbFrame = orbTexture.Frame(1, 5, 0, 0);
            var orbOrigin = orbFrame.Size() / 2f;
            float f = Main.GlobalTimeWrappedHourly % (MathHelper.TwoPi / 5f) - MathHelper.PiOver2;
            int k = 0;
            for (; f <= MathHelper.Pi - MathHelper.PiOver2; f += MathHelper.TwoPi / 5f) {
                float wave = (float)Math.Sin(f);
                float z = (float)Math.Sin(f + MathHelper.PiOver2);
                orbFrame.Y = (int)MathHelper.Clamp(2 + z * 2.5f, 0f, 5f) * orbFrame.Height;
                k++;
                DrawGlowySprite(Main.spriteBatch, orbTexture, drawPos + new Vector2(wave * tileTexture.Width / 2f, wave * orbFrame.Height * 0.4f), orbFrame, color, orbOrigin, effects, offset);
            }
            DrawGlowySprite(Main.spriteBatch, tileTexture, drawPos, frame, color, origin, effects, offset);
            for (; k < 5; f += MathHelper.TwoPi / 5f) {
                float wave = (float)Math.Sin(f);
                float z = (float)Math.Sin(f + MathHelper.PiOver2);
                orbFrame.Y = (int)MathHelper.Clamp(2 + z * 2.5f, 0f, 5f) * orbFrame.Height;
                k++;
                DrawGlowySprite(Main.spriteBatch, orbTexture, drawPos + new Vector2(wave * tileTexture.Width / 2f, wave * orbFrame.Height * 0.4f), orbFrame, color, orbOrigin, effects, offset);
            }
        }

        void ISpecialTileRenderer.Render(int i, int j, byte layer) {
            Point p = new(i, j);
            var tile = Main.tile[p];
            if (!tile.HasTile) {
                return;
            }

            var tileTexture = AequusTextures.BossRelicsTile;
            int frameY = tile.TileFrameX / FrameWidth;
            var frame = tileTexture.Frame(1, FrameCount, 0, frameY);
            var origin = frame.Size() / 2f;
            var worldPos = p.ToWorldCoordinates(24f, 64f);
            var color = Lighting.GetColor(p.X, p.Y);
            var effects = tile.TileFrameY / FrameHeight != 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 5f);
            Vector2 drawPos = worldPos - Main.screenPosition + new Vector2(0f, -40f) + new Vector2(0f, offset * 4f);

            if (frameY == OmegaStarite) {
                Render_OmegaStarite(tileTexture, drawPos, frame, color, origin, effects, offset);
                return;
            }

            DrawGlowySprite(Main.spriteBatch, tileTexture, drawPos, frame, color, origin, effects, offset);
        }
    }
}