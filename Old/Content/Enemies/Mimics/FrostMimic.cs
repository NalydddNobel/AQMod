using Terraria.GameContent.Bestiary;

namespace Aequu2.Old.Content.Enemies.Mimics;

public class FrostMimic : AdamantiteMimic {
    protected override int CloneNPC => NPCID.IceMimic;
    protected override int DustType => DustID.Frost;
    protected override SpawnConditionBestiaryInfoElement Biome => BestiaryBiomeTag.UndergroundSnow;

    public override void AI() {
    }
}