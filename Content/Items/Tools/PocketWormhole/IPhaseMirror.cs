namespace Aequus.Content.Items.Tools.PocketWormhole;

public interface IPhaseMirror {
    void GetPhaseMirrorDust(Player player, out int dustType, out Color dustColor);

    void Teleport(Player player);
}