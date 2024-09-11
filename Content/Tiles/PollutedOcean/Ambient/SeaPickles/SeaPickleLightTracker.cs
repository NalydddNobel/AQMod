using Aequus.Common.Drawing.TileAnimations;
using Terraria.DataStructures;

namespace Aequus.Content.Tiles.PollutedOcean.Ambient.SeaPickles;

public class SeaPickleLightTracker : ITileAnimation {
    private bool noPlayer = true;

    public int DespawnTimer { get; set; }

    public float LightMagnitude { get; private set; }

    public Color SeaPickleColor { get; private set; }

    public bool Update(int x, int y) {
        if (Main.rand.NextBool(noPlayer ? 20 : 240)) {
            noPlayer = true;
            int size = 120;
            var rect = new Rectangle(x * 16 + 8 - size, y * 16 + 8 - size, size * 2, size * 2);
            for (int i = 0; i < Main.maxPlayers; i++) {
                if (Main.player[i].active && !Main.player[i].DeadOrGhost && (Main.player[i].Hitbox.Intersects(rect) || Collision.CanHitLine(new Vector2(x * 16f + 8f, y * 16f + 8f), 0, 0, Main.player[i].position, Main.player[i].width, Main.player[i].height))) {
                    noPlayer = false;
                    break;
                }
            }
        }

        if (noPlayer) {
            if (LightMagnitude > 0f) {
                LightMagnitude -= Main.rand.NextFloat(0.01f);
                if (LightMagnitude < 0f) {
                    LightMagnitude = 0f;
                }
            }
        }
        else {
            if (LightMagnitude < 1f) {
                LightMagnitude += Main.rand.NextFloat(0.033f);
                if (LightMagnitude > 1f) {
                    LightMagnitude = 1f;
                }
            }
        }

        if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(300)) {
            ModContent.GetInstance<SeaPickleAmbientParticles>().New(new Point16(x, y));
        }

        if (SeaPickleColor == Color.Transparent) {
            SeaPickleColor = new Color(255, 255, 150).HueAdd(Helper.Wave(x * 0.03f, -0.2f, 0.3f));
        }

        return DespawnTimer++ < 300;
    }
}