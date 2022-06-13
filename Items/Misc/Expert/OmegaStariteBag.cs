using Aequus.Common.ItemDrops;
using Aequus.Items.Accessories;
using Aequus.Items.Armor.Vanity;
using Aequus.Items.Weapons.Ranged;
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
            DropHelper.OneFromList(source, player, new List<int>()
            {
                ModContent.ItemType<Raygun>(),
            });
            if (player.RollLuck(7) == 0)
            {
                player.QuickSpawnItem(source, ModContent.ItemType<OmegaStariteMask>());
            }
            player.QuickSpawnItem(source, ModContent.ItemType<LightMatter>(), Main.rand.Next(7) + 18);
            player.QuickSpawnItem(source, ItemID.FallenStar, Main.rand.Next(11) + 20);

            //AQMod.AequusDeveloperItems(player, hardmode: true);
            //if (Main.rand.NextBool(7))
            //    player.QuickSpawnItem(ModContent.ItemType<OmegaStariteMask>());
            //player.QuickSpawnItem(ModContent.ItemType<CelesteTorus>());
            //if (Main.rand.NextBool(3))
            //    player.QuickSpawnItem(ModContent.ItemType<CosmicTelescope>());
        }
    }
}