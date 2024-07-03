using Aequu2.Content.Items.Materials;
using Aequu2.Core.Entities.Tiles.Rubblemaker;

namespace Aequu2.Content.Tiles.PollutedOcean.Ambient;

internal class PollutedOceanAmbient1x1 : Rubble1x1 {
    public PollutedOceanAmbient1x1() : base() { }
    public PollutedOceanAmbient1x1(string name, string texture, bool natural) : base(name, texture, natural) { }

    public override int UseItem => ModContent.ItemType<CompressedTrash>();

    public override int[] Styles => new[] { 0, };

    public override void SafeSetStaticDefaults() {
        base.SafeSetStaticDefaults();

        DustType = DustID.Stone;
        HitSound = SoundID.Dig;
        AddMapEntry(new Color(80, 80, 80));
    }
}