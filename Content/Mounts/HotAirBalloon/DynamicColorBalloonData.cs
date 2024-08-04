using System;

namespace Aequus.Content.Mounts.HotAirBalloon;

public struct DynamicColorBalloonData : IBalloonData {
    public int frame;
    public Func<Color> color;
    public Func<Color> flameColor;

    public DynamicColorBalloonData(int frame, Func<Color> color, Func<Color> flameColor = null) {
        this.frame = frame;
        this.color = color;
        this.flameColor = flameColor;
    }

    public int Frame => frame;

    public Color DrawColor() {
        return color();
    }

    public Color? FlameColor() {
        return flameColor?.Invoke();
    }

    public IBalloonData GetBalloonData(Player player) {
        return this;
    }
}
