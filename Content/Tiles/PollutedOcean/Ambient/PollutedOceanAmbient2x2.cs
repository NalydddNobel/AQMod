using Aequus.Content.Items.Materials;
using Aequus.Core.Entities.Tiles.Rubblemaker;

namespace Aequus.Content.Tiles.PollutedOcean.Ambient;

internal class PollutedOceanAmbient2x2 : Rubble2x2 {
    public PollutedOceanAmbient2x2() : base() { }
    public PollutedOceanAmbient2x2(string name, string texture, bool natural) : base(name, texture, natural) { }

    public override int UseItem => ModContent.ItemType<CompressedTrash>();

    public override int[] Styles => new[] { 0, };

    public override void SafeSetStaticDefaults() {
        base.SafeSetStaticDefaults();
        HitSound = SoundID.Dig;
        DustType = DustID.Stone;
        AddMapEntry(new Color(100, 100, 100));
    }
}