using Aequus.Items.Accessories.Healing;
using Aequus.Items.Armor.Vanity;
using Aequus.Items.Misc.Dyes;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Tools;
using Aequus.NPCs.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Expert
{
    public class DustDevilBag : TreasureBagBase
    {
        protected override int InternalRarity => ItemRarityID.LightPurple;
        public override int BossBagNPC => ModContent.NPCType<DustDevil>();
        protected override bool PreHardmode => false;

        public override void OpenBossBag(Player player)
        {
            var source = player.GetSource_OpenItem(Type);
            //player.QuickSpawnItem(source, ModContent.ItemType<Crabax>());
            if (player.RollLuck(7) == 0)
            {
                player.QuickSpawnItem(source, ModContent.ItemType<CrabsonMask>());
            }
            player.QuickSpawnItem(player.GetSource_OpenItem(Type), ModContent.ItemType<AtmosphericEnergy>(), 3);
        }
    }
}