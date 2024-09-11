using Aequus.Common.ContentTemplates.Tiles.Rubblemaker;
using Aequus.Content.Items.Materials.CompressedTrash;

namespace Aequus.Content.Tiles.PollutedOcean.Ambient;

internal class PollutedOceanAmbient1x1 : Rubble1x1 {
    public PollutedOceanAmbient1x1() : base() { }
    public PollutedOceanAmbient1x1(string name, bool natural) : base(name, natural) { }

    public override int UseItem => ModContent.ItemType<CompressedTrash>();

    public override int[] Styles => new[] { 0, };

    public override void SafeSetStaticDefaults() {
        base.SafeSetStaticDefaults();

        DustType = DustID.Stone;
        HitSound = SoundID.Dig;
        AddMapEntry(new Color(80, 80, 80));
    }
}