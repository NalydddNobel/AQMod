using Terraria.GameContent.Bestiary;
using Terraria.ID;

namespace Aequus.NPCs.Monsters {
    public class FrostMimic : AdamantiteMimic {
        protected override int CloneNPC => NPCID.IceMimic;
        protected override int DustType => DustID.Frost;
        protected override SpawnConditionBestiaryInfoElement Biome => BestiaryBuilder.UndergroundSnowBiome;

        public override void AI() {
        }
    }
}