using Aequus.Core.Assets;
using ReLogic.Content;

namespace Aequus;

public sealed partial class AequusTextures : AssetManager<Texture2D> {
    public const string TemporaryBuffIcon = "Terraria/Images/Buff_188";
    public const string TemporaryDebuffIcon = "Terraria/Images/Buff_164";

    public static Asset<Texture2D>[] LensFlares;

    protected override void OnLoad(Mod mod) {
        if (Main.netMode == NetmodeID.Server) { return; }

        LensFlares = GetAssets("LensFlare/FlareSprite", 9);

        static Asset<Texture2D>[] GetAssets(string path, int amount) {
            path = $"Aequus/Assets/Textures/{path}";
            Asset<Texture2D>[] arr = new Asset<Texture2D>[amount];
            for (int i = 0; i < amount; i++) {
                arr[i] = ModContent.Request<Texture2D>($"{path}{i}");
            }

            return arr;
        }
    }

    public static string Buff(int id) {
        return $"Terraria/Images/Buff_{id}";
    }

    public static string Tile(int id) {
        return $"Terraria/Images/Tiles_{id}";
    }

    public static string Extra(int id) {
        return $"Terraria/Images/Extra_{id}";
    }

    public static string Item(int id) {
        return $"Terraria/Images/Item_{id}";
    }

    public static string NPC(int id) {
        return $"Terraria/Images/NPC_{id}";
    }

    public static string Projectile(int id) {
        return $"Terraria/Images/Projectile_{id}";
    }
}