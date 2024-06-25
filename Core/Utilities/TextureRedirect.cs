global using static Aequus.Core.Utilities.TextureRedirect;
using ReLogic.Content;
using Terraria.GameContent;

namespace Aequus.Core.Utilities;

/// <summary>Lazy shorthand workaround for utilizing fields found in <see cref="TextureAssets"/>.</summary>
public class TextureRedirect {
    public static Asset<Texture2D>[] ProjectileTexture => TextureAssets.Projectile;
    public static Asset<Texture2D>[] ItemTexture => TextureAssets.Item;
    public static Asset<Texture2D>[] NPCTexture => TextureAssets.Npc;
}
