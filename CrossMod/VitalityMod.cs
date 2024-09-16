using Aequus.Content.CursorDyes.Items;
using Terraria.ModLoader;

namespace Aequus.CrossMod {
    internal class VitalityMod : ModSupport<VitalityMod> {
    }

    internal class VitalityModSupportGlobalNPC : GlobalNPC, IPostSetupContent, IModSupport<VitalityMod> {
        public override bool IsLoadingEnabled(Mod mod) {
            return this.LoadingAllowed();
        }

        public static int Miner_NPCID { get; private set; }

        public void PostSetupContent() {
            Miner_NPCID = this.GetNPC("Miner");
        }

        public override void ModifyShop(NPCShop shop) {
            if (shop.NpcType == Miner_NPCID) {
                shop.Add(ModContent.ItemType<SwordCursor>());
            }
        }
    }
}