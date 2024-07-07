using AequusRemake.Core.Assets;
using Terraria.DataStructures;

namespace AequusRemake.Core.Util.Helpers;

public sealed class GoreTool {
    /// <summary>Spawns gore using a Texture. Removes the need for using silent string literals for getting gores.</summary>
    /// <param name="Texture">The Gore texture. (Must be located in a "Gore/" directory)</param>
    /// <param name="Source">The Source of the gore.</param>
    /// <param name="Position">The Position to spawn the gore.</param>
    /// <param name="Velocity">The Velocity to spawn the gore.</param>
    /// <param name="Scale">The Scale of the gore.</param>
    internal static Gore NewGore(RequestCache<Texture2D> Texture, IEntitySource Source, Vector2 Position, Vector2 Velocity, float Scale = 1f) {
        return Gore.NewGoreDirect(Source, Position, Velocity, GetModdedGoreType(Texture), Scale);
    }

    internal static int GetModdedGoreType(RequestCache<Texture2D> Texture) {
        return AequusRemake.Instance.Find<ModGore>(Texture.Name)?.Type ?? 0;
    }
}
