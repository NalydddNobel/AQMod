using Aequu2.Core.ContentGeneration;
using Terraria.DataStructures;

namespace Aequu2.Old.Content.Tiles.Ambient.BigGems;

internal class BigGemGore : InstancedModGore {
    public BigGemGore(ModTile parent, string name) : base($"Big{name}Gore", $"{parent.NamespaceFilePath()}/Gores/{name}Gore") { }

    public override void OnSpawn(Gore gore, IEntitySource source) {
        gore.numFrames = 3;
        gore.frame = (byte)Main.rand.Next(3);
        gore.scale = Main.rand.NextFloat(0.5f, 1.1f);
        gore.drawOffset = new Vector2(0f, Main.rand.Next(3) * 2 + 6) * gore.scale;
    }
}