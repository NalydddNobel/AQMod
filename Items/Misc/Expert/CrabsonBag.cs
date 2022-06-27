using Aequus.Common.ItemDrops;
using Aequus.Items.Accessories.Healing;
using Aequus.Items.Armor.Vanity;
using Aequus.Items.Misc.Dyes;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Tools;
using Aequus.NPCs.Boss;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Expert
{
    public class CrabsonBag : TreasureBagBase
    {
        protected override int InternalRarity => ItemRarityID.Blue;
        public override int BossBagNPC => ModContent.NPCType<Crabson>();
        protected override bool PreHardmode => true;

        public override void OpenBossBag(Player player)
        {
            var source = player.GetSource_OpenItem(Type);
            player.QuickSpawnItem(source, ModContent.ItemType<Crabax>());
            if (player.RollLuck(7) == 0)
            {
                player.QuickSpawnItem(source, ModContent.ItemType<CrabsonMask>());
            }
            if (player.RollLuck(3) == 0)
            {
                player.QuickSpawnItem(source, ModContent.ItemType<BreakdownDye>());
            }
            player.QuickSpawnItem(player.GetSource_OpenItem(Type), ModContent.ItemType<AquaticEnergy>(), 3);
            player.QuickSpawnItem(player.GetSource_OpenItem(Type), ModContent.ItemType<Mendshroom>());
        }
    }
}