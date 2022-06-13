using Aequus;
using Aequus.Items.Placeable.BossTrophies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Furniture
{
    public sealed class BossRelics : ModTile
    {
        private const int FrameWidth = 18 * 3;
        private const int FrameHeight = 18 * 4;

        public const int OmegaStarite = 0;
        public const int Crabson = 1;
        public const int RedSprite = 2;
        public const int SpaceSquid = 3;
        public const int FrameCount = 4;

        public override string Texture => "Terraria/Images/Tiles_" + TileID.MasterTrophyBase;

        public static List<Point> RenderPoints { get; private set; }
        public Asset<Texture2D> RelicOrbs;
        public Asset<Texture2D> Relic;
        private string RelicPath => base.Texture;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                RelicOrbs = ModContent.Request<Texture2D>(RelicPath + "Orbs");
                Relic = ModContent.Request<Texture2D>(RelicPath);
                RenderPoints = new List<Point>();
                On.Terraria.GameContent.Drawing.TileDrawing.DrawMasterTrophies += TileDrawing_DrawMasterTrophies;
            }
        }

        private void TileDrawing_DrawMasterTrophies(On.Terraria.GameContent.Drawing.TileDrawing.orig_DrawMasterTrophies orig, Terraria.GameContent.Drawing.TileDrawing self)
        {
            orig(self);
            foreach (var p in RenderPoints)
            {
                DrawRelic(p.X, p.Y, Main.spriteBatch);
            }
        }

        public override void Unload()
        {
            RenderPoints?.Clear();
            RenderPoints = null;
            RelicOrbs = null;
            Relic = null;
        }

        public override void SetStaticDefaults()
        {
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

            if (!Main.dedServ)
            {
                AequusTile.ResetTileRenderPoints += () => RenderPoints.Clear();
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            switch (frameX / FrameWidth)
            {
                case OmegaStarite:
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<OmegaStariteRelic>());
                    break;

                case Crabson:
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<CrabsonRelic>());
                    break;

                case RedSprite:
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<RedSpriteRelic>());
                    break;

                case SpaceSquid:
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<SpaceSquidRelic>());
                    break;
            }
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            return false;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            tileFrameX %= FrameWidth;
            tileFrameY %= FrameHeight * 2;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0)
            {
                RenderPoints.Add(new Point(i, j));
            }
        }

        private void DrawRelic(int i, int j, SpriteBatch spriteBatch)
        {
            //Vector2 offScreen = new Vector2(Main.offScreenRange);
            //if (Main.drawToScreen)
            //{
            //    offScreen = Vector2.Zero;
            //}

            Point p = new Point(i, j);
            Tile tile = Main.tile[p.X, p.Y];
            if (!tile.HasTile)
            {
                return;
            }

            var texture = Relic.Value;
            int frameY = tile.TileFrameX / FrameWidth;
            Rectangle frame = texture.Frame(1, FrameCount, 0, frameY);

            Vector2 origin = frame.Size() / 2f;
            Vector2 worldPos = p.ToWorldCoordinates(24f, 64f);

            Color color = Lighting.GetColor(p.X, p.Y);

            bool direction = tile.TileFrameY / FrameHeight != 0;
            SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 5f);
            Vector2 drawPos = worldPos - Main.screenPosition + new Vector2(0f, -40f) + new Vector2(0f, offset * 4f);

            if (frameY == OmegaStarite)
            {
                var orbTexture = RelicOrbs.Value;
                var orbFrame = orbTexture.Frame(1, 5, 0, 0);
                var orbOrigin = orbFrame.Size() / 2f;
                float f = Main.GlobalTimeWrappedHourly % (MathHelper.TwoPi / 5f) - MathHelper.PiOver2;
                int k = 0;
                for (; f <= MathHelper.Pi - MathHelper.PiOver2; f += MathHelper.TwoPi / 5f)
                {
                    float wave = (float)Math.Sin(f);
                    float z = (float)Math.Sin(f + MathHelper.PiOver2);
                    orbFrame.Y = (int)MathHelper.Clamp(2 + z * 2.5f, 0f, 5f) * orbFrame.Height;
                    k++;
                    DrawWithGlowEffect(spriteBatch, orbTexture, drawPos + new Vector2(wave * texture.Width / 2f, wave * orbFrame.Height * 0.4f), orbFrame, color, orbOrigin, effects, offset);
                }
                DrawWithGlowEffect(spriteBatch, texture, drawPos, frame, color, origin, effects, offset);
                for (; k < 5; f += MathHelper.TwoPi / 5f)
                {
                    float wave = (float)Math.Sin(f);
                    float z = (float)Math.Sin(f + MathHelper.PiOver2);
                    orbFrame.Y = (int)MathHelper.Clamp(2 + z * 2.5f, 0f, 5f) * orbFrame.Height;
                    k++;
                    DrawWithGlowEffect(spriteBatch, orbTexture, drawPos + new Vector2(wave * texture.Width / 2f, wave * orbFrame.Height * 0.4f), orbFrame, color, orbOrigin, effects, offset);
                }
            }
            else
            {
                DrawWithGlowEffect(spriteBatch, texture, drawPos, frame, color, origin, effects, offset);
            }
        }
        private void DrawWithGlowEffect(SpriteBatch spriteBatch, Texture2D texture, Vector2 drawPos, Rectangle frame, Color color, Vector2 origin, SpriteEffects effects, float offset)
        {
            drawPos /= 4f;
            drawPos.Floor();
            drawPos *= 4f;
            spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);

            float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 2f) * 0.3f + 0.7f;
            Color effectColor = color;
            effectColor.A = 0;
            effectColor = effectColor * 0.1f * scale;
            for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI))
            {
                spriteBatch.Draw(texture, drawPos + (MathHelper.TwoPi * num5).ToRotationVector2() * (6f + offset * 2f), frame, effectColor, 0f, origin, 1f, effects, 0f);
            }
        }
    }
}