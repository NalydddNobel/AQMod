namespace Aequus.Content.Mounts.HotAirBalloon;

public struct BalloonData : IBalloonData {
    public int frame;
    public Color color;
    public Color? flameColor;

    public BalloonData(int frame, Color color, Color? flameColor = null) {
        this.frame = frame;
        this.color = color;
        this.flameColor = flameColor;
    }

    public int Frame => frame;

    public Color DrawColor() {
        return color;
    }

    public Color? FlameColor() {
        return flameColor;
    }

    public IBalloonData GetBalloonData(Player player) {
        return this;
    }
}
