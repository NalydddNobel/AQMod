using Aequus.Core.Graphics.Animations;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles.SeaPickles;

public class SeaPickleLightTracker : ITileAnimation {
    private bool noPlayer;

    public int DespawnTimer { get; set; }

    public float LightMagnitude { get; private set; }

    public bool Update(int x, int y) {
        if (Main.rand.NextBool(20)) {
            noPlayer = true;
            for (int i = 0; i < Main.maxPlayers; i++) {
                if (Main.player[i].active && !Main.player[i].DeadOrGhost && Collision.CanHitLine(new Vector2(x * 16f + 8f, y * 16f + 8f), 0, 0, Main.player[i].position, Main.player[i].width, Main.player[i].height)) {
                    noPlayer = false;
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
        return DespawnTimer++ < 30;
    }
}