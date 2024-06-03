using System.Collections.Generic;
using System.Linq;

namespace Aequus.Core.PhysicsBehaviors;

public static class ExtendVerletIntegration {
    public static IEnumerable<Vector2> ToVector2<T>(this VIString<T> verlet) where T : IVerletIntegrationNode, new() {
        return verlet.segments.Select(r => r.Position);
    }
}