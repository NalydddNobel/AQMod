using Aequus.Common.ItemDrops;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Weapons.Ranged;
using Aequus.NPCs.Boss;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class OmegaStariteBag : TreasureBag
    {
        protected override int InternalRarity => ItemRarityID.LightRed;
        public override int BossBagNPC => ModContent.NPCType<OmegaStarite>();
        protected override bool PreHardmode => false;

        public override void OpenBossBag(Player player)
        {
            var source = player.GetItemSource_OpenItem(Type);

            DropHelper.OneFromList(source, player, new List<int>() 
            { 
                ModContent.ItemType<Raygun>(), 
            });
            player.QuickSpawnItem(source, ModContent.ItemType<CosmicEnergy>(), player.RollHigherFromLuck(4) + 5);
            player.QuickSpawnItem(source, ModContent.ItemType<LightMatter>(), player.RollHigherFromLuck(7) + 18);

            //AQMod.AequusDeveloperItems(player, hardmode: true);
            //if (Main.rand.NextBool(7))
            //    player.QuickSpawnItem(ModContent.ItemType<OmegaStariteMask>());
            //player.QuickSpawnItem(ModContent.ItemType<CelesteTorus>());
            //if (Main.rand.NextBool(3))
            //    player.QuickSpawnItem(ModContent.ItemType<CosmicTelescope>());
            //int[] choices = new int[]
            //{
            //    ModContent.ItemType<MagicWand>(),
            //    ModContent.ItemType<Raygun>(),
            //};
            //player.QuickSpawnItem(choices[Main.rand.Next(choices.Length)]);
            //player.QuickSpawnItem(ItemID.FallenStar, Main.rand.NextVRand(20, 30));
        }
    }
}