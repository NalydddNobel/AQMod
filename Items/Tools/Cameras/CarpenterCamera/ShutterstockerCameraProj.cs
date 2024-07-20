using Aequus.Common.Carpentry;
using Aequus.Common.Carpentry.Results;
using System;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Aequus.Items.Tools.Cameras.CarpenterCamera;
public class ShutterstockerCameraProj : CameraShootProj {
    public class PhotoStatusData {
        public string name;
        public float completion;
    }

    public override int PhotoSizeX => (int)Projectile.ai[0];
    public override int PhotoSizeY => (int)Projectile.ai[0];

    public bool TryGetChallenge(out BuildChallenge challenge) {
        var player = Main.player[Projectile.owner];
        var carpentryPlayer = player.GetModPlayer<CarpentryPlayer>();
        if (!BuildChallengeLoader.registeredBuildChallenges.IndexInRange(carpentryPlayer.SelectedBounty)) {
            challenge = null;
            return false;
        }
        challenge = BuildChallengeLoader.registeredBuildChallenges[carpentryPlayer.SelectedBounty];
        return true;
    }

    public IScanResults[] scanResults;
    public HighlightInfo highlightInfo;
    public PhotoStatusData[] photoStatus;

    public override void SetDefaults() {
        base.SetDefaults();
        photoStatus = null;
    }

    private void UpdatePhotoStatus() {
        if (TryGetChallenge(out var buildChallenge)) {
            ScanInfo scanInfo = new(Area);
            highlightInfo = new(scanInfo.Area);
            scanResults = buildChallenge.Scan(ref highlightInfo, in scanInfo);
        }
    }

    protected override void Initialize() {
        Projectile.ai[0] = 20f;
    }

    protected override void SnapPhoto() {
        var player = Main.player[Projectile.owner];
        UpdatePhotoStatus();
        player.Aequus().SetCooldown(300, ignoreStats: true, Main.player[Projectile.owner].HeldItemFixed());
        if (Main.netMode != NetmodeID.Server) {
            ScreenCulling.Prepare(20);
            //if (ScreenCulling.OnScreenWorld(Projectile.getRect())) {
            //    Main.BlackFadeIn = 400;
            //}
            SoundEngine.PlaySound(SoundID.Camera);
        }

        if (scanResults != null && TryGetChallenge(out var buildChallenge)) {
            for (int i = 0; i < scanResults.Length; i++) {
                if (scanResults[i].ResultType != StepResultType.Success) {
                    return;
                }
            }
            CarpentrySystem.Complete(buildChallenge);
            player.GetModPlayer<CarpentryPlayer>().SelectedBounty = -1;
            int c = CombatText.NewText(player.Hitbox, Color.LimeGreen, "Completed: " + buildChallenge.GetDisplayName().Value);
            if (c < 0 || c >= Main.maxCombatText) {
                return;
            }
            Main.combatText[c].rotation = 0f;
            Main.combatText[c].lifeTime *= 2;
        }
    }

    protected override void UpdateScrollWheel(int scrollAmount) {
        Projectile.ai[0] = Math.Clamp((int)Projectile.ai[0] + scrollAmount, 10, 60);
    }

    public override void AI() {
        base.AI();
        if (Main.myPlayer == Projectile.owner) {
            UpdatePhotoStatus();
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

    private void DrawScanResults(Vector2 camEnd, Vector2 size, StepRequirement[] passes) {
        var icons = AequusTextures.ShutterstockerIcons;
        int j = 0;
        for (int i = 0; i < scanResults.Length; i++) {
            var status = scanResults[i];
            if (status == null || passes[i] == null) {
                continue;
            }

            var linePosition = camEnd + (size with { Y = -size.Y } + new Vector2(10f, j * 20f));
            string name = passes[i]?.GetDisplayName()?.Value;
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, (string.IsNullOrEmpty(name) ? "NoName" : name) + ": " + status.GetResultText(), linePosition + new Vector2(icons.Width() + 4f, 0f), Color.White, 0f, Vector2.Zero, Vector2.One);

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

    protected override void CustomDraw(Color color, Vector2 camStart, Vector2 camEnd, Vector2 size, Vector2 topLeftEnd) {
        if (scanResults == null || highlightInfo.ShapeMap == null || !TryGetChallenge(out var buildChallenge)) {
            return;
        }

        var shapeColor = Color.Cyan;
        DrawScanResults(camEnd, size, buildChallenge.Steps);

        DrawScanMap(topLeftEnd, highlightInfo.ShapeMap, Color.Cyan, out int minX, out int maxX, out int minY, out int maxY);
        DrawScanOutline(topLeftEnd, Color.CadetBlue with { A = 100 } * 0.15f, minX, maxX, minY, maxY);
        for (int k = 0; k < scanResults.Length; k++) {
            DrawScanMap(topLeftEnd, highlightInfo.InterestMap, Color.Orange.HueAdd(MathF.Sin(k * 0.33f + Main.GlobalTimeWrappedHourly * 0.5f) * 0.04f));
        }
        for (int k = 0; k < scanResults.Length; k++) {
            DrawScanMap(topLeftEnd, highlightInfo.ErrorMap, Color.Red.HueAdd(MathF.Sin(k * 0.33f + Main.GlobalTimeWrappedHourly * 0.5f) * 0.04f));
        }
    }
}