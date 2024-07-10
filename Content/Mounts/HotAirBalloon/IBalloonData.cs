namespace AequusRemake.Content.Mounts.HotAirBalloon;

public interface IBalloonData {
    int Frame { get; }
    Color DrawColor();
    Color? FlameColor();
}
