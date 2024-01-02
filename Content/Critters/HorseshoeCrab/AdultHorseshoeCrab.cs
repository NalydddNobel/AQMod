namespace Aequus.Content.Critters.HorseshoeCrab;

public class AdultHorseshoeCrab : HorseshoeCrab {
    public override void SetDefaults() {
        NPC.width = 10;
        NPC.height = 10;
        DrawOffsetY = -1;
        base.SetDefaults();
    }
}