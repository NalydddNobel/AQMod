#if POLLUTED_OCEAN
using Aequus.Common.ContentTemplates.Tiles.Rubblemaker;
using Aequus.Content.Items.Materials.CompressedTrash;

namespace Aequus.Content.Tiles.PollutedOcean.Ambient;

internal class PollutedOceanAmbient2x2 : Rubble2x2 {
    public PollutedOceanAmbient2x2() : base() { }
    public PollutedOceanAmbient2x2(string name, bool natural) : base(name, natural) { }

    public override int UseItem => ModContent.ItemType<CompressedTrash>();

    public override int[] Styles => [0,];

    public override void SafeSetStaticDefaults() {
        base.SafeSetStaticDefaults();
        HitSound = SoundID.Dig;
        DustType = DustID.Stone;
        AddMapEntry(new Color(100, 100, 100));
    }
}
#endif