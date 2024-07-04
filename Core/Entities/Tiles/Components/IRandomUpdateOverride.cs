namespace AequusRemake.Core.Entities.Tiles.Components;

public interface IRandomUpdateOverride {
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns>Whether to override random update logic. Returns false by default.</returns>
    bool PreRandomUpdate(int i, int j) { return false; }
    void PostRandomUpdate(int i, int j) { }
}
