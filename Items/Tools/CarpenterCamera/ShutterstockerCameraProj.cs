using Aequus.Common.Building;
using Aequus.Content.Building.Challenges;
using Aequus.NPCs.Town.CarpenterNPC.Quest;
using Aequus.NPCs.Town.CarpenterNPC.Quest.Bounties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.CarpenterCamera {
    public class ShutterstockerCameraProj : ModProjectile {
        public class PhotoStatusData {
            public string name;
            public float completion;
        }

        public Vector2 mouseWorld;

        public virtual float PhotoSize { get => Projectile.ai[0]; set => Projectile.ai[0] = MathHelper.Clamp(value, 3f, 60f); }
        public virtual int PhotoSizeX => (int)Projectile.ai[0];
        public virtual int PhotoSizeY => (int)Projectile.ai[0];
        public Rectangle Area => new((int)(Projectile.Center.X / 16f) - PhotoSizeX / 2, (int)(Projectile.Center.Y / 16f) - PhotoSizeY / 2, PhotoSizeX, PhotoSizeY);

        public BuildChallenge Challenge => ModContent.GetInstance<FountainChallenge>();

        public IStepResults[] scanResults;
        public HighlightInfo highlightInfo;
        public PhotoStatusData[] photoStatus;

        public override void SetDefaults() {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            photoStatus = null;
        }

        private void UpdateLegacyPhotoStatus(Player player) {
            if (!player.TryGetModPlayer<CarpenterBountyPlayer>(out var bounty) || bounty.SelectedBounty < 0) {
                return;
            }

            var b = CarpenterSystem.GetBounty(bounty.SelectedBounty);
            photoStatus ??= new PhotoStatusData[b.steps.Count];
            var coords = Projectile.Center.ToTileCoordinates();
            var result = b.CheckConditions(new(Area));
            CarpenterBountyPlayer.LastPhotoTakenResults = result.perStepResults;

            for (int i = 0; i < photoStatus.Length; i++) {
                if (photoStatus[i] == null) {
                    photoStatus[i] = new PhotoStatusData();
                    string name = b.steps[i].GetStepText(b);
                    if (!string.IsNullOrEmpty(name)) {
                        photoStatus[i].name = Language.GetTextValue(name);
                    }
                }

                var status = result.perStepResults[i];
                if (status.success) {
                    photoStatus[i].completion = 1f;
                }
                else {
                    photoStatus[i].completion = 0f;
                }
            }
        }
        private void UpdatePhotoStatus(Player player) {
            ScanInfo scanInfo = new(Area);
            highlightInfo = new(scanInfo.Area);
            scanResults = Challenge.Scan(ref highlightInfo, in scanInfo);

            UpdateLegacyPhotoStatus(player);
        }

        private void SnapPhoto(Player player) {
            UpdatePhotoStatus(player);
            Projectile.ai[1] = 1f;
            //player.Aequus().SetCooldown(300, ignoreStats: true, Main.player[Projectile.owner].HeldItemFixed());
            if (Main.netMode != NetmodeID.Server) {
                ScreenCulling.Prepare(20);
                if (ScreenCulling.OnScreenWorld(Projectile.getRect())) {
                    //Main.BlackFadeIn = 400;
                }
                SoundEngine.PlaySound(SoundID.Camera);
            }
        }

        public override void AI() {
            var player = Main.player[Projectile.owner];

            Projectile.timeLeft = 2;

            Helper.ShootRotation(Projectile, MathHelper.WrapAngle((Projectile.Center - Main.player[Projectile.owner].Center).ToRotation() + (float)Math.PI / 2f));
            player.itemRotation = Projectile.DirectionTo(player).ToRotation();

            Projectile.velocity *= 0.9f;
            if ((int)Projectile.ai[1] > 0) {
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 160f) {
                    Projectile.Kill();
                    return;
                }

                if (Main.myPlayer != Projectile.owner) {
                    return;
                }
                if ((int)Projectile.ai[1] == 30) {
                    Projectile.velocity.Y = -10f;
                }
                if ((int)Projectile.ai[1] >= 30 && (int)Projectile.ai[1] < 60) {
                    Projectile.scale *= 1.01f;
                }
                if ((int)Projectile.ai[1] >= 120) {
                    Projectile.scale *= 0.9f;
                    Projectile.velocity = (Main.player[Projectile.owner].Center - Projectile.Center) / 10f;
                }
                return;
            }

            Projectile.velocity = Vector2.Zero;
            if ((int)Projectile.ai[0] == 0) {
                PhotoSize = 20f;
            }

            if (!player.channel || !player.controlUseItem) {
                SnapPhoto(player);
                return;
            }

            if (Main.myPlayer == Projectile.owner) {
                if (Main.mouseRight && Main.mouseRightRelease) {
                    Projectile.Kill();
                    return;
                }

                var oldMouseWorld = player.MouseWorld();
                mouseWorld = player.MouseWorld();
                if (mouseWorld.X != oldMouseWorld.X || mouseWorld.Y != oldMouseWorld.Y) {
                    Projectile.netUpdate = true;
                }
                int scrollWheel = PlayerInput.ScrollWheelDelta / 120;
                if (scrollWheel != 0) {
                    PhotoSize += scrollWheel;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.Center = mouseWorld.ToTileCoordinates().ToWorldCoordinates();
            if (Main.myPlayer == Projectile.owner) {
                UpdatePhotoStatus(player);
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
            overWiresUI.Add(index);
        }

        private void DrawScanMap(Vector2 topLeft, ScanMap<bool> map, Color shapeColor, out int minX, out int maxX, out int minY, out int maxY) {
            minX = int.MaxValue;
            maxX = int.MinValue;
            minY = int.MaxValue;
            maxY = int.MinValue;

            for (int x = 0; x < map.Width; x++) {
                for (int y = 0; y < map.Height; y++) {
                    if (map[x, y]) {
                        minX = Math.Min(x, minX);
                        maxX = Math.Max(x, maxX);
                        minY = Math.Min(y, minY);
                        maxY = Math.Max(y, maxY);

                        var topLeftBlock = new Vector2(x * 16f, y * 16f) + topLeft;
                        Main.spriteBatch.Draw(
                            AequusTextures.Pixel,
                            topLeftBlock,
                            null,
                            shapeColor * 0.1f,
                            0f,
                            Vector2.Zero,
                            16f,
                            SpriteEffects.None,
                            0f
                        );
                        if (map.SafeGet(x - 1, y, out var left) && !left) {
                            Helper.DrawLine(topLeftBlock, topLeftBlock + new Vector2(0f, 16f), 2f, shapeColor);
                        }
                        if (map.SafeGet(x + 1, y, out var right) && !right) {
                            Helper.DrawLine(topLeftBlock + new Vector2(16f, 0f), topLeftBlock + new Vector2(16f, 16f), 2f, shapeColor);
                        }
                        if (map.SafeGet(x, y + 1, out var bottom) && !bottom) {
                            Helper.DrawLine(topLeftBlock + new Vector2(0f, 16f), topLeftBlock + new Vector2(16f, 16f), 2f, shapeColor);
                        }
                        if (map.SafeGet(x, y - 1, out var top) && !top) {
                            Helper.DrawLine(topLeftBlock, topLeftBlock + new Vector2(16f, 0f), 2f, shapeColor);
                        }
                    }
                }
            }
        }
        private void DrawScanMap(Vector2 topLeft, ScanMap<bool> map, Color shapeColor) {
            DrawScanMap(topLeft, map, shapeColor, out _, out _, out _, out _);
        }

        private void DrawScanOutline(Vector2 topLeft, Color color, int minX, int maxX, int minY, int maxY) {
            if (minX == int.MinValue) {
                return;
            }

            var shapeTopLeft = new Vector2(topLeft.X + minX * 16f, topLeft.Y + minY * 16f);
            var shapeTopRight = new Vector2(topLeft.X + maxX * 16f + 16f, topLeft.Y + minY * 16f);
            var shapeBottomLeft = new Vector2(topLeft.X + minX * 16f, topLeft.Y + maxY * 16f + 16f);
            var shapeBottomRight = new Vector2(topLeft.X + maxX * 16f + 16f, topLeft.Y + maxY * 16f + 16f);
            Helper.DrawLine(shapeTopLeft, shapeBottomLeft, 2f, color);
            Helper.DrawLine(shapeTopRight, shapeBottomRight, 2f, color);
            Helper.DrawLine(shapeTopRight, shapeTopLeft, 2f, color);
            Helper.DrawLine(shapeBottomRight, shapeBottomLeft, 2f, color);
        }

        private void DrawScanResults(Vector2 camEnd, Vector2 size, List<StepRequirement> passes) {
            var icons = AequusTextures.ShutterstockerIcons;
            int j = 0;
            for (int i = 0; i < scanResults.Length; i++) {
                var status = scanResults[i];
                if (status == null || passes[i] == null) {
                    continue;
                }

                var linePosition = camEnd + (size with { Y = -size.Y } + new Vector2(10f, j * 20f));
                Helper.DebugTextDraw(passes[i].GetDisplayName().Value + ": " + status.GetResultText(), linePosition + new Vector2(icons.Width + 4f, 0f));
                int frameY = (int)status.ResultType;
                var frame = icons.Frame(verticalFrames: 3, frameY: frameY);
                var origin = frame.Size() / 2f;
                Main.spriteBatch.Draw(
                    icons,
                    linePosition + origin,
                    frame,
                    Color.White,
                    0f,
                    origin,
                    1f,
                    SpriteEffects.None,
                    0f
                );
                j++;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            var player = Main.player[Projectile.owner];
            var color = Color.White with { A = 100 } * 0.6f * Projectile.scale;
            var camStart = player.Center + player.DirectionTo(Projectile) * 20f - Main.screenPosition;
            var camEnd = Projectile.Center - Main.screenPosition;
            if (PhotoSizeX % 2 == 0) {
                camEnd.X -= 8f;
            }
            if (PhotoSizeY % 2 == 0) {
                camEnd.Y -= 8f;
            }

            var size = new Vector2(PhotoSizeX * 16f, PhotoSizeY * 16f) / 2f * Projectile.scale;
            float startOffset = 8f;

            var bottomRightStart = camStart + new Vector2(startOffset, startOffset);
            var bottomRightEnd = camEnd + size;
            var bottomLeftStart = camStart + new Vector2(-startOffset, startOffset);
            var bottomLeftEnd = camEnd + size with { X = -size.X };
            var topRightStart = camStart + new Vector2(startOffset, -startOffset);
            var topRightEnd = camEnd + size with { Y = -size.Y };
            var topLeftStart = camStart + new Vector2(-startOffset, -startOffset);
            var topLeftEnd = camEnd - size;

            Helper.DrawLine(bottomRightStart, bottomRightEnd, 2f, color * 0.1f);
            Helper.DrawLine(bottomLeftStart, bottomLeftEnd, 2f, color * 0.1f);
            Helper.DrawLine(topRightStart, topRightEnd, 2f, color * 0.1f);
            Helper.DrawLine(topLeftStart, topLeftEnd, 2f, color * 0.1f);
            //Helper.DrawLine(topLeftStart, bottomLeftStart, 2f, color);
            //Helper.DrawLine(topRightStart, bottomRightStart, 2f, color);
            //Helper.DrawLine(topRightStart, topLeftStart, 2f, color);
            //Helper.DrawLine(bottomRightStart, bottomLeftStart, 2f, color);
            Helper.DrawLine(topLeftEnd, bottomLeftEnd, 2f, color);
            Helper.DrawLine(topRightEnd, bottomRightEnd, 2f, color);
            Helper.DrawLine(topRightEnd, topLeftEnd, 2f, color);
            Helper.DrawLine(bottomRightEnd, bottomLeftEnd, 2f, color);

            if ((int)Projectile.ai[1] > 0) {
                if (Projectile.ai[1] < 8f) {
                    Helper.DrawRectangle(new Rectangle((int)topLeftEnd.X, (int)topLeftEnd.Y, (int)(topRightEnd.X - topLeftEnd.X), (int)(bottomLeftEnd.Y - topLeftEnd.Y)), Color.White * MathF.Sin(1f - Projectile.ai[1] / 8f));
                }

                if (Main.myPlayer != Projectile.owner) {
                    return false;
                }

                return false;
            }

            //if (photoStatus != null) {
            //    var icons = AequusTextures.ShutterstockerIcons;
            //    int j = 0;
            //    for (int i = 0; i < photoStatus.Length; i++) {
            //        var status = photoStatus[i];
            //        if (string.IsNullOrEmpty(status.name)) {
            //            continue;
            //        }
            //        var linePosition = camEnd + (size with { Y = -size.Y } + new Vector2(10f, j * 20f));
            //        Helper.DebugTextDraw(status.name + ": " + status.completion, linePosition + new Vector2(icons.Width + 4f, 0f));
            //        int frameY = status.completion >= 1f ? 2 : 0;
            //        var frame = icons.Frame(verticalFrames: 3, frameY: frameY);
            //        var origin = frame.Size() / 2f;
            //        Main.spriteBatch.Draw(
            //            icons,
            //            linePosition + origin,
            //            frame,
            //            Color.White,
            //            0f,
            //            origin,
            //            1f,
            //            SpriteEffects.None,
            //            0f
            //        );
            //        j++;
            //    }
            //}
            if (scanResults == null || highlightInfo.ShapeMap == null) {
                return false;
            }

            var shapeColor = Color.Cyan;
            DrawScanResults(camEnd, size, Challenge.Steps);

            DrawScanMap(topLeftEnd, highlightInfo.ShapeMap, Color.Cyan, out int minX, out int maxX, out int minY, out int maxY);
            DrawScanOutline(topLeftEnd, Color.CadetBlue with { A = 100 } * 0.15f, minX, maxX, minY, maxY);
            for (int k = 0; k < scanResults.Length; k++) {
                DrawScanMap(topLeftEnd, highlightInfo.InterestMap, Color.Orange.HueAdd(MathF.Sin(k * 0.33f + Main.GlobalTimeWrappedHourly * 0.5f) * 0.04f));
            }
            for (int k = 0; k < scanResults.Length; k++) {
                DrawScanMap(topLeftEnd, highlightInfo.ErrorMap, Color.Red.HueAdd(MathF.Sin(k * 0.33f + Main.GlobalTimeWrappedHourly * 0.5f) * 0.04f));
            }
            return false;
        }
    }
}