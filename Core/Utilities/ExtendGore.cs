using Aequus.Core.Assets;
using Terraria.DataStructures;

namespace Aequus.Core.Utilities;

public static class ExtendGore {

    /// <summary>Spawns gore using a Texture. Removes the need for using silent string literals for getting gores.</summary>
    /// <param name="Texture">The Gore texture. (Must be located in a "Gore/" directory)</param>
    /// <param name="Source">The Source of the gore.</param>
    /// <param name="Position">The Position to spawn the gore.</param>
    /// <param name="Velocity">The Velocity to spawn the gore.</param>
    /// <param name="Scale">The Scale of the gore.</param>
    internal static Gore NewGore(RequestCache<Texture2D> Texture, IEntitySource Source, Vector2 Position, Vector2 Velocity, System.Single Scale = 1f) {
        return Gore.NewGoreDirect(Source, Position, Velocity, GetModdedGoreType(Texture), Scale);
    }
    /// <summary><inheritdoc cref="NewGore(RequestCache{Texture2D}, IEntitySource, Vector2, Vector2, System.Single)"/></summary>
    /// <param name="npc"></param>
    /// <param name="Texture">The Gore texture. (Must be located in a "Gore/" directory)</param>
    /// <param name="Position">The Position to spawn the gore.</param>
    /// <param name="Velocity">The Velocity to spawn the gore.</param>
    /// <param name="Source">The Source of the gore, if null, defaults to <see cref="Entity.GetSource_FromThis(global::System.String?)"/>.</param>
    /// <param name="Scale">The Scale of the gore.</param>
    internal static Gore NewGore(this NPC npc, RequestCache<Texture2D> Texture, Vector2 Position, Vector2 Velocity, IEntitySource Source = null, System.Single Scale = 1f) {
        return NewGore(Texture, Source ?? npc.GetSource_FromThis(), Position, Velocity, Scale);
    }

    internal static System.Int32 GetModdedGoreType(RequestCache<Texture2D> Texture) {
        return Aequus.Instance.Find<ModGore>(Texture.Name)?.Type ?? 0;
    }
}
