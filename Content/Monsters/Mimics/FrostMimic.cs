using Aequus.Common.NPCs;
using Terraria.GameContent.Bestiary;

namespace Aequus.Content.Monsters.Mimics;

public class FrostMimic : AdamantiteMimic {
    protected override int CloneNPC => NPCID.IceMimic;
    protected override int DustType => DustID.Frost;
    protected override SpawnConditionBestiaryInfoElement Biome => BestiaryBuilder.UndergroundSnowBiome;

    public override void AI() {
    }
}