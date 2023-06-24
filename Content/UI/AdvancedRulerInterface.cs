using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Aequus.Content.UI {
    public class AdvancedRulerInterface : ILoadable {
        public static AdvancedRulerInterface Instance;

        public const int maxFixedPoints = 6;

        public bool Enabled;
        public bool Holding;
        public Point[] FixedPoints;

        public int Type;

        void ILoadable.Load(Mod mod) {
            Instance = this;
            FixedPoints = new Point[maxFixedPoints];
            Reset();
            //On.Terraria.Main.DrawBuilderAccToggles += Main_DrawBuilderAccToggles;
        }

        //public static void GetBuilderAccsCountToShow(Player plr, out int totalDrawnIcons)
        //{
        //    totalDrawnIcons = plr.InfoAccMechShowWires.ToInt() * 6 + plr.rulerLine.ToInt() + plr.rulerGrid.ToInt() + plr.autoActuator.ToInt() + plr.autoPaint.ToInt() + 1 + (plr.unlockedBiomeTorches ? 1 : 0);
        //}

        //public static void Main_DrawBuilderAccToggles(On.Terraria.Main.orig_DrawBuilderAccToggles orig, Main self, Vector2 start)
        //{
        //    orig(self, start);
        //    if (Instance.Show)
        //    {
        //        GetBuilderAccsCountToShow(Main.LocalPlayer, out int drawnIcons);
        //        int startY = (int)start.Y + 24 * drawnIcons - 24;

        //        if (drawnIcons >= 10)
        //        {
        //            startY -= 24;
        //        }
        //        var texture = ModContent.Request<Texture2D>(Aequus.AssetsPath + "UI/BuilderIcons", AssetRequestMode.ImmediateLoad);

        //        int icon = 0;
        //        for (int j = 0; j < 4; j++)
        //        {
        //            bool flag = true;
        //            bool hovering = new Rectangle((int)start.X - 7, startY + 24 * icon - 7, 14, 14).Contains(Main.mouseX, Main.mouseY);
        //            bool leftClick = hovering && Main.mouseLeft && Main.mouseLeftRelease;
        //            switch (j)
        //            {
        //                case 0:
        //                    if (leftClick)
        //                        Instance.Enabled = !Instance.Enabled;
        //                    flag = Instance.Enabled;
        //                    if (hovering)
        //                        Main.instance.MouseText("Hide Advanced Ruler");
        //                    break;
        //                case 1:
        //                    if (leftClick)
        //                        Instance.HideQuadrants = !Instance.HideQuadrants;
        //                    flag = !Instance.HideQuadrants;
        //                    if (hovering)
        //                        Main.instance.MouseText("Hide Quadrants");
        //                    break;
        //                case 2:
        //                    if (Instance.HideQuadrants)
        //                        goto Continue;

        //                    if (leftClick)
        //                        Instance.HideQuadrantLines = !Instance.HideQuadrantLines;
        //                    flag = !Instance.HideQuadrantLines;
        //                    if (hovering)
        //                        Main.instance.MouseText("Hide Quadrant Lines");
        //                    break;
        //                case 3:
        //                    if (Instance.HideQuadrants)
        //                        goto Continue;
        //                    if (leftClick)
        //                        Instance.HideQuadrantIntersections = !Instance.HideQuadrantIntersections;
        //                    flag = !Instance.HideQuadrantIntersections;
        //                    if (hovering)
        //                        Main.instance.MouseText("Hide Quadrant Intersections");
        //                    break;
        //            }

        //            if (j > 0 && !Instance.Show)
        //                break;
        //            var color = flag ? Color.White : new Color(190, 190, 190, 255);
        //            var frame = new Rectangle(20 * j, 0, 18, 18);
        //            if (hovering)
        //            {
        //                Main.LocalPlayer.mouseInterface = true;
        //                Main.spriteBatch.Draw(texture.Value, new Vector2(start.X, startY + 24 * icon), new Rectangle(frame.X, frame.Y + 20, frame.Width, frame.Height),
        //                    Main.OurFavoriteColor, 0f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);
        //            }

        //            if (leftClick)
        //            {
        //                SoundEngine.PlaySound(SoundID.MenuTick);
        //            }

        //            Main.spriteBatch.Draw(texture.Value, new Vector2(start.X, startY + 24 * icon), frame, color, 0f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);
        //            icon++;
        //        Continue:
        //            continue;
        //        }
        //    }
        //}

        void ILoadable.Unload() {
            Instance = null;
            FixedPoints = null;
        }

        public void Reset() {
            Type = 0;
            Enabled = false;
            for (int i = 0; i < maxFixedPoints; i++) {
                FixedPoints[i] = Point.Zero;
            }
        }

        public static Color TileDropColor(Tile tile) {
            if (tile.HasTile) {
                if (tile.SolidTopType()) {
                    return new Color(255, 60, 200, 100);
                }
                else if (tile.IsSolid()) {
                    return new Color(255, 60, 20, 100);
                }
                else if (TileID.Sets.HasOutlines[tile.TileType]) {
                    return new Color(255, 200, 120, 100);
                }
                return new Color(60, 255, 120, 100);
            }
            return new Color(60, 120, 255, 100);
        }

        public void Render(SpriteBatch spritebatch) {
            var mousePoint = Main.MouseWorld.ToTileCoordinates();
            if (Holding && Aequus.GameWorldActive) {
                HandleUsage(mousePoint);
            }

            var points = new List<Point>();
            if (Holding) {
                points.Add(mousePoint);
            }
            for (int i = 0; i < maxFixedPoints; i++) {
                if (FixedPoints[i] != Point.Zero)
                    points.Add(FixedPoints[i]);
            }

            ScreenCulling.Prepare(18);

            switch (Type) {
                case 0:
                    foreach (var p in points) {
                        if (Cull(p, 360)) {
                            continue;
                        }
                        spritebatch.Draw(TextureAssets.SmartDig.Value, p.ToWorldCoordinates() - Main.screenPosition, null, TileDropColor(Main.tile[p]), 0f, TextureAssets.SmartDig.Value.Size() / 2f, 1f, SpriteEffects.None, 0f);

                        CastTileScannerLine(spritebatch, p, new Point(0, 1));
                        CastTileScannerLine(spritebatch, p, new Point(0, -1));
                        CastTileScannerLine(spritebatch, p, new Point(1, 0));
                        CastTileScannerLine(spritebatch, p, new Point(-1, 0));
                    }
                    break;

                case 1:
                    for (int i = 0; i < maxFixedPoints / 2; i++) {
                        Point p = FixedPoints[i * 2];
                        Point p2 = FixedPoints[i * 2 + 1];

                        if (Holding) {
                            spritebatch.Draw(TextureAssets.SmartDig.Value, mousePoint.ToWorldCoordinates() - Main.screenPosition, null, TileDropColor(Main.tile[p]), 0f, TextureAssets.SmartDig.Value.Size() / 2f, 1f, SpriteEffects.None, 0f);
                            if (p2 == Point.Zero) {
                                p2 = mousePoint;
                            }
                        }

                        if (ScreenCulling.OnScreenWorld(p.ToWorldCoordinates()))
                            spritebatch.Draw(TextureAssets.SmartDig.Value, p.ToWorldCoordinates() - Main.screenPosition, null, TileDropColor(Main.tile[p]), 0f, TextureAssets.SmartDig.Value.Size() / 2f, 1f, SpriteEffects.None, 0f);
                        if (ScreenCulling.OnScreenWorld(p2.ToWorldCoordinates()))
                            spritebatch.Draw(TextureAssets.SmartDig.Value, p2.ToWorldCoordinates() - Main.screenPosition, null, TileDropColor(Main.tile[p]), 0f, TextureAssets.SmartDig.Value.Size() / 2f, 1f, SpriteEffects.None, 0f);
                        if (p == Point.Zero || p2 == Point.Zero) {
                            continue;
                        }

                        if (p.Y < p2.Y) {
                            var p3 = p;
                            p = p2;
                            p2 = p3;
                        }

                        int offX = p2.X - p.X;
                        int dirX = Math.Sign(offX);
                        int offY = p2.Y - p.Y;
                        int dirY = Math.Sign(offY);

                        var cullRect = new Rectangle(p.X, p.Y - dirY, offX + dirX, offY + dirY);

                        if (cullRect.Width == 0) {
                            cullRect.Width = 1;
                        }
                        else if (cullRect.Width < 0) {
                            cullRect.X += cullRect.Width + 1;
                            cullRect.Width = -cullRect.Width;
                        }

                        if (cullRect.Height == 0) {
                            cullRect.Height = 1;
                        }
                        else if (cullRect.Height < 0) {
                            cullRect.Y += cullRect.Height;
                            cullRect.Height = -cullRect.Height;
                        }
                        cullRect = cullRect.WorldRectangle();

                        if (!ScreenCulling.OnScreenWorld(cullRect)) {
                            continue;
                        }

                        var screen = Main.screenPosition.ToTileCoordinates();

                        int startX = p.X + dirX;
                        int endX = p2.X;
                        if (startX < screen.X - 2) {
                            startX = screen.X - 2;
                        }
                        else if (startX > screen.X + Main.screenWidth / 16 + 2) {
                            startX = screen.X + Main.screenWidth / 16 + 2;
                        }
                        if (endX < screen.X - 2) {
                            endX = screen.X - 2;
                        }
                        else if (endX > screen.X + Main.screenWidth / 16 + 2) {
                            endX = screen.X + Main.screenWidth / 16 + 2;
                        }

                        var gridTexture = TextureAssets.MagicPixel.Value;
                        var gridFrame = new Rectangle(0, 0, 1, 1);
                        for (int k = startX; k >= screen.X && k != endX; k += dirX) {
                            if (!WorldGen.InWorld(k, p.Y)) {
                                break;
                            }
                            var clr = TileDropColor(Main.tile[k, p.Y]);
                            var drawCoords = new Point(k, p.Y).ToWorldCoordinates(0f, 0f) - Main.screenPosition;
                            spritebatch.Draw(gridTexture, drawCoords, gridFrame, clr * 0.3f, 0f, Vector2.Zero, 16f, SpriteEffects.None, 0f);
                            spritebatch.Draw(gridTexture, drawCoords + new Vector2(2f), gridFrame, clr * 0.3f, 0f, Vector2.Zero, 12f, SpriteEffects.None, 0f);
                        }

                        var font = FontAssets.MouseText.Value;
                        var text = (offX.Abs() + 1).ToString();
                        var measurement = font.MeasureString(text);
                        ChatManager.DrawColorCodedStringWithShadow(spritebatch, font, text, new Vector2((startX + endX + 1) * 8, p.Y * 16f) - Main.screenPosition, new Color(100, 180, 255, 255), 0f, new Vector2(measurement.X / 2f, measurement.Y), new Vector2((float)Math.Sqrt(Math.Max(offX * 0.06f, 1f))));

                        int startY = p.Y;
                        int endY = p2.Y;
                        if (startY < screen.Y) {
                            startY = screen.Y;
                        }
                        if (startY > screen.Y + Main.screenHeight / 16) {
                            startY = screen.Y + Main.screenHeight / 16;
                        }
                        if (endY < screen.Y) {
                            endY = screen.Y;
                        }
                        else if (endY > screen.Y + Main.screenHeight / 16) {
                            endY = screen.Y + Main.screenHeight / 16;
                        }

                        startY = Math.Clamp(startY, p2.Y, p.Y);

                        for (int l = startY; l >= screen.Y && l <= screen.Y + Main.screenHeight / 16 && l != p2.Y; l += dirY) {
                            if (!WorldGen.InWorld(p.X + offX, l)) {
                                break;
                            }
                            var clr = TileDropColor(Main.tile[p.X + offX, l]);
                            var drawCoords = new Point(p.X + offX, l).ToWorldCoordinates(0f, 0f) - Main.screenPosition;
                            spritebatch.Draw(gridTexture, drawCoords, gridFrame, clr * 0.3f, 0f, Vector2.Zero, 16f, SpriteEffects.None, 0f);
                            spritebatch.Draw(gridTexture, drawCoords + new Vector2(2f), gridFrame, clr * 0.3f, 0f, Vector2.Zero, 12f, SpriteEffects.None, 0f);
                        }

                        text = (offY.Abs() + 1).ToString();
                        measurement = font.MeasureString(text);
                        ChatManager.DrawColorCodedStringWithShadow(spritebatch, font, text, new Vector2((p.X + offX) * 16 + 32f, (startY + endY + 1) * 8) - Main.screenPosition, new Color(100, 180, 255, 255), 0f, new Vector2(measurement.X / 2f, measurement.Y), new Vector2((float)Math.Sqrt(Math.Max(offX * 0.06f, 1f))));

                        //AequusHelpers.DrawLine(p.ToWorldCoordinates() - Main.screenPosition, p2.ToWorldCoordinates() - Main.screenPosition, 8f, Color.White * 0.2f);
                    }
                    break;
            }
        }

        public bool Cull(Point p, int size = 240) {
            return !ScreenCulling.OnScreenWorld(new Rectangle(p.X - size / 2, p.Y - size / 2, size, size).WorldRectangle());
        }

        public void HandleUsage(Point mousePoint) {
            var player = Main.LocalPlayer;
            if (player.mouseInterface || player.lastMouseInterface)
                return;

            bool anyFixedPoints = FixedPoints[0] != Point.Zero;

            if (Main.keyState.IsKeyDown(Keys.LeftControl)) {
                Main.cursorOverride = 2;
                if (Main.mouseLeft && Main.mouseLeftRelease) {
                    IncrementType();
                }
                return;
            }
            if (anyFixedPoints && Main.keyState.IsKeyDown(Keys.LeftShift)) {
                Main.cursorOverride = 6;
                if (Main.mouseRight && Main.mouseRightRelease || Main.mouseLeft && Main.mouseLeftRelease) {
                    for (int i = maxFixedPoints - 1; i >= 0; i--) {
                        if (FixedPoints[i] == mousePoint) {
                            FixedPoints[i] = Point.Zero;
                            SoundEngine.PlaySound(SoundID.MenuTick.WithPitchOffset(-0.5f));
                            return;
                        }
                    }
                    for (int i = maxFixedPoints - 1; i >= 0; i--) {
                        if (FixedPoints[i] != Point.Zero) {
                            FixedPoints[i] = Point.Zero;
                            SoundEngine.PlaySound(SoundID.MenuTick.WithPitchOffset(-0.5f));
                            return;
                        }
                    }
                }
            }

            if (Main.mouseRight && Main.mouseLeftRelease) {
                Main.cursorOverride = 2;
                if (Main.mouseLeft && Main.mouseLeftRelease) {
                    IncrementType();
                }
            }
            else if (Main.mouseLeft) {
                bool leftRelease = Main.mouseLeftRelease;
                if (Main.mouseRight && Main.mouseRightRelease) {
                    mousePoint = Main.LocalPlayer.Center.ToTileCoordinates();
                    leftRelease = true;
                }
                if (leftRelease) {
                    if (Type == 1) {
                        for (int i = 2; i < maxFixedPoints; i++) {
                            FixedPoints[i] = Point.Zero;
                        }
                        if (FixedPoints[0] == Point.Zero) {
                            FixedPoints[0] = mousePoint;
                        }
                        else if (FixedPoints[1] == Point.Zero) {
                            FixedPoints[1] = mousePoint;
                        }
                        else {
                            FixedPoints[0] = mousePoint;
                            FixedPoints[1] = Point.Zero;
                        }
                        SoundEngine.PlaySound(SoundID.MenuTick);
                        return;
                    }
                    else {
                        for (int i = maxFixedPoints - 1; i >= 0; i--) {
                            if (FixedPoints[i] == mousePoint) {
                                FixedPoints[i] = Point.Zero;
                                SoundEngine.PlaySound(SoundID.MenuTick.WithPitchOffset(-0.5f));
                                return;
                            }
                        }
                        for (int i = 0; i < maxFixedPoints; i++) {
                            if (FixedPoints[i] == Point.Zero) {
                                FixedPoints[i] = mousePoint;
                                SoundEngine.PlaySound(SoundID.MenuTick);
                                return;
                            }
                        }
                    }

                    for (int i = maxFixedPoints - 1; i > 0; i--) {
                        FixedPoints[i] = FixedPoints[i - 1];
                    }

                    FixedPoints[0] = mousePoint;
                    SoundEngine.PlaySound(SoundID.MenuTick);
                }
            }
        }

        public void IncrementType() {
            Type = (Type + 1) % 2;

            for (int i = 0; i < maxFixedPoints; i++) {
                FixedPoints[i] = Point.Zero;
            }
            SoundEngine.PlaySound(SoundID.MenuTick.WithPitchOffset(-1f));
        }

        public void CastTileScannerLine(SpriteBatch spritebatch, Point startingLoc, Point dir) {
            var endingLoc = startingLoc;
            float opacity = 1f;
            int start = 0;

        TileScanLoop:
            for (int i = start; ; i++) {
                if (!WorldGen.InWorld(endingLoc.X, endingLoc.Y)) {
                    return;
                }

                if (Main.tile[endingLoc].HasTile) {
                    if (Main.tile[endingLoc].IsSolid()) {
                        if (i == 0)
                            return;

                        if (ScreenCulling.OnScreenWorld(endingLoc.ToWorldCoordinates()))
                            spritebatch.Draw(TextureAssets.SmartDig.Value, endingLoc.ToWorldCoordinates() - Main.screenPosition, null, TileDropColor(Main.tile[endingLoc]).SaturationMultiply(0.7f) * opacity, 0f, TextureAssets.SmartDig.Value.Size() / 2f, 1f, SpriteEffects.None, 0f);
                        break;
                    }
                    if (ScreenCulling.OnScreenWorld(endingLoc.ToWorldCoordinates()))
                        spritebatch.Draw(TextureAssets.SmartDig.Value, endingLoc.ToWorldCoordinates() - Main.screenPosition, null, TileDropColor(Main.tile[endingLoc]).SaturationMultiply(0.7f) * 0.33f * opacity, 0f, TextureAssets.SmartDig.Value.Size() / 2f, 1f, SpriteEffects.None, 0f);
                }

                if (i > 120) {
                    return;
                }
                endingLoc += dir;
            }

            Helper.DrawLine(startingLoc.ToWorldCoordinates() - Main.screenPosition, endingLoc.ToWorldCoordinates() - Main.screenPosition, 8f, new Color(160, 180, 200, 50) * 0.25f * opacity);

            if (Main.tile[endingLoc].SolidTopType()) {
                opacity -= 0.25f;
                if (opacity <= 0f) {
                    return;
                }
                start = 1;
                startingLoc = endingLoc;
                endingLoc += dir;
                goto TileScanLoop;
            }
        }
    }
}