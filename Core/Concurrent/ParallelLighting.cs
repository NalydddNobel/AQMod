using System.Collections.Concurrent;

namespace AequusRemake.Core.Concurrent;

/// <summary>Allows for lights to be safely created in a parallel process.</summary>
public class ParallelLighting {
    public static readonly ParallelLighting Instance = new();

    private record struct LightCast(Vector2 Position, Vector3 RGB);

    private readonly ConcurrentStack<LightCast> _lights = new();

    public void Begin() {
        _lights.Clear();
    }

    public void Add(Vector2 Position, Vector3 RGB) {
        _lights.Push(new LightCast(Position, RGB));
    }

    public void End() {
        while (_lights.TryPop(out LightCast l)) {
            Lighting.AddLight(l.Position, l.RGB);
        }
    }
}
