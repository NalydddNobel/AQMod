using Aequus.Common.ItemDrops;
using Aequus.Items.Accessories;
using Aequus.Items.Armor.Vanity;
using Aequus.Items.Misc.Dyes;
using Aequus.Items.Misc.Energies;
using Aequus.NPCs.Boss;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Expert
{
    public class OmegaStariteBag : TreasureBagBase
    {
        protected override int InternalRarity => ItemRarityID.LightRed;
        public override int BossBagNPC => ModContent.NPCType<OmegaStarite>();
        protected override bool PreHardmode => false;

        public override void OpenBossBag(Player player)
        {
            var source = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(source, ModContent.ItemType<CelesteTorus>());
            if (player.RollLuck(7) == 0)
            {
                player.QuickSpawnItem(source, ModContent.ItemType<OmegaStariteMask>());
            }
            if (player.RollLuck(3) == 0)
            {
                DropHelper.OneFromList(source, player, new List<int>()
                {
                    ModContent.ItemType<ScrollDye>(),
                    ModContent.ItemType<OutlineDye>(),
                });
            }
            player.QuickSpawnItem(source, ModContent.ItemType<CosmicEnergy>(), 3);
        }
    }
}