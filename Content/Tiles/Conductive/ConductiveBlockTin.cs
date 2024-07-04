namespace AequusRemake.Content.Tiles.Conductive;

[LegacyName("ConductiveBlockTileTin")]
public class ConductiveBlockTin : ConductiveBlock {
    public override int BarItem => ItemID.TinBar;
    public override Color MapColor => new(187, 165, 124);

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        DustType = DustID.Tin;
    }
}