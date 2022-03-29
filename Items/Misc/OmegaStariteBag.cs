using Aequus.NPCs.Boss;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class OmegaStariteBag : TreasureBag
    {
        protected override int InternalRarity => ItemRarityID.LightRed;
        public override int BossBagNPC => ModContent.NPCType<OmegaStarite>();
        protected override bool PreHardmode => false;
    }
}